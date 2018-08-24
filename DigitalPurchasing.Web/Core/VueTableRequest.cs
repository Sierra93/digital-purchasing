using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Core
{
    public class VueTableRequest
    {
        [FromQuery(Name="sort")]
        public string Sort { get; set; }

        [FromQuery(Name="page")]
        public int Page { get; set; }

        [FromQuery(Name="per_page")]
        public int PerPage { get; set; }

        public VueTableRequest NextPageRequest()
        {
            var next = this;
            next.Page += 1;
            return next;
        }

        public VueTableRequest PrevPageRequest()
        {
            var prev = this;
            prev.Page -= 1;
            if (prev.Page <= 0) prev.Page = 1;
            return prev;
        }

        public string SortField
        {
            get
            {
                if (string.IsNullOrEmpty(Sort)) return null;
                return Sort.Split('|')[0];
            }
        }

        public bool SortAsc
        {
            get
            {
                if (string.IsNullOrEmpty(Sort)) return true;
                return Sort.Split('|')[1] == "asc";
            }
        }
    }
}
