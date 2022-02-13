using System;
using FluentAssertions;
using Io.Juenger.Common.Util;
using NUnit.Framework;

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

        [Test]
        public void GetCountOfExcludedDaysWithinBusinessDaysUntil_Over_One_Holiday()
        {
            var start = new DateTime(2022, 04, 11);
            var end = new DateTime(2022, 04, 15);
            var goodFriday = new DateTime(2022, 04, 15);

            start.GetCountOfExcludedDaysWithinBusinessDaysUntil(end, goodFriday).Should().Be(1);
        }
        
        [Test]
        public void GetCountOfExcludedDaysWithinBusinessDaysUntil_Over_one_Holiday_And_One_Weekend()
        {
            var start = new DateTime(2022, 04, 11);
            var end = new DateTime(2022, 04, 17);
            var goodFriday = new DateTime(2022, 04, 15);

            start.GetCountOfExcludedDaysWithinBusinessDaysUntil(end, goodFriday).Should().Be(3);
        }
        
        [TestCaseSource(nameof(_getBusinessDueDateByWorkTimeTestCases))]
        public void GetBusinessDueDate_From_Required_Total_Time(DateTime start, TimeSpan totalWorkTime, DateTime expectedDueDate)
        {
            start.GetBusinessDueDate(totalWorkTime).Should().Be(expectedDueDate);
        }

        [TestCaseSource(nameof(_getBusinessDueDateByWorkDaysTestCases))]
        public void GetBusinessDueDate_From_Required_Total_Work_Days(DateTime start, float totalWorkDays, DateTime expectedDueDate)
        {
            start.GetBusinessDueDate(totalWorkDays).Should().Be(expectedDueDate);
        }

        [TestCaseSource(nameof(_getExcludedDaysTestCases))]
        public void GetCountOfExcludedDaysWithinBusinessDays(DateTime startDate, double totalDays, int expectedCountOfExcluded)
        {
            startDate.GetCountOfExcludedDaysWithinBusinessDays(totalDays).Should().Be(expectedCountOfExcluded);
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
        
        private static object[] _getBusinessDueDateByWorkTimeTestCases =
        {
            new object[] // 1 day 
            {
                new DateTime(2022, 02, 07), // Monday  (inclusive) 
                TimeSpan.FromHours(8),
                new DateTime(2022, 02, 07) // expected due day
            },
            new object[] // 1 business week
            {
                new DateTime(2022, 02, 07), // Monday  (inclusive) 
                TimeSpan.FromHours(40),
                new DateTime(2022, 02, 11) // expected due day
            },
            new object[] // 1 whole week
            {
                new DateTime(2022, 02, 07), // Saturday  (inclusive) 
                TimeSpan.FromHours(56),
                new DateTime(2022, 02, 15) // expected due day
            },
            new object[] // 1 weekend
            {
                new DateTime(2022, 02, 12), // Saturday  (inclusive) 
                TimeSpan.FromHours(16),
                new DateTime(2022, 02, 15) // expected due day
            },
            new object[] // whole February
            {
                new DateTime(2022, 02, 01), // Tuesday  (inclusive) 
                TimeSpan.FromHours(160),
                new DateTime(2022, 02, 28) // expected due day
            }
        };
        
        private static object[] _getBusinessDueDateByWorkDaysTestCases =
        {
            new object[] // 1 day 
            {
                new DateTime(2022, 02, 07), // Monday  (inclusive) 
                1f,
                new DateTime(2022, 02, 07) // expected due day
            },
            new object[] // 1 business week
            {
                new DateTime(2022, 02, 07), // Monday  (inclusive) 
                5f,
                new DateTime(2022, 02, 11) // expected due day
            },
            new object[] // 1 whole week
            {
                new DateTime(2022, 02, 07), // Saturday  (inclusive) 
                7f,
                new DateTime(2022, 02, 15) // expected due day
            },
            new object[] // 1 weekend
            {
                new DateTime(2022, 02, 12), // Saturday  (inclusive) 
                2f,
                new DateTime(2022, 02, 15) // expected due day
            },
            new object[] // whole February
            {
                new DateTime(2022, 02, 01), // Tuesday  (inclusive) 
                20f,
                new DateTime(2022, 02, 28) // expected due day
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