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

namespace Edubase.Web.UI.Controllers
{
    [Authorize(Roles = IdentityConstants.AccessAllSchoolsRoleName)]
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
                    Mapper.Map(model, dataModel);
                    dc.SaveChanges();
                    return RedirectToAction("Details", "Schools", new { id = model.Urn.Value });
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