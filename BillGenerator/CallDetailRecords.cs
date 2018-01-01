using System;
using System.Collections.Generic;
using System.Text;

namespace BillGenerator
{
    public class CallDetailRecords
    {
        public string phoneNumberOfCallingParty { get; set; }

        public string phoneNumberOfCalledParty { get; set; }

        public DateTime startingTimeOfTheCall { get; set; }

        public int callDuaration { get; set; }

    }
}
