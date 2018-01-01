using System;
using System.Collections.Generic;
using System.Text;

namespace BillGenerator
{
    public class Bill
    {
        public string fullName { get; set; }

        public string phoneNumber { get; set; }

        public string billingAddress { get; set; }

        public double totalCallCharges { get; set; }

        public double totalDiscount { get; set; }

        public double tax { get; set; }

        public double monthlyRental { get; set; }

        public double billAmount { get; set; }

        public List<ListOfCallDetails> listOfCallRecords { get; set; }
    }
}
