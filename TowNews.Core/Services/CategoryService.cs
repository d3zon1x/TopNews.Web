﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.Category;
using TopNews.Core.Entities.Site;
using TopNews.Core.Interfaces;

namespace TopNews.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category> _categoryRepo;

        public CategoryService(IMapper mapper, IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<List<CategoryDTO>> GetAll()
        {
            var result = await _categoryRepo.GetAll();
            return _mapper.Map<List<CategoryDTO>>(result);
        }
        public async Task Create(CategoryDTO model)
        {
            var list = await GetAll();
            foreach (var item in list)
            {
                if (item.Name.ToLower() == model.Name.ToLower())
                {
                    return;
                }
            }
            await _categoryRepo.Insert(_mapper.Map<Category>(model));
            await _categoryRepo.Save();
        }

        public async Task Delete(int id)
        {
            var model = await Get(id);
            if (model == null) return;

            await _categoryRepo.Delete(id);
            await _categoryRepo.Save();
        }

        public async Task<CategoryDTO?> Get(int id)
        {
            if (id < 0) return null;
            var category = await _categoryRepo.GetByID(id);
            if (category == null) return null;
            return _mapper.Map<CategoryDTO?>(category);
        }
        public async Task Update(CategoryDTO model)
        {
            var list = await GetAll();
            foreach (var item in list)
            {
                if (item.Name.ToLower() == model.Name.ToLower())
                {
                    return;
                }
            }
            await _categoryRepo.Update(_mapper.Map<Category>(model));
            await _categoryRepo.Save();
        }

    }
}
