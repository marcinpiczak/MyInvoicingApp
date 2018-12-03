namespace MyInvoicingApp.Models
{
    public enum Actions
    {
        Index = 1,
        Add = 2,
        Edit = 4,
        Close = 8,
        Open = 16,
        Cancel = 32,
        Send = 64,
        Details = 128,
        Approve = 256,
        Remove = 512
    }
}