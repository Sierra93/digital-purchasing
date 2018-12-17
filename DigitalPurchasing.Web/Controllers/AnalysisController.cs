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
        private readonly IAnalysisService _analysisService;

        public AnalysisController(IAnalysisService analysisService) => _analysisService = analysisService;

        [Route("competitionlist/{clId}/analysis")]
        public IActionResult Index(Guid clId)
        {
            var vm = new AnalysisIndexVm {ClId = clId};
            return View(vm);
        }

        [Route("competitionlist/{clId}/analysisdata")]
        public IActionResult Data(Guid clId) => Json(_analysisService.GetData(clId));

        public IActionResult VariantData(Guid vId) => Json(_analysisService.GetVariantData(vId));

        [HttpPost, Route("competitionlist/{clId}/analysis/add")]
        public IActionResult AddVariant(Guid clId) => Json(_analysisService.AddVariant(clId));

        [HttpPost]
        public IActionResult SaveVariant([FromBody]AnalysisSaveVariant vm)
        {
            _analysisService.SaveVariant(vm);
            return Ok();
        }

        [HttpOptions]
        public IActionResult DeleteVariant([FromBody]Guid id)
        {
            _analysisService.DeleteVariant(id);
            return Ok();
        }
    }
}
