using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Edubase.Web.UI.Models;
using Edubase.Data.Entity;
using AutoMapper;
using Edubase.Web.UI.Identity;
using FluentValidation.Mvc;
using Edubase.Data.Identity;
using Edubase.Data.Entity.Permissions;
using MoreLinq;
using System.Dynamic;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Common;

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
                    var dataModel = dc.Establishments.FirstOrDefault(x => x.Urn == model.Urn);

                    if (User.IsInRole(Roles.Admin))
                    {
                        Mapper.Map(model, dataModel);
                        dc.SaveChanges();
                        return RedirectToAction("Details", "Schools", new { id = model.Urn.Value });
                    }

                    else // user is in restrictive role
                    {
                        var role = Roles.RestrictiveRoles.FirstOrDefault(x => User.IsInRole(x));
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
                        mapper.Map(model, dataModel);

                        var establishment = Mapper.Map<CreateEditEstablishmentModel, Establishment>(model);

                        permissions.ForEach(p =>
                        {
                            var newValue = ReflectionHelper.GetProperty(establishment, p.PropertyName).Clean();
                            var oldValue = ReflectionHelper.GetProperty(dataModel, p.PropertyName).Clean();
                            if (newValue != oldValue)
                            {
                                dc.EstablishmentApprovalQueue.Add(new EstablishmentApprovalQueue
                                {
                                    Urn = dataModel.Urn,
                                    Name = p.PropertyName,
                                    Value = newValue
                                });
                            }
                        });
                        dc.SaveChanges();
                        return RedirectToAction("Details", "Schools", new { id = model.Urn.Value, pendingUpdates = true });
                    }
                }
            }
            return View("CreateEdit", model);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View("CreateEdit", new CreateEditEstablishmentModel());
        }


        [HttpPost, ValidateAntiForgeryToken]
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