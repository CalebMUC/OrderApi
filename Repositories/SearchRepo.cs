using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;

namespace Minimart_Api.Repositories
{
    public class SearchRepo : ISearchRepo
    {
        private readonly MinimartDBContext _dbContext;

        public SearchRepo(MinimartDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseStatus> UpdateColumnJson()
        {
            try
            {
                var rows = _dbContext.TProducts.ToList();
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
                                return new ResponseStatus
                                {
                                    ResponseStatusId = 500,
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
                return new ResponseStatus
                {
                    ResponseStatusId = 500,
                    ResponseMessage = $"Error saving changes to the database: {ex.Message}"
                };
            }

            return new ResponseStatus
            {
                ResponseStatusId = 200,
                ResponseMessage = "Success"
            };
        }

        public string ConvertToJson(string data) {

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
        public async Task<IEnumerable<TSubcategoryid>> GetSearchResults(string queryname)
        {

            if (string.IsNullOrWhiteSpace(queryname))
                return new List<TSubcategoryid>();

            var response = await _dbContext.TSubcategoryids
                .Where(s => s.ProductName.Contains(queryname))
                .OrderBy(s => s.ProductName)
                .Take(10)
                .ToListAsync();

            return response;

        }


    }
}
