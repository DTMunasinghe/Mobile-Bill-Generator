using System;
using System.Collections.Generic;
using System.IO;

namespace BillGenerator
{
    public class CreateCallDetailRecords
    {
        private TimeSpan peakStartingTime;
        private TimeSpan peakEndingTime;
        private List<ListOfCallDetails> listOfCallDetailsOfPackages;

        public CreateCallDetailRecords()
        {
            peakStartingTime = new TimeSpan(8, 0, 0);
            peakEndingTime = new TimeSpan(20, 0, 0);
            listOfCallDetailsOfPackages = new List<ListOfCallDetails>();
        }

        public void SetPeakAndOffPeakHours(TimeSpan startingTime, TimeSpan EndingTime)
        {
            peakStartingTime = startingTime;
            peakEndingTime = EndingTime;
        }

        public List<int> GetCallDurations(string filePath)
        {
            var callDurations = new List<int>();
            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // skip the heading

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(',');
                    callDurations.Add(int.Parse(line[3]));
                }
            }
            return callDurations;
        }

        public List<CallDetailRecords> GetCallDetailRecords()
        {
            var callDetailRecords = new List<CallDetailRecords>();
            string fileName = "CDR.csv";
            using (var reader = new StreamReader(fileName))
            {
                string line;
                reader.ReadLine(); //skip the heading

                while ((line = reader.ReadLine()) != null)
                {
                    String[] tokens = line.Split(',');
                    DateTime.TryParse(tokens[2], out DateTime dateAndTime);

                    CallDetailRecords callDetailRecord = new CallDetailRecords
                    {
                        phoneNumberOfCallingParty = tokens[0],
                        phoneNumberOfCalledParty = tokens[1],
                        startingTimeOfTheCall = dateAndTime,
                        callDuaration = int.Parse(tokens[3])
                    };
                    callDetailRecords.Add(callDetailRecord);
                }
                return callDetailRecords;
            }
        }

        public List<CallDetailRecords> GetCallRecords(string phoneNumber)
        {
            var calllRecords = new List<CallDetailRecords>();
            string fileName = "CDR.csv";

            using (var reader = new StreamReader(fileName))
            {
                string line;
                reader.ReadLine();  //skip the heading

                while ((line = reader.ReadLine()) != null)
                {
                    String[] tokens = line.Split(',');
                    DateTime.TryParse(tokens[2], out DateTime dateAndTime);

                    if (tokens[0] == phoneNumber)
                    {
                        CallDetailRecords callDetailRecords = new CallDetailRecords
                        {
                            phoneNumberOfCallingParty = tokens[0],
                            phoneNumberOfCalledParty = tokens[1],
                            startingTimeOfTheCall = dateAndTime,
                            callDuaration = int.Parse(tokens[3])
                        };
                        calllRecords.Add(callDetailRecords);
                    }
                }
            }
            return calllRecords;
        }

        public bool IsLocalCall(string phoneNumberOfCallingParty, string phoneNumberOfCalledParty)
        {
            string sub1 = phoneNumberOfCallingParty.Substring(0, 3);
            string sub2 = phoneNumberOfCalledParty.Substring(0, 3);

            if (sub1.Equals(sub2))
            {
                return true;
            }
            return false;
        }

        public double CalculateTotalChargePerMinute(string callersPhoneNumber, List<CallDetailRecords> callDetailRecords)
        {
            int perMinuteCharge = 0;
            int perMinuteChargePeakTime = 0;
            int perMinuteChargeOffPeakTime = 0;
            double totalCharge = 0;
            CreateCustomer customer = new CreateCustomer();

            foreach (CallDetailRecords cdr in callDetailRecords)
            {
                double callCharge = 0;
                double peakTimeCharge = 0;
                double offPeakTimeCharge = 0;
                string packageCode = customer.GetPackageCode(callersPhoneNumber);
                bool isLocalCall = IsLocalCall(cdr.phoneNumberOfCallingParty, cdr.phoneNumberOfCalledParty);
                double durationInMinutes = Math.Ceiling(cdr.callDuaration / 60.0);
                TimeSpan callStartTime = cdr.startingTimeOfTheCall.TimeOfDay;
                TimeSpan callEndTime = callStartTime.Add(TimeSpan.FromSeconds(cdr.callDuaration));

                //Call inside the peak time
                if (callStartTime >= peakStartingTime && callEndTime < peakEndingTime)
                {
                    //local call 
                    if (isLocalCall)
                    {
                        if (packageCode == "A")
                        {
                            perMinuteCharge = 3;
                        }
                        else
                        {
                            perMinuteCharge = 2;
                        }
                        callCharge = durationInMinutes * perMinuteCharge;
                    }
                    else
                    {
                        if (packageCode == "A")
                        {
                            perMinuteCharge = 5;
                        }
                        else
                        {
                            perMinuteCharge = 3;
                        }
                        callCharge = durationInMinutes * perMinuteCharge;
                    }
                    ListOfCallDetails callDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(callDetails);

                    totalCharge = totalCharge + callCharge;
                }
                //Call inside the off peak time
                else if ((callStartTime >= peakEndingTime && callEndTime < peakStartingTime) ||
                        (callStartTime < peakStartingTime && callEndTime < peakStartingTime) ||
                        (callStartTime >= peakEndingTime && callEndTime > peakEndingTime))
                {
                    if (isLocalCall)
                    {
                        if (packageCode == "A")
                        {
                            perMinuteCharge = 2;
                        }
                        else
                        {
                            perMinuteCharge = 1;
                        }
                        callCharge = durationInMinutes * perMinuteCharge;
                    }
                    else
                    {
                        if (packageCode == "A")
                        {
                            perMinuteCharge = 4;
                        }
                        else
                        {
                            perMinuteCharge = 2;
                        }
                        callCharge = durationInMinutes * perMinuteCharge;
                    }
                    ListOfCallDetails callDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(callDetails);

                    totalCharge = totalCharge + callCharge;
                }
                //call start at peak time and call end at off peak time
                else if ((callStartTime >= peakStartingTime && callStartTime < peakEndingTime) && (callEndTime >= peakEndingTime && callEndTime < peakStartingTime))
                {
                    TimeSpan callTimeInPeakTime = peakEndingTime - callStartTime;
                    TimeSpan callTimeInOffPeakTime = callEndTime - peakEndingTime;

                    if (isLocalCall)
                    {
                        if (packageCode == "A")
                        {
                            perMinuteChargePeakTime = 3; perMinuteChargeOffPeakTime = 2;
                        }
                        else
                        {
                            perMinuteChargePeakTime = 2; perMinuteChargeOffPeakTime = 1;
                        }
                        peakTimeCharge = Math.Ceiling(callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                        offPeakTimeCharge = Math.Ceiling(callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                    }
                    else
                    {
                        if (packageCode == "A")
                        {
                            perMinuteChargePeakTime = 5; perMinuteChargeOffPeakTime = 4;
                        }
                        else
                        {
                            perMinuteChargePeakTime = 3; perMinuteChargeOffPeakTime = 2;
                        }
                        peakTimeCharge = Math.Ceiling(callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                        offPeakTimeCharge = Math.Ceiling(callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                    }
                    ListOfCallDetails callDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(callDetails);

                    totalCharge = totalCharge + callCharge;
                }
                //call start at off peak time and call end at peak time
                else
                {
                    TimeSpan callTimeInOffPeakTime = peakStartingTime - callStartTime;
                    TimeSpan callTimeInPeakTime = callEndTime - peakStartingTime;

                    if (isLocalCall)
                    {
                        if (packageCode == "A")
                        {
                            perMinuteChargePeakTime = 3; perMinuteChargeOffPeakTime = 2;
                        }
                        else
                        {
                            perMinuteChargePeakTime = 2; perMinuteChargeOffPeakTime = 1;
                        }
                        peakTimeCharge = Math.Ceiling(callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                        offPeakTimeCharge = Math.Ceiling(callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                    }
                    else
                    {
                        if (packageCode == "A")
                        {
                            perMinuteChargePeakTime = 5; perMinuteChargeOffPeakTime = 4;
                        }
                        else
                        {
                            perMinuteChargePeakTime = 3; perMinuteChargeOffPeakTime = 2;
                        }
                        peakTimeCharge = Math.Ceiling(callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                        offPeakTimeCharge = Math.Ceiling(callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                    }
                    ListOfCallDetails callDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(callDetails);

                    totalCharge = totalCharge + callCharge;
                }
            }
            return totalCharge;
        }

        public double CalculateTotalChargePerSecond(string callersPhoneNumber, List<CallDetailRecords> callDetailRecords)
        {
            int perMinuteCharge = 0;
            int perMinuteChargePeakTime = 0;
            int perMinuteChargeOffPeakTime = 0;
            double totalCharge = 0;
            CreateCustomer customer = new CreateCustomer();

            foreach (CallDetailRecords cdr in callDetailRecords)
            {
                double callCharge = 0;
                double peakTimeCharge = 0;
                double offPeakTimeCharge = 0;
                string packageCode = customer.GetPackageCode(callersPhoneNumber);
                bool isLocalCall = IsLocalCall(cdr.phoneNumberOfCallingParty, cdr.phoneNumberOfCalledParty);
                TimeSpan callStartTime = cdr.startingTimeOfTheCall.TimeOfDay;
                TimeSpan callEndTime = callStartTime.Add(TimeSpan.FromSeconds(cdr.callDuaration));
                double callDurationInMinutes = cdr.callDuaration / 60.0;

                //Call inside the peak time
                if (callStartTime >= peakStartingTime && callEndTime < peakEndingTime)
                {
                    //local call 
                    if (isLocalCall)
                    {
                        if (packageCode == "B")
                        {
                            perMinuteCharge = 4;
                        }
                        else
                        {
                            perMinuteCharge = 3;
                        }
                        callCharge = callDurationInMinutes * perMinuteCharge;
                        callCharge = Math.Round(callCharge, 2);
                    }
                    else
                    {
                        if (packageCode == "B")
                        {
                            perMinuteCharge = 6;
                        }
                        else
                        {
                            perMinuteCharge = 5;
                        }
                        callCharge = callDurationInMinutes * perMinuteCharge;
                        callCharge = Math.Round(callCharge, 2);
                    }
                    ListOfCallDetails listOfCallDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(listOfCallDetails);

                    totalCharge = totalCharge + callCharge;
                }
                //Call inside the off peak time
                else if ((callStartTime >= peakEndingTime && callEndTime < peakStartingTime) || 
                        (callStartTime < peakStartingTime && callEndTime < peakStartingTime) || 
                        (callStartTime >= peakEndingTime && callEndTime > peakEndingTime))
                {
                    if (isLocalCall)
                    {
                        if (packageCode == "B")
                        {
                            if (cdr.callDuaration >= 60)
                            {
                                perMinuteCharge = 3;
                                callCharge = callDurationInMinutes * perMinuteCharge - perMinuteCharge;
                            }
                            else
                            {
                                perMinuteCharge = 0;
                                callCharge = callDurationInMinutes * perMinuteCharge;
                            }
                        }
                        else
                        {
                            perMinuteCharge = 2;
                            callCharge = callDurationInMinutes * perMinuteCharge;
                        }
                        callCharge = Math.Round(callCharge, 2);
                    }
                    else
                    {
                        if (packageCode == "B")
                        {
                            perMinuteCharge = 5;
                        }
                        else
                        {
                            perMinuteCharge = 4;
                        }
                        callCharge = callDurationInMinutes * perMinuteCharge;
                        callCharge = Math.Round(callCharge, 2);
                    }
                    ListOfCallDetails listOfCallDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(listOfCallDetails);

                    totalCharge = totalCharge + callCharge;
                }
                //call start at peak time and call end at off peak time
                else if ((callStartTime >= peakStartingTime && callStartTime < peakEndingTime) && (callEndTime >= peakEndingTime && callEndTime < peakStartingTime))
                {
                    TimeSpan callTimeInPeakTime = peakEndingTime - callStartTime;
                    TimeSpan callTimeInOffPeakTime = callEndTime - peakEndingTime;
                    
                    if (isLocalCall)
                    {
                        if (packageCode == "B")
                        {
                            perMinuteChargePeakTime = 4; perMinuteChargeOffPeakTime = 3;
                        }
                        else
                        {
                            perMinuteChargePeakTime = 3; perMinuteChargeOffPeakTime = 2;
                        }
                        peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                        offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                        callCharge = Math.Round(callCharge, 2);
                    }
                    else
                    {
                        if (packageCode == "B")
                        {
                            perMinuteChargePeakTime = 6; perMinuteChargeOffPeakTime = 5;
                        }
                        else
                        {
                            perMinuteChargePeakTime = 5; perMinuteChargeOffPeakTime = 4;
                        }
                        peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                        offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                        callCharge = Math.Round(callCharge, 2);
                    }
                    ListOfCallDetails listOfCallDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(listOfCallDetails);

                    totalCharge = totalCharge + callCharge;
                }
                //call start at off peak time and call end at peak time
                else
                {
                    TimeSpan callTimeInOffPeakTime = peakStartingTime - callStartTime;
                    TimeSpan callTimeInPeakTime = callEndTime - peakStartingTime;

                    if (isLocalCall)
                    {
                        if (packageCode == "B")
                        {
                            if (cdr.callDuaration >= 60)
                            {
                                perMinuteChargePeakTime = 4; perMinuteChargeOffPeakTime = 3;
                                offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime - perMinuteChargeOffPeakTime;
                                peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                                callCharge = offPeakTimeCharge + peakTimeCharge;
                            }
                            else
                            {
                                perMinuteChargePeakTime = 4; perMinuteChargeOffPeakTime = 0;
                                offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                                peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                                callCharge = offPeakTimeCharge + peakTimeCharge;
                            }
                        }
                        else
                        {
                            perMinuteChargePeakTime = 3; perMinuteChargeOffPeakTime = 2;
                            offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                            peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                            callCharge = offPeakTimeCharge + peakTimeCharge;
                        }
                        callCharge = Math.Round(callCharge, 2);
                    }
                    else
                    {
                        if (packageCode == "B")
                        {
                            perMinuteChargePeakTime = 6; perMinuteChargeOffPeakTime = 5;
                        }
                        else
                        {
                            perMinuteChargePeakTime = 5; perMinuteChargeOffPeakTime = 4;
                        }
                        offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * perMinuteChargeOffPeakTime;
                        peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * perMinuteChargePeakTime;
                        callCharge = offPeakTimeCharge + peakTimeCharge;
                        callCharge = Math.Round(callCharge, 2);
                    }
                    ListOfCallDetails listOfCallDetails = new ListOfCallDetails
                    {
                        startTime = cdr.startingTimeOfTheCall,
                        durationInSeconds = cdr.callDuaration,
                        destinationNumber = cdr.phoneNumberOfCalledParty,
                        charge = callCharge
                    };
                    listOfCallDetailsOfPackages.Add(listOfCallDetails);

                    totalCharge = totalCharge + callCharge;
                }
            }
            totalCharge = Math.Round(totalCharge, 2);
            return totalCharge;
        }
        
        public Bill GenerateMonthlyBill(string callersPhoneNumber)
        {
            double monthlyRental = 0;
            CreateCustomer customer = new CreateCustomer();
            Customer customerDetails = customer.GetCustomerDetailsForPhoneNumber(callersPhoneNumber);
            List<CallDetailRecords> callDetailRecords = GetCallRecords(callersPhoneNumber);
            double totalCallCharges = CalculateTotalCharge(callersPhoneNumber, callDetailRecords);
            string packageCode = customer.GetPackageCode(callersPhoneNumber);

            if (packageCode == "A" || packageCode == "B")
            {
                monthlyRental = 100;
            }
            else
            {
                monthlyRental = 300;
            }
            double disCount = 0.0;
            double tax = (monthlyRental + totalCallCharges) * (20 / 100.0);
            tax = Math.Round(tax, 2);
            double billAmount = totalCallCharges + monthlyRental + tax - disCount;

            Bill billReport = new Bill
            {
                fullName = customerDetails.fullName,
                phoneNumber = callersPhoneNumber,
                billingAddress = customerDetails.billingAddress,
                totalCallCharges = totalCallCharges,
                totalDiscount = disCount,
                tax = tax,
                monthlyRental = monthlyRental,
                billAmount = billAmount,
                listOfCallRecords = listOfCallDetailsOfPackages
            };
            return billReport;
        }

        public double CalculateTotalCharge(string customersNumber, List<CallDetailRecords> callDetailRecords)
        {
            double totalCharge = 0;
            CreateCustomer createCustomer = new CreateCustomer();

            string packageCode = createCustomer.GetPackageCode(customersNumber);

            if (packageCode == "A")
            {
                SetPeakAndOffPeakHours(new TimeSpan(10, 0, 0), new TimeSpan(18, 0, 0));
                totalCharge = CalculateTotalChargePerMinute(customersNumber, callDetailRecords);
            }
            else if (packageCode == "C")
            {
                SetPeakAndOffPeakHours(new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0));
                totalCharge = CalculateTotalChargePerMinute(customersNumber, callDetailRecords);
            }
            else
            {
                totalCharge = CalculateTotalChargePerSecond(customersNumber, callDetailRecords);
            }
            return totalCharge;
        }
    }
}