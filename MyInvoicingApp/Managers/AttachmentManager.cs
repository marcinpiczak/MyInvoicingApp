using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Managers
{
    public class AttachmentManager : IManager, IAttachmentManager
    {
        protected EFCDbContext Context { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected IInvoiceManager InvoiceManager { get; set; }
        protected IBudgetManager BudgetManager { get; set; }
        protected IHostingEnvironment HostingEnvironment { get; set; }
        protected IDateHelper DateHelper { get; set; }
        protected IFileHelper FileHelper { get; set; }
        protected readonly string AttachmentsFolder = "Attachments";

        public AttachmentManager(EFCDbContext context, IHostingEnvironment environment, UserManager<ApplicationUser> userManager, IInvoiceManager invoiceManager, IBudgetManager budgetManager, IDateHelper dateHelper, IFileHelper fileHelper)
        {
            Context = context;
            HostingEnvironment = environment;
            UserManager = userManager;
            InvoiceManager = invoiceManager;
            BudgetManager = budgetManager;
            DateHelper = dateHelper;
            FileHelper = fileHelper;
        }

        /// <summary>
        /// Saves attachment in Attachments folder and creates entry in attachments table in database
        /// </summary>
        /// <param name="model">An AttachmentViewModel with all data passed from form: file and description</param>
        /// <param name="createdBy">ApplicationUser which is adding attachment</param>
        /// <returns>created attachment model from ViewModel</returns>
        public async Task<Attachment> Add(AttachmentViewModel model, ApplicationUser createdBy)
        {
            var uniqueFileName = FileHelper.GetUniqueFileName(model.File.FileName);
            //var uploads = Path.Combine(HostingEnvironment.WebRootPath, AttachmentsFolder);
            //var uploads = Path.Combine("wwwroot", AttachmentsFolder);
            var filePath = Path.Combine(AttachmentsFolder, uniqueFileName);

            await FileHelper.SaveFile(model, filePath);

            var newAttachment = new Attachment()
            {
                CreatedById = createdBy.Id,
                CreatedDate = DateHelper.GetCurrentDatetime(),
                DocumentType = model.DocumentType,
                DocumentId = model.DocumentId,
                FileDescription = model.FileDescription,
                OriginalFileName = model.File.FileName,
                FilePath = filePath,
                ContentType = model.File.ContentType
            };

            Context.Attachments.Add(newAttachment);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }

            return newAttachment;
        }

        /// <summary>
        /// Gets list of all attachments model for given document type and document Id.
        /// </summary>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>List of attachment models</returns>
        public IEnumerable<Attachment> GetAttachmentsForDocument(DocumentType documentType ,string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentNullException(nameof(documentId) ,"Nieprawidłowe parametry");
            }

            if (documentType == DocumentType.Budget)
            {
                BudgetManager.GetBudgetById(documentId, IncludeLevel.None);
            }
            else if (documentType == DocumentType.Invoice)
            {
                InvoiceManager.GetInvoiceById(documentId, IncludeLevel.None);
            }

            var attachments = Context.Attachments
                .Where(x => x.Status == Status.Opened)
                .Where(x => x.DocumentType == documentType && x.DocumentId == documentId);

            return attachments;
        }

        /// <summary>
        /// Gets list of all attachment View Models for given document type and document Id.
        /// </summary>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>List of attachment View Models</returns>
        public IEnumerable<AttachmentViewModel> GetAttachmentViewModelsForDocument(DocumentType documentType, string documentId)
        {
            var attachments = GetAttachmentsForDocument(documentType, documentId)
                .Select(x => new AttachmentViewModel(x));

            return attachments;
        }

        /// <summary>
        /// Gets attachment model for given attachment id, document type and document Id.
        /// </summary>
        /// <param name="id">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <param name="checkIfExists">Checks if attachment exists in Attachment folder</param>
        /// <returns>Attachment model for given attachment id, document type and document Id</returns>
        public Attachment GetAttachmentById(string id, DocumentType documentType, string documentId, bool checkIfExists)
        {
            var attachment = Context.Attachments
                .Where(x => x.Status == Status.Opened)
                .SingleOrDefault(x =>x.Id == id && x.DocumentType == documentType && x.DocumentId == documentId);

            if (attachment == null)
            {
                throw new Exception("Nie istnieje załącznik dla podanych parametrów");
            }

            if (checkIfExists)
            {
                var exists = FileHelper.FileExists(attachment.FilePath);

                if (!exists)
                {
                    throw new Exception("Plik nie istnieje");
                }
            }

            return attachment;
        }

        /// <summary>
        /// Gets attachment View Model for given attachment id, document type and document Id.
        /// </summary>
        /// <param name="id">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        /// <returns>Attachment View Model for given attachment id, document type and document Id</returns>
        public AttachmentViewModel GetAttachmentViewModelById(string id, DocumentType documentType, string documentId)
        {
            var attachment = GetAttachmentById(id, documentType, documentId, false);

            var model = new AttachmentViewModel(attachment);

            return model;
        }

        /// <summary>
        /// Removes attachment from Attachment folder and changes status of database entry to Closed
        /// </summary>
        /// <param name="id">Attachment Id for attachment</param>
        /// <param name="documentType">Document type for attachment</param>
        /// <param name="documentId">Document Id for attachment</param>
        public void RemoveAttachmentForDocumentById(string id, DocumentType documentType, string documentId)
        {
            var attachment = GetAttachmentById(id, documentType, documentId, true);

            var exists = FileHelper.DeleteFile(attachment.FilePath);

            if (!exists)
            {
                throw new Exception("Plik nie istnieje lub nie udało się usunąć pliku");
            }

            attachment.Status = Status.Closed;

            Context.Attachments.Update(attachment);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }
        }
    }
}