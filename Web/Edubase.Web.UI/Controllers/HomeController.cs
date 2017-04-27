using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Common.Text;
using Edubase.Data.Repositories.Establishments;
using Edubase.Services;
using Edubase.Services.Domain;
using Edubase.Services.Lookup;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Home"), Route("{action=index}")]
    public class HomeController : EduBaseController
    {
        private readonly ILookupService _lookup;

        public HomeController(ILookupService lookup)
        {
            _lookup = lookup;
        }

        [Route]
        public ActionResult Index()
        {
            var model = new Models.HomepageViewModel();
            model.AllowApprovals = User.Identity.IsAuthenticated;
            model.AllowSchoolCreation = User.Identity.IsAuthenticated;
            model.AllowTrustCreation = User.Identity.IsAuthenticated;
            return View(model);
        }

        [Route("enums/now")]
        public async Task<ActionResult> Enums()
        {
            var sb = new StringBuilder();
            //Action<string, IEnumerable<LookupDto>> f = (name, list) =>
            //{
            //    sb.AppendLine($"\tpublic enum {name}");
            //    sb.AppendLine("\t{");
            //    foreach (var item in list)
            //    {
            //        sb.AppendLine($"\t\t{AsEnumName(item.Name)} = {item.Id},");
            //    }
            //    sb.AppendLine("\t}");
            //    sb.AppendLine();
            //    sb.AppendLine();
            //    sb.AppendLine();
            //};

            //var l = (Services.Texuna.Lookup.LookupApiService)_lookup;

            //foreach (var key in l._map.Keys)
            //{
            //    try
            //    {
            //        f("e" + AsEnumName(key), await l._map[key]());
            //    }
            //    catch (Exception ex)
            //    {
            //        sb.AppendLine(key+$" failed with {ex}");
            //    }
                
            //}


            //f("eLookupGovernorRole", await _lookup.GovernorRolesGetAllAsync());
            //f("eLookupGovernorAppointingBody", await _lookup.GovernorAppointingBodiesGetAllAsync());
            //f("eLookupGroupType", await _lookup.GroupTypesGetAllAsync());
            //f("eLookupEstablishmentTypeGroup", await _lookup.EstablishmentTypeGroupsGetAllAsync());
            //f("eLookupEstablishmentType", await _lookup.EstablishmentTypesGetAllAsync());
            //f("eLookupEstablishmentStatus", await _lookup.EstablishmentStatusesGetAllAsync());
            //f("eLookupGroupStatus", await _lookup.GroupStatusesGetAllAsync());

            return Content(sb.ToString(), "text/plain");
        }

        public static string AsEnumName(string text)
        {
            var retVal = text.CleanOfNonChars(true).ToTitleCase().Replace(" ", "");
            if (retVal.IsInteger()) retVal = "v_" + retVal;
            if (char.IsNumber(retVal[0])) retVal = "_" + retVal;
            return retVal;
        }
    }
}