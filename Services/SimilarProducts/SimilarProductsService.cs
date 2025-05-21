using Minimart_Api.DTOS.Products;
using Minimart_Api.Repositories.ProductRepository;

namespace Minimart_Api.Services.SimilarProducts
{
    public class SimilarProductsService : ISimilarProductsService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<SimilarProductsService> _logger;

        public SimilarProductsService(
            IProductRepository productRepository,
            ILogger<SimilarProductsService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<SimilarProductDto>> GetSimilarProductsAsync(string productId, int limit = 5)
        {
            try
            {
                var targetProduct = await _productRepository.GetByIdAsync(productId);
                if (targetProduct == null)
                {
                    _logger.LogWarning("Product {ProductId} not found", productId);
                    return Enumerable.Empty<SimilarProductDto>();
                }

                var similarProducts = await FindSimilarProducts(targetProduct, limit);
                var filteredProducts = FilterBySimilarityScore(similarProducts, targetProduct, 50);
                return filteredProducts.Take(limit);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar products for {ProductId}", productId);
                throw;
            }
        }

        private IEnumerable<SimilarProductDto> FilterBySimilarityScore(
        IEnumerable<Products> products,
        Products targetProduct,
        int minScore)
        {
            return products
                .Select(p => new SimilarProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price ?? 0,
                    Discount = p.Discount,
                    InStock = p.InStock,
                    CategoryName = p.CategoryName,
                    ProductDescription = p.ProductDescription,
                    SimilarityScore = CalculateSimilarityScore(targetProduct, p)
                })
                .Where(p => p.SimilarityScore >= minScore)
                .OrderByDescending(p => p.SimilarityScore);
        }

        private async Task<IEnumerable<Products>> FindSimilarProducts(Products targetProduct, int limit)
        {
            var results = new List<Products>();

            // 1. Same category products
            if (targetProduct.CategoryId.HasValue)
            {
                var sameCategoryProducts = await _productRepository
                    .GetProductsByCategoryAsync(targetProduct.CategoryId.Value, limit, targetProduct.ProductId);
                results.AddRange(sameCategoryProducts);
            }

            // 2. Same sub-category products if we need more
            if (results.Count < limit && targetProduct.SubCategoryId.HasValue)
            {
                var sameSubCategoryProducts = await _productRepository
                    .GetProductsBySubCategoryAsync(targetProduct.SubCategoryId.Value,
                                                 limit - results.Count,
                                                 targetProduct.ProductId);
                results.AddRange(sameSubCategoryProducts);
            }

            // 3. Keyword matching if we still need more
            if (results.Count < limit)
            {
                var keywords = targetProduct.SearchKeyWord?.Split(' ', ',', ';') ??
                             targetProduct.ProductName?.Split(' ') ?? Array.Empty<string>();
                if (keywords.Any())
                {
                    var keywordProducts = await _productRepository
                        .GetProductsByKeywordsAsync(keywords, limit - results.Count, targetProduct.ProductId);
                    results.AddRange(keywordProducts);
                }
            }

            // 4. Popular products as fallback
            if (results.Count < limit)
            {
                var popularProducts = await _productRepository
                    .GetPopularProductsAsync(limit - results.Count, targetProduct.ProductId);
                results.AddRange(popularProducts);
            }

            return results.Distinct().Take(limit);
        }

        private IEnumerable<SimilarProductDto> MapToDto(IEnumerable<Products> products, Products targetProduct)
        {
            return products.Select(p => new SimilarProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ImageUrl = p.ImageUrl,
                Price = p.Price ?? 0,
                Discount = p.Discount,
                InStock = p.InStock,
                CategoryName = p.CategoryName,
                ProductDescription = p.ProductDescription,
                SimilarityScore = CalculateSimilarityScore(targetProduct, p)
            });
        }

        private double CalculateSimilarityScore(Products product1, Products product2)
        {
            double score = 0;

            // Category match (50% weight)
            if (product1.CategoryId == product2.CategoryId) score += 0.5;
            // Sub-category match (30% weight)
            else if (product1.SubCategoryId == product2.SubCategoryId) score += 0.3;

            // Price similarity (20% weight)
            if (product1.Price.HasValue && product2.Price.HasValue)
            {
                var priceDiff = Math.Abs((double)(product1.Price.Value - product2.Price.Value));
                var maxPrice = Math.Max((double)product1.Price.Value, (double)product2.Price.Value);
                score += 0.2 * (1 - Math.Min(priceDiff / maxPrice, 1));
            }

            return Math.Round(score * 100, 2);
        }
    }
}
