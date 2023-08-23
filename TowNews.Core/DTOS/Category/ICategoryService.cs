using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopNews.Core.DTOS.Category
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAll();
    }
}
