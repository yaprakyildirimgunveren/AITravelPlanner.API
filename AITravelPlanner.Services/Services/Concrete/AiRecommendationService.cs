using System.Net.Http.Json;
using AITravelPlanner.Services.Models;
using AITravelPlanner.Services.Options;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AITravelPlanner.Services.Services.Concrete
{
    public class AiRecommendationService : IRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiRecommendationService> _logger;

        public AiRecommendationService(
            HttpClient httpClient,
            IOptions<AiServiceOptions> options,
            ILogger<AiRecommendationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var baseUrl = options.Value.BaseUrl;
            if (!string.IsNullOrWhiteSpace(baseUrl) && _httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<RecommendationResponse> GetRecommendationsAsync(
            RecommendationRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/recommendations",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("AI service returned {StatusCode}", response.StatusCode);
                return new RecommendationResponse();
            }

            var result = await response.Content.ReadFromJsonAsync<RecommendationResponse>(
                cancellationToken: cancellationToken);

            return result ?? new RecommendationResponse();
        }
    }
}
