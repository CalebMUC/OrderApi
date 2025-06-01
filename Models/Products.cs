using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Minimart_Api.Models;
using OpenSearch.Client;

public class Products
{
    [Required]
    public int MerchantID { get; set; }

    [MaxLength(255)]
    [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
    public string? ProductName { get; set; }

    [Column(TypeName = "text")]  // Changed from nvarchar(max)
    public string? Description { get; set; }

    [Column(TypeName = "numeric(18,2)")]  // Changed from decimal
    public decimal? Price { get; set; }

    public int StockQuantity { get; set; }

    public int? CategoryId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
    [Key]
    public string ProductId { get; set; } = null!;

    [Required]
    [Column(TypeName = "text")]  // Changed from nvarchar(max)
    public string ProductDescription { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
    public string CategoryName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
    public string ImageType { get; set; } = null!;

    [Required]
    [Column(TypeName = "text")]  // Changed from nvarchar(Max)
    public string ImageUrl { get; set; } = "[]";

    [NotMapped]
    public string[] ImageUrlJson
    {
        get => JsonSerializer.Deserialize<string[]>(ImageUrl) ?? Array.Empty<string>();
        set => ImageUrl = JsonSerializer.Serialize(value);
    }

    public bool InStock { get; set; }

    [Column(TypeName = "double precision")]  // Changed from float
    public double Discount { get; set; }

    [Required]
    [Column(TypeName = "varchar(500)")]  // Changed from nvarchar
    public string SearchKeyWord { get; set; } = null!;

    [Required]
    [Column(TypeName = "text")]  // Changed from nvarchar(max)
    public string KeyFeatures { get; set; } = null!;

    [Required]
    [Column(TypeName = "text")]  // Changed from nvarchar(max)
    public string Specification { get; set; } = null!;

    [Required]
    [Column(TypeName = "text")]  // Changed from nvarchar(max)
    public string Box { get; set; } = null!;

    public int? SubCategoryId { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
    public string? SubCategoryName { get; set; }

    public int? SubSubCategoryId { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
    public string? SubSubCategoryName { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
    public string? ProductType { get; set; }

    [Column(TypeName = "timestamp")]  // Changed from datetime
    public DateTime? CreatedOn { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
    public string? CreatedBy { get; set; }

    [Column(TypeName = "timestamp")]  // Changed from datetime
    public DateTime? UpdatedOn { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
    public string? UpdatedBy { get; set; }

    public bool IsSaved { get; set; }

    [NotMapped]
    public CompletionField Suggest { get; set; }

    // Navigation properties remain unchanged
    public ICollection<OrderTracking> OrderTrackings { get; set; }
    public virtual Categories Categories { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual ICollection<Reviews> Reviews { get; set; }
}