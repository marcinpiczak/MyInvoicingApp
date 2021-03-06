﻿using Microsoft.AspNetCore.Mvc;

namespace MyInvoicingApp.Interfaces
{
    public interface IPdfManager
    {
        /// <summary>
        /// Gets PDF file for invoice
        /// </summary>
        /// <param name="invoiceId">invoice Id for which file should be generated</param>
        /// <returns>PDF file for invoice</returns>
        FileStreamResult GetInvoicePdf(string invoiceId);
    }
}