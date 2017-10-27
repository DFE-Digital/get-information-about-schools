﻿using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Downloads;
using Edubase.Services.Governors.Search;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Controllers;
using Edubase.Web.UI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors"), RoutePrefix("Search"), Route("{action=index}")]
    public class GovernorSearchController : EduBaseController
    {
        private IGovernorDownloadService _governorDownloadService;
        private IGovernorsReadService _governorsReadService;
        private ICachedLookupService _cachedLookupService;
        private IGroupReadService _groupReadService;
        private IEstablishmentReadService _establishmentReadService;

        public GovernorSearchController(IGovernorDownloadService governorDownloadService,
                    IGovernorsReadService governorsReadService,
                    ICachedLookupService cachedLookupService,
                    IGroupReadService groupReadService,
                    IEstablishmentReadService establishmentReadService)
        {
            _governorDownloadService = governorDownloadService;
            _governorsReadService = governorsReadService;
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }


        [HttpGet]
        public async Task<ActionResult> Index(GovernorSearchViewModel model) => await SearchGovernors(model);

        [HttpGet, Route("results-js")]
        public async Task<ActionResult> ResultsPartial(GovernorSearchViewModel model)
        {
            var result = await SearchGovernors(model);
            HttpContext.Response.Headers.Add("x-count", model.Count.ToString());
            return PartialView("Partials/_GovernorSearchResults", model);
        }

        [HttpGet, Route("PrepareDownload")]
        public async Task<ActionResult> PrepareDownload(GovernorSearchDownloadViewModel viewModel)
        {
            viewModel.SearchSource = eLookupSearchSource.Governors;
            if(viewModel.SearchQueryString == null) viewModel.SearchQueryString = Request.QueryString.ToString();

            var allowNonPublicDataDownload = User.InRole(EdubaseRoles.EDUBASE, EdubaseRoles.EDUBASE_CMT, EdubaseRoles.EFADO, EdubaseRoles.edubase_ddce, EdubaseRoles.SFC);
            viewModel.TotalSteps = allowNonPublicDataDownload ? 4 : 3;
            viewModel.Step++;

            if (allowNonPublicDataDownload && !viewModel.IncludeNonPublicData.HasValue)
                return View("Downloads/SelectDataset", viewModel);

            if (!viewModel.FileFormat.HasValue)
                return View("Downloads/SelectFormat", viewModel);

            var progressId = await _governorDownloadService.SearchWithDownloadGenerationAsync(
                new GovernorSearchDownloadPayload
                {
                    SearchPayload = CreateSearchPayload(viewModel),
                    FileFormat = viewModel.FileFormat.Value,
                    IncludeNonPublicData = allowNonPublicDataDownload && viewModel.IncludeNonPublicData.GetValueOrDefault()
                }, User);

            return RedirectToAction(nameof(Download), new { id = progressId, fileFormat = viewModel.FileFormat.Value, step = viewModel.Step + 1, viewModel.TotalSteps, viewModel.SearchQueryString, viewModel.SearchSource });
        }

        [HttpGet, Route("Download")]
        public async Task<ActionResult> Download(Guid id, eFileFormat fileFormat, int step, int totalSteps, string searchQueryString = null, eLookupSearchSource? searchSource = null)
        {
            var model = await _governorDownloadService.GetDownloadGenerationProgressAsync(id, User);
            var viewModel = new GovernorSearchDownloadGenerationProgressViewModel(model)
            {
                FileFormat = fileFormat,
                SearchSource = searchSource,
                SearchQueryString = searchQueryString,
                Step = step,
                TotalSteps = totalSteps
            };

            if (model.HasErrored)
                throw new Exception($"Download generation failed; Underlying error: '{model.Error}'");

            if (!model.IsComplete)
                return View("Downloads/PreparingFilePleaseWait", viewModel);

            viewModel.Step++;

            return View("Downloads/ReadyToDownload", viewModel);
        }

        private async Task<ActionResult> SearchGovernors(GovernorSearchViewModel model)
        {
            if (model.GovernorSearchModel?.RoleId != null && model.GovernorSearchModel.RoleId.Any())
            {
                model.SelectedRoleIds.AddRange(model.GovernorSearchModel.RoleId
                    .Where(r => !model.SelectedRoleIds.Contains(r))
                    .Cast<int>());
            }

            model.SearchQueryString = Request.QueryString.ToString();
            model.GovernorRoles = (await _cachedLookupService.GovernorRolesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
            model.AppointingBodies = (await _cachedLookupService.GovernorAppointingBodiesGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();
            model.LocalAuthorities = (await _cachedLookupService.LocalAuthorityGetAllAsync()).Select(x => new LookupItemViewModel(x)).ToList();

            var payload = CreateSearchPayload(model);
            var results = await _governorsReadService.SearchAsync(payload, User);
            model.Results = results.Items;

            if (model.StartIndex == 0) model.Count = results.Count;
            
            return View("Index", model);
        }

        private GovernorSearchPayload CreateSearchPayload(GovernorSearchViewModel model) => new GovernorSearchPayload(model.StartIndex, model.PageSize)
        {
            Gid = model.GovernorSearchModel.Gid?.ToString(),
            FirstName = model.GovernorSearchModel.Forename.Clean(),
            LastName = model.GovernorSearchModel.Surname.Clean(),
            RoleIds = model.SelectedRoleIds.ToArray(),
            SortBy = model.SortOption,
            IncludeHistoric = model.GovernorSearchModel.IncludeHistoric,
            GovernorTypesFlags = model.SelectedGovernorTypeFlagIds.Select(x => (eGovernorTypesFlag)x).ToArray(),
            LocalAuthorityIds = model.SelectedLocalAuthorityIds.ToArray()
        };

    }
}