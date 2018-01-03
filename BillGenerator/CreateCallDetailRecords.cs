using System;
using System.Collections.Generic;
using System.IO;

namespace BillGenerator
{
    public class CreateCallDetailRecords
    {
        private TimeSpan peakStartingTime = new TimeSpan(8, 0, 0);
        private TimeSpan peakEndingTime = new TimeSpan(20, 0, 0);
        private List<ListOfCallDetails> listOfCallDetailsForPerMinutePackages = new List<ListOfCallDetails>();
        private List<ListOfCallDetails> listOfCallDetailsForPerSecondPackages = new List<ListOfCallDetails>();

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
            double totalCharge = 0;
            foreach (CallDetailRecords cdr in callDetailRecords)
            {
                double callCharge = 0;
                bool isLocalCall = IsLocalCall(cdr.phoneNumberOfCallingParty, cdr.phoneNumberOfCalledParty);
                double durationInMinutes = Math.Ceiling(cdr.callDuaration / 60.0);

                for (int i = 0; i < durationInMinutes; i++)
                {
                    if (cdr.startingTimeOfTheCall.TimeOfDay >= peakStartingTime && cdr.startingTimeOfTheCall.TimeOfDay < peakEndingTime)
                    {
                        if (isLocalCall)
                        {
                            perMinuteCharge = 3;
                        }
                        else
                        {
                            perMinuteCharge = 5;
                        }
                    }
                    else
                    {
                        if (isLocalCall)
                        {
                            perMinuteCharge = 2;
                        }
                        else
                        {
                            perMinuteCharge = 4;
                        }
                    }
                    callCharge = callCharge + perMinuteCharge;
                    totalCharge = totalCharge + perMinuteCharge;
                    cdr.startingTimeOfTheCall = cdr.startingTimeOfTheCall.AddMinutes(1);
                }

                ListOfCallDetails listOfCallDetails = new ListOfCallDetails
                {
                    startTime = cdr.startingTimeOfTheCall,
                    durationInSeconds = cdr.callDuaration,
                    destinationNumber = cdr.phoneNumberOfCalledParty,
                    charge = callCharge
                };
                listOfCallDetailsForPerMinutePackages.Add(listOfCallDetails);
            }
            return totalCharge;
        }

        public double CalculateTotalChargePerSecond(string callersPhoneNumber, List<CallDetailRecords> callDetailRecords)
        {
            double totalCharge = 0;
            foreach (CallDetailRecords cdr in callDetailRecords)
            {
                double callCharge = 0;
                double peakTimeCharge = 0;
                double offPeakTimeCharge = 0;
                bool isLocalCall = IsLocalCall(cdr.phoneNumberOfCallingParty, cdr.phoneNumberOfCalledParty);
                TimeSpan callStartTime = cdr.startingTimeOfTheCall.TimeOfDay;
                TimeSpan callEndTime = callStartTime.Add(TimeSpan.FromSeconds(cdr.callDuaration));
                double callDurationInMinutes = cdr.callDuaration / 60.0;

                //peak time
                if (callStartTime >= peakStartingTime && callEndTime < peakEndingTime)
                {
                    //local call 
                    if (isLocalCall)
                    { 
                        callCharge = callDurationInMinutes * 4;
                    }
                    else
                    {
                        callCharge = callDurationInMinutes * 6;
                    }
                    totalCharge = totalCharge + callCharge;
                }
                //off peak time
                else if ((callStartTime >= peakEndingTime && callEndTime < peakStartingTime) || 
                        (callStartTime < peakStartingTime && callEndTime < peakStartingTime) || 
                        (callStartTime >= peakEndingTime && callEndTime > peakEndingTime))
                {
                    if (isLocalCall)
                    {
                        callCharge = callDurationInMinutes * 3;
                    }
                    else
                    {
                        callCharge = callDurationInMinutes * 5;
                    }
                    totalCharge = totalCharge + callCharge;
                }
                //call start at peak time and call end at off peak time
                else if ((callStartTime >= peakStartingTime && callStartTime < peakEndingTime) && (callEndTime >= peakEndingTime && callEndTime < peakStartingTime))
                {
                    TimeSpan callTimeInPeakTime = peakEndingTime - callStartTime;
                    TimeSpan callTimeInOffPeakTime = callEndTime - peakEndingTime;
                    
                    if (isLocalCall)
                    {
                        peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * 4.0;
                        offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * 3.0;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                    }
                    else
                    {
                        peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * 6.0;
                        offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * 5.0;
                        callCharge = peakTimeCharge + offPeakTimeCharge;
                    }
                    totalCharge = totalCharge + callCharge;
                }
                //call start at off peak time and call end at peak time
                else
                {
                    TimeSpan callTimeInOffPeakTime = peakStartingTime - callStartTime;
                    TimeSpan callTimeInPeakTime = callEndTime - peakStartingTime;

                    if (isLocalCall)
                    {
                        offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * 3;
                        peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * 4;
                        callCharge = offPeakTimeCharge + peakTimeCharge;
                    }
                    else
                    {
                        offPeakTimeCharge = (callTimeInOffPeakTime.TotalSeconds / 60.0) * 5;
                        peakTimeCharge = (callTimeInPeakTime.TotalSeconds / 60.0) * 6;
                        callCharge = offPeakTimeCharge + peakTimeCharge;
                    }
                    totalCharge = totalCharge + callCharge;
                }
            }
            totalCharge = Math.Round(totalCharge, 2);
            return totalCharge;
        }
        
        public Bill GenerateMonthlyBillForPerMinutePackage(string callersPhoneNumber)
        {
            CreateCustomer customer = new CreateCustomer();
            Customer customerDetails = customer.GetCustomerDetailsForPhoneNumber(callersPhoneNumber);
            List<CallDetailRecords> callDetailRecords = GetCallRecords(callersPhoneNumber);
            double totalCallCharges = CalculateTotalChargePerMinute(callersPhoneNumber, callDetailRecords);
            double monthlyRental = 100.0;
            double disCount = 0.0;
            double tax = (monthlyRental + totalCallCharges) * (20 / 100.0);
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
                listOfCallRecords = listOfCallDetailsForPerMinutePackages
            };
            return billReport;
        }

        public double CalculateTotalCharge(string customersNumber, List<CallDetailRecords> callDetailRecords)
        {
            double totalCharge = 0;
            CreateCustomer createCustomer = new CreateCustomer();

            int packageCode = createCustomer.GetPackageCode(customersNumber);

            if (packageCode == 1)
            {
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