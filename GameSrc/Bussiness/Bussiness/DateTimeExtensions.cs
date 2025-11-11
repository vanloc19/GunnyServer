using System;
using System.Globalization;

namespace Bussiness
{
    public static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();

        public static int GetWeekOfYear(this DateTime time)
        {
            CultureInfo myCI = new CultureInfo("vi-VN");
            Calendar myCal = myCI.Calendar;

            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            return myCal.GetWeekOfYear(time, myCWR, myFirstDOW);
        }
    }
}