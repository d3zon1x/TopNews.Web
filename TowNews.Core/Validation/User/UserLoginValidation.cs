using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.User;

namespace TopNews.Core.Validation.User
{
    public class UserLoginValidation : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidation()
        {
            RuleFor(r => r.email).NotEmpty().WithMessage("Field can`t be empty!").EmailAddress().WithMessage("Invalid email form!");
            RuleFor(r => r.password).NotEmpty().WithMessage("Field can`t be empty").MinimumLength(6).MaximumLength(128);
        }
    }   
}