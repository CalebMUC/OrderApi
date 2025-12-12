using AutoMapper;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;

namespace Minimart_Api.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between Product models and DTOs
    /// </summary>
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Product -> ProductResponseDto
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Merchant, opt => opt.MapFrom(src => src.Merchant))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.SubCategory, opt => opt.MapFrom(src => src.SubCategory))
                .ForMember(dest => dest.SubSubCategory, opt => opt.MapFrom(src => src.SubSubCategory));

            // Product -> ProductListDto
            CreateMap<Product, ProductListDto>();

            // Product -> ProductSummaryDto
            CreateMap<Product, ProductSummaryDto>();

            // CreateProductDto -> Product
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.MerchantID, opt => opt.MapFrom(src => src.MerchantID))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategory, opt => opt.Ignore())
                .ForMember(dest => dest.SubSubCategory, opt => opt.Ignore());

            // UpdateProductDto -> Product
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Merchant, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategory, opt => opt.Ignore())
                .ForMember(dest => dest.SubSubCategory, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.MerchantID, opt => opt.Ignore()); // Only if you don't allow changing MerchantID


            // Merchant -> ProductMerchantDto
            CreateMap<Merchants, ProductMerchantDto>()
                .ForMember(dest => dest.MerchantId, opt => opt.MapFrom(src => src.MerchantID))
                .ForMember(dest => dest.MerchantName, opt => opt.MapFrom(src => src.MerchantName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // Category -> ProductCategoryDto
            CreateMap<Category, ProductCategoryDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

            // SubCategory -> ProductSubCategoryDto
            CreateMap<SubCategory, ProductSubCategoryDto>()
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.SubCategoryId));

            // SubSubCategory -> ProductSubSubCategoryDto
            CreateMap<SubSubCategory, ProductSubSubCategoryDto>()
                .ForMember(dest => dest.SubSubCategoryId, opt => opt.MapFrom(src => src.SubSubCategoryId));
        }
    }
}