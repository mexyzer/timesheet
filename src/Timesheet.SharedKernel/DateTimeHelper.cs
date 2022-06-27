namespace Timesheet.SharedKernel;

public static class DateTimeHelper
{
    /// <summary>
    /// Get business days (Monday - Friday) only, between 2 dates.
    /// 
    /// Source : https://stackoverflow.com/questions/1617049/calculate-the-number-of-business-days-between-two-dates
    /// 
    /// </summary>
    /// <param name="firstDate">First date to compare</param>
    /// <param name="secondDate">Second date to compare</param>
    /// <param name="datesSkipped">DateTimes to get skipped</param>
    /// <returns></returns>
    public static int GetBusinessDays(this DateTime firstDate, DateTime secondDate, params DateTime[] datesSkipped)
    {
        // erase hours
        firstDate = firstDate.Date;
        secondDate = secondDate.Date;

        if (firstDate > secondDate)
        {
            throw new ArgumentException($"Incorrect second date {secondDate}");
        }

        TimeSpan timeSpan = secondDate - firstDate;
        int businessDays = timeSpan.Days + 1;
        int fullWeekCount = businessDays / 7;

        // find out if there are weekends during the time exceeding the full weeks
        if (businessDays > fullWeekCount * 7)
        {
            // we are here to find out if there is a 1-day or 2-days weekend
            // in the time interval remaining after subtracting the complete weeks
            int firstDayOfWeek = firstDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDate.DayOfWeek;
            int lastDayOfWeek = secondDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)secondDate.DayOfWeek;

            if (lastDayOfWeek < firstDayOfWeek)
            {
                lastDayOfWeek += 7;
            }

            if (firstDayOfWeek <= 6)
            {
                if (lastDayOfWeek >= 7) // Both Saturday and Sunday are in the remaining time interval
                {
                    businessDays -= 2;
                }
                else if (lastDayOfWeek >= 6) // Only Saturday is in the remaining time interval
                {
                    businessDays -= 1;
                }
            }
            else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7) // Only Sunday is in the remaining time interval
            {
                businessDays -= 1;
            }
        }

        // subtract the weekends during the full weeks in the interval
        businessDays -= fullWeekCount + fullWeekCount;

        // subtract the number of bank holidays during the time interval
        foreach (DateTime date in datesSkipped)
        {
            DateTime bh = date.Date;
            if (firstDate <= bh && bh <= secondDate)
            {
                --businessDays;
            }
        }

        return businessDays;
    }

    public static long ToMilliseconds(this DateTime dt)
    {
        return (long)dt.ToUniversalTime().Subtract(
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        ).TotalMilliseconds;
    }
}