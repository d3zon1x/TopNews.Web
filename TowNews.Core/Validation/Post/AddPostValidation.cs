using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.Post;

namespace TopNews.Core.Validation.Post
{
    public class AddPostValidation : AbstractValidator<PostDTO>
    {
        public AddPostValidation() 
        {
            RuleFor(r => r.Title).NotEmpty();
            RuleFor(r => r.FullText).NotEmpty();
            RuleFor(r => r.Description).NotEmpty();
            RuleFor(r => r.CategoryId).NotEmpty();
        }
    }
}
