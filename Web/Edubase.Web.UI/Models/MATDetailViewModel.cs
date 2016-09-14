using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class MATDetailViewModel
    {
        public MAT Data { get; set; }
        public dynamic Schools { get; set; }

        public MATDetailViewModel(MAT model, dynamic schools)
        {
            Data = model;
            Schools = schools;
        }
    }
}