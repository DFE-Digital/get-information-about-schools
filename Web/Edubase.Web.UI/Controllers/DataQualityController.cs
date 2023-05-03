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
using Edubase.Web.UI.Helpers;

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
            { EdubaseRoles.FST,  DataQualityStatus.DataQualityEstablishmentType.FreeSchoolOpeners},
            { EdubaseRoles.YCS,  DataQualityStatus.DataQualityEstablishmentType.AcademySecure16to19Openers}
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
            }).OrderBy(x => x.EstablishmentType.GetEnumMember()).ToList();

            var urgent = false;
            foreach (var kvp in _roleToDataSetMappings)
            {
                if (User.InRole(kvp.Key, AuthorizedRoles.IsAdmin))
                {
                    var lastUpdated = items.FirstOrDefault(d => d.EstablishmentType == kvp.Value)?.LastUpdated;
                    if (lastUpdated?.ToDateTime() == null || 
                        lastUpdated.ToDateTime().Value.AddDays(dataQualityUpdatePeriod) < DateTime.Now.Date)
                    {
                        urgent = true;
                    }
                }
            }

            var data = new DataQualityStatusViewModel
            {
                Items = items,
                Urgent = urgent
            };

            return View(data);
        }

        [HttpGet, Route("DataQuality/Edit")]
        public async Task<ActionResult> EditStatus()
        {
            var datasets = (await dataQualityWriteService.GetDataQualityStatus()).Select(d => new DataQualityStatusItem
                {
                    EstablishmentType = d.EstablishmentType,
                    LastUpdated = new DateTimeViewModel(d.LastUpdated)
                })
                .ToList();

            var data = new EditDataQualityStatusViewModel
            {
                Items = new List<DataQualityStatusItem>()
            };

            foreach (var kvp in _roleToDataSetMappings)
            {
                if (User.InRole(kvp.Key, AuthorizedRoles.IsAdmin))
                {
                    var item = new DataQualityStatusItem {EstablishmentType = kvp.Value};

                    if (User.IsInRole(AuthorizedRoles.IsAdmin))
                    {
                        item.LastUpdated = datasets.FirstOrDefault(d => d.EstablishmentType == kvp.Value)?.LastUpdated;
                    }

                    data.Items.Add(item);
                }
            }
            data.Items.Sort((x,y) => x.EstablishmentType.GetEnumMember().CompareTo(y.EstablishmentType.GetEnumMember()));
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
                        item.LastUpdated.ToDateTime(DateTimeKind.Utc).Value);
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
                }).OrderBy(x=>x.EstablishmentType.GetEnumMember()).ToList(),
                DataUpdated = dataUpdated
            };

            data.UserCanUpdate = Api.UserRolesController.UserRequiresDataQualityPrompt(User);

            return View(data);
        }
    }
}
