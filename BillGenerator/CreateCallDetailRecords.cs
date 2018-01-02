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
            int perMinuteCharge = 0;
            double totalCharge = 0;

            foreach (CallDetailRecords cdr in callDetailRecords)
            {
                double callCharge = 0;
                bool isLocalCall = IsLocalCall(cdr.phoneNumberOfCallingParty, cdr.phoneNumberOfCalledParty);
                double minutesPortionInTheCall = cdr.callDuaration / 60;
                double secondsPortionInTheCall = cdr.callDuaration % 60;
                double secondsPortionInTheCallInTermOfMinutes = (cdr.callDuaration % 60) / 60.0;

                for (int i = 0; i < minutesPortionInTheCall; i++)
                {
                    if (cdr.startingTimeOfTheCall.TimeOfDay >= peakStartingTime && cdr.startingTimeOfTheCall.TimeOfDay < peakEndingTime)
                    {
                        if (isLocalCall)
                        {
                            perMinuteCharge = 4;
                        }
                        else
                        {
                            perMinuteCharge = 6;
                        }
                    }
                    else
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
                    callCharge = callCharge + perMinuteCharge;
                    totalCharge = totalCharge + perMinuteCharge;
                    cdr.startingTimeOfTheCall = cdr.startingTimeOfTheCall.AddMinutes(1);
                }
                if (cdr.startingTimeOfTheCall.TimeOfDay >= peakStartingTime && cdr.startingTimeOfTheCall.TimeOfDay < peakEndingTime)
                {
                    if (isLocalCall)
                    {
                        perMinuteCharge = 4;
                    }
                    else
                    {
                        perMinuteCharge = 6;
                    }
                }
                else
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
                callCharge = callCharge + perMinuteCharge * secondsPortionInTheCallInTermOfMinutes;
                totalCharge = totalCharge + perMinuteCharge * secondsPortionInTheCallInTermOfMinutes;
                DateTime callEndTime = cdr.startingTimeOfTheCall.AddSeconds(secondsPortionInTheCall);

                ListOfCallDetails listOfCallDetails = new ListOfCallDetails
                {
                    startTime = cdr.startingTimeOfTheCall,
                    durationInSeconds = cdr.callDuaration,
                    destinationNumber = cdr.phoneNumberOfCalledParty,
                    charge = callCharge
                };
                listOfCallDetailsForPerSecondPackages.Add(listOfCallDetails);
            }
            return totalCharge;
        }

        public Bill GenerateMonthlyBill(string callersPhoneNumber)
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
    }
}