using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace TrackMe.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _db;
        public CategoryController(ApplicationDBContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Category Name.");
            //    //Custom Validation
            //}
            //if (obj.Name != null && obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("", "Test is an invalid Value.");
            //    //Custom Validation , To Show Validsummary Types
            //}
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Created SuccessFully";       //To Show Notification
                return RedirectToAction("Index");
            }
            return View();
        }

        //----------------------------------------------- Edit Section
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
           
            Category? categoryFromDb = _db.Categories.Find(id); //Category obj , Find() is a method  // one way of Retrieving the Category
           // Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id==id); //it ll use link operation , // 2nd Way - Recommended
          //  Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault(); //3rd Way 
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb); // if data found we will pass it to our view
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
          //when we are updating , we will do the client side validation
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Updated SuccessFully";
                return RedirectToAction("Index");
            }
            return View();
        }


        //----------------------------------------------- Delete Section
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDb = _db.Categories.Find(id); 
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb); // if data found we will pass it to our view
        }

        [HttpPost, ActionName("Delete")]   //explicitly say that this endpoints action name is Delete
        public IActionResult DeletePOST(int? id)  //Do not Use Get & Post Method name Same when parameters are same
        {
            //1st we have to find that category from data base
            Category? obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);   //remove method, which expects the category obj .
            _db.SaveChanges();
            TempData["success"] = "Category Deleted SuccessFully";
            return RedirectToAction("Index");
          
        }
    }
}
