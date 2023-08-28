using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopNews.Core.DTOS.Post;
using TopNews.Core.Interfaces;

namespace TopNews.WebUI.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IPostService _postService;

        public PostController(ICategoryService categoryService, IPostService postService)
        {
            _categoryService = categoryService;
            _postService = postService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _postService.GetAll();
            return View(result);
        }
        public async Task<IActionResult> AddPost()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPost(PostDTO model)
        {
            if (model != null) 
            {
                await _postService.CreateAsync(model);
            }
            return View();
        }
    }
}
