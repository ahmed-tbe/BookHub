using BookHub.DataAccess.Data;
using BookHub.DataAccess.Repository.IRepository;
using BookHub.Models;
using BookHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unit, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unit;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };
            if (id == null || id==0) //functionality for create
            {
                return View(productVM);
            }
            else //its an update
            {
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
                return View(productVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM dummyObject, IFormFile? file)
        {
            //below this does the same job as client side validation
            if (ModelState.IsValid) //checks if the category object we have is valid, goes and examines validation
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(dummyObject.Product.ImageUrl)) // then we're uploading new img
                    {
                        //delete old image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, dummyObject.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    dummyObject.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (dummyObject.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(dummyObject.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(dummyObject.Product);
                }
                _unitOfWork.Save(); //updates the actual DB and table/s
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index"); //since we need to reload to display the new category in Create.cshtml
            }
            else
            {
                dummyObject.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(dummyObject);
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product productFromDB = _unitOfWork.Product.Get(u => u.Id == id);
            if (productFromDB == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(productFromDB);
            _unitOfWork.Save(); //updates the actual DB and table/s
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productDelete = _unitOfWork.Product.Get(u=>u.Id == id);
            if (productDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //removing image too before deleting
            var oldImagePath =
                Path.Combine(_webHostEnvironment.WebRootPath, productDelete.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion 
    }
}
