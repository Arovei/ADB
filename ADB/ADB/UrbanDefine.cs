using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace ADB
{
    public class Word
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Example { get; set; }
    }

    class UrbanDefine
    {

        private const string apiBaseUrl = "http://api.urbandictionary.com";
        private const string apiQueryPath = "/v0/define?";
        private const string apiTermPath = "term=";
        private const string apiDefIdPath = "defid=";

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            Console.WriteLine("HTTPClient: REQUEST HTTP ADDR: " + apiBaseUrl);
            return client;
        }

        private async Task<T> ExecuteAsync<T>(string queryStr)
        {
            string CallUrl = apiBaseUrl + apiQueryPath + queryStr;

            Console.WriteLine("ExecuteAsync: REQUEST HTTP ADDR: " + CallUrl);

            using (var client = GetHttpClient())
            {
                var json = await client.GetStringAsync(CallUrl);
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }

        public async Task<UrbanDefineObj> QueryByTerm(string queryStr)
        {
            var queryContent = string.Format(apiTermPath + Uri.EscapeDataString(queryStr));
            var result = await this.ExecuteAsync<UrbanDefineObj>(queryContent);
            return result;
        }

        public async Task<UrbanDefineObj> QueryById(string queryStr)
        {
            var queryContent = string.Format(apiDefIdPath + Uri.EscapeDataString(queryStr));
            var result = await this.ExecuteAsync<UrbanDefineObj>(queryContent);
            return result;
        }

    }
}