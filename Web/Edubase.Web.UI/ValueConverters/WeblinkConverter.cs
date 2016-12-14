﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Common;

namespace Edubase.Web.UI.ValueConverters
{
    public class WeblinkConverter
    {
        public static string Convert(string url)
        {
            url = url.Clean();
            if (url != null && !url.StartsWith("http")) url = "http://" + url;
            return url;
        }
    }
}