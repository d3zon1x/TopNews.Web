using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopNews.Core.DTOS.Category;
using TopNews.Core.DTOS.Post;
using TopNews.Core.Interfaces;
using TopNews.Core.Validation.Post;

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
            await LoadCategories();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPost(PostDTO model)
        {
            var validator = new AddPostValidation();
            var validationResult = await validator.ValidateAsync(model);

            if (validationResult.IsValid) 
            {
                var files = HttpContext.Request.Form.Files;
                model.File = files;
                await _postService.CreateAsync(model);
                return RedirectToAction("Index", "Post");
            }
            ViewBag.Errors = validationResult.Errors[0];
            return View();
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
