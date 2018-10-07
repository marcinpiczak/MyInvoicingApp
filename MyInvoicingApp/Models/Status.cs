namespace MyInvoicingApp.Models
{
    public enum Status
    {
        Opened = 1,
        Sent = 2,
        Closed = 4,
        Cancelled = 8,
        Approved = 16,
        Rejected = 32

    }
}