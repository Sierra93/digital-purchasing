using System.ComponentModel;

namespace DigitalPurchasing.Core
{
    public enum DeliveryTerms
    {
        [Description("Нет требований")]
        NoRequirements = 0,
        [Description("Доставка до склада покупателя")]
        CustomerWarehouse = 10,
        [Description("Самовывоз")]
        SelfDelivery = 20,
        [Description("DDP")]
        DDP = 30,
        [Description("DAP")]
        DAP = 40,
        [Description("EXW")]
        EXW = 50,
        [Description("FCA")]
        FCA = 60,
        [Description("CPT")]
        CPT = 70,
        [Description("CIF")]
        CIF = 80
    }
}
