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
