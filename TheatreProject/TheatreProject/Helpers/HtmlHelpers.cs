using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheatreProject.Helpers
{
    public static class HtmlHelpers
    {
        // From: http://stackoverflow.com/questions/11/how-can-relative-time-be-calculated-in-c
        public static string RelativeDate(this HtmlHelper helper, DateTime then)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - then.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
            {
                if (ts.Seconds == 0)
                    return "a moment ago";
                if (ts.Seconds == 1)
                    return "about one second ago";
                return "about " + ts.Seconds + " seconds ago";
            }

            if (delta < 2 * MINUTE)
                return "about a minute ago";

            if (delta < 45 * MINUTE)
                return "about " + ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "about an hour ago";

            if (delta < 24 * HOUR)
                return "about " + ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "about a day ago";

            if (delta < 30 * DAY)
                return "about " + ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "about a month ago" : "about " + months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "about a year ago" : "about " + years + " years ago";
            }
        } 

        public static MvcHtmlString Capitalize(this HtmlHelper helper, string value)
        {
            if (value != null && value.Length > 1)
            {
                return new MvcHtmlString(Char.ToUpper(value[0]) + value.Substring(1));
            }
            return new MvcHtmlString(value);
        }
    }
}