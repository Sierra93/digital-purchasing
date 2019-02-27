using System;
using System.Collections.Generic;
using OfficeOpenXml;

namespace DigitalPurchasing.ExcelReader
{
    public class ExcelQr
    {
        string _currencyCellFormat = "### ### ##0.00";  

        public class DataItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal Qty { get; set; }
            public string Uom { get; set; }
        }

        public byte[] Build(IEnumerable<DataItem> dataItems)
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("RFQ");
                ws.Cells[1, 1].Value = "Код";
                ws.Cells[1, 2].Value = "Название";
                ws.Cells[1, 3].Value = "Кол-во";
                ws.Cells[1, 4].Value = "EИ";
                var i = 2;
                foreach (var item in dataItems)
                {
                    ws.Cells[i, 1].Value = item.Code;
                    ws.Cells[i, 2].Value = item.Name;
                    using(var cellQty = ws.Cells[i, 3]) {  
                        cellQty.Style.Numberformat.Format = _currencyCellFormat;
                        cellQty.Value = item.Qty;
                    }  
                    ws.Cells[i, 4].Value = item.Uom;
                    i++;
                }
                ws.Column(1).AutoFit();
                ws.Column(2).AutoFit();
                ws.Column(3).AutoFit();
                ws.Column(4).AutoFit();
                return excel.GetAsByteArray();
            }
        }
    }
}
