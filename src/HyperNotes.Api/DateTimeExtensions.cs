using System;

namespace HyperNotes.Api {
    public static class DateTimeExtensions {
         public static double ToJavascriptDate(this DateTime self) {
             return self.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
         }
    }
}