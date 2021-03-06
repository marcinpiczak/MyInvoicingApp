﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;

namespace MyInvoicingApp.Controllers
{
    [Authorize(Roles = "Admin,Accountant,Manager")]
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
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas pobierania PDF dla faktury: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index", "Invoice");
        }
    }
}
