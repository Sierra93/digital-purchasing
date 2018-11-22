using System.ComponentModel;

namespace DigitalPurchasing.Core
{
    public enum DeliveryTerms
    {
        [Order(0), Description("Нет требований")]
        NoRequirements = 0,

        [Order(1), Description("Доставка до склада покупателя")]
        CustomerWarehouse = 10,

        [Order(2), Description("Самовывоз")]
        SelfDelivery = 20,

        [Order(13), Description("DDP")]
        DDP = 30,

        [Order(12), Description("DAP")]
        DAP = 40,

        [Order(3), Description("EXW")]
        EXW = 50,

        [Order(4), Description("FCA")]
        FCA = 60,

        [Order(9), Description("CPT")]
        CPT = 70,

        [Order(8), Description("CIF")]
        CIF = 80,

        [Order(11), Description("DAT")]
        DAT = 90,

        [Order(10), Description("CIP")]
        CIP = 100,

        [Order(7), Description("CFR")]
        CFR = 110,

        [Order(5), Description("FAS")]
        FAS = 120,

        [Order(6), Description("FOB")]
        FOB = 130,
    }
}
