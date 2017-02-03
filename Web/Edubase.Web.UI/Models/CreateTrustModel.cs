//using Edubase.Data.Entity;
//using Edubase.Services;
//using Edubase.Web.UI.Models.Validators;
//using FluentValidation.Attributes;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Edubase.Services.Domain;
//using System.ComponentModel;
//using System.Web.Mvc;

//namespace Edubase.Web.UI.Models
//{
//    public class CreateGroupModel
//    {
//        public CreateGroupModel(CompanyProfileDto companyProfile)
//        {
//            Name = companyProfile.Name;
//            OpenDate = companyProfile.IncorporationDate;
//            CompaniesHouseNumber = companyProfile.Number;
//            Address = companyProfile.Address;
//            TrustExists = false;
//        }

//        public CreateGroupModel()
//        {

//        }

//        public CreateGroupModel(CompanyProfileDto companyProfile, IEnumerable<SelectListItem> groupTypes) 
//            : this(companyProfile)
//        {
//            GroupTypes = groupTypes;
//        }

//        public string Name { get; set; }

//        [Required, DisplayName("Trust type")]
//        public int? TypeId { get; set; }
//        public DateTime? OpenDate { get; set; }
//        public string CompaniesHouseNumber { get; set; }
//        public string Address { get; set; }
//        public bool TrustExists { get; internal set; }
//        public IEnumerable<SelectListItem> GroupTypes { get; set; }
//    }
//}