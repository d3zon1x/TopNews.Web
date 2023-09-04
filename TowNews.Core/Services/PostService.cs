using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepository<Post> _postRepo;


        public PostService(IRepository<Post> postRepo, IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _postRepo = postRepo;
            _configuration= configuration;
            _webHostEnvironment = webHostEnvironment;
        }       

        public async Task<List<PostDTO>> GetByCategory(int id)
        {
            var result = await _postRepo.GetListBySpec(new Posts.ByCategory(id));
            return _mapper.Map<List<PostDTO>>(result);
        }
        public async Task CreateAsync(PostDTO model)
        {
            if(model.File.Count > 0)
            {
                string webHostPath = _webHostEnvironment.WebRootPath;
                string upload = webHostPath + _configuration.GetValue<string>("ImageSettings:ImagePath");
                var files = model.File;
                var fileName = Guid.NewGuid().ToString();
                string extensions = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extensions), FileMode.Create ))
                {
                    files[0].CopyTo(fileStream);
                }
                    model.ImagePath = fileName + extensions;        
            }
            else
            {
                model.ImagePath = "Default.png";
            }

            DateTime currentDate = DateTime.Today;
            string formatedDate = currentDate.ToString("d");
            model.PublishDate = formatedDate;

            await _postRepo.Insert(_mapper.Map<Post>(model));
            await _postRepo.Save();
        }
        

        public async Task<PostDTO?> Get(int id)
        {
            if (id < 0) return null; // exception handling
            var post = await _postRepo.GetByID(id);
            if (post == null) return null; // exception handling
            return _mapper.Map<PostDTO>(post);
        }
        public async Task Delete(int id)
        {
            var currentPost = await Get(id);

            if (currentPost == null) return; // exception

            string webPathRoot = _webHostEnvironment.WebRootPath;
            string upload = webPathRoot + _configuration.GetValue<string>("ImageSettings:ImagePath");

            string existingFilePath = Path.Combine(upload, currentPost.ImagePath);

            if (File.Exists(existingFilePath) && currentPost.ImagePath != "Default.png")
            {
                File.Delete(existingFilePath);
            }

            await _postRepo.Delete(id);
            await _postRepo.Save();
        }

        public async Task<List<PostDTO>> GetAll()
        {
            var result = await _postRepo.GetListBySpec(new Posts.All());//.ToList();
            return _mapper.Map<List<PostDTO>>(result);
        }

        public async Task<PostDTO> GetById(int id)
        {
            if (id < 0) return null; // exception handling

            var post = await _postRepo.GetByID(id);

            if (post == null) return null; // exception handling

            return _mapper.Map<PostDTO>(post);
        }

        public async Task<List<PostDTO>> Search(string searchString)
        {
            var result = await _postRepo.GetListBySpec(new Posts.Search(searchString));
            return _mapper.Map<List<PostDTO>>(result);
        }

        public async Task Update(PostDTO model)
        {
            var currentPost = await _postRepo.GetByID(model.Id);
            if (model.File.Count > 0)
            {
                string webPathRoot = _webHostEnvironment.WebRootPath;
                string upload = webPathRoot + _configuration.GetValue<string>("ImageSettings:ImagePath");

                string existingFilePath = Path.Combine(upload, currentPost.ImagePath);

                if (File.Exists(existingFilePath) && model.ImagePath != "Default.png")
                {
                    File.Delete(existingFilePath);
                }

                var files = model.File;

                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                model.ImagePath = fileName + extension;

            }
            else
            {
                model.ImagePath = currentPost.ImagePath;
            }
            await _postRepo.Update(_mapper.Map<Post>(model));
            await _postRepo.Save();
        }
    }
}
