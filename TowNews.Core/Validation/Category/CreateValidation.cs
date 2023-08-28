using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.Category;

namespace TopNews.Core.Validation.Category
{
    public class CreateValidation : AbstractValidator<CategoryDTO>
    {
        public CreateValidation()
        {
            RuleFor(r => r.Name).NotEmpty();
        }
    }
}
