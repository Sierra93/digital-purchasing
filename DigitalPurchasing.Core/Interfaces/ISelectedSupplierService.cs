using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ISelectedSupplierService
    {
        Task<GenerateReportDataResult> GenerateReportData(Guid ownerId, Guid userId, Guid variantId);
        Task<IEnumerable<SSReportSimple>> GetReports(Guid clId);
        Task<SSReportDto> GetReport(Guid reportId);
    }

    public class SSReportDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime CLCreatedOn { get; set; }
        public int CLNumber { get; set; }

        public UserDto User { get; set; }

        public SSCustomerDto Customer { get; set; }
        public List<SSCustomerItemDto> CustomerItems { get; set; }

        public List<SSSupplierDto> Suppliers { get; set; }
    }

    public class SSCustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid InternalId { get; set; }

        public DateTime PRCreatedOn { get; set; }
        public int PRNumber { get; set; }
    }

    public class SSSupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid InternalId { get; set; }
        public DateTime SOCreatedOn { get; set; }
        public int SONumber { get; set; }
    }

    public class SSCustomerItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public decimal Quantity { get; set; }

        public string Code { get; set; }

        public string Uom { get; set; }

        public Guid CustomerId { get; set; }

        public Guid InternalId { get; set; }
        public int Position { get; set; }

        public Guid NomenclatureId { get; set; }
    }

    public class GenerateReportDataResult
    {
        public Guid ReportId { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class SSReportSimple
    {
        public Guid ReportId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }

        public string SelectedBy => $"{UserLastName} {UserFirstName}";
    }
}
