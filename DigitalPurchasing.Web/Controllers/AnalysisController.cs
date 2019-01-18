using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels.Analysis;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class AnalysisController : Controller
    {
        public class DeleteVariantVm
        {
            public Guid Id { get; set; }
        }

        private readonly IAnalysisService _analysisService;

        public AnalysisController(IAnalysisService analysisService) => _analysisService = analysisService;

        [Route("competitionlist/{clId}/analysis")]
        public IActionResult Index(Guid clId)
        {
            var vm = new AnalysisIndexVm {ClId = clId};
            return View(vm);
        }

        [Route("competitionlist/{clId}/analysis/details")]
        public IActionResult Details(Guid clId)
        {
            var data = _analysisService.GetDetails(clId);
            var vm = new AnalysisDetailsVm { ClId = clId, Data = data };
            return View(vm);
        }

        [Route("competitionlist/{clId}/analysisdata")]
        public IActionResult Data(Guid clId) => Json(_analysisService.GetData(clId));

        public IActionResult DefaultData() => Json(_analysisService.GetDefaultData());

        public IActionResult VariantData(Guid vId) => Json(_analysisService.GetVariantData(vId));

        [HttpPost, Route("competitionlist/{clId}/analysis/add")]
        public IActionResult AddVariant(Guid clId) => Json(_analysisService.AddVariant(clId));

        [HttpPost]
        public IActionResult AddDefaultVariant() => Json(_analysisService.AddDefaultVariant());

        [HttpPost]
        public IActionResult SaveVariant([FromBody]AnalysisSaveVariant vm)
        {
            _analysisService.SaveVariant(vm);
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteVariant([FromBody]DeleteVariantVm vm)
        {
            _analysisService.DeleteVariant(vm.Id);
            return Ok();
        }
    }
}
