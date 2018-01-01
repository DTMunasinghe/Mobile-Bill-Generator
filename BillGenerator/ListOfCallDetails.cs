using System;
using System.Collections.Generic;
using System.Text;

namespace BillGenerator
{
    public class ListOfCallDetails
    {
        public DateTime startTime { get; set; }

        public int durationInSeconds { get; set; }

        public string destinationNumber { get; set; }

        public double charge { get; set; }
    }
}
