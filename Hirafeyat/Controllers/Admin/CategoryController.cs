using System.Threading.Tasks;
using Hirafeyat.AdminServices;
using Hirafeyat.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers.Admin
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;
        private readonly HirafeyatContext _context;
        public CategoryController(ICategoryService service, HirafeyatContext context)
        {
            _service = service;
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _service.GetAll();
            return View(categories);
        }
        public IActionResult Details(int id)
        {
            var category = _service.GetByIdAsync(id);
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

        public IActionResult Edit(int id)
        {
            var category = _service.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = _service.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
