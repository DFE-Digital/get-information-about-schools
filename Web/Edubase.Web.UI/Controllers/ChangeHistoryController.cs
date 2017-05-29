using Edubase.Services.Texuna.ChangeHistory;
using Edubase.Web.Resources;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("ChangeHistory"), Route("{action=index}")]
    public class ChangeHistoryController : Controller
    {
        readonly IChangeHistoryService _svc;

        public ChangeHistoryController(IChangeHistoryService svc)
        {
            _svc = svc;
        }

        [HttpGet, EdubaseAuthorize, Route(Name = "ChangeHistory")]
        public async Task<ActionResult> Index()
        {
            var vm = new ChangeHistoryViewModel();

            vm.EstablishmentFields = await _svc.GetEstablishmentFieldsAsync(User);
            
            return View(vm);
        }
    }
}