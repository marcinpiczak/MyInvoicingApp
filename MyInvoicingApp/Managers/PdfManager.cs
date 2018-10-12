using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.ViewModels;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;

namespace MyInvoicingApp.Managers
{
    public class PdfManager : IManager, IPdfManager
    {
        protected IInvoiceManager InvoiceManager { get; set; }
        protected DateHelper DateHelper { get; set; }

        public PdfManager(IInvoiceManager invoiceManager, DateHelper dateHelper)
        {
            InvoiceManager = invoiceManager;
            DateHelper = dateHelper;
        }

        public FileStreamResult GetInvoicePdf(string invoiceId)
        {
            //Declaring colors        
            var black = Color.FromArgb(255, 0, 0, 0);
            var white = Color.FromArgb(255, 255, 255, 255);
            var lightGray = Color.FromArgb(255, 220, 220, 220);

            var model = InvoiceManager.GetInvoiceViewModelById(invoiceId);
            model.InvoiceLines = InvoiceManager.GetInvoiceLineViewModels(invoiceId);

            var invoiceNo = model.InvoiceNumber;
            var invoiceHdrText = $"Faktura VAT: {invoiceNo}";

            var customer = model.Customer;

            var invoiceLines = model.InvoiceLines;

            var lightGrayBrush = new PdfSolidBrush(lightGray);

            //Creating new PDF document instance
            PdfDocument document = new PdfDocument();

            //Setting margin
            document.PageSettings.Margins.All = 20;

            //Adding a new page
            PdfPage page = document.Pages.Add();
            PdfGraphics g = page.Graphics;

            //Load the custom font as stream 
            Stream fontStream = System.IO.File.OpenRead("wwwroot/fonts/Roboto-Regular.ttf");

            //Create a new PDF true type font. 
            PdfTrueTypeFont customFont8 = new PdfTrueTypeFont(fontStream, 8, PdfFontStyle.Regular);
            PdfTrueTypeFont customFont8Bold = new PdfTrueTypeFont(fontStream, 8, PdfFontStyle.Bold);
            PdfTrueTypeFont customFont10 = new PdfTrueTypeFont(fontStream, 10, PdfFontStyle.Regular);
            PdfTrueTypeFont customFont12 = new PdfTrueTypeFont(fontStream, 12, PdfFontStyle.Regular);
            PdfTrueTypeFont customFont12Bold = new PdfTrueTypeFont(fontStream, 12, PdfFontStyle.Bold);
            PdfTrueTypeFont customFont30 = new PdfTrueTypeFont(fontStream, 30, PdfFontStyle.Regular);

            //Creating font instances
            PdfFont headerFont = customFont30;
            //PdfFont standardText12 = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            PdfFont standardText12 = customFont12;
            PdfFont standardText12Bold = customFont12Bold;
            PdfFont standardText10 = customFont10;
            PdfFont standardText8 = customFont8;
            PdfFont standardText8Bold = customFont8Bold;

            //Set page size
            document.PageSettings.Size = PdfPageSize.A4;

            //Set page orientation
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;

            //pen
            var pen = new PdfPen(Color.Black)
            {
                Width = 0.5F
            };

            //Invoice number printing
            var invoiceNoRectangle = new Syncfusion.Drawing.RectangleF(180, 30, 200, 100);

            g.DrawRectangle(lightGrayBrush, invoiceNoRectangle);
            var result = BodyContent(invoiceHdrText, headerFont, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, invoiceNoRectangle);

            //Customer data printing
            var customerDataHeaderRectangle = new Syncfusion.Drawing.RectangleF(10, 200, 200, 15);
            var customerDataContentRectangle = new Syncfusion.Drawing.RectangleF(10, 215, 200, 100);

            g.DrawRectangle(lightGrayBrush, customerDataHeaderRectangle);
            result = BodyContent("Klient", standardText12Bold, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, customerDataHeaderRectangle);

            g.DrawRectangle(pen, new PdfSolidBrush(white), customerDataContentRectangle);
            var rect = new RectangleF(12, 215, 200, 25);
            result = BodyContent(customer.Name, standardText10, black, PdfTextAlignment.Left, PdfVerticalAlignment.Top, page, rect);
            rect = new RectangleF(12, result.Bounds.Bottom + 5, 200, 25);
            result = BodyContent($"{customer.Street}/{customer.BuildingNumber}", standardText10, black, PdfTextAlignment.Left, PdfVerticalAlignment.Top, page, rect);
            rect = new RectangleF(12, result.Bounds.Bottom, 200, 25);
            result = BodyContent($"{customer.PostalCode} {customer.City}", standardText10, black, PdfTextAlignment.Left, PdfVerticalAlignment.Top, page, rect);

            //Dates data printing
            var datesX = page.Graphics.ClientSize.Width - customerDataHeaderRectangle.Width - customerDataHeaderRectangle.X;
            var datesY = customerDataHeaderRectangle.Y;
            var datesWidth = customerDataHeaderRectangle.Width;
            var datesHeight = customerDataHeaderRectangle.Height;

            //issue date
            var issueDateHeaderRectangle = new Syncfusion.Drawing.RectangleF(datesX, datesY, datesWidth, datesHeight);
            var issueDateDataContentRectangle = new Syncfusion.Drawing.RectangleF(datesX, datesY + datesHeight, datesWidth, 20);

            g.DrawRectangle(lightGrayBrush, issueDateHeaderRectangle);
            BodyContent("Data wydania", standardText12Bold, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, issueDateHeaderRectangle);

            g.DrawRectangle(pen, new PdfSolidBrush(white), issueDateDataContentRectangle);
            result = BodyContent("2018-10-04", standardText10, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, issueDateDataContentRectangle);

            //receipt date
            var receiptDateHeaderRectangle = new Syncfusion.Drawing.RectangleF(datesX, result.Bounds.Bottom + 9, datesWidth, datesHeight);
            var receiptDateDataContentRectangle = new Syncfusion.Drawing.RectangleF(datesX, result.Bounds.Bottom + 9 + datesHeight, datesWidth, 20);

            g.DrawRectangle(lightGrayBrush, receiptDateHeaderRectangle);
            BodyContent("Data przyjęcia", standardText12Bold, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, receiptDateHeaderRectangle);

            g.DrawRectangle(pen, new PdfSolidBrush(white), receiptDateDataContentRectangle);
            result = BodyContent("2018-10-04", standardText10, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, receiptDateDataContentRectangle);

            //receipt date
            var paymentDateHeaderRectangle = new Syncfusion.Drawing.RectangleF(datesX, result.Bounds.Bottom + 9, datesWidth, datesHeight);
            var paymentDateDataContentRectangle = new Syncfusion.Drawing.RectangleF(datesX, result.Bounds.Bottom + 9 + datesHeight, datesWidth, 20);

            g.DrawRectangle(lightGrayBrush, paymentDateHeaderRectangle);
            BodyContent("Data płatności", standardText12Bold, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, paymentDateHeaderRectangle);

            g.DrawRectangle(pen, new PdfSolidBrush(white), paymentDateDataContentRectangle);
            result = BodyContent("2018-10-04", standardText10, black, PdfTextAlignment.Center, PdfVerticalAlignment.Middle, page, paymentDateDataContentRectangle);

            //adding grid for invoice lines
            PdfGrid grid = new PdfGrid();
            grid.Style.AllowHorizontalOverflow = false;

            //Set Data source for invoice lines
            if (invoiceLines.Count() == 0)
            {
                invoiceLines = new List<InvoiceLineViewModel>() { new InvoiceLineViewModel() };
            }

            grid.DataSource = invoiceLines.Select(x => new
            {
                ItemName = x.ItemName,
                Description = x.Description,
                Quantity = x.Quantity,
                Price = x.Price.ToString("N"),
                TaxRate = x.TaxRate.ToString("##"),
                NetValue = x.BaseNetto.ToString("N"),
                TaxValue = x.BaseTax.ToString("N"),
                GrossValue = x.BaseGross.ToString("N")
            });

            //grid styling
            var styleName = "GridTable4";
            PdfGridBuiltinStyleSettings setting = new PdfGridBuiltinStyleSettings();
            setting.ApplyStyleForHeaderRow = false;
            setting.ApplyStyleForBandedRows = false;
            setting.ApplyStyleForBandedColumns = false;
            setting.ApplyStyleForFirstColumn = false;
            setting.ApplyStyleForLastColumn = false;
            setting.ApplyStyleForLastRow = false;

            var gridHeaderStyle = new PdfGridRowStyle()
            {
                BackgroundBrush = new PdfSolidBrush(lightGray),
                Font = standardText8Bold
            };

            //Set layout properties
            PdfLayoutFormat format = new PdfLayoutFormat();
            format.Break = PdfLayoutBreakType.FitElement;
            format.Layout = PdfLayoutType.Paginate;

            PdfGridBuiltinStyle style = (PdfGridBuiltinStyle)Enum.Parse(typeof(PdfGridBuiltinStyle), styleName);

            grid.ApplyBuiltinStyle(style, setting);
            grid.Style.Font = standardText8;
            grid.Style.CellPadding.All = 2;
            grid.Style.AllowHorizontalOverflow = false;

            //set header texts
            PdfGridRow pdfGridHeader = grid.Headers[0];
            pdfGridHeader.Style = gridHeaderStyle;
            pdfGridHeader.Cells[0].Value = "Kod pozycji";
            pdfGridHeader.Cells[1].Value = "Opis";
            pdfGridHeader.Cells[2].Value = "Ilość";
            pdfGridHeader.Cells[3].Value = "Cena";
            pdfGridHeader.Cells[4].Value = "VAT(%)";
            pdfGridHeader.Cells[5].Value = "Netto";
            pdfGridHeader.Cells[6].Value = "VAT";
            pdfGridHeader.Cells[7].Value = "Brutto";

            //grid location
            var gridStartLocation = new PointF(0, result.Bounds.Bottom + 50);

            //Draw table
            result = grid.Draw(page, gridStartLocation, format);

            var summaryByTaxRate = invoiceLines
                .GroupBy(x => x.TaxRate)
                .Select(x => new
                {
                    TaxRate = x.Key.ToString("##"),
                    NetValue = x.Sum(y => y.BaseNetto).ToString("N"),
                    TaxValue = x.Sum(y => y.BaseTax).ToString("N"),
                    GrossValue = x.Sum(y => y.BaseGross).ToString("N")
                });

            //adding summary grid
            PdfGrid summaryGrid = new PdfGrid();
            summaryGrid.Style.AllowHorizontalOverflow = false;

            //Set Data source
            summaryGrid.DataSource = summaryByTaxRate;

            summaryGrid.ApplyBuiltinStyle(style, setting);
            summaryGrid.Style.Font = standardText8;
            summaryGrid.Style.CellPadding.All = 2;
            summaryGrid.Style.AllowHorizontalOverflow = false;


            //set header texts
            PdfGridRow pdfSummaryGridHeader = summaryGrid.Headers[0];
            pdfSummaryGridHeader.Style = gridHeaderStyle;
            pdfSummaryGridHeader.Cells[0].Value = "według stawki VAT";
            pdfSummaryGridHeader.Cells[1].Value = "wartość netto";
            pdfSummaryGridHeader.Cells[2].Value = "wartość VAT";
            pdfSummaryGridHeader.Cells[3].Value = "wartość brutto";

            summaryGrid.Columns[0].Width = 80;
            summaryGrid.Columns[1].Width = 60;
            summaryGrid.Columns[2].Width = 60;
            summaryGrid.Columns[3].Width = 60;

            //create summary total GridRow
            var summaryTotalNetto = invoiceLines.Sum(x => x.BaseNetto);
            var summaryTotalTax = invoiceLines.Sum(x => x.BaseTax);
            var summaryTotalGross = invoiceLines.Sum(x => x.BaseGross);

            var summaryTotalRow = new PdfGridRow(summaryGrid);
            summaryGrid.Rows.Add(summaryTotalRow);
            summaryTotalRow.Cells[0].Value = "Razem:";
            summaryTotalRow.Cells[0].Style.Font = customFont8Bold;
            summaryTotalRow.Cells[1].Value = summaryTotalNetto.ToString("N");
            summaryTotalRow.Cells[2].Value = summaryTotalTax.ToString("N");
            summaryTotalRow.Cells[3].Value = summaryTotalGross.ToString("N");

            float columnsWidth = 0;
            foreach (PdfGridColumn column in summaryGrid.Columns)
            {
                columnsWidth += column.Width;
            }

            //Summary grid location
            var summaryGridStartLocation = new PointF(page.GetClientSize().Width - columnsWidth, result.Bounds.Bottom + 20);

            //Draw table
            result = summaryGrid.Draw(page, summaryGridStartLocation, format);

            //Saving the PDF to the MemoryStreamcolumnsWidth
            MemoryStream ms = new MemoryStream();
            document.Save(ms);

            //If the position is not set to '0' then the PDF will be empty.
            ms.Position = 0;

            //Download the PDF document in the browser.
            FileStreamResult fileStreamResult = new FileStreamResult(ms, "application/pdf");
            fileStreamResult.FileDownloadName = $"{invoiceNo.Replace("/", "_")}_{DateHelper.GetCurrentDatetime():yyyyMMdd}.pdf";
            return fileStreamResult;
        }

        private PdfLayoutResult BodyContent(string text, PdfFont font, Color color, PdfTextAlignment alignment, PdfVerticalAlignment hAlignment, PdfPage page, RectangleF rectangleF)
        {
            PdfTextElement txtElement = new PdfTextElement(text);
            txtElement.Font = font;
            txtElement.Brush = new PdfSolidBrush(color);
            txtElement.StringFormat = new PdfStringFormat();
            txtElement.StringFormat.WordWrap = PdfWordWrapType.Word;
            txtElement.StringFormat.LineLimit = true;
            txtElement.StringFormat.Alignment = alignment;
            txtElement.StringFormat.LineAlignment = hAlignment;
            PdfLayoutResult result = txtElement.Draw(page, rectangleF);
            return result;
        }
    }
}