using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BillGenerator.Tests
{
    [TestFixture]
    public class CreateCustomerTests
    {
        private CreateCustomer _sut;
        

        [SetUp]
        public void Init()
        {
            _sut = new CreateCustomer();
        }

        [Test]
        public void OnGetCustomersFullNames_WhenInputFullNamesOfCustomers_ShouldPrintSameFullNames()
        {
            //Arrange
            string filePath = "customerDetails.csv";
            List<string> expected = new List<string> { "M.A Silva", "D.T Perera", "M.N Sahabandu", "V.C Munasinge", "T.C Wellappili", "M.N Perera", "W.A Appuhamu"};

            //Act
            var actual = _sut.GetCustomersFullNames(filePath);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnGetCustomers_WhenInputCustomerObjectValues_ShouldReturnSameValuesOfTheCorrespondingObject()
        {
            //Arrange
            List<Customer> expected = new List<Customer>();

            DateTime.TryParse("10/25/2017 14:01:16", out DateTime dateAndTime1);
            Customer customer1 = new Customer()
            {
                fullName = "M.A Silva",
                billingAddress = "No.101 Galle Road Dehiwala",
                phoneNumber = "0719633911",
                packageCode = "A",
                registeredDate = dateAndTime1

            };
            expected.Add(customer1);

            DateTime.TryParse("04/17/2017 10:25:34", out DateTime dateAndTime2);
            Customer customer2 = new Customer()
            {
                fullName = "D.T Perera",
                billingAddress = "No.123 Sumagi Mawatha Walgama",
                phoneNumber = "0715535452",
                packageCode = "A",
                registeredDate = dateAndTime2

            };
            expected.Add(customer2);

            DateTime.TryParse("09/10/2017 15:14:19", out DateTime dateAndTime3);
            Customer customer3 = new Customer()
            {
                fullName = "M.N Sahabandu",
                billingAddress = "No.201 Perera Road Nawalapitiya",
                phoneNumber = "0775678765",
                packageCode = "A",
                registeredDate = dateAndTime3

            };
            expected.Add(customer3);
            
            //Act
            var actual = _sut.CreateCustomers();

            //Assert
            Assert.AreEqual(expected[0].fullName, actual[0].fullName);
            Assert.AreEqual(expected[0].billingAddress, actual[0].billingAddress);
            Assert.AreEqual(expected[0].phoneNumber, actual[0].phoneNumber);
            Assert.AreEqual(expected[0].packageCode, actual[0].packageCode);
            Assert.AreEqual(expected[0].registeredDate, actual[0].registeredDate);
        }

        [Test]
        public void OnGetCustomerDetailsForPhoneNumber_WhenInputCustomerPhoneNumber_ShouldReturnCustomerDetails()
        {
            //Arrange
            string phoneNumber = "0719633911";
            DateTime.TryParse("10/25/2017 14:01:16", out DateTime dateAndTime);

            Customer customerDetailsForPhoneNumber = new Customer
            {
                fullName = "M.A Silva",
                billingAddress = "No.101 Galle Road Dehiwala",
                phoneNumber = "0719633911",
                packageCode = "A",
                registeredDate = dateAndTime
            };

            //Act
            var actual = _sut.GetCustomerDetailsForPhoneNumber(phoneNumber);

            //Assert
            Assert.AreEqual(customerDetailsForPhoneNumber.fullName, actual.fullName);
            Assert.AreEqual(customerDetailsForPhoneNumber.billingAddress, actual.billingAddress);
            Assert.AreEqual(customerDetailsForPhoneNumber.phoneNumber, actual.phoneNumber);
            Assert.AreEqual(customerDetailsForPhoneNumber.packageCode, actual.packageCode);
            Assert.AreEqual(customerDetailsForPhoneNumber.registeredDate, actual.registeredDate);
        }

        [Test]
        public void OnGetPackageCode_WhenInputCustomerPhoneNumber_ShouldReturnPackageNumber()
        {
            //Arrange
            string customersPhoneNumber = "0775678765";
            string actualPackageCode = "A";

            //Act
            string expectedPackageCode = _sut.GetPackageCode(customersPhoneNumber);

            //Assert
            Assert.AreEqual(expectedPackageCode, actualPackageCode);
        }
    }
}
