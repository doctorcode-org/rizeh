using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Parsnet
{
    public static class ShamsiDate
    {
        public static String PersianDayName(DateTime targetDate)
        {
            String DayName = "نامعین";
            switch (targetDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    DayName = "شنبه";
                    break;
                case DayOfWeek.Sunday:
                    DayName = "یکشنبه";
                    break;
                case DayOfWeek.Monday:
                    DayName = "دوشنبه";
                    break;
                case DayOfWeek.Tuesday:
                    DayName = "سه شنبه";
                    break;
                case DayOfWeek.Wednesday:
                    DayName = "چهارشنبه";
                    break;
                case DayOfWeek.Thursday:
                    DayName = "پنجشنبه";
                    break;
                case DayOfWeek.Friday:
                    DayName = "آدینه";
                    break;
            }
            return DayName;
        }

        public static String PersianMonthName(int month)
        {
            String MonthName = "نامعین";
            switch (month)
            {
                case 1:
                    MonthName = "فروردین";
                    break;
                case 2:
                    MonthName = "اردیبهشت";
                    break;
                case 3:
                    MonthName = "خرداد";
                    break;
                case 4:
                    MonthName = "تیر";
                    break;
                case 5:
                    MonthName = "مرداد";
                    break;
                case 6:
                    MonthName = "شهریور";
                    break;
                case 7:
                    MonthName = "مهر";
                    break;
                case 8:
                    MonthName = "آبان";
                    break;
                case 9:
                    MonthName = "آذر";
                    break;
                case 10:
                    MonthName = "دی";
                    break;
                case 11:
                    MonthName = "بهمن";
                    break;
                case 12:
                    MonthName = "اسفند";
                    break;
            }
            return MonthName;
        }

        public static String ToShortShamsiDate(DateTime targetDate)
        {
            PersianCalendar perCalendar = new PersianCalendar();
            return String.Format("{0}/{1}/{2}", perCalendar.GetYear(targetDate), perCalendar.GetMonth(targetDate), perCalendar.GetDayOfMonth(targetDate));
        }

        public static String ToLongShamsiDate(DateTime targetDate)
        {
            string[] ShamsiDate = ToShortShamsiDate(targetDate).Split('/');

            return String.Format("{0} {1} {2}", ShamsiDate[2], PersianMonthName(Int32.Parse(ShamsiDate[1])), ShamsiDate[0]);
        }

        public static String ToFullShamsiDate(DateTime targetDate)
        {
            string[] ShamsiDate = ToShortShamsiDate(targetDate).Split('/');
            return String.Format("{0} {1}", PersianDayName(targetDate), ToLongShamsiDate(targetDate));
        }


    }
}