using System;
using FluentAssertions;
using Io.Juenger.Common.Util;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Io.Juenger.Common.Tests
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [TestCaseSource(nameof(_getBusinessDaysUntilTestCases))]
        public void GetBusinessDaysUntil(DateTime start, DateTime end, int expectedBusinessDays)
        {
            start.GetBusinessDaysUntil(end).Should().Be(expectedBusinessDays);
        }

        [TestCaseSource(nameof(_getWeekendDaysUntilTestCases))]
        public void GetWeekendDaysUntil(DateTime start, DateTime end, int expectedWeekendDays)
        {
            start.GetWeekendDaysUntil(end).Should().Be(expectedWeekendDays);
        }

        [TestCaseSource(nameof(_getBusinessDueDateTestCases))]
        public void GetBusinessDueDate_From_Required_Total_Work_Days(DateTime start, float totalWorkDays, DateTime expectedDueDate)
        {
            start.GetBusinessDueDate(totalWorkDays).Should().Be(expectedDueDate);
        }
        
        [Test]
        public void GetBusinessDueDate_From_Required_Total_Time()
        {
            Assert.Fail();
        }

        [TestCaseSource(nameof(_getExcludedDaysTestCases))]
        public void GetExcludedDays(DateTime startDate, double totalDays, int expectedCountOfExcluded)
        {
            startDate.GetExcludedDays(totalDays).Should().Be(expectedCountOfExcluded);
        }

        private static object[] _getBusinessDaysUntilTestCases =
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
        
        private static object[] _getWeekendDaysUntilTestCases =
        {
            new object[] // A week from Sunday to Saturday
            {
                new DateTime(2022, 02, 06), // Sunday
                new DateTime(2022, 02, 12), // Saturday
                2 // expected weekend days
            },
            new object[] // a week from Monday to Sunday
            {
                new DateTime(2022, 02, 07), // Monday
                new DateTime(2022, 02, 13), // Sunday
                2 // expected weekend days
            },
            new object[] // a business week
            {
                new DateTime(2022, 02, 07), // Monday
                new DateTime(2022, 02, 11), // Friday
                0 // expected weekend days
            },
            new object[] // a weekend
            {
                new DateTime(2022, 02, 12), // Saturday
                new DateTime(2022, 02, 13), // Sunday
                2 // expected weekend days
            },
            new object[] // just one Saturday
            {
                new DateTime(2022, 02, 12), // Saturday
                new DateTime(2022, 02, 12), // Same Saturday
                1 // expected weekend days
            },
            new object[] // just one Sunday
            {
                new DateTime(2022, 02, 13), // Sunday
                new DateTime(2022, 02, 13), // Same Sunday
                1 // expected weekend days
            },
            new object[] // whole February
            {
                new DateTime(2022, 02, 01), // Tuesday
                new DateTime(2022, 02, 28), // Monday
                8 // expected weekend days
            }
            
        };
        
        private static object[] _getBusinessDueDateTestCases =
        {
            new object[] // whole February
            {
                new DateTime(2022, 02, 01), // Tuesday
                20f,
                new DateTime(2022, 02, 28) // expected due day
            },
            new object[] // 3 days from Tuesday (inclusive) 
            {
                new DateTime(2022, 02, 01), // Tuesday
                3f,
                new DateTime(2022, 02, 03) // expected due day
            },
            new object[] // 4 days from Tuesday (inclusive) 
            {
                new DateTime(2022, 02, 01), // Tuesday
                4f,
                new DateTime(2022, 02, 04) // expected due day
            },
            new object[] // 5 days from Tuesday (inclusive; reaches into weekend) 
            {
                new DateTime(2022, 02, 01), // Tuesday
                5f,
                new DateTime(2022, 02, 06) // expected due day
            }
        };
        
        private static object[] _getExcludedDaysTestCases =
        {
            new object[] // Monday
            {
                new DateTime(2022, 02, 07),
                1f,
                0
            },
            new object[] // Friday
            {
                new DateTime(2022, 02, 11),
                1f,
                0
            },
            new object[] // Monday to Friday
            {
                new DateTime(2022, 02, 07),
                5f,
                0
            },
            new object[] // Saturday
            {
                new DateTime(2022, 02, 12),
                1f,
                1
            },
            new object[] // Sunday
            {
                new DateTime(2022, 02, 13), 
                1f,
                1
            },
            new object[] // Saturday to Sunday
            {
                new DateTime(2022, 02, 12), 
                2f,
                2
            },
            new object[] // Monday to Sunday
            {
                new DateTime(2022, 02, 7), 
                7f,
                2
            },
            new object[] // Sunday to Saturday
            {
                new DateTime(2022, 02, 6), 
                7f,
                2
            }
        };
    }
}