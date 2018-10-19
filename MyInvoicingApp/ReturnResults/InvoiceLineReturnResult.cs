namespace MyInvoicingApp.ReturnResults
{
    public class InvoiceLineReturnResult
    {
        public string Id { get; set; }

        public string InvoiceId { get; set; }

        public string InvoiceNumber { get; set; }

        public int LineNumber { get; set; }

        public string Status { get; set; }
    }
}