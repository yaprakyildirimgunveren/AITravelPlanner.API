using AITravelPlanner.Services.Models;

namespace AITravelPlanner.Services.Services.Abstract
{
    public interface IRecommendationService
    {
        Task<RecommendationResponse> GetRecommendationsAsync(
            RecommendationRequest request,
            CancellationToken cancellationToken = default);
    }
}
