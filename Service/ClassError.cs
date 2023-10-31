using Microsoft.AspNetCore.Identity;

namespace Project.Service
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $""
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"Email '{email}' đã tồn tại. \n Nếu vẫn không đăng nhập được hãy gửi xác thức lại email ở phần đăng nhập "
            };
        }
    }
}
