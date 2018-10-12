using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected ICustomerManager CustomerManager { get; set; }

        public CustomerController(UserManager<ApplicationUser> userManager, ICustomerManager customerManager)
        {
            UserManager = userManager;
            CustomerManager = customerManager;
        }

        public IActionResult Index()
        {
            try
            {
                var customerViewModels = CustomerManager.GetCustomerViewModels().OrderByDescending(x => x.CreatedDate);

                return View(customerViewModels);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(CustomerViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerManager.Add(model, CurrentUser);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                //throw;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            try
            {
                var customerViewModel = CustomerManager.GetCustomerViewModelById(id);

                return View(customerViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        public IActionResult Edit(CustomerViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerManager.Edit(model, CurrentUser);


                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                //throw;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Close(string id)
        {
            try
            {
                CustomerManager.ChangeStatus(id, Status.Closed, CurrentUser);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult ReOpen(string id)
        {
            try
            {
                CustomerManager.ChangeStatus(id, Status.Opened, CurrentUser);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            try
            {
                var model = CustomerManager.GetCustomerViewModelById(id);

                var invoiceList = CustomerManager.GetInvoiceViewModels(id);

                model.Invoices = invoiceList.ToList();
            
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
