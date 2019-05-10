using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DigitalPurchasing.ExcelReader
{
    public class ExcelSSR
    {
        public class CLData
        {
            public int Number { get; set; }
            public DateTime Date { get; set; }
        }

        public class CustomerRequestData
        {
            public int Number { get; set; }
            public DateTime Date { get; set; }
            public string CustomerName { get; set; }
        }

        public class Data
        {
            public int Number { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string BatchUomName { get; set; }
            public decimal CustomerQuantity { get; set; }
        }

        public CLData CL;
        public CustomerRequestData Request;
        public List<Data> Datas;

        public ExcelSSR(CLData cl, CustomerRequestData request, List<Data> datas)
        {
            CL = cl;
            Request = request;
            Datas = datas;
        }

        public byte[] Build()
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("Подробности");

                ws.Column(1).Width = 1.33;
                ws.Column(2).Width = 4.5;
                ws.Column(3).Width = 14.67;
                ws.Column(4).Width = 27.67;
                ws.Column(5).Width = 10;
                ws.Column(6).Width = 10;
                ws.Column(7).Width = 0.73;

                ws.Cells[1, 2].Value = "Анализ коммерческих предложений, приведенных к единицам измерения запроса";
                ws.Cells[1, 2].Style.Font.Bold = true;
                ws.Cells[2, 2].Value = $"Конкурентный лист № {CL.Number} от {CL.Date:dd.MM.yyyy HH:mm}";
                ws.Cells[4, 2].Value = $"Клиент: {Request.CustomerName}";
                ws.Cells[4, 4].Value = $"Заявка № {Request.Number} от {Request.Date:dd.MM.yyyy HH:mm}";

                ws.Row(5).Height = 48;
                ws.Cells[5, 2].HeaderText("№");
                ws.Cells[5, 3].HeaderText("Код");
                ws.Cells[5, 4].HeaderText("Наименование");
                ws.Cells[5, 5].HeaderText("ЕИ");
                ws.Cells[5, 6].HeaderText("Кол-во для заказа, ЕИ");

                var i = 6;
                foreach (var data in Datas)
                {
                    ws.Row(i).Height = 48;
                    ws.Cells[i, 2].TableText(data.Number);
                    ws.Cells[i, 3].TableText(data.Code);
                    ws.Cells[i, 4].TableText(data.Name);
                    ws.Cells[i, 5].TableText(data.BatchUomName);
                    ws.Cells[i, 6].TableText(data.CustomerQuantity, "N1");
                    i++;
                }

                var titleRow = i + 4;
                var headerRow = titleRow + 2;
                var dataRow = headerRow + 1;

                ws.Row(i).Height = 48;
                ws.Cells[i, 4].Value = "ИТОГО";

                ws.Cells[titleRow, 4].Value = "Решение о выборе поставщиков";

                ws.Row(headerRow).Height = 48;
                ws.Cells[headerRow, 3].HeaderText("Дата и время подтверждения");
                ws.Cells[headerRow, 4].HeaderText("Вариант выбора");
                ws.Cells[headerRow, 5].HeaderText("Общая сумма");
                ws.Cells[headerRow, 6].HeaderText("Валюта");
                ws.Cells[headerRow, 8].HeaderText("Выбор подтвердил");

                ws.Row(dataRow).Height = 48;
                ws.Cells[dataRow, 3].TableText("00.00.0000 00:00");
                ws.Cells[dataRow, 4].TableText(0);
                ws.Cells[dataRow, 5].TableText(0m, "N2");
                ws.Cells[dataRow, 6].TableText("RUB");
                ws.Cells[dataRow, 8].TableText("Иванов Иван");

                return excel.GetAsByteArray();
            }
        }
    }

    public static class Extensions
    {
        public static ExcelRange HeaderText(this ExcelRange cell, string text)
        {
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.WrapText = true;
            cell.Value = text;
            return cell;
        }

        public static ExcelRange TableText(this ExcelRange cell, int value) => TableText(cell, value.ToString());

        public static ExcelRange TableText(this ExcelRange cell, decimal value, string format = "N") => TableText(cell, value.ToString(format));

        public static ExcelRange TableText(this ExcelRange cell, string text)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Value = text;
            return cell;
        }

        public static ExcelRange AlignLeft(this ExcelRange cell)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            return cell;
        }
        
        public static ExcelRange AlignRight(this ExcelRange cell)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            return cell;
        }
    }
}
