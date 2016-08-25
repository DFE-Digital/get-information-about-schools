using System.Security.Claims;
using System.Security.Principal;

namespace Web.UI.Models
{
    public class BaseViewModel
    {
        private IPrincipal _userPrincipal;

        public BaseViewModel(IPrincipal userPrincipal)
        {
            _userPrincipal = userPrincipal;
        }
    }
}