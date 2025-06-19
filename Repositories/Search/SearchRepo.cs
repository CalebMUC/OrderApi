using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;
using Minimart_Api.Data;
using Minimart_Api.DTOS.General;
using Minimart_Api.Models;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Products;
using System.Text;
using System.Data.SqlClient;
using OpenSearch.Client;
using Minimart_Api.DTOS.Search;
using Microsoft.IdentityModel.Tokens;

namespace Minimart_Api.Repositories.Search
{
    public class SearchRepo : ISearchRepo
    {
        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<SearchRepo> _logger;

        public SearchRepo(MinimartDBContext dbContext, ILogger<SearchRepo> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<IEnumerable<string>> GetSearchSuggestion(string queryName, int limit = 10)
        {
            try {

                var suggestions = await _dbContext.Products
                    .Where(p => p.ProductName.ToLower().Contains(queryName.ToLower()))
                    .OrderBy(p => p.SearchKeyWord)
                    .Take(limit)
                    .Select(p => p.SearchKeyWord)
                    .ToListAsync();

                return suggestions;

            }
            catch (Exception ex) {

                _logger.LogError(ex, "Error Retrieving suggestions");
                return Enumerable.Empty<string>();
            }
        }

        public async Task<IEnumerable<GetProductsDto>> SearchProductsAsync(string queryName)
        {
            try
            {
                var suggestions = await _dbContext.Products
                    .Where(p => p.ProductName.ToLower().Contains(queryName.ToLower()))
                    .Select(p => new GetProductsDto
                    {
                        MerchantID = p.MerchantID,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        CategoryId = p.CategoryId,
                        ProductId = p.ProductId,
                        ProductDescription = p.ProductDescription,
                        CategoryName = p.CategoryName,
                        ImageUrl = p.ImageUrl,
                        InStock = p.InStock,
                        Discount = p.Discount,
                        SearchKeyWord = p.SearchKeyWord,
                        KeyFeatures = p.KeyFeatures,
                        Specification = p.Specification,
                        Box = p.Box,
                        SubCategoryId = p.SubCategoryId,
                        SubCategoryName = p.SubCategoryName,
                        SubSubCategoryName = p.SubSubCategoryName,
                        ProductType = p.ProductType,
                        CreatedOn = p.CreatedOn,
                        CreatedBy = p.CreatedBy,
                        UpdatedOn = p.UpdatedOn,
                        UpdatedBy = p.UpdatedBy
                    })
                    //.Take(10) // Limit to 10 suggestions for performance
                    .ToListAsync();

                return suggestions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Retrieving suggestions");
                return Enumerable.Empty<GetProductsDto>();
            }
        }

        public async Task<Status> UpdateColumnJson()
        {
            try
            {
                var rows = _dbContext.Products.ToList();
                var columns = new List<string> { "KeyFeatures", "Specification", "Box" };

                // Loop through each row
                foreach (var row in rows)
                {
                    // Use reflection to get all properties of the row
                    var properties = row.GetType().GetProperties();

                    foreach (var property in properties)
                    {
                        // Check if the property name is in the list of columns to convert
                        if (columns.Contains(property.Name))
                        {
                            try
                            {
                                // Get the column data as a string
                                var columnData = property.GetValue(row) as string;

                                if (!string.IsNullOrEmpty(columnData))
                                {
                                    // Convert the column data to JSON
                                    var jsonValue = ConvertToJson(columnData);

                                    // Set the JSON value back to the property
                                    property.SetValue(row, jsonValue);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log or handle the error for this specific property
                                Console.WriteLine($"Error converting column '{property.Name}' ': {ex.Message}");
                                return new Status
                                {
                                    ResponseCode = 500,
                                    ResponseMessage = $"Error converting column '{property.Name}' for row ID : {ex.Message}"
                                };
                            }
                        }
                    }
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception thrown by SaveChangesAsync
                Console.WriteLine($"Error saving changes to the database: {ex.Message}");
                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = $"Error saving changes to the database: {ex.Message}"
                };
            }

            return new Status
            {
                ResponseCode = 200,
                ResponseMessage = "Success"
            };
        }

        public string ConvertToJson(string data)
        {

            var keyValuePattern = new Regex(@"(?<key>[A-Za-z\s]+)\s(?<value>[^:]+)");
            var matches = keyValuePattern.Matches(data);

            var dictionary = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                var key = match.Groups["key"].Value.Trim();
                var value = match.Groups["value"].Value.Trim();
                dictionary[key] = value;
            }

            return JsonConvert.SerializeObject(dictionary);

        }
        public async Task<IEnumerable<Categories>> GetSearchResults(string queryname)
        {

            if (string.IsNullOrWhiteSpace(queryname))
                return new List<Categories>();

            var response = await _dbContext.Categories
                .Where(s => s.CategoryName.Contains(queryname))
                .OrderBy(s => s.CategoryName)
                .Take(10)
                .ToListAsync();

            return response;

        }

        public async Task<IEnumerable<CartResults>> GetSearchProducts(int subCategoryId)
        {
            return await _dbContext.Products
                .Where(tp => tp.CategoryId == subCategoryId)
                .Select(tp => new CartResults
                {
                    productID = tp.ProductId,
                    ProductName = tp.ProductName,
                    ProductImage = tp.ImageUrl,
                    InStock = tp.InStock,
                    price = tp.Price,
                })
                .ToListAsync();
        }


        // SearchService.cs
        public async Task<PaginatedResult<Products>> GetFilteredProducts(ProductFilterParams filterParams)
        {
            var query = _dbContext.Products.AsQueryable();

            // Search term filter
            if (!string.IsNullOrEmpty(filterParams.SearchTerm))
            {
                query = query.Where(p =>
                    p.ProductName.Contains(filterParams.SearchTerm) 
                    //p.Description.Contains(filterParams.SearchTerm) ||
                    //p.SearchKeyWord.Contains(filterParams.SearchTerm)
                    );
            }

            // Category filters
            if (filterParams.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filterParams.CategoryId);
            }

            if (filterParams.SubCategoryId != null)
            {
                query = query.Where(p => p.SubCategoryId == filterParams.SubCategoryId);
            }

            // Price range filter
            if (filterParams.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filterParams.MinPrice.Value);
            }

            if (filterParams.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filterParams.MaxPrice.Value);
            }

            // Feature filters (LIKE fallback for SQL Server)
            foreach (var featureFilter in filterParams.Features)
            {
                foreach (var value in featureFilter.Value)
                {
                    var searchPattern = $"\"{featureFilter.Key}\":\"{value}\"";
                    query = query.Where(p => p.KeyFeatures.Contains(searchPattern));
                }
            }

            // Total count
            var totalCount = await query.CountAsync();

            // Pagination
            var items = await query
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .ToListAsync();

            return new PaginatedResult<Products>
            {
                Items = items,
                TotalCount = totalCount
            };
        }



    }
}
