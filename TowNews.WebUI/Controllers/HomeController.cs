using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using TopNews.Core.DTOS.Category;
using TopNews.Core.DTOS.Post;
using TopNews.Core.Entities.Site;
using TopNews.Core.Entities.Specification;
using TopNews.Core.Interfaces;
using TopNews.Core.Services;
using TowNews.WebUI.Models;
using X.PagedList;

namespace TowNews.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IPostService _postService;
        public HomeController(ICategoryService categoryService, IPostService postService)
        {
            _categoryService = categoryService;
            _postService = postService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            List<PostDTO> posts = await _postService.GetAll();
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([FromForm] string searchString)
        {
            List<PostDTO> posts = await _postService.Search(searchString);
            int pageSize = 20;
            int pageNumber = 1;
            return View("Index", posts.ToPagedList(pageNumber, pageSize));
        }

        [Route("Error/{statusCode}")]
        public  IActionResult Error(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("NotFound");
                default:
                    return View("Error");
            }
        }
        public async Task<IActionResult> ViewMore(int? id)
        {
            PostDTO? post = await _postService.Get(id ?? 0);
            if (post == null)
            {
                return RedirectToAction(nameof(Index));
            }
            post.CategoryName = (await _categoryService.Get(post.CategoryId)).Name;
            return View(post);
        }
        private async Task LoadCategories()
        {
            ViewBag.CategoryList = new SelectList(
                await _categoryService.GetAll(),
                nameof(CategoryDTO.Id),
                nameof(CategoryDTO.Name)
                );
        }
    }
}