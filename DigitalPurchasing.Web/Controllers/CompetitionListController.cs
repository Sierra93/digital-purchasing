using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class CompetitionListController : BaseController
    {
        private readonly ICompetitionListService _competitionListService;

        public CompetitionListController(ICompetitionListService competitionListService) => _competitionListService = competitionListService;

        public IActionResult Index()
        {
            var vm = 1;
            return View();
        }
    }
}
