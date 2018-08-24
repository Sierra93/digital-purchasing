using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Core
{
    public class VueTableResponse<TData> where TData: class
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("last_page")]
        public int LastPage { get; set; }

        [JsonProperty("next_page_url")]
        public string NextPageUrl { get; set; }

        [JsonProperty("prev_page_url")]
        public string PrevPageUrl { get; set; }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("data")]
        public List<TData> Data { get; set; }

        public VueTableResponse(List<TData> data, VueTableRequest request, int total, string nextPageUrl, string prevPageUrl)
        {
            Data = data;
            CurrentPage = request.Page;
            PerPage = request.PerPage;
            Total = total;

            var lastPage = (total + request.PerPage - 1) / request.PerPage;
            var from = ( request.Page - 1 ) * request.PerPage + 1;
            var to = from + request.PerPage;
            if (to > total)
            {
                to = total;
            }

            LastPage = lastPage;
            From = from;
            To = to;

            if (request.Page > 1)
            {
                PrevPageUrl = prevPageUrl;
            }

            if (request.Page < lastPage)
            {
                NextPageUrl = nextPageUrl;
            }
        }
    }
}
