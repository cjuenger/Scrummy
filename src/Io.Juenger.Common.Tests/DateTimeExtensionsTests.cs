using System;
using FluentAssertions;
using Io.Juenger.Common.Util;
using NUnit.Framework;

namespace Io.Juenger.Common.Tests
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [TestCaseSource(nameof(_businessDaysUntilTestCases))]
        public void GetBusinessDaysUntil(DateTime start, DateTime end, int expectedBusinessDays)
        {
            start.GetBusinessDaysUntil(end).Should().Be(expectedBusinessDays);
        }

        [Test]
        public void GetTotalDaysUntilDueDate()
        {
            Assert.Fail();
        }
        
        [Test]
        public void GetBusinessDueDate_From_Required_Total_Time()
        {
            Assert.Fail();
        }
        
        [Test]
        public void GetBusinessDueDate_From_Required_Total_Work_Days()
        {
            Assert.Fail();
        }
        
        private static object[] _businessDaysUntilTestCases =
        {
            new object[] // A week from Sunday to Saturday
            {
                new DateTime(2022, 02, 05), // Sunday
                new DateTime(2022, 02, 12), // Saturday
                5 // expected business days
            },
            new object[] // a week from Monday to Sunday
            {
                new DateTime(2022, 02, 07), // Monday
                new DateTime(2022, 02, 13), // Sunday
                5 // expected business days
            },
            new object[] // a business week
            {
                new DateTime(2022, 02, 07), // Monday
                new DateTime(2022, 02, 11), // Friday
                5 // expected business days
            },
            new object[] // a weekend
            {
                new DateTime(2022, 02, 12), // Saturday
                new DateTime(2022, 02, 13), // Sunday
                0 // expected business days
            },
            new object[] // just one Saturday
            {
                new DateTime(2022, 02, 12), // Saturday
                new DateTime(2022, 02, 12), // Same Saturday
                0 // expected business days
            },
            new object[] // just one Sunday
            {
                new DateTime(2022, 02, 13), // Sunday
                new DateTime(2022, 02, 13), // Same Sunday
                0 // expected business days
            },
            new object[] // whole February
            {
                new DateTime(2022, 02, 01), // Tuesday
                new DateTime(2022, 02, 28), // Monday
                20 // expected business days
            }
            
        };
    }
}