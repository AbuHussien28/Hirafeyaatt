using System.Threading.Tasks;
using Hirafeyat.AdminServices;
using Hirafeyat.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers.Admin
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;
       

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var categories = _service.GetAll();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _service.Create(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Category/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            // Proceed to delete the category
            await _service.DeleteAsync(id);

            // After deletion, redirect back to the index page
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Search(string query)
        {
            var categories = _service.GetByNameAsync(query);

            return PartialView("_CategoryTablePartial", categories);
        }

    }
}
