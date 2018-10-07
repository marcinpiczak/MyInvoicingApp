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
        protected DateHelper DateHelper { get; set; }
        protected IDocumentNumberModelManager DocumentNumberModelManager { get; set; }

        public DocumentNumberingManager(EFCDbContext context, DateHelper dateHelper, IDocumentNumberModelManager documentNumberModelManager)
        {
            Context = context;
            DateHelper = dateHelper;
            DocumentNumberModelManager = documentNumberModelManager;
        }

        private readonly object _lock = new object();

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