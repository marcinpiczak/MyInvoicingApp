namespace MyInvoicingApp.Models
{
    public enum IncludeLevel
    {
        None = 1,
        Level1 = 2,
        Level2 = 4,
        Level3 = 8,
        All = 16
    }
}