﻿using Edubase.Web.UI.Filters;
using System.Configuration;
using System.Web.Mvc;

namespace Edubase.Web.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, ExceptionHandler exceptionHandler)
        {
            filters.Add(new UnauthorizedAccessAttribute());
            filters.Add(exceptionHandler);
        }
    }
}
