using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Notifications
{
    public enum eNotificationsTemplateAction
    {
        Message,
        Review
    }

    public class NotificationsTemplateViewModel
    {
        public string Id { get; set; }
        public eNotificationsTemplateAction Action { get; set; }

        [Required, MaxLength(400, ErrorMessage = "The Content field cannot have more than 400 characters")]
        public string Content { get; set; }

        public string OriginalContent { get; set; }

        public bool GoBack { get; set; }
    }
}
