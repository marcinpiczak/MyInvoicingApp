using Microsoft.AspNetCore.Mvc;

namespace MyInvoicingApp.Interfaces
{
    public interface IPdfManager
    {
        FileStreamResult GetInvoicePdf(string invoiceId);
    }
}