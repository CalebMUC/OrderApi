using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Features;
using Minimart_Api.DTOS.General;
using Newtonsoft.Json;
using Minimart_Api.Models;
namespace Minimart_Api.Repositories.Features
{
    public class FeatureRepo : IFeatureRepo
    {

        private readonly MinimartDBContext _dbContext;
        public FeatureRepo(MinimartDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        //public async Task<ResponseStatus> AddFeatures(int SubCategoryID, List<FeatureDTO> features)
        public async Task<Status> AddFeatures(FeatureDTO features)
        {
            try
            {
                if (features == null)
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Invalid input data"
                    };
                }

                // Simplified query since we know all values are non-null
                var existingFeature = await _dbContext.Features
                    .FirstOrDefaultAsync(f => f.SubCategoryID == features.SubCategoryId
                                          && f.CategoryID == features.CategoryId
                                          && f.FeatureName == features.FeatureName);

                if (existingFeature == null)
                {
                    var newFeature = new Models.Features
                    {
                        FeatureName = features.FeatureName,
                        FeatureOptions = JsonConvert.SerializeObject(features.FeatureOptions),
                        SubCategoryID = features.SubCategoryId,
                        CategoryID = features.CategoryId,
                        SubSubCategoryID = features.SubSubCategoryId // This can be null
                    };

                    await _dbContext.Features.AddAsync(newFeature);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    existingFeature.FeatureOptions = JsonConvert.SerializeObject(features.FeatureOptions);
                    await _dbContext.SaveChangesAsync();
                }

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = existingFeature == null
                        ? "Feature added successfully"
                        : "Feature updated successfully"
                };
            }
            catch (Exception ex)
            {
                // Log the exception here
                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<AddFeaturesDTO>> GetAllFeatures()
        {
            return await _dbContext.Features
                // First join with Categories (for CategoryName)
                .Join(_dbContext.Categories,
                    f => f.CategoryID,
                    c => c.CategoryId,
                    (f, c) => new { Feature = f, Category = c })

                // Left join with Categories again (for SubCategoryName)
                .GroupJoin(_dbContext.Categories,
                    fc => fc.Feature.SubCategoryID,
                    sc => sc.CategoryId,
                    (fc, subCategories) => new { fc.Feature, fc.Category, SubCategories = subCategories })
                .SelectMany(
                    x => x.SubCategories.DefaultIfEmpty(),
                    (fc, sc) => new { fc.Feature, fc.Category, SubCategory = sc })

                // Left join with Categories again (for SubSubCategoryName if needed)
                .GroupJoin(_dbContext.Categories,
                    fcs => fcs.Feature.SubSubCategoryID,
                    ssc => ssc.CategoryId,
                    (fcs, subSubCategories) => new { fcs.Feature, fcs.Category, fcs.SubCategory, SubSubCategories = subSubCategories })
                .SelectMany(
                    x => x.SubSubCategories.DefaultIfEmpty(),
                    (fcs, ssc) => new AddFeaturesDTO
                    {
                        FeatureID = fcs.Feature.FeatureID,
                        FeatureName = fcs.Feature.FeatureName,
                        FeatureOptions = fcs.Feature.FeatureOptions ?? string.Empty,
                        CategoryID = fcs.Feature.CategoryID ?? 0,
                        CategoryName = fcs.Category.CategoryName,
                        SubCategoryID = fcs.Feature.SubCategoryID ?? 0,
                        SubCategoryName = fcs.SubCategory != null ? fcs.SubCategory.CategoryName : null,
                        SubSubCategoryID = fcs.Feature.SubSubCategoryID ?? 0,
                        SubSubCategoryName = ssc != null ? ssc.CategoryName : null
                    })
                .ToListAsync();
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
                                && f.SubCategoryID == feature.SubCategoryID
                                && f.SubSubCategoryID == (feature.SubSubCategoryID == 0 ? null : feature.SubSubCategoryID))
                                .Select(f => new FeatureDTO
                                {
                                    FeatureName = f.FeatureName,
                                    FeatureOptions = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(f.FeatureOptions),
                                    CategoryId = f.CategoryID,
                                    SubCategoryId = f.SubCategoryID,
                                    SubSubCategoryId = f.SubSubCategoryID
                                }).ToListAsync();

            return features;

        }

    }
}
