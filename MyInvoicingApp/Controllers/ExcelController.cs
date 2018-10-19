using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;

namespace MyInvoicingApp.Controllers
{
    [Authorize(Roles = "Admin,Accountant,Manager")]
    public class ExcelController : Controller
    {
        protected IExcelManager ExcelManager { get; set; }

        public ExcelController(IExcelManager excelManager)
        {
            ExcelManager = excelManager;
        }

        public IActionResult GetInvoiceExcel(string invoiceId)
        {
            try
            {
                return ExcelManager.GetInvoiceExcel(invoiceId);
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas pobierania Excel dla faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index", "Invoice");
        }
    }
}
