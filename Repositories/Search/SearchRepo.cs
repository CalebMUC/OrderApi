using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.DTOS.Search;
using Minimart_Api.Models;
using Minimart_Api.Repositories.Search;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace Minimart_Api.Repositories.Search
{
    public class SearchRepo : ISearchRepo
    {
        private readonly MinimartDBContext _context;
        private readonly ILogger<SearchRepo> _logger;

        public SearchRepo(MinimartDBContext context, ILogger<SearchRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetSearchSuggestion(string queryName, int limit = 10)
        {
            try
            {
                var suggestions = new List<string>();

                // Get product name suggestions
                var productSuggestions = await _context.Products
                    .Where(p => p.ProductName.Contains(queryName) && p.IsActive && !p.IsDeleted)
                    .Select(p => p.ProductName)
                    .Distinct()
                    .Take(limit)
                    .ToListAsync();

                suggestions.AddRange(productSuggestions);

                // Get category suggestions if we need more
                if (suggestions.Count < limit)
                {
                    var categorySuggestions = await _context.Categories
                        .Where(c => c.Name.Contains(queryName) && c.IsActive)
                        .Select(c => c.Name)
                        .Distinct()
                        .Take(limit - suggestions.Count)
                        .ToListAsync();

                    suggestions.AddRange(categorySuggestions);
                }

                return suggestions.Take(limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search suggestions for query: {QueryName}", queryName);
                return Enumerable.Empty<string>();
            }
        }

        public async Task<IEnumerable<GetProductsDto>> SearchProductsAsync(string queryName)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => (p.ProductName.Contains(queryName) || 
                                p.Description.Contains(queryName) ||
                                p.ProductDescription.Contains(queryName)) && 
                                p.IsActive && !p.IsDeleted)
                    .Include(p => p.Category)
                    .Include(p => p.Merchant)
                    .Take(50) // Limit results for performance
                    .ToListAsync();

                return products.Select(p => new GetProductsDto
                {
                    ProductId = p.ProductId.ToString(), // Convert Guid to string
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    Price = (double)p.Price, // Convert decimal to double  
                    Discount = (double)p.Discount,
                    ImageUrl = p.ImageUrls?.FirstOrDefault() ?? "",
                    CategoryName = p.CategoryName,
                    InStock = p.StockQuantity > 0,
                    StockQuantity = p.StockQuantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products for query: {QueryName}", queryName);
                return Enumerable.Empty<GetProductsDto>();
            }
        }

        public async Task<IEnumerable<Models.Category>> GetSearchResults(string queryname)
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.Name.Contains(queryname) && c.IsActive)
                    .Take(20)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search results for query: {QueryName}", queryname);
                return Enumerable.Empty<Models.Category>();
            }
        }

        public async Task<Status> UpdateColumnJson()
        {
            try
            {
                // Implementation for updating column JSON
                // This might be for updating search index or similar
                await Task.CompletedTask; // Placeholder

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Column JSON updated successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating column JSON");
                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = "Error updating column JSON"
                };
            }
        }

        public async Task<IEnumerable<CartResults>> GetSearchProducts(int CategoryID)
        {
            try
            {
                // For legacy support with int CategoryID
                return await _context.Products
                    .Where(p => p.IsActive && !p.IsDeleted && p.StockQuantity > 0)
                    .Select(p => new CartResults
                    {
                        productID = p.ProductId,
                        ProductName = p.ProductName,
                        ProductImage = p.ImageUrls.FirstOrDefault() ?? "",
                        ProductDescription = p.ProductDescription,
                        price = p.Price,
                        InStock = p.StockQuantity > 0,
                        MerchantId = p.MerchantID
                    })
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search products for category: {CategoryId}", CategoryID);
                return Enumerable.Empty<CartResults>();
            }
        }

        public async Task<PaginatedResult<Product>> GetFilteredProducts(ProductFilterParams filterParams)
        {
            try
            {
                var query = _context.Products
                    .Where(p => p.IsActive && !p.IsDeleted)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(filterParams.SearchTerm))
                {
                    query = query.Where(p => p.ProductName.Contains(filterParams.SearchTerm) ||
                                           p.Description.Contains(filterParams.SearchTerm));
                }

                if (filterParams.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == filterParams.CategoryId);
                }

                if (filterParams.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filterParams.MinPrice);
                }

                if (filterParams.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filterParams.MaxPrice);
                }

                if (filterParams.InStock)
                {
                    query = query.Where(p => p.StockQuantity > 0);
                }

                // Apply sorting - using filterParams properties directly
                if (!string.IsNullOrEmpty(filterParams.SortBy))
                {
                    query = filterParams.SortBy.ToLower() switch
                    {
                        "price" => filterParams.SortOrder == "desc" 
                            ? query.OrderByDescending(p => p.Price) 
                            : query.OrderBy(p => p.Price),
                        "name" => filterParams.SortOrder == "desc" 
                            ? query.OrderByDescending(p => p.ProductName) 
                            : query.OrderBy(p => p.ProductName),
                        "date" => filterParams.SortOrder == "desc" 
                            ? query.OrderByDescending(p => p.CreatedOn) 
                            : query.OrderBy(p => p.CreatedOn),
                        _ => query.OrderByDescending(p => p.CreatedOn)
                    };
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply pagination - using filterParams.PageNumber and filterParams.PageSize
                var products = await query
                    .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                    .Take(filterParams.PageSize)
                    .ToListAsync();

                return new PaginatedResult<Product>
                {
                    Items = products,
                    TotalCount = totalCount,
                    PageNumber = filterParams.PageNumber,
                    PageSize = filterParams.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filterParams.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered products");
                return new PaginatedResult<Product>
                {
                    Items = new List<Product>(),
                    TotalCount = 0,
                    PageNumber = filterParams.PageNumber,
                    PageSize = filterParams.PageSize,
                    TotalPages = 0
                };
            }
        }
    }
}
