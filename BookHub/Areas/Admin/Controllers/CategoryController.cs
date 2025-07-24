using BookHub.DataAccess.Data;
using BookHub.DataAccess.Repository.IRepository;
using BookHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unit)
        {
            _unitOfWork = unit;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category dummyObject)
        {
            if (ModelState.IsValid && dummyObject.Name.ToLower() == dummyObject.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name & display order can't be the same");
            }

            if (dummyObject.Name != null && dummyObject.Name.ToLower() == "x")
            {
                ModelState.AddModelError("Name", "This name is invalid");
            }

            //below this does the same job as client side validation
            if (ModelState.IsValid) //checks if the category object we have is valid, goes and examines validation
            {
                _unitOfWork.Category.Add(dummyObject); //db object stores all categories, adds this new one
                _unitOfWork.Save(); //updates the actual DB and table/s
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index"); //since we need to reload to display the new category in Create.cshtml
            }
            //if not valid then we stay and display error messages
            return View();
        }

        public IActionResult Edit(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDB == null)
            {
                return NotFound();
            }

            return View(categoryFromDB);
        }

        [HttpPost]
        public IActionResult Edit(Category dummyObject)
        {
            //below this does the same job as client side validation
            if (ModelState.IsValid) //checks if the category object we have is valid, goes and examines validation
            {
                _unitOfWork.Category.Update(dummyObject); //db object stores all categories, adds this new one
                _unitOfWork.Save(); //updates the actual DB and table/s
                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index"); //since we need to reload to display the new category in Create.cshtml
            }
            //if not valid then we stay and display error messages
            return View();
        }

        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDB == null)
            {
                return NotFound();
            }

            return View(categoryFromDB);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category categoryFromDB = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDB == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(categoryFromDB);
            _unitOfWork.Save(); //updates the actual DB and table/s
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }

    }
}
