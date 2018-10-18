using System.Collections.Generic;
using System.Threading.Tasks;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IAttachmentManager
    {
        /// <summary>
        /// Saves attachment in Attachments folder and creates entry in attachments table in database
        /// </summary>
        /// <param name="model">An AttachmentViewModel with all data passed from form: file and description</param>
        /// <param name="createdBy">ApplicationUser which is adding attachment</param>
        /// <returns>created attachment model from ViewModel</returns>
        Task<Attachment> Add(AttachmentViewModel model, ApplicationUser createdBy);

        /// <summary>
        /// Gets list of all attachments model for given document type and document Id.
        /// </summary>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>List of attachment models</returns>
        IEnumerable<Attachment> GetAttachmentsForDocument(DocumentType documentType, string documentId);

        /// <summary>
        /// Gets list of all attachment View Models for given document type and document Id.
        /// </summary>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>List of attachment View Models</returns>
        IEnumerable<AttachmentViewModel> GetAttachmentViewModelsForDocument(DocumentType documentType, string documentId);

        /// <summary>
        /// Gets attachment model for given attachment id, document type and document Id.
        /// </summary>
        /// <param name="id">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <param name="checkIfExists">Checks if attachment exists in Attachment folder</param>
        /// <returns>Attachment model for given attachment id, document type and document Id</returns>
        Attachment GetAttachmentById(string id, DocumentType documentType, string documentId, bool checkIfExists);

        /// <summary>
        /// Gets attachment View Model for given attachment id, document type and document Id.
        /// </summary>
        /// <param name="id">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>Attachment View Model for given attachment id, document type and document Id</returns>
        AttachmentViewModel GetAttachmentViewModelById(string id, DocumentType documentType, string documentId);

        /// <summary>
        /// Removes attachment from Attachment folder and changes status of database entry to Closed
        /// </summary>
        /// <param name="id">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        void RemoveAttachmentForDocumentById(string id, DocumentType documentType, string documentId);
    }
}