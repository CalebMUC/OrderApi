using System.ComponentModel.DataAnnotations.Schema;
using Minimart_Api.TempModels;
using OpenSearch.Client;

public class TProduct
{
    public int merchantID { get; set; }
    public string? ProductName { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public int RowId { get; set; }
    public string ProductId { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string ImageType { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public int InStock { get; set; }
    public double Discount { get; set; }
    public string SearchKeyWord { get; set; } = null!;
    public string KeyFeatures { get; set; } = null!;
    public string Specification { get; set; } = null!;
    public string Box { get; set; } = null!;
    public string? SubCategoryId { get; set; }

    public string? SubCategoryName { get; set; }

    public string? ProductType { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public int IsSaved { get; set; }

    // Add this property for autocomplete
    [NotMapped]
    public CompletionField Suggest { get; set; }

    public virtual TCategory? CategoryNavigation { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
    public virtual ICollection<TOrderItem> TOrderItems { get; set; } = new HashSet<TOrderItem>();
    public virtual ICollection<TReview> TReviews { get; set; } = new HashSet<TReview>();
}