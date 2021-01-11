using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Newtonsoft.Json;

namespace Edubase.Services.Establishments.EditPolicies
{
    public class ApprovalPolicy
    {
        public bool RequiresApproval { get; set; }
        public string ApproverName { get; set; }
    }

    public class EstablishmentApprovalsPolicy : EstablishmentFieldListBase<ApprovalPolicy>
    {
        [JsonProperty("iebtDetail")]
        public IEBTFieldList<ApprovalPolicy> IEBTDetail { get; set; }
        
        public string[] GetFieldsRequiringApproval()
        {
            var map = GetApprovalPolicies();
            return map.Where(x => (x.Value?.RequiresApproval).GetValueOrDefault()).Select(x => x.Key).ToArray();
        }

        public string GetApproverName(string fieldName)
        {
            var map = GetApprovalPolicies();
            return map.ContainsKey(fieldName) ? map[fieldName]?.ApproverName : null;
        }

        private Dictionary<string, ApprovalPolicy> GetApprovalPolicies()
        {
            var establishmentMap = GetType().GetProperties().Where(x => x.PropertyType == typeof(ApprovalPolicy))
                            .ToDictionary(x => x.Name, x => (ApprovalPolicy) x.GetValue(this, null));

            var iebtDetailMap = IEBTDetail.GetType().GetProperties().Where(x => x.PropertyType == typeof(ApprovalPolicy))
                .ToDictionary(x => $"{nameof(IEBTModel)}.{x.Name}", x => (ApprovalPolicy) x.GetValue(IEBTDetail, null));

            var iebtProprietorsMap = IEBTDetail.Proprietors.GetType().GetProperties().Where(x => x.PropertyType == typeof(ApprovalPolicy))
                .ToDictionary(x => $"{nameof(IEBTModel)}.{nameof(IEBTModel.Proprietors)}.{x.Name}", x => (ApprovalPolicy) x.GetValue(IEBTDetail.Proprietors, null));

            var iebtChairOfProprietorsMap = IEBTDetail.ChairOfProprietor.GetType().GetProperties().Where(x => x.PropertyType == typeof(ApprovalPolicy))
                .ToDictionary(x => $"{nameof(IEBTModel)}.{nameof(IEBTModel.ChairOfProprietor)}.{x.Name}", x => (ApprovalPolicy) x.GetValue(IEBTDetail.ChairOfProprietor, null));

            var iebtMap = iebtDetailMap
                .Concat(iebtProprietorsMap).ToDictionary(x => x.Key, x => x.Value)
                .Concat(iebtChairOfProprietorsMap).ToDictionary(x => x.Key, x => x.Value);

            var map = establishmentMap.Concat(iebtMap).ToDictionary(x => x.Key, x => x.Value);

            return map;
        }
    }

    /// <summary>
    /// Describes which fields can be edited and which user group is responsible for approving changes to a field.
    /// </summary>
    public class EstablishmentEditPolicyEnvelope
    {
        public EstablishmentDisplayEditPolicy EditPolicy { get; set; }
        public EstablishmentApprovalsPolicy ApprovalsPolicy { get; set; }

        public EstablishmentEditPolicyEnvelope Initialise(EstablishmentModel establishment)
        {
            EditPolicy.Initialise(establishment);
            return this;
        }
    }
}
