using System.Collections.Generic;

namespace MyInvoicingApp.Helpers
{
    public class ControllerNameHelper
    {
        private Dictionary<string, string> _controllers { get; } = new Dictionary<string, string>()
        {
            {"Invoice", "Faktury" },
            {"Budget", "Budżety" },
            {"Customer", "Klienci" },
            {"Role", "Stanowiska" }

        };

        private Dictionary<string, string> _actions { get; } = new Dictionary<string, string>()
        {
            {"Index", "Lista" },
            {"Add", "Dodawanie" },
            {"Edit", "Edycja" },
            {"Details", "Szczegóły" },
            {"AddLine", "Dodawania linii" },
            {"AsignUser", "Przypisanie użytkownika" }

        };


        public string GetName(string controller, string action = null)
        {
            if (string.IsNullOrWhiteSpace(controller))
            {
                return "Nieznany";
            }

            if (string.IsNullOrWhiteSpace(action))
            {
                var exists = _controllers.TryGetValue(controller, out string name);

                if (exists)
                {
                    return name;
                }

                return controller;
            }
            else
            {
                var exists = _actions.TryGetValue(action, out string name);

                if (exists)
                {
                    return name;
                }

                return action;
            }
        }
    }
}