using System.Collections.Generic;
using System.Threading.Tasks;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IAttachmentManager
    {
        Task Add(AttachmentViewModel model, ApplicationUser createdBy);

        IEnumerable<Attachment> GetAttachmentsForDocumentById(DocumentType documentType, string documentId);

        IEnumerable<AttachmentViewModel> GetAttachmentViewModelsForDocumentById(DocumentType documentType, string documentId);
    }
}