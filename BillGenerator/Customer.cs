using System;
using System.Collections.Generic;
using System.Text;

namespace BillGenerator
{
    public class Customer
    {
        public string fullName { get; set; }

        public string billingAddress { get; set; }

        public string phoneNumber { get; set; }

        public int packageCode { get; set; }

        public DateTime registeredDate { get; set; }

        public Customer()
        {

        }
    }
}
