using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IModuleAccessManager
    {
        /// <summary>
        /// Checks whether user have access to action in module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="action"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> CheckModuleActionAccessAsync(Models.Controllers module, Actions action, ApplicationUser user);

        IEnumerable<ModuleAccess> GetUserModuleAccesses(string userId);

        IEnumerable<ModuleAccess> GetRoleModuleAccesses(string roleId);

        ModuleAccess GetUserModuleAccessesForModule(string userId, Models.Controllers module);

        ModuleAccess GetRoleModuleAccessesForModule(string roleId, Models.Controllers module);

        IEnumerable<ModuleAccessViewModel> GetUserModuleAccessViewModels(string userId);

        IEnumerable<ModuleAccessViewModel> GetRoleModuleAccessViewModels(string roleId);

        ModuleAccessViewModel GetUserModuleAccessViewModelsForModule(string userId, Models.Controllers module);

        ModuleAccessViewModel GetRoleModuleAccessViewModelsForModule(string roleId, Models.Controllers module);

        void SaveRoleModuleAccesses(IEnumerable<ModuleAccessViewModel> accesses, ApplicationUser user);
    }
}