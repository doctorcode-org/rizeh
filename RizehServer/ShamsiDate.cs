namespace Parsnet
{
    using System;
    using System.Globalization;

    public static class ShamsiDate
    {
        public static string PersianDayName(DateTime targetDate)
        {
            switch (targetDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "یکشنبه";

                case DayOfWeek.Monday:
                    return "دوشنبه";

                case DayOfWeek.Tuesday:
                    return "سه شنبه";

                case DayOfWeek.Wednesday:
                    return "چهارشنبه";

                case DayOfWeek.Thursday:
                    return "پنجشنبه";

                case DayOfWeek.Friday:
                    return "آدینه";

                case DayOfWeek.Saturday:
                    return "شنبه";
            }
            return "نامعین";
        }

        public static string PersianMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "فروردین";

                case 2:
                    return "اردیبهشت";

                case 3:
                    return "خرداد";

                case 4:
                    return "تیر";

                case 5:
                    return "مرداد";

                case 6:
                    return "شهریور";

                case 7:
                    return "مهر";

                case 8:
                    return "آبان";

                case 9:
                    return "آذر";

                case 10:
                    return "دی";

                case 11:
                    return "بهمن";

                case 12:
                    return "اسفند";
            }
            return "نامعین";
        }

        public static string ToFullShamsiDate(DateTime targetDate)
        {
            ToShortShamsiDate(targetDate).Split(new char[] { '/' });
            return string.Format("{0} {1}", PersianDayName(targetDate), ToLongShamsiDate(targetDate));
        }

        public static string ToLongShamsiDate(DateTime targetDate)
        {
            string[] strArray = ToShortShamsiDate(targetDate).Split(new char[] { '/' });
            return string.Format("{0} {1} {2}", strArray[2], PersianMonthName(int.Parse(strArray[1])), strArray[0]);
        }

        public static string ToShortShamsiDate(DateTime targetDate)
        {
            PersianCalendar calendar = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", calendar.GetYear(targetDate), calendar.GetMonth(targetDate), calendar.GetDayOfMonth(targetDate));
        }
    }
}

