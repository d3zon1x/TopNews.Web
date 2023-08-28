using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.Category;
using TopNews.Core.DTOS.Post;
using TopNews.Core.Entities.Site;
using TopNews.Core.Entities.Specification;
using TopNews.Core.Interfaces;

namespace TopNews.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Post> _postRepo;

        public PostService(IRepository<Post> postRepo, IMapper mapper)
        {
            _mapper = mapper;
            _postRepo = postRepo;
        }
        public async Task<PostDTO?> Get(int id)
        {
            if (id < 0) return null; 

            var post = await _postRepo.GetByID(id);

            if (post == null) return null; 

            return _mapper.Map<PostDTO>(post);
        }
        public async Task<List<PostDTO>> GetAll()
        {
            var result = await _postRepo.GetAll();
            return _mapper.Map<List<PostDTO>>(result);
        }

        public async Task<List<PostDTO>> GetByCategory(int id)
        {
            var result = await _postRepo.GetListBySpec(new Posts.ByCategory(id));
            return _mapper.Map<List<PostDTO>>(result);
        }
        public async Task CreateAsync(PostDTO model)
        {
            var list = await GetAll();
            foreach (var item in list)
            {
                if (item.Title.ToLower() == model.Title.ToLower())
                {
                    return;
                }
            }

            await _postRepo.Insert(_mapper.Map<Post>(model));
            await _postRepo.Save();
        }
    }
}
