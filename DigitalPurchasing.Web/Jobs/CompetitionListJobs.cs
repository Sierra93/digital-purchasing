using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Web.Jobs
{
    public class CompetitionListJobs
    {
        private readonly ICompetitionListService _competitionListService;

        public CompetitionListJobs(ICompetitionListService competitionListService)
            => _competitionListService = competitionListService;

        public async Task Close(Guid competitionListId)
            => await _competitionListService.Close(competitionListId);

        public async Task CloseExpired()
            => await _competitionListService.CloseExpired();
    }


}
