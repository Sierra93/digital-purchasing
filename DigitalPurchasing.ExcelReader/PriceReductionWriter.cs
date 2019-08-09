using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using EPPlus.Core.Extensions.Style;
using OfficeOpenXml;

namespace DigitalPurchasing.ExcelReader
{
    public class PriceReductionData
    {
        public class DataItem
        {
            public int Position { get; private set; }

            public string RequestCode { get; private set; }
            public string RequestName { get; private set; }
            public string RequestUom { get; private set; }
            public decimal RequestQuantity { get; private set; }

            public string OfferCode { get; private set; }
            public string OfferName { get; private set; }
            public string OfferUom { get; private set; }
            public decimal OfferQuantity { get; private set; }
            public decimal OfferPrice { get; private set; }
            public decimal OfferTotal => OfferQuantity * OfferPrice;

            public decimal TargetDiscount => 1 - TargetPrice / OfferPrice;
            public decimal TargetPrice { get; set; }
            public decimal TargetDiff => OfferPrice - TargetPrice;
            public decimal TargetTotal => OfferQuantity * TargetPrice;
            public decimal TargetTotalDiscount => OfferTotal - TargetTotal;

            public DataItem SetPosition(int position)
            {
                Position = position;
                return this;
            }

            public DataItem SetRequest(
                string code,
                string name,
                string uom,
                decimal quantity)
            {
                RequestCode = code;
                RequestName = name;
                RequestUom = uom;
                RequestQuantity = quantity;
                return this;
            }

            public DataItem SetOffer(
                string code,
                string name,
                string uom,
                decimal quantity,
                decimal price)
            {
                OfferCode = code;
                OfferName = name;
                OfferUom = uom;
                OfferQuantity = quantity;
                OfferPrice = price;
                return this;
            }

            public DataItem SetTargetPrice(decimal targetPrice)
            {
                TargetPrice = targetPrice;
                return this;
            }
        }

        public List<DataItem> Items { get; set; } = new List<DataItem>();
        public string InvoiceData { get; set; }
        public string Currency { get; set; }
    }

    public class PriceReductionWriter
    {
        private readonly PriceReductionData _data;

        public PriceReductionWriter(PriceReductionData data)
            => _data = data;

        public byte[] Build()
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("Запрос");
                ws.SetBackgroundColor(Color.White);
                
                //[row, col]
                ws.Cells[1, 2].HeaderText("Запрос на изменение условий коммерческого предложения (КП)/ Счета")
                    .AlignLeft().NoWrapText();
                ws.Cells[4, 8].HeaderText("Номер КП/Счета: " + _data.InvoiceData).NoWrapText().BoldFont().AlignLeft();
                ws.Cells[4, 10].HeaderText("Валюта КП/счета: " + _data.Currency).NoWrapText().BoldFont(false).AlignLeft();

                ws.Row(5).Height = 64;
                ws.Cells[5, 2].HeaderText("№"); 	    
                ws.Cells[5, 3].HeaderText("Код");
                ws.Cells[5, 4].HeaderText("Наименование товара в запросе").AlignLeft();
                ws.Cells[5, 5].HeaderText("ЕИ");
                ws.Cells[5, 6].HeaderText("Кол-во для запроса, ЕИ");
                ws.Cells[5, 2, 5, 6].BackgroundLight().HeaderBorders();
                //7
                ws.Cells[5, 8].HeaderText("Код");
                ws.Cells[5, 9].HeaderText("Наименование товара у поставщика").AlignLeft();
                ws.Cells[5, 10].HeaderText("Кол-во, предложенное Поставщиком, ЕИ поставщика");
                ws.Cells[5, 11].HeaderText("ЕИ поставщика");
                ws.Cells[5, 12].HeaderText("Цена поставщика, валюта КП");
                ws.Cells[5, 13].HeaderText("Сумма, валюта КП");
                ws.Cells[5, 8, 5, 13].BackgroundLight().HeaderBorders();
                //14
                ws.Cells[5, 15].HeaderText("Целевая цена, валюта за ЕИ поставщика");
                ws.Cells[5, 16].HeaderText("Целевая скидка к первой цене, %");
                ws.Cells[5, 17].HeaderText("Целевая скидка к первой цене");
                ws.Cells[5, 18].HeaderText("Целевая сумма, валюта");
                ws.Cells[5, 19].HeaderText("Итого сумма скидки");
                ws.Cells[5, 15, 5, 19].BackgroundLight().HeaderBorders();

                var row = 5;
                foreach (var item in _data.Items.OrderBy(q => q.Position))
                {
                    row++;
                    ws.Row(row).Height = 48;
                    ws.Cells[row, 2].TableText((row-5).ToString());
                    ws.Cells[row, 3].TableText(item.RequestCode);
                    ws.Cells[row, 4].TableText(item.RequestName).AlignLeft();
                    ws.Cells[row, 5].TableText(item.RequestUom);
                    ws.Cells[row, 6].TableText(item.RequestQuantity);
                    ws.Cells[row, 2, row, 6].ItemBorders();
                    //7
                    ws.Cells[row, 8].TableText(item.OfferCode);
                    ws.Cells[row, 9].TableText(item.OfferName).AlignLeft();
                    ws.Cells[row, 10].TableText(item.OfferQuantity);
                    ws.Cells[row, 11].TableText(item.OfferUom);
                    ws.Cells[row, 12].TableText(item.OfferPrice);
                    ws.Cells[row, 13].TableText(item.OfferTotal);
                    ws.Cells[row, 8, row, 13].ItemBorders();
                    //14
                    ws.Cells[row, 15].TableText(item.TargetPrice);
                    ws.Cells[row, 16].Percentage(item.TargetDiscount, "#0.00%").BoldFont().AlignRight();
                    ws.Cells[row, 17].TableText(item.TargetDiff);
                    ws.Cells[row, 18].TableText(item.TargetTotal);
                    ws.Cells[row, 19].TableText(item.TargetTotalDiscount);
                    ws.Cells[row, 15, row, 19].ItemBorders();
                }

                row++;
                ws.Row(row).Height = 64;
                ws.Cells[row, 2, row, 6].BackgroundLight().HeaderBorders().HeaderText("ИТОГО");
                ws.Cells[row, 2, row, 6].Merge = true;
                ws.Cells[row, 8, row, 13].BackgroundLight().HeaderBorders();
                ws.Cells[row, 15, row, 19].BackgroundLight().HeaderBorders();
                ws.Cells[row, 13].TableText(_data.Items.Sum(q => q.OfferTotal)).BoldFont();
                ws.Cells[row, 18].TableText(_data.Items.Sum(q => q.TargetTotal)).BoldFont();
                ws.Cells[row, 19].TableText(_data.Items.Sum(q => q.TargetTotalDiscount)).BoldFont();

                SetColumnsWidth(ws);

                return excel.GetAsByteArray();
            }
        }

        private void SetColumnsWidth(ExcelWorksheet ws)
        {
            const double separatorWidth = 0.73;

            ws.Column(1).Width = separatorWidth * 2;
            //ws.Column(2).AutoFit();
            if (_data.Items.Any(q => !string.IsNullOrEmpty(q.RequestCode)))
            {
                ws.Column(3).AutoFit();
            }
            else
            {
                ws.Column(3).Width = 8.33;
            }
            ws.Column(4).AutoFit();
            ws.Column(5).AutoFit();
            ws.Column(6).AutoFit();
            ws.Column(7).Width = separatorWidth;
            if (_data.Items.Any(q => !string.IsNullOrEmpty(q.OfferCode)))
            {
                ws.Column(8).AutoFit();
            }
            else
            {
                ws.Column(8).Width = 8.33;
            }
            ws.Column(9).AutoFit();
            ws.Column(10).Width = 16;
            ws.Column(11).AutoFit();
            ws.Column(12).AutoFit();
            ws.Column(13).AutoFit();
            ws.Column(14).Width = separatorWidth;
            ws.Column(15).Width = 12;
            ws.Column(16).AutoFit();
            ws.Column(17).AutoFit();
            ws.Column(18).AutoFit();
            ws.Column(19).AutoFit();
        }
    }
}
