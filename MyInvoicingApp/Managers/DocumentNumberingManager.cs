using System;
using System.Text;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Managers
{
    public class DocumentNumberingManager : IManager, IDocumentNumberingManager
    {
        protected EFCDbContext Context { get; set; }
        protected IDateHelper DateHelper { get; set; }
        protected IDocumentNumberModelManager DocumentNumberModelManager { get; set; }

        public DocumentNumberingManager(EFCDbContext context, IDateHelper dateHelper, IDocumentNumberModelManager documentNumberModelManager)
        {
            Context = context;
            DateHelper = dateHelper;
            DocumentNumberModelManager = documentNumberModelManager;
        }

        /// <summary>
        /// Gets next formatted document number for given document type and date
        /// </summary>
        /// <param name="type">type of document</param>
        /// <param name="forDate">date for which document number should be taken</param>
        /// <param name="modifiedBy">ApplicationUser that is getting document number</param>
        /// <returns>formatted document number</returns>
        public string GetNextDocumentNumber(DocumentType type, DateTime forDate, ApplicationUser modifiedBy)
        {
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy), "Nieprawidłowe parametry");
            }

            var documentNumberingRecord = DocumentNumberModelManager.GetDocumentNumberModel(Context, DateHelper, type, forDate, modifiedBy);

            var nextDocumentNumber = new StringBuilder(documentNumberingRecord.DocumentNumberPart1);
            nextDocumentNumber.Append(documentNumberingRecord.DocumentNumberPart1Separator);
            nextDocumentNumber.Append(forDate.Year);
            nextDocumentNumber.Append(documentNumberingRecord.DocumentNumberPart1Separator);
            nextDocumentNumber.Append(documentNumberingRecord.DocumentSequence.NextNumber);

            return nextDocumentNumber.ToString();
        }
    }
}