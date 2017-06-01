using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.DataQuality;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.DataQuality;

namespace Edubase.Web.UI.Controllers
{
    public class DataQualityController : Controller
    {
        private readonly IDataQualityWriteService dataQualityWriteService;
        private readonly int dataQualityUpdatePeriod;

        public DataQualityController(IDataQualityWriteService dataQualityWriteService)
        {
            this.dataQualityWriteService = dataQualityWriteService;
            if (!int.TryParse(ConfigurationManager.AppSettings["DataQualityUpdatePeriod"], out dataQualityUpdatePeriod))
            {
                this.dataQualityUpdatePeriod = 7;
            }
        }

        [Route("DataQuality/Status")]
        public async Task<ActionResult> Status()
        {
            var items = (await dataQualityWriteService.GetDataQualityStatus()).Select(d => new DataQualityStatusItem
            {
                EstablishmentType = d.EstablishmentType.GetEnumMember(),
                LastUpdated = new DateTimeViewModel(d.LastUpdated)
            }).ToList();

            var data = new DataQualityStatusViewModel
            {
                Items = items,
                Urgent = items.Any(i => i.LastUpdated.ToDateTime().Value.AddDays(dataQualityUpdatePeriod) < DateTime.Now.Date)
            };

            return View(data);
        }

        [Route("DataQuality/EditStatus")]
        public async Task<ActionResult> EditStatus()
        {
            var data = new EditDataQualityStatusViewModel
            {
                Items = (await dataQualityWriteService.GetDataQualityStatus()).Select(d => new DataQualityStatusItem
                {
                    EstablishmentType = d.EstablishmentType.GetEnumMember(),
                    LastUpdated = new DateTimeViewModel(d.LastUpdated)
                }).ToList()
            };

            return View(data);
        }

        [HttpPost]
        public ActionResult EditStatus(EditDataQualityStatusViewModel model)
        {
            return null;
        }

        public ActionResult ViewStatus()
        {
            return null;
        }
    }
}