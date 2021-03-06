﻿using System;
using System.Collections.Generic;
using System.IO;

namespace BillGenerator
{
    public class CreateCustomer
    {
        public List<string> GetCustomersFullNames(string filePath)
        {
            var fullNames = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // skip the heading

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(',');
                    fullNames.Add(line[0]);
                }
            }
            return fullNames;
        }

        public List<Customer> CreateCustomers()
        {
            var customers = new List<Customer>();
            string fileName = "customerDetails.csv";
            using (var reader = new StreamReader(fileName))
            {
                String line;
                reader.ReadLine(); //skip the heading

                while ((line = reader.ReadLine()) != null)
                {
                    String[] tokens = line.Split(',');
                    //Console.WriteLine(tokens[4]);
                    DateTime.TryParse(tokens[4], out DateTime dateAndTime);

                    Customer customer = new Customer
                    {
                        fullName = tokens[0],
                        billingAddress = tokens[1],
                        phoneNumber = tokens[2],
                        packageCode = tokens[3],
                        registeredDate = dateAndTime
                    };
                    customers.Add(customer);
                }
                return customers;
            }
        }

        public bool CheckPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.Substring(0, 1) == "0" && phoneNumber.Length == 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Customer GetCustomerDetailsForPhoneNumber(string phoneNumber)
        {
            string fileName = "customerDetails.csv";
            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    String line;
                    reader.ReadLine(); //skip the heading

                    while ((line = reader.ReadLine()) != null)
                    {
                        String[] tokens = line.Split(',');
                        DateTime.TryParse(tokens[4], out DateTime dateAndTime);

                        if (tokens[2] == phoneNumber)
                        {
                            return new Customer
                            {
                                fullName = tokens[0],
                                billingAddress = tokens[1],
                                phoneNumber = tokens[2],
                                packageCode = tokens[3],
                                registeredDate = dateAndTime
                            };
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return null;
        }

        public string GetPackageCode(string customersPhoneNumber)
        {
            if (CheckPhoneNumber(customersPhoneNumber))
            {
                Customer customerDetails = GetCustomerDetailsForPhoneNumber(customersPhoneNumber);
                string packageCode = customerDetails.packageCode;
                return packageCode;
            }
            else
            {
                Console.WriteLine("Wrong Phone Number");
                return null;
            }
        }
    }
}
