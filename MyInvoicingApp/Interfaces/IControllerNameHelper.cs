namespace MyInvoicingApp.Interfaces
{
    public interface IControllerNameHelper
    {
        string GetControllerName(string controller);

        string GetActionName(string controller, string action);
    }
}