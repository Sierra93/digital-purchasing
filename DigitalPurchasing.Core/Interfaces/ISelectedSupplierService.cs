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

        public decimal SelectedVariantTotalPrice { get; set; }
        public int SelectedVariantNumber { get; set; }

        public UserDto User { get; set; }

        public SSCustomerDto Customer { get; set; }
        public List<SSCustomerItemDto> CustomerItems { get; set; }

        public List<SSSupplierDto> Suppliers { get; set; }
        public List<SSSupplierItemDto> SupplierItems { get; set; }

        public List<SSVariantDto> Variants { get; set; }

        public List<SSDataDto> Datas { get; set; }
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

    public class SSSupplierItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public SSSupplierDto Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Guid InternalId { get; set; }

        public Guid NomenclatureId { get; set; }

        public decimal ConvertedQuantity { get; set; }
        public decimal ConvertedPrice { get; set; }

        public string UomStr { get; set; }

        public string OfferInvoiceData { get; set; }
    }

    public class SSVariantDto
    {
        public Guid ReportId { get; set; }

        public int Number { get; set; }

        public bool IsSelected { get; set; }

        public Guid InternalId { get; set; }

        public DateTime CreatedOn { get; set; }
        public Guid Id { get; set; }
    }

    public class SSVariantDtoComparer : IEqualityComparer<SSVariantDto>
    {
        public bool Equals(SSVariantDto x, SSVariantDto y) => x.Id == y.Id;

        public int GetHashCode(SSVariantDto obj) => obj.Id.GetHashCode();
    }

    public class SSDataDto
    {
        public Guid Id { get; set; }

        public SSVariantDto Variant { get; set; }
        public Guid VariantId { get; set; }

        public SSSupplierDto Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Guid NomenclatureId { get; set; }

        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class SSSupplierDtoComparer : IEqualityComparer<SSSupplierDto>
    {
        public bool Equals(SSSupplierDto x, SSSupplierDto y) => x.Id == y.Id;

        public int GetHashCode(SSSupplierDto obj) => obj.Id.GetHashCode();
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
        public string Currency { get; set; }
        public int SelectedVariantNumber { get; set; }
        public decimal SelectedVariantTotalPrice { get; set; }
    }
}
