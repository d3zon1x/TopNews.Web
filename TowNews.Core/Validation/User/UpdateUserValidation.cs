using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.User;

namespace TopNews.Core.Validation.User
{
    public class UpdateUserValidation : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserValidation()
        {
            RuleFor(r => r.FirstName).NotEmpty().MinimumLength(2);
            RuleFor(r => r.LastName).NotEmpty().MinimumLength(2);
            RuleFor(r => r.Email).NotEmpty().WithMessage("Field can`t be empty!").EmailAddress().WithMessage("Invalid email form!");
            RuleFor(r => r.PhoneNumber).NotEmpty().MinimumLength(11).MinimumLength(13);
        }
    }
}
