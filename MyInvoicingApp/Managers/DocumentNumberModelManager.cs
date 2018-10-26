using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Managers
{
    public class DocumentNumberModelManager : IManager, IDocumentNumberModelManager
    {
        private readonly object _lock = new object();

        /// <summary>
        /// Get DocumentNumber model for given document type and date
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dateHelper"></param>
        /// <param name="type">type of document</param>
        /// <param name="forDate">date for which document number should be taken</param>
        /// <param name="modifiedBy">ApplicationUser that is getting document number</param>
        /// <returns>DocumentNumber model for given document type and date</returns>
        public DocumentNumber GetDocumentNumberModel(EFCDbContext context, IDateHelper dateHelper, DocumentType type, DateTime forDate, ApplicationUser modifiedBy)
        {
            if (context == null || dateHelper == null || modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy), "Nieprawidłowe parametry");
            }

            lock (_lock)
            {
                var documentNumberingRecord = context.DocumentNumbers
                    .Include(x => x.DocumentSequence)
                    .SingleOrDefault(x => x.DocumentType == type && x.DocumentSequence.EffectiveFrom <= forDate && x.DocumentSequence.EffectiveTo > forDate);

                if (documentNumberingRecord == null)
                {
                    throw new ArgumentException($"Brak numeratora dla {type.ToString()} dla daty {forDate.ToShortDateString()}", nameof(documentNumberingRecord));
                }

                var nextSequence = documentNumberingRecord.DocumentSequence.NextNumber;

                if (nextSequence > documentNumberingRecord.DocumentSequence.MaxNumber)
                {
                    throw new ArgumentException($"Osiągnięto maksymalną wartość dla numeratora dla {type.ToString()}", nameof(documentNumberingRecord));
                }

                var now = dateHelper.GetCurrentDatetime();

                documentNumberingRecord.DocumentSequence.NextNumber++;
                documentNumberingRecord.DocumentSequence.LastModifiedById = modifiedBy.Id;
                documentNumberingRecord.DocumentSequence.LastModifiedDate = now;

                context.DocumentSequences.Update(documentNumberingRecord.DocumentSequence);
                context.SaveChanges();

                context.Entry(documentNumberingRecord).State = EntityState.Detached;
                documentNumberingRecord.DocumentSequence.NextNumber = nextSequence;

                return documentNumberingRecord;
            }
        }
    }
}