using Autofac;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers
{
    public static class LangExt
    {
        public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<LookupDto> items, int? currentId) 
            => items.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = currentId.HasValue && currentId.Value == x.Id });
        
    }
}