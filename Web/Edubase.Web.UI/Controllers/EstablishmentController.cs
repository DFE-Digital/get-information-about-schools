using AutoMapper;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Data.Identity;
using Edubase.Services;
using Edubase.Web.UI.Models;
using FluentValidation.Mvc;
using Microsoft.ServiceBus.Messaging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Authorize]
    public class EstablishmentController : Controller
    {
        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var dc = ApplicationDbContext.Create())
            {
                var dataModel = dc.Establishments.FirstOrDefault(x => x.Urn == id);
                var viewModel = Mapper.Map<Establishment, CreateEditEstablishmentModel>(dataModel);
                return View("CreateEdit", viewModel);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(CreateEditEstablishmentModel model)
        {
            if (ModelState.IsValid)
            {
                using (var dc = ApplicationDbContext.Create())
                {
                    Establishment dataModel2 = null;
                    using (var dc2 = ApplicationDbContext.Create()) dataModel2 = dc2.Establishments.FirstOrDefault(x => x.Urn == model.Urn);
                    var dataModel = dc.Establishments.FirstOrDefault(x => x.Urn == model.Urn);

                    if (User.IsInRole(Roles.Admin) && 1 == 2) // temporarily disabled this clause
                    {
                        Mapper.Map(model, dataModel);
                        dc.SaveChanges();
                        return RedirectToAction("Details", "Schools", new { id = model.Urn.Value });
                    }

                    else // user is in restrictive role
                    {
                        var role = Roles.RestrictiveRoles.FirstOrDefault(x => User.IsInRole(x)) ?? Roles.Academy;
                        var permissions = dc.Permissions.Where(x => x.RoleName == role).ToArray();

                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<ContactDetailsViewModel, ContactDetail>();
                            cfg.CreateMap<AddressViewModel, Address>();
                            cfg.CreateMap<DateTimeViewModel, DateTime?>().ConvertUsing<DateTimeTypeConverter>();

                            var map = cfg.CreateMap<CreateEditEstablishmentModel, Establishment>();
                            permissions.Where(x => !x.PropertyName.Contains("_"))
                                .ForEach(p => map.ForMember(p.PropertyName, opt => opt.Ignore()));
                        });

                        var mapper = config.CreateMapper();
                        var estabTemp = mapper.Map(model, dataModel2);
                        var changes = ReflectionHelper.DetectChanges(estabTemp, dataModel, typeof(Address), typeof(ContactDetail));
                        mapper.Map(model, dataModel);



                        var establishment = Mapper.Map<CreateEditEstablishmentModel, Establishment>(model);
                        List<string> permPropertiesThatChanged = new List<string>();
                        permissions.ForEach(p =>
                        {
                            var newValue = ReflectionHelper.GetProperty(establishment, p.PropertyName).Clean();
                            var oldValue = ReflectionHelper.GetProperty(dataModel, p.PropertyName).Clean();
                            if (newValue != oldValue)
                            {
                                permPropertiesThatChanged.Add(p.PropertyName);
                                dc.EstablishmentApprovalQueue.Add(new EstablishmentApprovalQueue
                                {
                                    Urn = dataModel.Urn,
                                    Name = p.PropertyName,
                                    NewValue = newValue,
                                    OldValue = oldValue,
                                    OriginatorUserId = ((ClaimsPrincipal)(User)).FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value
                                });
                            }
                        });

                        dc.SaveChanges();

                        if (changes.Count > 0)
                        {
                            new SmtpClient().Send("kris.dyson@contentsupport.co.uk", ConfigurationManager.AppSettings["DataOwnerEmailAddress"], "Establishment data changed",
                                $"For Establishment URN: {dataModel.Urn}, the following has changed: \r\n" + string.Join("\r\n", changes));
                        }

                        if (permPropertiesThatChanged.Count > 0)
                        {
                            new SmtpClient().Send("kris.dyson@contentsupport.co.uk", ConfigurationManager.AppSettings["DataOwnerEmailAddress"], "Establishment data changes require approval",
                                $"For Establishment URN: {dataModel.Urn}, the following has changed and requires approval: \r\n" + string.Join("\r\n", permPropertiesThatChanged));
                        }

                        var t = new BusMessagingService().SendEstablishmentUpdateMessage(dataModel);

                        return RedirectToAction("Details", "Schools", new { id = model.Urn.Value, pendingUpdates = permPropertiesThatChanged.Count > 0 });
                    }
                }
            }
            return View("CreateEdit", model);
        }
        
        [HttpGet, Authorize(Roles="Admin,LA")]
        public ActionResult Create() => View("CreateEdit", new CreateEditEstablishmentModel());


        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,LA")]
        public ActionResult Create([CustomizeValidator(RuleSet = "oncreate")] CreateEditEstablishmentModel model)
        {
            if (ModelState.IsValid)
            {
                using (var dc = ApplicationDbContext.Create())
                {
                    var dataModel = Mapper.Map<Establishment>(model);
                    dc.Establishments.Add(dataModel);
                    dc.SaveChanges();
                    return RedirectToAction("Details", "Schools", new { id = dataModel.Urn });
                }
            }
            else return View("CreateEdit", model);
        }

        




    }
}