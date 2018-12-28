namespace MyInvoicingApp.ViewModels
{
    public class AccessViewModel
    {
        public bool CanView { get; set; }

        public bool CanEdit { get; set; }

        public bool CanClose { get; set; }

        public bool CanOpen { get; set; }

        public bool CanCancel { get; set; }

        public bool CanApprove { get; set; }

        public bool CanReject { get; set; }

        public bool CanSentToApprove { get; set; }
    }
}