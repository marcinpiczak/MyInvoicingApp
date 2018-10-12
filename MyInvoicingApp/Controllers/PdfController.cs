using System;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;

namespace MyInvoicingApp.Controllers
{
    public class PdfController : Controller
    {

        protected IPdfManager PdfManager { get; set; }

        public PdfController(IPdfManager pdfManager)
        {
            PdfManager = pdfManager;
        }
    
        [HttpGet]
        public ActionResult GetInvoicePdf(string invoiceId)
        {
            try
            {
                return PdfManager.GetInvoicePdf(invoiceId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
