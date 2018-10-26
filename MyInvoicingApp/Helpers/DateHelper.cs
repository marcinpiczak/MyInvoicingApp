using System;
using MyInvoicingApp.Interfaces;

namespace MyInvoicingApp.Helpers
{
    public class DateHelper : IDateHelper
    {
        /// <summary>
        /// Returns current date with time
        /// </summary>
        /// <returns>DateTime.Now</returns>
        public DateTime GetCurrentDatetime()
        {
            var currentDateTime = DateTime.Now;

            return currentDateTime;
        }
    }
}