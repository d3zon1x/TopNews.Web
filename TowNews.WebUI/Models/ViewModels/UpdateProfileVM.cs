using TopNews.Core.DTOS.User;

namespace TopNews.WebUI.Models.ViewModels
{
    public class UpdateProfileVM
    {
        public UpdateUserDTO UserInfo { get; set; }
        public UpdatePasswordDTO UserPassword { get; set; }
    }
}
