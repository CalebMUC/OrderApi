using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Repositories.Order;
using Minimart_Api.Repositories.ProductRepository;
using Minimart_Api.Repositories.Recommendation;
using Org.BouncyCastle.Utilities.Collections;
using StackExchange.Redis;

namespace Minimart_Api.Services.Recommedation
{
    
    public class RecommendationService : IRecomedationService
    {
        private readonly IorderRepository _orderRepository;

        private readonly IProductRepository _productRepository;

        private readonly IRecommendationRepository _recommendationRepository;
        public RecommendationService(IorderRepository iorderRepository, IProductRepository productRepository,IRecommendationRepository recommendationRepository) {
            _orderRepository = iorderRepository;
            _productRepository = productRepository;
            _recommendationRepository = recommendationRepository;
        }

        // RecommendationService.cs
        public async Task<IEnumerable<SavedProductsDto>> GetPersonalizedRecommendations(int userId, int limit = 5)
        {
            // 1. Get user's order history
            var userOrders = await _recommendationRepository.GetUserOrders(userId);

            // 2. Extract frequently purchased categories
            var topCategories = userOrders
                .SelectMany(o => o.OrderProducts)
                .GroupBy(op => op.Product.CategoryId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key);

            // 3. Get popular products in those categories
            var recommendations = new List<Products>();
            foreach (var categoryId in topCategories)
            {
                var categoryProducts = await _recommendationRepository
                    .GetPopularProductsByCategory(categoryId, limit);
                recommendations.AddRange(categoryProducts);
            }

            // 4. If not enough, add general popular products
            if (recommendations.Count < limit)
            {
                var popularProducts = await _recommendationRepository
                    .GetPopularProducts(limit - recommendations.Count);
                recommendations.AddRange(popularProducts);
            }

            return MapToDto(recommendations.Take(limit));
        }

        //Algorithm: Use order history to find products frequently purchased together

        // RecommendationService.cs
        public async Task<IEnumerable<SavedProductsDto>> GetComplementaryProducts(string productId, int limit = 5)
        {
            // 1. Get all orders containing this product
            var ordersWithProduct = await _recommendationRepository.GetOrdersContainingProduct(productId);

            // 2. Find products frequently bought with it
            var complementaryProducts = ordersWithProduct
                .SelectMany(o => o.OrderProducts)
                .Where(op => op.ProductID!= productId)
                .GroupBy(op => op.ProductID)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.First().Product);

            // 3. If not enough, use category-based fallback
            if (complementaryProducts.Count() < limit)
            {
                var product = await _productRepository.GetByIdAsync(productId);
                var sameCategory = await _recommendationRepository
                    .GetProductsByCategory(product.CategoryId, limit - complementaryProducts.Count());
                complementaryProducts = complementaryProducts.Concat(sameCategory);
            }

            return MapToDto(complementaryProducts.Take(limit));
        }

        //Algorithm: Find products frequently purchased by customers who bought this product
        // RecommendationService.cs
        public async Task<IEnumerable<SavedProductsDto>> GetFrequentlyBoughtTogether(string productId, int limit = 5)
        {
            // 1. Get all users who bought this product
            var userIds = (await _recommendationRepository.GetOrdersContainingProduct(productId))
                .Select(o => o.UserID)
                .Distinct();

            // 2. Get other products these users bought
            var otherProducts = new List<Products>();
            foreach (var userId in userIds)
            {
                var userOrders = await _recommendationRepository.GetUserOrders(userId);
                var products = userOrders
                    .SelectMany(o => o.OrderProducts)
                    .Where(op => op.ProductID != productId)
                    .Select(op => op.Product);
                otherProducts.AddRange(products);
            }

            // 3. Rank by frequency
            var recommendations = otherProducts
                .GroupBy(p => p.ProductId)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.First());

            return MapToDto(recommendations);
        }

        private IEnumerable<SavedProductsDto> MapToDto(IEnumerable<Products> products)
        {
            return products.Select(p => new SavedProductsDto
            {
                ProductID = p.ProductId,
                ProductName = p.ProductName,
                ProductImage = p.ImageUrl,
                Price = p.Price ?? 0,
                Discount = p.Discount,
                InStock = p.InStock,
                CategoryName = p.CategoryName,
                // Add any other relevant fields
            });
        }






    }
}
