using System.Collections.Generic;
using System.Threading.Tasks;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IAttachmentManager
    {
        Task<Attachment> Add(AttachmentViewModel model, ApplicationUser createdBy);

        IEnumerable<Attachment> GetAttachmentsForDocument(DocumentType documentType, string documentId);

        IEnumerable<AttachmentViewModel> GetAttachmentViewModelsForDocument(DocumentType documentType, string documentId);

        Attachment GetAttachmentById(string id, DocumentType documentType, string documentId);

        AttachmentViewModel GetAttachmentViewModelById(string id, DocumentType documentType, string documentId);

        Attachment GetAttachmentAndCheckPathForDocumentById(string id, DocumentType documentType, string documentId);

        void RemoveAttachmentForDocumentById(string id, DocumentType documentType, string documentId);
    }
}