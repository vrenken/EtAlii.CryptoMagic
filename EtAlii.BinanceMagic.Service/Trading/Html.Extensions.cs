namespace EtAlii.BinanceMagic.Service
{
    using System;
    using Microsoft.AspNetCore.Components;

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

        public static MarkupString ToMultiLineHtml(this DateTime dateTime) => ToMultiLineHtml((DateTime?) dateTime);

        public static MarkupString ToMultiLineHtml(this DateTime? dateTime)
        {
            if (!dateTime.HasValue || dateTime == DateTime.MinValue)
            {
                return (MarkupString)"";
            }

            return (MarkupString)$"{dateTime:d}<br/>{dateTime:T}";
        }
    }
}