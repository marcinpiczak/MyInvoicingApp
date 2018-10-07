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

        public async Task Add(AttachmentViewModel model, ApplicationUser createdBy)
        {
            //var result = System.IO.Directory.CreateDirectory(AttachmentsFolder);

            var uniqueFileName = FileHelper.GetUniqueFileName(model.File.FileName);
            var uploads = Path.Combine(HostingEnvironment.WebRootPath, AttachmentsFolder);
            var filePath = Path.Combine(uploads, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await model.File.CopyToAsync(stream);
            }

            var newAttachment = new Attachment()
            {
                CreatedById = createdBy.Id,
                CreatedDate = DateHelper.GetCurrentDatetime(),
                DocumentType = model.DocumentType,
                DocumentId = model.DocumentId,
                FileDescription = model.FileDescription,
                OriginalFileName = model.File.FileName,
                FilePath = filePath
            };

            Context.Attachments.Add(newAttachment);
            var result = Context.SaveChanges();

            if (result == 0)
            {
                throw new Exception("Nie zapisano żadnych danych.");
            }
        }

        public IEnumerable<Attachment> GetAttachmentsForDocumentById(DocumentType documentType ,string documentId)
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
                .Where(x => x.DocumentType == documentType && x.DocumentId == documentId)
                .Where(x => x.Status == Status.Opened);

            return list;
        }

        public IEnumerable<AttachmentViewModel> GetAttachmentViewModelsForDocumentById(DocumentType documentType, string documentId)
        {
            var list = GetAttachmentsForDocumentById(documentType, documentId)
                .Select(x => new AttachmentViewModel(x));

            return list;
        }
    }
}