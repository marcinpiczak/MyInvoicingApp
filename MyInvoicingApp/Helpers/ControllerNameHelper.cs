using System.Collections.Generic;
using MyInvoicingApp.Interfaces;

namespace MyInvoicingApp.Helpers
{
    public class ControllerNameHelper : IControllerNameHelper
    {
        private Dictionary<string, string> _controllers { get; } = new Dictionary<string, string>()
        {
            {"Invoice", "Faktury" },
            {"Budget", "Budżety" },
            {"Customer", "Klienci" },
            {"Role", "Stanowiska" },
            {"ModuleAccess", "Uprawnienia do modułów" },
            {"Account", "Konta" }

        };

        private Dictionary<string, string> _actions { get; } = new Dictionary<string, string>()
        {
            {"Index", "Lista" },
            {"Add", "Dodawanie" },
            {"Edit", "Edycja" },
            {"Details", "Szczegóły" },
            {"AddLine", "Dodawania linii" },
            {"AsignUser", "Przypisanie użytkownika" },
            {"Login", "Logowanie" },
            {"Register", "Rejesracja" },
            {"ChangeRoleAccess", "Zmiana uprawnień dla stanowisk" },
        };

        /// <summary>
        /// Returns localised name of the controller
        /// </summary>
        /// <param name="controller">Controller name</param>
        /// <returns>localized name of the controller</returns>
        public string GetControllerName(string controller)
        {
            if (string.IsNullOrWhiteSpace(controller))
            {
                return "Nieznany";
            }

            var exists = _controllers.TryGetValue(controller, out string name);

            if (exists)
            {
                return name;
            }

            return controller;
        }

        /// <summary>
        /// Returns localised name of the controllers action
        /// </summary>
        /// <param name="controller">Controller name</param>
        /// <param name="action">Action name</param>
        /// <returns>localized name of the controllers action</returns>
        public string GetActionName(string controller, string action = null)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return "Nieznana";
            }

            var exists = _actions.TryGetValue(action, out string name);

            if (exists)
            {
                return name;
            }

            return action;
        }
    }
}