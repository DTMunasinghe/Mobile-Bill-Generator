using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BillGenerator.Tests
{
    [TestFixture]
    class CreateCallDetailRecordsTests
    {
        private CreateCallDetailRecords _sut;

        [SetUp]
        public void Init()
        {
            _sut = new CreateCallDetailRecords();
        }

        [Test]
        public void OnGetCallDurations_WhenInputCallDuarations_ShouldPrintSameCallDuartions()
        {
            //Arrange
            string filePath = "CDR.csv";
            List<int> expected = new List<int> { 350, 245, 490, 1530, 156, 567, 193, 234, 123, 145, 359, 45, 489, 378 };

            //Act
            var actual = _sut.GetCallDurations(filePath);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OnGetCallDetailRecords_WhenInputCallDetailRecordsObjectValues_ShouldReturnSameValuesOfTheCorrespondingObject()
        {
            //Arrange
            List<CallDetailRecords> expected = new List<CallDetailRecords>();

            DateTime.TryParse("12/01/2017 06:20:00", out DateTime dateAndTime1);
            CallDetailRecords callDetailRecords1 = new CallDetailRecords()
            {
                phoneNumberOfCallingParty = "0719633911",
                phoneNumberOfCalledParty = "0715535452",
                startingTimeOfTheCall = dateAndTime1,
                callDuaration = 350
            };
            expected.Add(callDetailRecords1);

            DateTime.TryParse("12/01/2017 20:10:30", out DateTime dateAndTime2);
            CallDetailRecords callDetailRecords2 = new CallDetailRecords()
            {
                phoneNumberOfCallingParty = "0715535452",
                phoneNumberOfCalledParty = "0724487106",
                startingTimeOfTheCall = dateAndTime2,
                callDuaration = 245
            };
            expected.Add(callDetailRecords2);

            DateTime.TryParse("12/01/2017 20:30:45", out DateTime dateAndTime3);
            CallDetailRecords callDetailRecords3 = new CallDetailRecords()
            {
                phoneNumberOfCallingParty = "0719633911",
                phoneNumberOfCalledParty = "0719633979",
                startingTimeOfTheCall = dateAndTime3,
                callDuaration = 490
            };
            expected.Add(callDetailRecords3);

            //Act
            var actual = _sut.GetCallDetailRecords();

            //Assert
            Assert.AreEqual(expected[1].phoneNumberOfCallingParty, actual[1].phoneNumberOfCallingParty);
            Assert.AreEqual(expected[1].phoneNumberOfCalledParty, actual[1].phoneNumberOfCalledParty);
            Assert.AreEqual(expected[1].startingTimeOfTheCall, actual[1].startingTimeOfTheCall);
            Assert.AreEqual(expected[1].callDuaration, actual[1].callDuaration);
        }

        [Test]
        public void OnGetCallRecords_WhenInputCallDetailRecordsForOneNumber_ShouldReturnCallRecordsForCorrespondingNumber()
        {
            //Arrange
            List<CallDetailRecords> expected = new List<CallDetailRecords>();
            string phoneNumber = "0775678765";

            DateTime.TryParse("12/18/2017 18:15:53", out DateTime dateAndTime1);
            CallDetailRecords callDetailRecords1 = new CallDetailRecords()
            {
                phoneNumberOfCallingParty = "0775678765",
                phoneNumberOfCalledParty = "0719633979",
                startingTimeOfTheCall = dateAndTime1,
                callDuaration = 359
            };
            expected.Add(callDetailRecords1);

            DateTime.TryParse("12/24/2017 16:55:53", out DateTime dateAndTime2);
            CallDetailRecords callDetailRecords2 = new CallDetailRecords()
            {
                phoneNumberOfCallingParty = "0775678765",
                phoneNumberOfCalledParty = "0724487106",
                startingTimeOfTheCall = dateAndTime2,
                callDuaration = 45
            };
            expected.Add(callDetailRecords2);

            DateTime.TryParse("12/30/2017 07:14:57", out DateTime dateAndTime3);
            CallDetailRecords callDetailRecords3 = new CallDetailRecords()
            {
                phoneNumberOfCallingParty = "0775678765",
                phoneNumberOfCalledParty = "0771239087",
                startingTimeOfTheCall = dateAndTime3,
                callDuaration = 378
            };
            expected.Add(callDetailRecords3);

            //Act
            var actual = _sut.GetCallRecords(phoneNumber);

            //Assert
            Assert.AreEqual(expected[0].phoneNumberOfCallingParty, actual[0].phoneNumberOfCallingParty);
            Assert.AreEqual(expected[1].phoneNumberOfCallingParty, actual[1].phoneNumberOfCallingParty);
            Assert.AreEqual(expected[2].phoneNumberOfCallingParty, actual[2].phoneNumberOfCallingParty);

            Assert.AreEqual(expected[0].phoneNumberOfCalledParty, actual[0].phoneNumberOfCalledParty);
            Assert.AreEqual(expected[1].phoneNumberOfCalledParty, actual[1].phoneNumberOfCalledParty);
            Assert.AreEqual(expected[2].phoneNumberOfCalledParty, actual[2].phoneNumberOfCalledParty);

            Assert.AreEqual(expected[0].startingTimeOfTheCall, actual[0].startingTimeOfTheCall);
            Assert.AreEqual(expected[1].startingTimeOfTheCall, actual[1].startingTimeOfTheCall);
            Assert.AreEqual(expected[2].startingTimeOfTheCall, actual[2].startingTimeOfTheCall);

            Assert.AreEqual(expected[0].callDuaration, actual[0].callDuaration);
            Assert.AreEqual(expected[1].callDuaration, actual[1].callDuaration);
            Assert.AreEqual(expected[2].callDuaration, actual[2].callDuaration);
        }

        [Test]
        public void OnIsLongDistanceCall_WhenInputTwoPhoneNumbersWithSameExtension_ShoudReturnTrue()
        {
            //Arrange
            DateTime.TryParse("12/01/2017 06:20:00", out DateTime dateAndTime);
            CallDetailRecords callDetailRecords = new CallDetailRecords
            {
                phoneNumberOfCallingParty = "0719633911",
                phoneNumberOfCalledParty = "0719633979",
                startingTimeOfTheCall = dateAndTime,
                callDuaration = 350,
            };

            //Act
            bool actual = _sut.IsLocalCall(callDetailRecords.phoneNumberOfCallingParty, callDetailRecords.phoneNumberOfCalledParty);

            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void OnIsLongDistanceCall_WhenInputTwoPhoneNumbersWithDifferentExtension_ShoudReturnFalse()
        {
            //Arrange
            DateTime.TryParse("12/01/2017 20:10:30", out DateTime dateAndTime);
            CallDetailRecords callDetailRecords = new CallDetailRecords
            {
                phoneNumberOfCallingParty = "0715535452",
                phoneNumberOfCalledParty = "0724487106",
                startingTimeOfTheCall = dateAndTime,
                callDuaration = 245,
            };

            //Act
            bool actual = _sut.IsLocalCall(callDetailRecords.phoneNumberOfCallingParty, callDetailRecords.phoneNumberOfCalledParty);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void OnCalculateTotalChargePerMinute_WhenInputCallersNumberAndCallDetailRecordsList_ShouldReturnTotalCharge()
        {
            //Arrange
            string callersPhoneNumber = "0775678765";
            List<CallDetailRecords> callDetailRecords = _sut.GetCallRecords(callersPhoneNumber);

            //Act
            double expectedTotalCharge = _sut.CalculateTotalChargePerMinute(callersPhoneNumber, callDetailRecords);
            double actualTotalCharge = 49.0;

            //Assert
            Assert.AreEqual(expectedTotalCharge, actualTotalCharge);
        }

        [Test]
        public void OnCalculateTotalChargePerMinute_WhenInputCallersNumberAndCallDetailRecordsListWithCallTimeBetweenPeakAndOffPeakHours_ShouldReturnCorrectTotalCharge()
        {
            //Arrange
            string callersPhoneNumber = "0719633911";
            List<CallDetailRecords> callDetailRecords = _sut.GetCallRecords(callersPhoneNumber);

            //Act
            double expectedTotalCharge = _sut.CalculateTotalChargePerMinute(callersPhoneNumber, callDetailRecords);
            double actualTotalCharge = 127.0;

            //Assert
            Assert.AreEqual(expectedTotalCharge, actualTotalCharge);
        }

        [Test]
        public void OnCalculateTotalChargePerSecond_WhenInputCallersNumberAndCallDetailRecordsList_ShouldReturnTotalCharge()
        {
            //Arrange
            string callersPhoneNumber = "0775678765";
            List<CallDetailRecords> callDetailRecords = _sut.GetCallRecords(callersPhoneNumber);

            //Act
            double expectdTotalCharge = _sut.CalculateTotalChargePerSecond(callersPhoneNumber, callDetailRecords);
            double actualTotalCharge = 59.30;

            //Assert
            Assert.AreEqual(expectdTotalCharge, actualTotalCharge);
        }

        [Test]
        public void OnCalculateTotalChargePerSecond_WhenInputCallersNumberAndCallDetailRecordsListWithCallTimeBetweenPeakAndOffPeakHours_ShouldReturnCorrectTotalCharge()
        {
            //Arrange
            string callersPhoneNumber = "0719633911";
            List<CallDetailRecords> callDetailRecords = _sut.GetCallRecords(callersPhoneNumber);

            //Act
            double expectedTotalCharge = _sut.CalculateTotalChargePerSecond(callersPhoneNumber, callDetailRecords);
            double actualTotalCharge = 175.27;

            //Assert
            Assert.AreEqual(expectedTotalCharge, actualTotalCharge);
        }

        [Test]
        public void OnGenerateMonthlyBill_WhenInputCustomersPhoneNumber_ShouldReturnMonthlyBillReport()
        {
            //Arrange
            string callersPhoneNumber = "0775678765";
            string address = "No.201 Perera Road Nawalapitiya";
            double totalCallCharges = 49.0;
            double tax = 29.8;
            double totalDiscount = 0.0;
            double monthlyRental = 100.0;
            double billAmount = 178.8;
            double firstCallCharge = 30.0;
            double secondCallCharge = 5.0;
            double thirdCallCharge = 14.0;
            
            //Act
            Bill billReport = _sut.GenerateMonthlyBillForPerMinutePackage(callersPhoneNumber);

            //Assert
            Assert.AreEqual(callersPhoneNumber, billReport.phoneNumber);
            Assert.AreEqual(address, billReport.billingAddress);
            Assert.AreEqual(totalCallCharges, billReport.totalCallCharges);
            Assert.AreEqual(totalDiscount, billReport.totalDiscount);
            Assert.AreEqual(tax, billReport.tax);
            Assert.AreEqual(monthlyRental, billReport.monthlyRental);
            Assert.AreEqual(billAmount, billReport.billAmount);
            Assert.AreEqual(firstCallCharge, billReport.listOfCallRecords[0].charge);
            Assert.AreEqual(secondCallCharge, billReport.listOfCallRecords[1].charge);
            Assert.AreEqual(thirdCallCharge, billReport.listOfCallRecords[2].charge);
        }
    }
}
