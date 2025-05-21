namespace Minimart_Api.Repositories.Recommendation
{
    public interface IRecommendationRepository
    {
        Task<IEnumerable<Models.Orders>> GetUserOrders(int userId);
        Task<IEnumerable<Products>> GetPopularProductsByCategory(int? categoryId, int limit);
        Task<IEnumerable<Products>> GetPopularProducts(int limit);
        Task<IEnumerable<Models.Orders>> GetOrdersContainingProduct(string productId);
        Task<IEnumerable<Products>> GetProductsByCategory(int? categoryId, int limit);
    }
}
