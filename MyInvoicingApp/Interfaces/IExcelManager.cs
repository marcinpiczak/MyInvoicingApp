using Microsoft.AspNetCore.Mvc;

namespace MyInvoicingApp.Interfaces
{
    public interface IExcelManager
    {
        /// <summary>
        /// Gets Excel file for invoice
        /// </summary>
        /// <param name="invoiceId">invoice Id for which file should be generated</param>
        /// <returns>Excel file for invoice</returns>
        FileStreamResult GetInvoiceExcel(string invoiceId);
    }
}