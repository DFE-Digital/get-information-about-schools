﻿using Edubase.Common;
using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    using Services.Groups.Models;
    using GT = eLookupGroupType;

    public class GroupEditorViewModelBase : IGroupPageViewModel
    {
        public enum eGroupTypeMode
        {
            Federation, // Federation
            ChildrensCentre, // Group or Collaboration
            Trust, // School trust
            AcademyTrust, // MATs and SATs,
            Sponsor // Academy sponsor
        }
        
        private static readonly Dictionary<eGroupTypeMode, string> _entityNames = new Dictionary<eGroupTypeMode, string>
        {
            [eGroupTypeMode.ChildrensCentre] = "children's centre group or collaboration",
            [eGroupTypeMode.Trust] = "school trust",
            [eGroupTypeMode.Federation] = "school federation",
            [eGroupTypeMode.AcademyTrust] = "Academy Trust",
            [eGroupTypeMode.Sponsor] = "academy sponsor",
        };

        public eGroupTypeMode GroupTypeMode
        {
            get
            {
                if (GroupTypeId.OneOfThese(GT.ChildrensCentresCollaboration, GT.ChildrensCentresGroup)) return eGroupTypeMode.ChildrensCentre;
                else if (GroupTypeId.OneOfThese(GT.Federation)) return eGroupTypeMode.Federation;
                else if (GroupTypeId.OneOfThese(GT.Trust)) return eGroupTypeMode.Trust;
                else if (GroupTypeId.OneOfThese(GT.SchoolSponsor)) return eGroupTypeMode.Sponsor;
                else if (GroupTypeId.OneOfThese(GT.MultiacademyTrust, GT.SingleacademyTrust)) return eGroupTypeMode.AcademyTrust;
                else throw new NotImplementedException($"GroupTypeId '{GroupTypeId}' is not supported");
            }
        }

        public string ListOfEstablishmentsPluralName { get; set; }
        public string PageTitle => string.Concat(GroupUId.HasValue ? "Edit " : "Create ", EntityName);
        public string EntityName => _entityNames.Get(GroupTypeMode);

        public string Layout { get; set; }

        public int? GroupUId { get; set; }
        public string GroupName { get; set; }
        public int? GroupTypeId { get; set; }
        public string GroupTypeName { get; set; }
        public string SelectedTabName { get; set; }
        public int? StatusId { get; set; }
        
    }
}