using System;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.Interfaces
{
    public interface IDataAccessManagerOld
    {
        bool CanView<T>(T item, ApplicationUser user);

        bool CanEdit<T>(T item, ApplicationUser user);

        bool CanClose<T>(T item, ApplicationUser user);

        bool CanOpen<T>(T item, ApplicationUser user);
    }

    public class DataAccessManagerOld : IDataAccessManagerOld
    {
        public Func<Budget, ApplicationUser, bool> BudgetViewCondition = (budget, user) => budget.Owner == user;


        public bool CanView<T>(T item, ApplicationUser user)
        {
            if (item is Budget)
            {
                var budget = item as Budget;

                return BudgetViewCondition.Invoke(budget, user);
            }

            return false;
        }

        public bool CanEdit<T>(T item, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public bool CanClose<T>(T item, ApplicationUser user)
        {
            throw new NotImplementedException();
        }
        
        public bool CanOpen<T>(T item, ApplicationUser user)
        {
            throw new NotImplementedException();
        }
        
    }
}