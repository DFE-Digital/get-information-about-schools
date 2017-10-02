using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class UserRolesViewModel
    {
        public bool UserRequiresDataPrompt { get; set; }

        public UserRolesViewModel(bool userRequiresDataPrompt)
        {
            UserRequiresDataPrompt = userRequiresDataPrompt;
        }
    }
}