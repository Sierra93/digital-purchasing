using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Web.ViewModels
{
    public class DeleteVm
    {
        public Guid Id { get; set; }
    }

    public class MultipleDeleteVm
    {
        public List<Guid> Ids { get; set; }
    }
}
