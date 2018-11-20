using System;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.ViewModels.Uom
{
    public class UomEditVm : UomCreateVm
    {
        [HiddenInput]
        public Guid Id { get; set; }
    }
}
