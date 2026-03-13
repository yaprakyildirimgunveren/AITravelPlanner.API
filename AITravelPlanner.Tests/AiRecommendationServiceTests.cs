using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using AITravelPlanner.Services.Models;
using AITravelPlanner.Services.Options;
using AITravelPlanner.Services.Services.Concrete;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AITravelPlanner.Tests
{
    public class AiRecommendationServiceTests
    {
        [Fact]
        public async Task GetRecommendations_ReturnsResponse_WhenServiceIsHealthy()
        {
            var handler = new FakeHttpMessageHandler((_, __) =>
            {
                var payload = new RecommendationResponse
                {
                    Recommendations = new List<RecommendationItem>
                    {
                        new RecommendationItem
                        {
                            Destination = "Athens",
                            Reason = "Test",
                            EstimatedCost = 250,
                            Activities = new List<string> { "Walk" }
                        }
                    }
                };

                var content = JsonSerializer.Serialize(payload);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };
            });

            var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:8000") };
            var options = Options.Create(new AiServiceOptions { BaseUrl = "http://localhost:8000" });
            var service = new AiRecommendationService(client, options, NullLogger<AiRecommendationService>.Instance);

            var result = await service.GetRecommendationsAsync(new RecommendationRequest
            {
                UserId = 1,
                From = "IST",
                To = "ATH",
                TravelDate = DateTime.UtcNow.Date.AddDays(1),
                Budget = 500,
                Travelers = 1
            });

            Assert.Single(result.Recommendations);
            Assert.Equal("Athens", result.Recommendations[0].Destination);
        }

        private sealed class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _handler;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(_handler(request, cancellationToken));
            }
        }
    }
}
