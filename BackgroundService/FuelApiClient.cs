using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

using Newtonsoft.Json.Linq;
using RestSharp;

namespace BackgroundService
{
    public sealed class FuelApiClient
    {
        public FuelApiClient(string url)
        {
            if(String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            Client = new RestClient(url);
        }

        private RestClient Client { get; }

        public ICollection<FuelPriceDto> GetPrices(int? daysCount = null)
        {
            const string SeriesNode = "series";
            const string DataNode = "data";
            const string DateTimeFormat = "yyyyMMdd";

            var request = new RestRequest();
            var response = Client.Execute(request);
            if(!response.IsSuccessful)
            {
                throw new FluentApiException(response.ErrorMessage ?? response.StatusDescription, response.StatusCode);
            }

            try
            {
                var releases = JObject.Parse(response.Content);
                var data = releases[SeriesNode][0][DataNode];

                var fromDate = daysCount != null
                    ? DateTime.UtcNow.Date.AddDays(-daysCount.Value)
                    : default(DateTime?);

                return (
                    from item in data
                    let date = DateTime.ParseExact(item[0].ToString(), DateTimeFormat, CultureInfo.InvariantCulture)
                    where fromDate == null || date >= fromDate
                    let price = double.Parse(item[1].ToString(), CultureInfo.InvariantCulture)
                    select new FuelPriceDto(date, price)
                ).ToList();
            }
            catch(Exception ex)
            {
                throw new FluentApiException("Can't extract fuel prices from response.", response.StatusCode, ex);
            }
        }

        public sealed class FluentApiException : Exception
        {
            public FluentApiException(string message, HttpStatusCode statusCode) : base(message)
            {
                StatusCode = statusCode;
            }

            public FluentApiException(string message, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
            {
                StatusCode = statusCode;
            }

            public HttpStatusCode StatusCode { get; }
        }
    }
}
