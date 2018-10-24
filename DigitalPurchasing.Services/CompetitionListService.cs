using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;

namespace DigitalPurchasing.Services
{
    public class CompetitionListService : ICompetitionListService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICounterService _counterService;

        public CompetitionListService(ApplicationDbContext db, ICounterService counterService)
        {
            _db = db;
            _counterService = counterService;
        }
    }
}
