using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.DTOS.Products
{
    public class GetProductsDto
    {
        public int MerchantID { get; set; }

        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int StockQuantity { get; set; }

        public int? CategoryId { get; set; }



        public string ProductId { get; set; } = null!;

 
        public string ProductDescription { get; set; } = null!;
        public string CategoryName { get; set; } = null;

        public string ImageUrl { get; set; } = "[]";

        //[NotMapped]
        //public string[] ImageUrlJson
        //{
        //    get => JsonSerializer.Deserialize<string[]>(ImageUrl) ?? Array.Empty<string>();
        //    set => ImageUrl = JsonSerializer.Serialize(value);
        //}

        public bool InStock { get; set; }

        public double Discount { get; set; }

        public string SearchKeyWord { get; set; } = null!;

        public string KeyFeatures { get; set; } = null!;

        public string Specification { get; set; } = null!;

        public string Box { get; set; } = null!;

        public int? SubCategoryId { get; set; }

        public string? SubCategoryName { get; set; }


        public string? SubSubCategoryName { get; set; }

        public string? ProductType { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
