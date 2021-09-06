namespace EtAlii.CryptoMagic
{
    using System;

    public static class HtmlExtensions
    {
        public static string ToShortHtml(this decimal d)
        {
            return $"{d:000.00}";
        }

        public static string ToHtml(this decimal d, string prefix = null)
        {
            prefix ??= d >= 0 ? "+" : "";
            return $"{prefix}{d:000.000000000}";
        }
        public static string ToHtml(this DateTime? dateTime, string placeHolder = "")
        {
            if (!dateTime.HasValue || dateTime == DateTime.MinValue)
            {
                return placeHolder;
            }

            return $"{dateTime:G}";
        }

        public static string ToProfitHtml(this decimal d)
        {
            return d < 100 ? $"{d:0.00}" : $"{d:0}";
        }

        public static string ToMultiLineHtml(this DateTime dateTime, string placeHolder = "") => ToMultiLineHtml((DateTime?) dateTime, placeHolder);
        public static string ToMultiLineHtml(this DateTime? dateTime, string placeHolder = "")
        {
            if (!dateTime.HasValue || dateTime == DateTime.MinValue)
            {
                return placeHolder;
            }

            return $"{dateTime:d}<br/>{dateTime:T}";
        }
    }
}