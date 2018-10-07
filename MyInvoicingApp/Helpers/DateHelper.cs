using System;

namespace MyInvoicingApp.Helpers
{
    public class DateHelper
    {
        /// <summary>
        /// Zwraca aktualną datę
        /// </summary>
        /// <returns>DateTime.Now</returns>
        public DateTime GetCurrentDatetime()
        {
            var currentDateTime = DateTime.Now;

            return currentDateTime;
        }
    }
}