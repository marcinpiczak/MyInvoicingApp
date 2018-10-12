using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Helpers;
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
        protected DateHelper DateHelper { get; set; }
        protected FileHelper FileHelper { get; set; }
        protected readonly string AttachmentsFolder = "Attachments";

        public AttachmentManager(EFCDbContext context, IHostingEnvironment environment, UserManager<ApplicationUser> userManager, IInvoiceManager invoiceManager, IBudgetManager budgetManager, DateHelper dateHelper, FileHelper fileHelper)
        {
            Context = context;
            HostingEnvironment = environment;
            UserManager = userManager;
            InvoiceManager = invoiceManager;
            BudgetManager = budgetManager;
            DateHelper = dateHelper;
            FileHelper = fileHelper;
        }

        public async Task<Attachment> Add(AttachmentViewModel model, ApplicationUser createdBy)
        {
            var uniqueFileName = FileHelper.GetUniqueFileName(model.File.FileName);
            //var uploads = Path.Combine(HostingEnvironment.WebRootPath, AttachmentsFolder);
            var uploads = Path.Combine("wwwroot", AttachmentsFolder);
            var filePath = Path.Combine(uploads, uniqueFileName);

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

        public IEnumerable<Attachment> GetAttachmentsForDocument(DocumentType documentType ,string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentNullException("Nieprawidłowe parametry");
            }

            if (documentType == DocumentType.Budget)
            {
                BudgetManager.GetBudgetByIdSimple(documentId);
            }
            else if (documentType == DocumentType.Invoice)
            {
                InvoiceManager.GetInvoiceByIdSimple(documentId);
            }

            var list = Context.Attachments
                .Where(x => x.Status == Status.Opened)
                .Where(x => x.DocumentType == documentType && x.DocumentId == documentId);

            return list;
        }

        public IEnumerable<AttachmentViewModel> GetAttachmentViewModelsForDocument(DocumentType documentType, string documentId)
        {
            var list = GetAttachmentsForDocument(documentType, documentId)
                .Select(x => new AttachmentViewModel(x));

            return list;
        }

        public Attachment GetAttachmentById(string id, DocumentType documentType, string documentId)
        {
            var attachment = Context.Attachments
                .Where(x => x.Status == Status.Opened)
                .SingleOrDefault(x =>x.Id == id && x.DocumentType == documentType && x.DocumentId == documentId);

            if (attachment == null)
            {
                throw new Exception("Nie istnieje załącznik dla podanych parametrów");
            }

            return attachment;
        }

        public AttachmentViewModel GetAttachmentViewModelById(string id, DocumentType documentType, string documentId)
        {
            var attachment = GetAttachmentById(id, documentType, documentId);

            var model = new AttachmentViewModel(attachment);

            return model;
        }

        public Attachment GetAttachmentAndCheckPathForDocumentById(string id, DocumentType documentType, string documentId)
        {
            var attachment = GetAttachmentById(id, documentType, documentId);

            var exists = FileHelper.FileExists(attachment.FilePath);

            if (!exists)
            {
                throw new Exception("Plik nie istnieje");
            }

            return attachment;
        }

        public void RemoveAttachmentForDocumentById(string id, DocumentType documentType, string documentId)
        {
            var attachment = GetAttachmentAndCheckPathForDocumentById(id, documentType, documentId);

            var exists = FileHelper.DeleteFile(attachment.FilePath);

            if (!exists)
            {
                throw new Exception("Nie udało się usunąć pliku");
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