using System;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Interfaces
{
    public interface IDocumentNumberModelManager
    {
        DocumentNumber GetDocumentNumberModel(EFCDbContext context, DateHelper dateHelper, DocumentType type, DateTime forDate, ApplicationUser modifiedBy);
    }
}