using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Features;
using Minimart_Api.DTOS.General;
using Newtonsoft.Json;
using Minimart_Api.Models;
namespace Minimart_Api.Repositories.Features
{
    public class FeatureRepo:IFeatureRepo
    {

        private readonly MinimartDBContext _dbContext;
        public FeatureRepo(MinimartDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        //public async Task<ResponseStatus> AddFeatures(int SubCategoryID, List<FeatureDTO> features)
        public async Task<Status> AddFeatures(AddFeaturesDTO addFeatures)
        {
            // Validate input
            if (addFeatures == null || addFeatures.Features == null || !addFeatures.Features.Any())
            {
                return new Status
                {
                    ResponseCode = 400,
                    ResponseMessage = "Invalid input data"
                };
            }

            foreach (var feature in addFeatures.Features)
            {
                // Check if the feature already exists in the Features table
                var existingFeature = await _dbContext.Features
                    .FirstOrDefaultAsync(f => f.SubCategoryID == addFeatures.SubCategoryID
                                            && f.CategoryID == addFeatures.CategoryID
                                            && f.FeatureName == feature.FeatureName);

                if (existingFeature == null)
                {
                    // Feature does not exist; add it to the Features table
                    var newFeature = new Models.Features
                    {
                        FeatureName = feature.FeatureName,
                        FeatureOptions = JsonConvert.SerializeObject(feature.FeatureOptions),
                        SubCategoryID = addFeatures.SubCategoryID, // Include SubCategoryID
                        CategoryID = addFeatures.CategoryID       // Include CategoryID
                    };

                    await _dbContext.Features.AddAsync(newFeature);
                    await _dbContext.SaveChangesAsync(); // Save to get the FeatureID assigned

                    existingFeature = newFeature; // Update the reference to the newly added feature
                }
                else
                {
                    // Update existing feature if it already exists
                    existingFeature.FeatureOptions = JsonConvert.SerializeObject(feature.FeatureOptions);
                    await _dbContext.SaveChangesAsync();
                }

                //// Ensure FeatureID is available
                //if (existingFeature?.FeatureID != null)
                //{
                //    // Check if the link between SubCategory and Feature already exists
                //    var subcategoryFeatureExists = await _dbContext.SubCategoryFeatures
                //        .AnyAsync(sf => sf.SubCategoryId == addFeatures.SubCategoryID
                //                     && sf.FeatureID == existingFeature.FeatureID);

                //    if (!subcategoryFeatureExists)
                //    {
                //        // Add a new link between SubCategory and Feature
                //        var subcategoryFeature = new SubCategoryFeatures
                //        {
                //            SubCategoryId = addFeatures.SubCategoryID,
                //            FeatureID = existingFeature.FeatureID
                //        };

                //        await _dbContext.SubCategoryFeatures.AddAsync(subcategoryFeature);
                //    }
                //}
                //else
                //{
                //    // Log or handle the error if FeatureID is unexpectedly null
                //    Console.WriteLine($"FeatureID for feature '{feature.FeatureName}' could not be determined.");
                //}
            }

            // Save all changes to the SubCategoryFeatures at once
            await _dbContext.SaveChangesAsync();

            return new Status
            {
                ResponseCode = 200,
                ResponseMessage = "Features Added Successfully"
            };
        }
        //Get Features Linked to a SubCategory
        public async Task<List<FeatureDTO>> GetFeatures(FeatureRequestDTO feature)
        {

            //var features = await _dbContext.SubCategoryFeatures
            //                    .Where(f => f.SubCategoryId == feature.CategoryID)
            //                    .Select(f => new FeatureDTO
            //                    {
            //                        FeatureName = f.features.FeatureName,
            //                        FeatureOptions = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(f.features.FeatureOptions)
            //                    }).ToListAsync();

            var features = await _dbContext.Features
                                .Where(f => f.CategoryID == feature.CategoryID
                                && f.SubCategoryID == feature.SubCategoryID)
                                .Select(f => new FeatureDTO
                                {
                                    FeatureName = f.FeatureName,
                                    FeatureOptions = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(f.FeatureOptions)
                                }).ToListAsync();

            return features;

        }

    }
}
