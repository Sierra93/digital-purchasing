using System;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Core
{
    public class VueTableRequestWithId : VueTableRequest<VueTableRequestWithId>
    {
        [FromRoute(Name="id")]
        public Guid Id { get; set; }
    }

    public class VueTableRequest : VueTableRequest<VueTableRequest>
    {
    }

    public class VueTableRequest<T> where T: VueTableRequest<T>
    {
        [FromQuery(Name="sort")]
        public string Sort { get; set; }

        [FromQuery(Name="page")]
        public int Page { get; set; }

        [FromQuery(Name="per_page")]
        public int PerPage { get; set; }

        [FromQuery(Name="s")]
        public string Search { get; set; }

        public T NextPageRequest()
        {
            var next = this;
            next.Page += 1;
            return (T) next;
        }

        public T PrevPageRequest()
        {
            var prev = this;
            prev.Page -= 1;
            if (prev.Page <= 0) prev.Page = 1;
            return (T) prev;
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
