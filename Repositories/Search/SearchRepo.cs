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

namespace Minimart_Api.Repositories.Search
{
    public class SearchRepo : ISearchRepo
    {
        private readonly MinimartDBContext _dbContext;

        public SearchRepo(MinimartDBContext dbContext)
        {
            _dbContext = dbContext;
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
                    Instock = tp.StockQuantity,
                    price = tp.Price,
                })
                .ToListAsync();
        }


        public async Task<List<CartResults>> GetFilteredProducts(FilteredProductsDTO filteredProducts)
        {
            try
            {
                // Start building the query
                var query = new StringBuilder("SELECT * FROM T_Products WHERE CategoryID = @CategoryID AND SubCategoryID = @SubCategoryID");

                // Base parameters
                var parameters = new List<object>
        {
            new SqlParameter("@CategoryID", filteredProducts.CategoryID),
            new SqlParameter("@SubCategoryID", filteredProducts.SubCategoryID)
        };

                // Add JSON filters for features
                int paramIndex = 0;
                foreach (var feature in filteredProducts.features)
                {
                    string jsonPath = $"$.\"{feature.Key}\"";

                    // Add conditions for each value in the feature
                    foreach (var value in feature.Value)
                    {
                        var parameterName = $"@Param{paramIndex++}";
                        query.Append($" AND JSON_VALUE(KeyFeatures, '{jsonPath}') = {parameterName}");
                        parameters.Add(new SqlParameter(parameterName, value));
                    }
                }

                // Convert query to string
                var finalQuery = query.ToString();

                // Execute the query
                return await _dbContext.Products
                    .FromSqlRaw(finalQuery, parameters.ToArray())
                    .Select(p => new CartResults
                    {
                        productID = p.ProductId,
                        ProductName = p.ProductName,
                        price = p.Price,
                        ProductImage = p.ImageUrl,
                        KeyFeatures = p.KeyFeatures
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.Error.WriteLine($"Error in GetFilteredProducts: {ex.Message}");
                throw;
            }
        }


    }
}
