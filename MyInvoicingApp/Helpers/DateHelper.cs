using System;

namespace MyInvoicingApp.Helpers
{
    public class DateHelper
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