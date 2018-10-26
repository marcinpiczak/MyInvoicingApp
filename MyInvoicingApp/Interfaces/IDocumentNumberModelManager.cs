using System;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Interfaces
{
    public interface IDocumentNumberModelManager
    {
        /// <summary>
        /// Get DocumentNumber model for given document type and date
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dateHelper"></param>
        /// <param name="type">type of document</param>
        /// <param name="forDate">date for which document number should be taken</param>
        /// <param name="modifiedBy">ApplicationUser that is getting document number</param>
        /// <returns>DocumentNumber model for given document type and date</returns>
        DocumentNumber GetDocumentNumberModel(EFCDbContext context, IDateHelper dateHelper, DocumentType type, DateTime forDate, ApplicationUser modifiedBy);
    }
}