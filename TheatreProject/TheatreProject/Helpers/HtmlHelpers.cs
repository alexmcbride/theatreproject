using System;
using System.Web.Mvc;

namespace TheatreProject.Helpers
{
    public static class HtmlHelpers
    {
        // From: http://stackoverflow.com/questions/11/how-can-relative-time-be-calculated-in-c
        public static string RelativeDate(this HtmlHelper helper, DateTime then)
        {
            const int Second = 1;
            const int Minute = 60 * Second;
            const int Hour = 60 * Minute;
            const int Day = 24 * Hour;
            const int Month = 30 * Day;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - then.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * Minute)
            {
                if (ts.Seconds == 0)
                    return "a moment ago";
                if (ts.Seconds == 1)
                    return "one second ago";
                return ts.Seconds + " seconds ago";
            }

            if (delta < 2 * Minute)
                return "a minute ago";

            if (delta < 45 * Minute)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * Minute)
                return "an hour ago";

            if (delta < 24 * Hour)
                return ts.Hours + " hours ago";

            if (delta < 48 * Hour)
                return "a day ago";

            if (delta < 30 * Day)
                return ts.Days + " days ago";

            if (delta < 12 * Month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "a month ago" : months + " months ago";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "a year ago" : years + " years ago";
        }

        // Replaces line breaks with HTML break rule tags
        public static MvcHtmlString KeepLineBreaks(this HtmlHelper helper, string value)
        {
            return new MvcHtmlString(value.Replace("\n", "<br>"));
        }
    }
}