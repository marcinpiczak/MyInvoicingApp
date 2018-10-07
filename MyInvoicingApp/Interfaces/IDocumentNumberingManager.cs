using System;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Interfaces
{
    public interface IDocumentNumberingManager
    {
        string GetNextDocumentNumber(DocumentType type, DateTime forDate, ApplicationUser modifiedBy);
    }
}