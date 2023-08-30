﻿using AutoMapper;
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
    }
}
