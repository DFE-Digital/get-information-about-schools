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
    // Nuance here -- logged in users may view, but not modify, data quality status or data owner details.
    [EdubaseAuthorize]
    public class DataQualityController : Controller
    {
        private readonly IDataQualityWriteService _dataQualityWriteService;
        private readonly int _dataQualityUpdatePeriod;

        private readonly Dictionary<string, DataQualityStatus.DataQualityEstablishmentType> _roleToDataSetMappings = new Dictionary<string, DataQualityStatus.DataQualityEstablishmentType>
        {
            { EdubaseRoles.EFADO,  DataQualityStatus.DataQualityEstablishmentType.OpenAcademiesAndFreeSchools},
            { EdubaseRoles.AP_AOS,  DataQualityStatus.DataQualityEstablishmentType.AcademyOpeners},
            { EdubaseRoles.IEBT,  DataQualityStatus.DataQualityEstablishmentType.IndependentSchools},
            { EdubaseRoles.APT,  DataQualityStatus.DataQualityEstablishmentType.PupilReferralUnits},
            { EdubaseRoles.SOU,  DataQualityStatus.DataQualityEstablishmentType.LaMaintainedSchools},
            { EdubaseRoles.FST,  DataQualityStatus.DataQualityEstablishmentType.FreeSchoolOpeners}
        };

        private const string AuthorisationRoles = AuthorizedRoles.IsAdmin + "," + EdubaseRoles.EFADO + "," + EdubaseRoles.AP_AOS + "," + EdubaseRoles.IEBT + "," + EdubaseRoles.APT + "," + EdubaseRoles.SOU + "," + EdubaseRoles.FST;

        public DataQualityController(IDataQualityWriteService dataQualityWriteService)
        {
            this._dataQualityWriteService = dataQualityWriteService;
            if (!int.TryParse(ConfigurationManager.AppSettings["DataQualityUpdatePeriod"], out _dataQualityUpdatePeriod))
            {
                this._dataQualityUpdatePeriod = 7;
            }
        }

        [HttpGet, Route("DataQuality/Status")]
        [EdubaseAuthorize]
        public async Task<ActionResult> Status()
        {
            var items = (await _dataQualityWriteService.GetDataQualityStatus())
                .Select(d => new DataQualityStatusItem
                {
                    EstablishmentType = d.EstablishmentType,
                    LastUpdated = new DateTimeViewModel(d.LastUpdated)
                })
                .OrderBy(x => x.EstablishmentType.GetEnumMember())
                .ToList();

            var urgent = false;
            foreach (var kvp in _roleToDataSetMappings)
            {
                if (User.InRole(kvp.Key, AuthorizedRoles.IsAdmin))
                {
                    var lastUpdated = items.FirstOrDefault(d => d.EstablishmentType == kvp.Value)?.LastUpdated;
                    if (lastUpdated?.ToDateTime() == null ||
                        lastUpdated.ToDateTime().Value.AddDays(_dataQualityUpdatePeriod) < DateTime.Now.Date)
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
        [EdubaseAuthorize(Roles = AuthorisationRoles)]
        public async Task<ActionResult> EditStatus()
        {
            var datasets = (await _dataQualityWriteService.GetDataQualityStatus()).Select(d => new DataQualityStatusItem
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
        [EdubaseAuthorize(Roles = AuthorisationRoles)]
        public async Task<ActionResult> EditStatus(EditDataQualityStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.Items)
                {
                    // Only update the item value if the user has permission to so for that _specific_ establishment type
                    var entry = _roleToDataSetMappings.SingleOrDefault(x => x.Value == item.EstablishmentType);
                    if (User.InRole(entry.Key, AuthorizedRoles.IsAdmin))
                    {
                        await _dataQualityWriteService.UpdateDataQualityDate(
                            item.EstablishmentType,
                            item.LastUpdated.ToDateTime(DateTimeKind.Utc).Value
                        );
                    }
                }

                return RedirectToAction("ViewStatus", new {dataUpdated = true});
            }

            return View(model);
        }


        [HttpGet, Route("DataQuality/EditDataOwnerDetails")]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditDataOwnerDetails()
        {
            var data = new EditDataQualityDataOwnerViewModel
            {
                Items = (await _dataQualityWriteService.GetDataQualityStatus())
                    .Select(d =>
                        new DataQualityDataOwnerItem()
                        {
                            EstablishmentType = d.EstablishmentType,
                            Name = d.DataOwner,
                            Email = d.Email,
                        })
                    // Only show items that the user has permission to edit
                    .Where(item => UserMayEditDataQualityOwnerDetails(item.EstablishmentType))
                    .ToList()
            };

            data.Items.Sort((x,y) => x.EstablishmentType.GetEnumMember().CompareTo(y.EstablishmentType.GetEnumMember()));

            return View(data);
        }

        [HttpPost, Route("DataQuality/EditDataOwnerDetails")]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditDataOwnerDetails(EditDataQualityDataOwnerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var permittedEstablishmentTypes = (await _dataQualityWriteService.GetDataQualityStatus())
                .Select(x => x.EstablishmentType)
                .ToList();

            foreach (var item in model.Items)
            {
                // Only allow editing of items that already exist
                if (!permittedEstablishmentTypes.Contains(item.EstablishmentType))
                {
                    continue;
                }

                // Update the item value, only if the user has permission to so for that establishment type
                if (!UserMayEditDataQualityOwnerDetails(item.EstablishmentType))
                {
                    continue;
                }

                await _dataQualityWriteService.UpdateDataQualityDataOwner(
                    item.EstablishmentType,
                    item.Name,
                    item.Email
                );
            }

            return RedirectToAction("ViewStatus", new {dataUpdated = true});

        }

        [HttpGet, Route("DataQuality")]
        public async Task<ActionResult> ViewStatus(bool dataUpdated = false)
        {
            var dataQualityStatusList = await _dataQualityWriteService.GetDataQualityStatus();
            var data = new FullDataQualityStatusViewModel
            {
                Items = dataQualityStatusList
                    .Select(d => new FullDataQualityStatusItem
                    {
                        EstablishmentType = d.EstablishmentType,
                        LastUpdated = new DateTimeViewModel(d.LastUpdated),
                        DataOwner = d.DataOwner,
                        Email = d.Email
                    })
                    .OrderBy(x => x.EstablishmentType.GetEnumMember())
                    .ToList(),
                DataUpdated = dataUpdated,

                // Backoffice role users do not need to be prompted (appears on logon and a separate config entry), but should be able to update the date
                UserCanUpdateLastUpdated = Api.UserRolesController.UserRequiresDataQualityPrompt(User) || User.InRole(AuthorizedRoles.IsAdmin),

                // Allow only admins (backoffice users) to update data owner details - data owners may not edit their own details
                UserCanUpdateDataOwnerDetails = User.InRole(AuthorizedRoles.IsAdmin) // || User.InRole(_roleToDataSetMappings.Keys.ToArray())
            };

            return View(data);
        }


        private bool UserMayEditDataQualityOwnerDetails(DataQualityStatus.DataQualityEstablishmentType establishmentType)
        {
            if(User.InRole(AuthorizedRoles.IsAdmin))
            {
                // Administrators (e.g., backoffice users) may edit all data owner details
                return true;
            }

            /* NOTE: Commented out as only allow admins permitted to edit data owner details */
            // // All other users may only edit data owner details if their user group is mapped to the given establishment type (note, might be multiple establishment types per user group
            // var entry = _roleToDataSetMappings.SingleOrDefault(x => x.Value == establishmentType);
            // var roleWhichMayEditThisEstablishmentType = entry.Key;
            // if(User.InRole(roleWhichMayEditThisEstablishmentType))
            // {
            //     return true;
            // }

            return false;
        }
    }
}
