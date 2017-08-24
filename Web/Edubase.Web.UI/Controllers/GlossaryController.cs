﻿using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Security;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("glossary"), Route("{action=index}")]
    public class GlossaryController : Controller
    {
        private readonly GlossaryRepository _glossaryRepository;

        public GlossaryController(GlossaryRepository glossaryRepository)
        {
            _glossaryRepository = glossaryRepository;
        }

        [Route(Name = "Glossary")]
        public async Task<ActionResult> Index()
        {
            var result = await _glossaryRepository.GetAllAsync(1000);
            return View(new GlossaryViewModel(result.Items) { UserCanEdit = User.IsInRole(EdubaseRoles.ROLE_BACKOFFICE) });
        }

        [Route("Create", Name = "CreateGlossaryItem"), HttpGet, EdubaseAuthorize(Roles = EdubaseRoles.ROLE_BACKOFFICE)]
        public ActionResult Create() => View("CreateEdit", new GlossaryItemViewModel());
        
        [Route("Edit/{id}", Name = "EditGlossaryItem"), HttpGet, EdubaseAuthorize(Roles = EdubaseRoles.ROLE_BACKOFFICE)]
        public async Task<ActionResult> EditAsync(string id)
        {
            var item = await _glossaryRepository.GetAsync(id);
            if (item == null) return HttpNotFound();
            else return View("CreateEdit", new GlossaryItemViewModel
            {
                Id = id,
                Content = item.Content,
                Title = item.Title
            });
        }

        [Route("Edit/{id}", Name = "PostEditGlossaryItem"), HttpPost, EdubaseAuthorize(Roles = EdubaseRoles.ROLE_BACKOFFICE)]
        public async Task<ActionResult> EditAsync(GlossaryItemViewModel viewModel)
        {
            var item = await _glossaryRepository.GetAsync(viewModel.Id);
            if (item == null) return HttpNotFound();

            if (viewModel.IsDeleting)
            {
                await _glossaryRepository.DeleteAsync(viewModel.Id);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                item.Title = viewModel.Title;
                item.Content = viewModel.Content;
                await _glossaryRepository.UpdateAsync(item);
                return RedirectToAction(nameof(Index));
            }
            else return View("CreateEdit", viewModel);
        }

        [Route("Create", Name = "PostCreateGlossaryItem"), HttpPost, EdubaseAuthorize(Roles = EdubaseRoles.ROLE_BACKOFFICE)]
        public async Task<ActionResult> CreateAsync(GlossaryItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var item = new GlossaryItem
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content
                };
                await _glossaryRepository.CreateAsync(item);
                return RedirectToAction(nameof(Index));
            }
            else return View("CreateEdit", viewModel);
        }

    }
}