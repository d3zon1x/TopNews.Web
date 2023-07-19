using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.User;

namespace TopNews.Core.Validation.User
{
    public class UpdatePasswordVilidation : AbstractValidator<UpdatePasswordDTO>
    {
        public UpdatePasswordVilidation()
        {
            RuleFor(r => r.OldPassword).NotEmpty().WithMessage("Field must not be empty.").MinimumLength(6) ;
            RuleFor(r => r.NewPassword).NotEmpty().WithMessage("Field must not be empty.").MinimumLength(6);
            RuleFor(r => r.ConfirmPassword).NotEmpty().WithMessage("Field must not be empty.").MinimumLength(6).Equal(r => r.NewPassword);
        }

    }
}
