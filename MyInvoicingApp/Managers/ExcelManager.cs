using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using Syncfusion.XlsIO;
using Syncfusion.Drawing;

namespace MyInvoicingApp.Managers
{
    public class ExcelManager : IExcelManager
    {
        protected IInvoiceManager InvoiceManager { get; set; }
        protected DateHelper DateHelper { get; set; }

        public ExcelManager(IInvoiceManager invoiceManager, DateHelper dateHelper)
        {
            InvoiceManager = invoiceManager;
            DateHelper = dateHelper;
        }

        /// <summary>
        /// Gets Excel file for invoice using Syncfusion.XlsIO
        /// </summary>
        /// <param name="invoiceId">invoice Id for which file should be generated</param>
        /// <returns>Excel file for invoice</returns>
        public FileStreamResult GetInvoiceExcel(string invoiceId)
        {
            var invoice = InvoiceManager.GetInvoiceViewModelById(invoiceId);
            invoice.InvoiceLines = InvoiceManager.GetInvoiceLineViewModels(invoiceId).OrderBy(x => x.LineNumber);

            //Step 1 : Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine(); 
            
            //Step 2 : Instantiate the excel application object.
            IApplication application = excelEngine.Excel; 
            
            // Creating new workbook
            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheet = workbook.Worksheets[0];
            sheet.Name = $"Faktura_{invoice.InvoiceNumber.Replace("/", "_")}";

            sheet.Name = sheet.Name.Length > 31 ? sheet.Name.Substring(0, 31) : sheet.Name;

            #region Generate Excel 
            sheet.Range["A2"].ColumnWidth = 15;
            sheet.Range["B2"].ColumnWidth = 15;
            sheet.Range["C2"].ColumnWidth = 25;
            sheet.Range["D2"].ColumnWidth = 15;
            sheet.Range["E2"].ColumnWidth = 15;
            sheet.Range["F2"].ColumnWidth = 15;
            sheet.Range["G2"].ColumnWidth = 15;
            sheet.Range["H2"].ColumnWidth = 15;
            sheet.Range["I2"].ColumnWidth = 15;
            sheet.Range["J2"].ColumnWidth = 15;
            sheet.Range["K2"].ColumnWidth = 15;
            sheet.Range["L2"].ColumnWidth = 15;
            sheet.Range["M2"].ColumnWidth = 15;
            sheet.Range["N2"].ColumnWidth = 15;
            sheet.Range["O2"].ColumnWidth = 15;

            sheet.Range["A2:O2"].Merge(true);
            sheet.Range["A2:O2"].BorderAround(ExcelLineStyle.Thick, Color.FromArgb(47, 117, 181));
            sheet.Range["A2:O2"].CellStyle.Color = Color.FromArgb(242, 242, 242);

            //Invoice Header
            sheet.Range["A2"].Text = invoice.InvoiceNumber;
            sheet.Range["A2"].CellStyle.Font.FontName = "Verdana";
            sheet.Range["A2"].CellStyle.Font.Bold = true;
            sheet.Range["A2"].CellStyle.Font.Size = 28;
            sheet.Range["A2"].CellStyle.Font.RGBColor = Color.FromArgb(0, 0, 112, 192);
            sheet.Range["A2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;

            sheet.Range["A5:B5"].Merge(true);
            sheet.Range["A5"].Text = "Numer referencyjny";
            sheet.Range["C5"].Text = invoice.ReferenceNumber;
            sheet.Range["A6:B6"].Merge(true);
            sheet.Range["A6"].Text = "Klient";
            sheet.Range["C6"].Text = invoice.Customer.Name;
            sheet.Range["A7:B7"].Merge(true);
            sheet.Range["A7"].Text = "Domyślny budżet";
            sheet.Range["C7"].Text = invoice.Budget.BudgetNumber;
            sheet.Range["A8:B8"].Merge(true);
            sheet.Range["A8"].Text = "Domyślna waluta";
            sheet.Range["C8"].Text = invoice.Currency;
            sheet.Range["A9:B9"].Merge(true);
            sheet.Range["A9"].Text = "Metoda płatności";
            sheet.Range["C9"].Text = invoice.PaymentMethod;

            sheet.Range["C11:C13"].NumberFormat = "yyyy/mm/dd";
            sheet.Range["A11:B11"].Merge(true);
            sheet.Range["A11"].Text = "Data płatności";
            sheet.Range["C11"].DateTime = invoice.PaymentDueDate;
            sheet.Range["A12:B12"].Merge(true);
            sheet.Range["A12"].Text = "Data wydania";
            sheet.Range["C12"].DateTime = invoice.IssueDate;
            sheet.Range["A13:B13"].Merge(true);
            sheet.Range["A13"].Text = "Data przyjęcia";
            sheet.Range["C13"].DateTime = invoice.ReceiveDate;

            sheet.Range["A5:C13"].CellStyle.Font.FontName = "Verdana";
            sheet.Range["A5:C13"].CellStyle.Font.Bold = true;
            sheet.Range["A5:C13"].CellStyle.Font.Size = 11;
            sheet.Range["A5:C13"].CellStyle.Font.RGBColor = Color.FromArgb(0, 128, 128, 128);
            sheet.Range["A5:C13"].HorizontalAlignment = ExcelHAlign.HAlignLeft;
            sheet.Range["B5:C13"].CellStyle.Font.RGBColor = Color.FromArgb(0, 174, 170, 170);
            sheet.Range["B5:C13"].HorizontalAlignment = ExcelHAlign.HAlignRight;

            //Invoice lines header
            var invoiceLineHdgIndex = 15;

            sheet.Range[$"A{invoiceLineHdgIndex}"].Text = "Numer linii";
            sheet.Range[$"B{invoiceLineHdgIndex}"].Text = "Kod pozycji";
            sheet.Range[$"C{invoiceLineHdgIndex}"].Text = "Opis";
            sheet.Range[$"D{invoiceLineHdgIndex}"].Text = "Ilość";
            sheet.Range[$"E{invoiceLineHdgIndex}"].Text = "Cena jedn.";
            sheet.Range[$"F{invoiceLineHdgIndex}"].Text = "Waluta";
            sheet.Range[$"G{invoiceLineHdgIndex}"].Text = "Kurs waluty";
            sheet.Range[$"H{invoiceLineHdgIndex}"].Text = "Stawka Vat(%)";
            sheet.Range[$"I{invoiceLineHdgIndex}"].Text = "Netto (waluta)";
            sheet.Range[$"J{invoiceLineHdgIndex}"].Text = "VAT (waluta)";
            sheet.Range[$"K{invoiceLineHdgIndex}"].Text = "Brutto (waluta)";
            sheet.Range[$"L{invoiceLineHdgIndex}"].Text = "Netto (PLN)";
            sheet.Range[$"M{invoiceLineHdgIndex}"].Text = "VAT (PLN)";
            sheet.Range[$"N{invoiceLineHdgIndex}"].Text = "Brutto (PLN)";
            sheet.Range[$"O{invoiceLineHdgIndex}"].Text = "Kod budżetu";

            sheet.Range[$"A{invoiceLineHdgIndex}"].RowHeight = 30;
            sheet.Range[$"A{invoiceLineHdgIndex}:O{invoiceLineHdgIndex}"].WrapText = true;

            sheet.Range[$"A{invoiceLineHdgIndex}:O{invoiceLineHdgIndex}"].CellStyle.Font.Bold = true;
            sheet.Range[$"A{invoiceLineHdgIndex}:O{invoiceLineHdgIndex}"].CellStyle.Font.Color = ExcelKnownColors.White;
            sheet.Range[$"A{invoiceLineHdgIndex}:O{invoiceLineHdgIndex}"].CellStyle.Color = Color.FromArgb(47, 117, 181);
            sheet.Range[$"A{invoiceLineHdgIndex}:O{invoiceLineHdgIndex}"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
            sheet.Range[$"A{invoiceLineHdgIndex}:O{invoiceLineHdgIndex}"].VerticalAlignment = ExcelVAlign.VAlignCenter;



            //Invoice lines details
            int i = 1;
            foreach (var line in invoice.InvoiceLines)
            {
                if (i % 2 == 0)
                {
                    sheet.Range[$"A{invoiceLineHdgIndex + i}:O{invoiceLineHdgIndex + i}"].CellStyle.Color = Color.FromArgb(230, 230, 230);
                }

                sheet.Range[$"A{invoiceLineHdgIndex + i}"].Number = line.LineNumber;
                sheet.Range[$"B{invoiceLineHdgIndex + i}"].Text = line.ItemName;
                sheet.Range[$"C{invoiceLineHdgIndex + i}"].Text = line.Description;
                sheet.Range[$"D{invoiceLineHdgIndex + i}"].Number = line.Quantity;
                sheet.Range[$"E{invoiceLineHdgIndex + i}"].Number = (double) line.Price;
                sheet.Range[$"E{invoiceLineHdgIndex + i}"].NumberFormat = "#,###0.00";
                sheet.Range[$"F{invoiceLineHdgIndex + i}"].Text = line.Currency;
                sheet.Range[$"G{invoiceLineHdgIndex + i}"].Number = (double) line.CurrencyRate;
                sheet.Range[$"G{invoiceLineHdgIndex + i}"].NumberFormat = "#,###0.0000";
                sheet.Range[$"H{invoiceLineHdgIndex + i}"].Number = (double) line.TaxRate;
                sheet.Range[$"I{invoiceLineHdgIndex + i}"].Number = (double) line.Netto;
                sheet.Range[$"J{invoiceLineHdgIndex + i}"].Number = (double) line.Tax;
                sheet.Range[$"K{invoiceLineHdgIndex + i}"].Number = (double) line.Gross;
                sheet.Range[$"L{invoiceLineHdgIndex + i}"].Number = (double) line.BaseNetto;
                sheet.Range[$"M{invoiceLineHdgIndex + i}"].Number = (double) line.BaseTax;
                sheet.Range[$"N{invoiceLineHdgIndex + i}"].Number = (double) line.BaseGross;
                sheet.Range[$"I{invoiceLineHdgIndex + i}:N{15 + i}"].NumberFormat = "#,###0.00";
                sheet.Range[$"O{invoiceLineHdgIndex + i}"].Text = line.Budget.BudgetNumber;

                i++;
            }

            sheet.Range[$"A{invoiceLineHdgIndex + i - 1}:O{invoiceLineHdgIndex + i - 1}"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

            sheet.Range[$"A15:O{invoiceLineHdgIndex + i - 1}"].CellStyle.Font.FontName = "Verdana";
            sheet.Range[$"A15:O{invoiceLineHdgIndex + i - 1}"].CellStyle.Font.Size = 11;
            sheet.InsertColumn(1,1,ExcelInsertOptions.FormatDefault);
            sheet.IsGridLinesVisible = false;

            #endregion

            workbook.Version = ExcelVersion.Excel2013;
            var contentType = "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{invoice.InvoiceNumber.Replace("/", "_")}_{DateHelper.GetCurrentDatetime():yyyyMMdd}.xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            FileStreamResult fileStreamResult = new FileStreamResult(ms, contentType);
            fileStreamResult.FileDownloadName = fileName;

            return fileStreamResult;
        }
    }
}