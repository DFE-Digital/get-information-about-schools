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


            // I feel dirty for writing the following...
            if (User.InRole(EdubaseRoles.EFADO, EdubaseRoles.ROLE_BACKOFFICE))
            {
                data.Items.Add(datasets.SingleOrDefault(d => d.EstablishmentType == DataQualityStatus.DataQualityEstablishmentType.OpenAcademiesAndFreeSchools));
            }

            if (User.InRole(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE))
            {
                data.Items.Add(datasets.SingleOrDefault(d => d.EstablishmentType == DataQualityStatus.DataQualityEstablishmentType.AcademyOpeners));
            }

            if (User.InRole(EdubaseRoles.IEBT, EdubaseRoles.ROLE_BACKOFFICE))
            {
                data.Items.Add(datasets.SingleOrDefault(d => d.EstablishmentType == DataQualityStatus.DataQualityEstablishmentType.IndependentSchools));
            }

            if (User.InRole(EdubaseRoles.APT, EdubaseRoles.ROLE_BACKOFFICE))
            {
                data.Items.Add(datasets.SingleOrDefault(d => d.EstablishmentType == DataQualityStatus.DataQualityEstablishmentType.PupilReferralUnits));
            }

            if (User.InRole(EdubaseRoles.SOU, EdubaseRoles.ROLE_BACKOFFICE))
            {
                data.Items.Add(datasets.SingleOrDefault(d => d.EstablishmentType == DataQualityStatus.DataQualityEstablishmentType.LaMaintainedSchools));
            }

            if (User.InRole(EdubaseRoles.FST, EdubaseRoles.ROLE_BACKOFFICE))
            {
                data.Items.Add(datasets.SingleOrDefault(d => d.EstablishmentType == DataQualityStatus.DataQualityEstablishmentType.FreeSchoolOpeners));
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