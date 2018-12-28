using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyInvoicingApp.Contexts;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Managers
{
    public class ModuleAccessManager : IManager, IModuleAccessManager
    {
        protected EFCDbContext Context { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected RoleManager<ApplicationRole> RoleManager { get; set; }
        protected IDateHelper DateHelper { get; set; }

        public ModuleAccessManager(EFCDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IDateHelper dateHelper)
        {
            Context = context;
            UserManager = userManager;
            RoleManager = roleManager;
            DateHelper = dateHelper;
        }

        public async Task<bool> CheckModuleActionAccessAsync(Models.Controllers module, Actions action, ApplicationUser user)
        {
            //pobranie nazw rol do ktorych przypisany jest uzytkownik
            var userRoleNames = await UserManager.GetRolesAsync(user);

            //pobranie obiektow ApplicationRole na podstawie nazw rol do ktorych nalezy uzytkownik
            var roles = RoleManager.Roles.Where(x => userRoleNames.Any(y => y == x.Name));

            List<ModuleAccess> access = new List<ModuleAccess>();

            //wypelnienia listy obiektami ModuleAccess zawierajacymi informacje o dostepie do modulow i akcji dla rol
            foreach (var role in roles)
            {
                var roleModuleAccess = GetRoleModuleAccessesForModule(role.Id, module);

                if (roleModuleAccess != null)
                {
                    access.Add(roleModuleAccess);
                }
            }

            //wypelnienia listy obiektami ModuleAccess zawierajacymi informacje o dostepie do modulow i akcji dla uzytkownika
            var userModuleAccess = GetUserModuleAccessesForModule(user.Id, module);

            if (userModuleAccess != null)
            {
                access.Add(userModuleAccess);
            }

            if (!access.Any())
            {
                return false;
            }

            //sprawdzenie czy jakakolwiek obiekt na liscie posiada wlasnosc action ustawiona na true
            var result = access.Any(x => (bool)x.GetType().GetProperty(action.ToString()).GetValue(x));

            return result;
        }

        public IEnumerable<ModuleAccess> GetUserModuleAccesses(string userId)
        {
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("Użytkownik o pdanym Id nie istnieje");
            }

            var access = Context.UserModuleAccesses
                //.Include(x => x.User)
                .Where(x => x.AccessorId == userId);

            return access;
        }

        public IEnumerable<ModuleAccessViewModel> GetUserModuleAccessViewModels(string userId)
        {
            var access = GetUserModuleAccesses(userId)
                .Select(x => new ModuleAccessViewModel(x));

            return access;
        }

        public ModuleAccess GetUserModuleAccessesForModule(string userId, Models.Controllers module)
        {
            var user = UserManager.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("Użytkownik o pdanym Id nie istnieje");
            }

            var access = Context.UserModuleAccesses
                .Where(x => x.AccessorId == userId && x.Module == module);

            if (access.Count() > 1)
            {
                throw new Exception($"Błąd danych - dla użytkownika {user.UserName} istnieje więcej niż jeden rekord z uprawnieniami dla modułu {module.ToString()}");
            }

            user = null;

            return access.FirstOrDefault();
        }

        public ModuleAccessViewModel GetUserModuleAccessViewModelsForModule(string userId, Models.Controllers module)
        {
            var access = GetUserModuleAccessesForModule(userId, module);

            var model = access == null ? null : new ModuleAccessViewModel(access);

            return model;
        }

        public IEnumerable<ModuleAccess> GetRoleModuleAccesses(string roleId)
        {
            var role = RoleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            if (role == null)
            {
                throw new ArgumentException("Rola/stanowisko o pdanym Id nie istnieje");
            }

            role = null;

            var access = Context.RoleModuleAccesses
                //.Include(x => x.Role)
                .Where(x => x.AccessorId == roleId);

            return access;
        }

        public IEnumerable<ModuleAccessViewModel> GetRoleModuleAccessViewModels(string roleId)
        {
            var access = GetRoleModuleAccesses(roleId)
                .Select(x => new ModuleAccessViewModel(x));

            return access;
        }

        public ModuleAccess GetRoleModuleAccessesForModule(string roleId, Models.Controllers module)
        {
            var role = RoleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            if (role == null)
            {
                throw new ArgumentException("Rola/stanowisko o pdanym Id nie istnieje");
            }

            var access = Context.RoleModuleAccesses
                .Where(x => x.AccessorId == roleId && x.Module == module);

            if (access.Count() > 1)
            {
                throw new Exception($"Błąd danych - dla roli {role.Name} istnieje więcej niż jeden rekord z uprawnieniami dla modułu {module.ToString()}");
            }

            role = null;

            return access.FirstOrDefault();
        }

        public ModuleAccessViewModel GetRoleModuleAccessViewModelsForModule(string roleId, Models.Controllers module)
        {
            var access = GetRoleModuleAccessesForModule(roleId, module);

            var model = access == null ? null : new ModuleAccessViewModel(access);

            return model;
        }

        public void SaveRoleModuleAccesses(IEnumerable<ModuleAccessViewModel> accesses, ApplicationUser user)
        {
            //pobranie dostepow tylko dla rol
            var rolesAccesses = accesses.Where(x => x.AccessorType == AccessorType.Role);

            //pobranie unikalnych id rol
            var rolesId = rolesAccesses.Select(x => x.AccessorId).Distinct();

            if (rolesId.Any())
            {
                //sprawdzenie czy role o podanych id istnieja
                foreach (var roleId in rolesId)
                {
                    var role = RoleManager.Roles.SingleOrDefault(x => x.Id == roleId);

                    if (role == null)
                    {
                        throw new ArgumentException($"Rola/stanowisko o pdanym Id: \"{roleId}\" nie istnieje");
                    }
                }

                //sprawdzenie przeslanych dostepow dla rol
                foreach (var roleAccess in rolesAccesses)
                {
                    //sprawdzenie czy istnieje wpis dla podanych: id, id roli i modulu 
                    var moduleAccess = Context.RoleModuleAccesses.SingleOrDefault(x => x.Id == roleAccess.Id && x.AccessorId == roleAccess.AccessorId && x.Module == roleAccess.Module);

                    if (moduleAccess == null)
                    {
                        //weryfikacja czy dla id roli i modulu istnieje jakis wpis
                        var entryForModule = GetRoleModuleAccessesForModule(roleAccess.AccessorId, roleAccess.Module);

                        if (entryForModule != null)
                        {
                            throw new ArgumentException($"Wpis dla roli o id {roleAccess.AccessorId} i modułu {roleAccess.Module.ToString()} już istnieje ale pod innym id {entryForModule.Id}");
                        }

                        //utworzenie nowego wpisu i jego dodanie
                        var model = new RoleModuleAccess()
                        {
                            Status = Status.Opened,
                            CreatedById = user.Id,
                            CreatedDate = DateHelper.GetCurrentDatetime(),
                            AccessorId = roleAccess.AccessorId,
                            Module = roleAccess.Module,
                            Index = roleAccess.Index,
                            Add = roleAccess.Add,
                            Edit = roleAccess.Edit,
                            Close = roleAccess.Close,
                            Open = roleAccess.Open,
                            Cancel = roleAccess.Cancel,
                            Send = roleAccess.Send,
                            Details = roleAccess.Details,
                            Approve = roleAccess.Approve,
                            Remove = roleAccess.Remove
                        };

                        Context.RoleModuleAccesses.Add(model);
                    }
                    else
                    {
                        //aktualizacja istniejacego wpisu o dane z modelu
                        moduleAccess.LastModifiedById = user.Id;
                        moduleAccess.LastModifiedDate = DateHelper.GetCurrentDatetime();
                        moduleAccess.Index = roleAccess.Index;
                        moduleAccess.Add = roleAccess.Add;
                        moduleAccess.Edit = roleAccess.Edit;
                        moduleAccess.Close = roleAccess.Close;
                        moduleAccess.Open = roleAccess.Open;
                        moduleAccess.Cancel = roleAccess.Cancel;
                        moduleAccess.Send = roleAccess.Send;
                        moduleAccess.Details = roleAccess.Details;
                        moduleAccess.Approve = roleAccess.Approve;
                        moduleAccess.Remove = roleAccess.Remove;

                        Context.RoleModuleAccesses.Update(moduleAccess);
                    }
                }

                Context.SaveChanges();
            }
        }
    }
}