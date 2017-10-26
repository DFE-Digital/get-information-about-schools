using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    public class BulkCreateFreeSchoolsViewModel
    {
        public HttpPostedFileBase BulkFile { get; set; }
        public BulkCreateFreeSchoolsResult Result { get; set; }
    }
}