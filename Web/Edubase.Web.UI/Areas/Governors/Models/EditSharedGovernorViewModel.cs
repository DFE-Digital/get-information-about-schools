﻿using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Establishments.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class EditSharedGovernorViewModel : IEstablishmentPageViewModel
    {
        public string GovernorType { get; set; }
        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }

        public SharedGovernorViewModel Governor { get; set; }
        public string TypeName { get; set; }
        GroupModel IEstablishmentPageViewModel.LegalParentGroup { get; set; }
        string IEstablishmentPageViewModel.LegalParentGroupToken { get; set; }
    }
}