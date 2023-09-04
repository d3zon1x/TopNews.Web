using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.Post;

namespace TopNews.Core.Interfaces
{
    public interface IPostService
    {
        Task<PostDTO?> Get(int id);
        Task<List<PostDTO>> GetByCategory(int id);
        Task<List<PostDTO>> GetAll();
        Task CreateAsync(PostDTO model);
        Task Update(PostDTO model);
        Task Delete(int id);
        Task<PostDTO> GetById(int id);
        Task<List<PostDTO>> Search(string searchString);
    }
}
