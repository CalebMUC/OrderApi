using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Minimart_Api.Models;
using OpenSearch.Client;

public class Products
{
    [Required]
    public int MerchantID { get; set; }

    [MaxLength(255)]
    [Column(TypeName = "nvarchar(255)")]
    public string? ProductName { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    public int StockQuantity { get; set; }

    //[ForeignKey("Categories")]
    public int? CategoryId { get; set; }

    
    //public int RowId { get; set; } // Assuming this is the primary key

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    [Key]
    public string ProductId { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string ProductDescription { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string CategoryName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string ImageType { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(Max)")]
    public string ImageUrl { get; set; } = null!;

    public bool InStock { get; set; }

    [Column(TypeName = "float")]
    public double Discount { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(500)")]
    public string SearchKeyWord { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string KeyFeatures { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Specification { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Box { get; set; } = null!;

    public int? ParentCategoryId { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string? ParentCategoryName { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string? ProductType { get; set; }

    public DateTime? CreatedOn { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string? UpdatedBy { get; set; }

    public bool IsSaved { get; set; }

    // Not mapped to DB - used for OpenSearch autocomplete
    [NotMapped]
    public CompletionField Suggest { get; set; }

    // Navigation properties
    public ICollection<OrderTracking> OrderTrackings { get; set; }

    public virtual Categories Categories { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } 
    public virtual ICollection<OrderItem> OrderItems { get; set; } //Foreign Key OrderItem
    public virtual ICollection<Reviews> Reviews { get; set; } //Foreign Key Reviews
}
