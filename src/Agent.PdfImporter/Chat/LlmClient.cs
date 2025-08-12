using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Agent.PdfImporter.Chat
{
    public class LlmClient
    {
        private readonly HttpClient _httpClient = new();
        private readonly LlmConfig _config;

        public LlmClient(LlmConfig config)
        {
            _config = config;
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var request = new
            {
                model = _config.Model,
                prompt,
                max_tokens = _config.MaxTokens
            };

            var response = await _httpClient.PostAsJsonAsync(_config.Url, request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}
