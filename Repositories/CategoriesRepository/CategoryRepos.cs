using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Category;
using Minimart_Api.DTOS.General;
using Minimart_Api.Models;
using System.Linq;

namespace Minimart_Api.Repositories.CategoriesRepository
{
    public class CategoryRepos : ICategoryRepos
    {
        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<Categories> _logger;

        public CategoryRepos(MinimartDBContext dBContext, ILogger<Categories> logger)
        {
            _dbContext = dBContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Categories>> GetAllCategoriesAsync()
        {
            return await _dbContext.Categories.Select(c=>new Categories { 
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Slug = c.Slug,
                Description = c.Description,
                IsActive = c.IsActive,
                ParentCategoryId = c.ParentCategoryId ?? 0,
                Path = c.Path,
                CreatedOn = c.CreatedOn,
                CreatedBy = c.CreatedBy,
                UpdatedOn = c.UpdatedOn,
                UpdatedBy = c.UpdatedBy
            }).ToListAsync();
        }

        public async Task<Categories> GetCategoryByIdAsync(int CategorId)
        {
            //return await _dbContext.Categories.FindAsync(CategorId);

            return await _dbContext.Categories
                .Where(c => c.CategoryId == CategorId)
                .Select(c => new Categories
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Slug = c.Slug,
                Description = c.Description,
                IsActive = c.IsActive,
                ParentCategoryId = c.ParentCategoryId ?? 0,
                Path = c.Path,
                CreatedOn = c.CreatedOn,
                CreatedBy = c.CreatedBy,
                UpdatedOn = c.UpdatedOn,
                UpdatedBy = c.UpdatedBy
            }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Categories>> GetNestedCategoriesAsync()
        {
            try
            {
                // Step 1: Fetch all Categories at once
                var allCategories = await _dbContext.Categories
                    .Select(c => new Categories
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        Slug = c.Slug,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        ParentCategoryId = c.ParentCategoryId ?? null, // Keep ParentCategoryId as null if it is null
                        Path = c.Path,
                        CreatedOn = c.CreatedOn,
                        CreatedBy = c.CreatedBy,
                        UpdatedOn = c.UpdatedOn,
                        UpdatedBy = c.UpdatedBy,
                        SubCategories = new List<Categories>() // Initialize empty list
                    })
                    .ToListAsync(); // Execute once (single DB query)

                // Step 2: Build the category tree in-memory
                var categoryDictionary = allCategories.ToDictionary(c => c.CategoryId);

                foreach (var category in allCategories)
                {
                    if (category.ParentCategoryId.HasValue && categoryDictionary.ContainsKey(category.ParentCategoryId.Value))
                    {
                        categoryDictionary[category.ParentCategoryId.Value].SubCategories.Add(category);
                    }
                }

                // Step 3: Return only the top-level Categories (where ParentCategoryId is NULL)
                return allCategories.Where(c => c.ParentCategoryId == null).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching nested Categories.");
                throw;
            }
        }


        public async Task<Status> AddCategoriesAsync(CategoriesDto categories)
        {
            try
            {
                if (categories == null)
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Invalid data"
                    };
                }

                // Check if the category already exists
                var existingCategory = await _dbContext.Categories.AnyAsync(c =>
                    c.CategoryName == categories.CategoryName || c.Slug == categories.Slug);
                if (existingCategory)
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Category Already Exists"
                    };
                }

                // Map DTO to entity
                var newCategory = mapDtoToEntity(categories);

                // Validate ParentCategoryId
                if (categories.ParentCategoryId.HasValue && categories.ParentCategoryId.Value != 0)
                {
                    var parentCategory = await _dbContext.Categories
                        .Where(c => c.CategoryId == categories.ParentCategoryId)
                        .Select(c => new { c.CategoryId, c.Path })
                        .FirstOrDefaultAsync();

                    if (parentCategory == null)
                    {
                        return new Status
                        {
                            ResponseCode = 400,
                            ResponseMessage = $"Parent Category with ID {categories.ParentCategoryId} Does Not Exist"
                        };
                    }

                    // Generate category path
                    newCategory.Path = string.IsNullOrEmpty(parentCategory.Path)
                        ? $"{parentCategory.CategoryId}"
                        : $"{parentCategory.Path}/{parentCategory.CategoryId}";
                }
                else
                {
                    // Handle NULL parent category safely
                    newCategory.ParentCategoryId = null;
                    newCategory.Path = ""; // Top-level category
                }


                // Add and save
                _dbContext.Categories.Add(newCategory);
                await _dbContext.SaveChangesAsync();

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Category Saved Successfully"
                };
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while adding Categories.");
                return new Status { ResponseCode = 500, ResponseMessage = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("System Error while adding Categories");
                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = ex.Message,
                };
            }
        }

        


        public async Task<Status> UpdateCategoriesAsync(CategoriesDto categories)
        {
            if (categories == null)
            {
                return new Status
                {
                    ResponseCode = 400,
                    ResponseMessage = "Invalid Categories Data"
                };
            }

            try
            {
                var existingCategory = await _dbContext.Categories
                    .Where(c => c.CategoryId == categories.CategoryId)
                   .Select(c => new Categories
                   {
                       CategoryId = c.CategoryId,
                       CategoryName = c.CategoryName,
                       Slug = c.Slug,
                       Description = c.Description,
                       IsActive = c.IsActive,
                       ParentCategoryId = c.ParentCategoryId ?? null,
                       Path = c.Path,
                       CreatedOn = c.CreatedOn,
                       CreatedBy = c.CreatedBy,
                       UpdatedOn = c.UpdatedOn,
                       UpdatedBy = c.UpdatedBy
                   })
                    .FirstOrDefaultAsync();
                if (existingCategory == null)
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Category Doesn't Exist, please pass a valid Category"
                    };
                }

                // Manually attach the entity to EF Core and mark it as modified
                _dbContext.Categories.Attach(existingCategory);
                _dbContext.Entry(existingCategory).State = EntityState.Modified;

                // Check if ParentCategoryId has changed
                if (existingCategory.ParentCategoryId != categories.ParentCategoryId)
                {
                    // Update the Path for the category and its descendants
                    await UpdateCategoryPathAsync(existingCategory, categories.ParentCategoryId);
                }

                


                // Update other fields
                UpdateEntityFromDto(existingCategory, categories);
                await _dbContext.SaveChangesAsync();

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Categories Updated Successfully"
                };
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while updating Category.");
                return new Status { ResponseCode = 500, ResponseMessage = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "System Error while updating Category.");
                return new Status { ResponseCode = 500, ResponseMessage = $"Internal server error: {ex.Message}" };
            }
        }

        public async Task<Status> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var existingCategory = await _dbContext.Categories.FindAsync(categoryId);
                if (existingCategory == null)
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Category doesn't exist"
                    };
                }

                _dbContext.Categories.Remove(existingCategory);
                await _dbContext.SaveChangesAsync();

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Category Deleted Successfully"
                };
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting category.");
                return new Status { ResponseCode = 500, ResponseMessage = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "System Error while deleting Category.");
                return new Status { ResponseCode = 500, ResponseMessage = $"Internal server error: {ex.Message}" };
            }
        }

        public async Task<IEnumerable<CartResults>> GetSubCategory(int CategoryId)
        {
            return await _dbContext.Products
                .Where(tp => tp.CategoryId == CategoryId)
                .Select(tp => new CartResults
                {
                    ProductName = tp.ProductName,
                    ProductImage = tp.ImageUrl,
                    Instock = tp.StockQuantity,
                    price = tp.Price,
                })
                .ToListAsync();
        }

        public Categories mapDtoToEntity(CategoriesDto dto)
        {
            return new Categories
            {
                CategoryName = dto.CategoryName,
                Slug = dto.Slug,
                Description = dto.Description,
                IsActive = dto.IsActive,
                ParentCategoryId = dto.ParentCategoryId, // Can be null
                CreatedBy = dto.UserName,
                CreatedOn = DateTime.Now,
                Path = "", // Path will be calculated later
                UpdatedBy = ""
            };
        }

        private void UpdateEntityFromDto(Categories entity, CategoriesDto dto)
        {
            entity.CategoryId = dto.CategoryId;
            entity.CategoryName = dto.CategoryName;
            entity.Slug = dto.Slug;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = dto.UserName;
            entity.UpdatedOn = DateTime.Now;
        }

        private async Task UpdateCategoryPathAsync(Categories category, int? newParentCategoryId)
        {
            // Get the new parent category
            var newParentCategory = newParentCategoryId.HasValue
                ? await _dbContext.Categories.FindAsync(newParentCategoryId)
                : null;

            // Calculate the new Path
            var newPath = newParentCategory != null
                ? $"{newParentCategory.Path}/{newParentCategory.CategoryId}"
                : "";

            // Update the Path for the category
            category.Path = newPath;

            // Update the Path for all descendants
            var descendants = await _dbContext.Categories
                .Where(c => c.Path.StartsWith($"{category.Path}/{category.CategoryId}"))
                .ToListAsync();

            foreach (var descendant in descendants)
            {
                descendant.Path = descendant.Path.Replace(
                    $"{category.Path}/{category.CategoryId}",
                    $"{newPath}/{category.CategoryId}"
                );
            }
        }
    }
}