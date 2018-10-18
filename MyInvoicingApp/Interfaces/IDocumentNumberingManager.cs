using System;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Interfaces
{
    public interface IDocumentNumberingManager
    {
        /// <summary>
        /// Gets next formatted document number for given document type and date
        /// </summary>
        /// <param name="type">type of document</param>
        /// <param name="forDate">date for which document number should be taken</param>
        /// <param name="modifiedBy">ApplicationUser that is getting document number</param>
        /// <returns>formatted document number</returns>
        string GetNextDocumentNumber(DocumentType type, DateTime forDate, ApplicationUser modifiedBy);
    }
}