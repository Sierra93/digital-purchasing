using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Mapster;

namespace DigitalPurchasing.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        private readonly IPurchaseRequestService _purchaseRequestService;
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly ICompetitionListService _competitionListService;
        private readonly IFileService _fileService;

        public AdminService(
            ICompanyService companyService,
            IUserService userService,
            IPurchaseRequestService purchaseRequestService,
            IQuotationRequestService quotationRequestService,
            ICompetitionListService competitionListService,
            IFileService fileService)
        {
            _companyService = companyService;
            _userService = userService;
            _purchaseRequestService = purchaseRequestService;
            _quotationRequestService = quotationRequestService;
            _competitionListService = competitionListService;
            _fileService = fileService;
        }

        public async Task<IEnumerable<AdminCompanyDto>> GetCompanies()
        {
            var companies = await _companyService.GetAll();
            var adminCompanies = companies.Adapt<List<AdminCompanyDto>>();
            foreach (var company in adminCompanies)
            {
                company.PRCount = await _purchaseRequestService.CountByCompany(company.Id);
                company.QRCount = await _quotationRequestService.CountByCompany(company.Id);
                company.CLCount = await _competitionListService.CountByCompany(company.Id);
                company.Owner = (await _userService.GetCompanyOwner(company.Id))
                    .Adapt<AdminCompanyDto.OwnerData>();
                company.UsersCount = await _userService.TotalCountByCompany(company.Id);
            }
            return adminCompanies;
        }

        public async Task<AdminDashboardDto> GetDashboard()
        {
            var dto = new AdminDashboardDto
            {
                CompaniesCount = await _companyService.Count(),
                UsersCount = await _userService.TotalCount(),
                ConfirmedUsersCount = await _userService.ConfirmedEmailCount(),
                TermsUploaded = _fileService.GetTermsFile() != null
            };
            return dto;
        }
    }
}
