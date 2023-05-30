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
    // TODO: Potential nuance here -- might need to be able to view, but not modify (thus need to push this down to specific methods?)
    [EdubaseAuthorize(Roles = AuthorisationRoles)]
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
        public async Task<ActionResult> EditStatus(EditDataQualityStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.Items)
                {
                    // Only update the item value if the user has permission to so for that specific establishment type
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
        public async Task<ActionResult> EditDataOwnerDetails()
        {
            var isAdmin = User.InRole(AuthorizedRoles.IsAdmin);
            var datasets = (await _dataQualityWriteService.GetDataQualityStatus()).Select(d => new DataQualityDataOwnerItem()
                {
                    EstablishmentType = d.EstablishmentType,
                    Name = d.DataOwner,
                    Email = d.Email,
                })
                .Where(item =>
                {
                    if (isAdmin)
                    {
                        // Admin (backoffice) users may update details for all data owner entries
                        return true;
                    }

                    // TODO: Extract this logic out to function/method

                    // Other users may edit the data owner details, if they are a data owner for that specific establishment type
                    var permissionsEntry = _roleToDataSetMappings
                        .SingleOrDefault(x => x.Value == item.EstablishmentType);

                    // TODO: What happens on `default`? is `permissionsEntry` populated? (concern - risk of null ref exception?)

                    var establishmentTypeMatches = permissionsEntry.Value == item.EstablishmentType;
                    var userPermittedToEditEstablishmentTypeOwnerDetails = User.InRole(permissionsEntry.Key);

                    var hasSpecificPermissionToEdit = establishmentTypeMatches && userPermittedToEditEstablishmentTypeOwnerDetails;

                    return hasSpecificPermissionToEdit;
                })
                .ToList();

            var data = new EditDataQualityDataOwnerViewModel
            {
                Items = datasets
            };


            data.Items.Sort((x,y) => x.EstablishmentType.GetEnumMember().CompareTo(y.EstablishmentType.GetEnumMember()));
            return View(data);
        }

        [HttpPost, Route("DataQuality/EditDataOwnerDetails")]
        public async Task<ActionResult> EditDataOwnerDetails(EditDataQualityDataOwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.Items)
                {
                    // Only update the item value if the user has permission to so for that specific establishment type
                    var entry = _roleToDataSetMappings.SingleOrDefault(x => x.Value == item.EstablishmentType);
                    if (User.InRole(entry.Key, AuthorizedRoles.IsAdmin))
                    {
                        await _dataQualityWriteService.UpdateDataQualityDataOwner(
                            item.EstablishmentType,
                            item.Name,
                            item.Email
                        );
                    }
                }

                return RedirectToAction("ViewStatus", new {dataUpdated = true});
            }

            return View(model);
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

                // Allow admins to update data owner details, and data owners may self-serve (TODO: Review)
                UserCanUpdateDataOwnerDetails = User.InRole(AuthorizedRoles.IsAdmin)
                    || User.InRole(_roleToDataSetMappings.Keys.ToArray())
            };

            return View(data);
        }
    }
}
