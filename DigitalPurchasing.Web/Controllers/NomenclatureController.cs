using System.Collections.Generic;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Controllers
{
    public class BaseController : Controller
    {
        //protected IActionResult VueTableData<TData>(List<TData> data, VueTableRequest request)
        //{
        //    return Json(new VueTableResponse<TData>(data, request));
        //}
    }

    public class NomenclatureController : BaseController
    {
        private readonly INomenclatureService _nomenclatureService;

        public NomenclatureController(INomenclatureService nomenclatureService) => _nomenclatureService = nomenclatureService;

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _nomenclatureService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "Nomenclature", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Nomenclature", request.PrevPageRequest(), Request.Scheme);
            return Json(new VueTableResponse<NomenclatureResult>(result.Data, request, result.Total, nextUrl, prevUrl));
        }
    }
}
