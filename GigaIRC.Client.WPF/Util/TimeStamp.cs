﻿using System;
using System.Diagnostics;
using System.Text;
using GigaIRC.Annotations;

namespace GigaIRC.Client.WPF.Util
{
    public class TimeStamp
    {
        [StringFormatMethod("format")]
        public static string Format(string format, params object[]args)
        {
            var now = DateTime.Now;

            return string.Format("[{0:hh}:{0:mm}:{0:ss}] {1}", now, string.Format(format, args));
        }
    }
}
