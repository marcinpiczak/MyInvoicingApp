﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyInvoicingApp.Interfaces;
using MyInvoicingApp.Models;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Controllers
{
    [Authorize(Roles = "Admin,Accountant,Manager")]
    public class CustomerController : Controller
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser CurrentUser => UserManager.Users.First(x => x.UserName == User.Identity.Name);
        protected ICustomerManager CustomerManager { get; set; }
        protected IInvoiceManager InvoiceManager { get; set; }

        public CustomerController(UserManager<ApplicationUser> userManager, ICustomerManager customerManager, IInvoiceManager invoiceManager)
        {
            UserManager = userManager;
            CustomerManager = customerManager;
            InvoiceManager = invoiceManager;
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
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas wyświetlania listy klientów: {e.Message}{innerMessage}";

                return RedirectToAction("Index", "Home");
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
                    var result = CustomerManager.Add(model, CurrentUser);

                    TempData["Success"] = $"Dodano nowego klienta <b>{result.Name}</b>";
                    //return RedirectToAction("Index");
                    return RedirectToAction("Add");
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas dodawania nowego klienta: {e.Message}{innerMessage}";
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
                //Console.WriteLine(e.Message);
                //throw;
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                TempData["Error"] = $"Wystąpił problem podczas edytowania budżetu: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(CustomerViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = CustomerManager.Edit(model, CurrentUser);

                    TempData["Success"] = $"Zapisano wprowadzone zmiany dla klienta <b>{result.Name}</b>";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                ModelState.AddModelError("", e.Message + innerMessage);
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas zapisywania zmian dla klienta: {e.Message}{innerMessage}";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Close(string id)
        {
            try
            {
                var result = CustomerManager.ChangeStatus(id, Status.Closed, CurrentUser);
                TempData["Success"] = $"Klient <b>{result.Name}</b> został zamknięty";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas zamykania budżetu: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ReOpen(string id)
        {
            try
            {
                var result = CustomerManager.ChangeStatus(id, Status.Opened, CurrentUser);
                TempData["Success"] = $"Klient <b>{result.Name}</b> został otwarty";
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas otwierania klienta: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            try
            {
                var model = CustomerManager.GetCustomerViewModelById(id);

                model.Invoices = InvoiceManager.GetInvoiceViewModelsForCustomer(id).ToList();
            
                return View(model);
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException == null ? "" : $": {e.InnerException.Message}";
                //throw;
                TempData["Error"] = $"Wystąpił problem podczas wyświelania szczegółów klienta: {e.Message}{innerMessage}";
            }

            return RedirectToAction("Index");
        }
    }
}
