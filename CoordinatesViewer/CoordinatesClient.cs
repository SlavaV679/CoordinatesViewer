using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CoordinatesViewer
{
    public class CoordinatesClient
    {
        private HttpClient _client;

        public CoordinatesClient(HttpClient client)
        {
            _client = client;
        }

        public async Task DeleteCoordinates()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Coordinates/delete");

            SetHeadersAsync(request);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<string>> GetAllCoordinates()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Coordinates/allcoordinates");
            
            SetHeadersAsync(request);           

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<string>>(resultString);

            return result;
        }


        public async Task<List<string>> GetCoordinates(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetCoordinatesTimeUri(from, to));
            
            SetHeadersAsync(request);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<string>>(resultString);

            return result;
        }

        public async Task<List<string>> GetCoordinates(string eventMouse)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Coordinates/coordinatesevent?eventMouse={eventMouse}");

            SetHeadersAsync(request);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var resultString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<string>>(resultString);

            return result;
        }


        public async Task InsertCoordinates(string x, string y, string eventMouse)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Coordinates/insert");
            
            SetHeadersAsync(request);

            var content = JsonConvert.SerializeObject(new Coordinate { X = x, Y = y, EventMouse = eventMouse });

            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        #region helperMethods

        private void SetHeadersAsync(HttpRequestMessage request)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        }

        private string GetCoordinatesTimeUri(DateTime from, DateTime to)
        {
            var dateTimeFromString = from.ToString("MM / dd / yyyy HH: mm: ss");

            var dateTimeToString = to.ToString("MM / dd / yyyy HH: mm: ss");

            return
                $"api/Coordinates/coordinatestime?dateTimeFrom={dateTimeFromString}&dateTimeTo={dateTimeToString}";
        }

        #endregion
    }
}
