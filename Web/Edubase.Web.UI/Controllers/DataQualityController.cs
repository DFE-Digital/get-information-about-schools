using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Data.Entity;
using Edubase.Services.DataQuality;
using Edubase.Services.Security;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.DataQuality;

namespace Edubase.Web.UI.Controllers
{
    [EdubaseAuthorize]
    public class DataQualityController : Controller
    {
        private readonly IDataQualityWriteService dataQualityWriteService;
        private readonly int dataQualityUpdatePeriod;
        private readonly Dictionary<string, DataQualityStatus.DataQualityEstablishmentType> _roleToDataSetMappings = new Dictionary<string, DataQualityStatus.DataQualityEstablishmentType>
        {
            { EdubaseRoles.EFADO,  DataQualityStatus.DataQualityEstablishmentType.OpenAcademiesAndFreeSchools},
            { EdubaseRoles.AP_AOS,  DataQualityStatus.DataQualityEstablishmentType.AcademyOpeners},
            { EdubaseRoles.IEBT,  DataQualityStatus.DataQualityEstablishmentType.IndependentSchools},
            { EdubaseRoles.APT,  DataQualityStatus.DataQualityEstablishmentType.PupilReferralUnits},
            { EdubaseRoles.SOU,  DataQualityStatus.DataQualityEstablishmentType.LaMaintainedSchools},
            { EdubaseRoles.FST,  DataQualityStatus.DataQualityEstablishmentType.FreeSchoolOpeners}
        };

        public DataQualityController(IDataQualityWriteService dataQualityWriteService)
        {
            this.dataQualityWriteService = dataQualityWriteService;
            if (!int.TryParse(ConfigurationManager.AppSettings["DataQualityUpdatePeriod"], out dataQualityUpdatePeriod))
            {
                this.dataQualityUpdatePeriod = 7;
            }
        }

        [HttpGet, Route("DataQuality/Status")]
        public async Task<ActionResult> Status()
        {
            var items = (await dataQualityWriteService.GetDataQualityStatus()).Select(d => new DataQualityStatusItem
            {
                EstablishmentType = d.EstablishmentType,
                LastUpdated = new DateTimeViewModel(d.LastUpdated)
            }).ToList();

            var data = new DataQualityStatusViewModel
            {
                Items = items,
                Urgent = items.Any(i => i.LastUpdated.ToDateTime().Value.AddDays(dataQualityUpdatePeriod) < DateTime.Now.Date)
            };

            return View(data);
        }

        [HttpGet, Route("DataQuality/Edit")]
        public async Task<ActionResult> EditStatus()
        {
            var data = new EditDataQualityStatusViewModel
            {
                Items = new List<DataQualityStatusItem>()
            };

            foreach (var kvp in _roleToDataSetMappings)
            {
                if (User.InRole(kvp.Key, EdubaseRoles.ROLE_BACKOFFICE))
                {
                    data.Items.Add(new DataQualityStatusItem { EstablishmentType = kvp.Value });
                }
            }

            return View(data);
        }

        [HttpPost, Route("DataQuality/Edit")]
        public async Task<ActionResult> EditStatus(EditDataQualityStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.Items)
                {
                    await dataQualityWriteService.UpdateDataQualityDate(item.EstablishmentType,
                        item.LastUpdated.ToDateTime().Value);
                }

                return RedirectToAction("ViewStatus", new {dataUpdated = true});
            }

            return View(model);
        }

        [HttpGet, Route("DataQuality")]
        public async Task<ActionResult> ViewStatus(bool dataUpdated = false)
        {
            var data = new FullDataQualityStatusViewModel
            {
                Items = (await dataQualityWriteService.GetDataQualityStatus()).Select(d => new FullDataQualityStatusItem
                {
                    EstablishmentType = d.EstablishmentType,
                    LastUpdated = new DateTimeViewModel(d.LastUpdated),
                    DataOwner = d.DataOwner,
                    Email = d.Email
                }).ToList(),
                DataUpdated = dataUpdated
            };

            return View(data);
        }
    }
}