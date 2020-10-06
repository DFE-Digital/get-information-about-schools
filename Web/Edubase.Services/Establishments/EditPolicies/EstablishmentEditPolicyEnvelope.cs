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
            // temporary fix while investigate 43808
            if (fieldName.StartsWith("IEBTModel.Proprietors", StringComparison.OrdinalIgnoreCase) ||
                fieldName.StartsWith("IEBTModel.ChairOfProprietors", StringComparison.OrdinalIgnoreCase))
            {
                fieldName = fieldName.Substring(0, fieldName.LastIndexOf(".", StringComparison.Ordinal));
            }

            var map = GetApprovalPolicies();
            return map.ContainsKey(fieldName) ? map[fieldName]?.ApproverName : null;
        }

        private Dictionary<string, ApprovalPolicy> GetApprovalPolicies()
        {
            var map1 = GetType().GetProperties().Where(x => x.PropertyType == typeof(ApprovalPolicy))
                            .ToDictionary(x => x.Name, x => (ApprovalPolicy) x.GetValue(this, null));

            var map2 = IEBTDetail.GetType().GetProperties().Where(x => x.PropertyType == typeof(ApprovalPolicy))
                .ToDictionary(x => $"{nameof(IEBTModel)}.{x.Name}", x => (ApprovalPolicy) x.GetValue(IEBTDetail, null));

            var map = map1.Concat(map2).ToDictionary(x => x.Key, x => x.Value);

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
