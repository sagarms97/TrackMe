using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TrackMe.Areas.Admin.Controllers
{
    [Area("Admin")]          //Attribute
    public class ProductController : Controller
    {
        //  private readonly ApplicationDBContext _db;    //we dont need this becz we use interface for Product 
        // private readonly IProductRepository _ProductRepo;          //we dont need this becz we use UnityOfWork interface for Product 
        private readonly IUnitOfWork _unitofwork;
        public ProductController(IUnitOfWork unitofwork)
        {
            // _db=db;
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            //  List<Product> objProductList = _db.Categories.ToList();
            //  List<Product> objProductList = _ProductRepo.GetAll().ToList();
            List<Product> objProductList = _unitofwork.Product.GetAll().ToList();
            return View(objProductList);
        }

        //public IActionResult Create()
        //{

        //    //IEnumerable<SelectListItem> CategoryList = _unitofwork.Category
        //    //    .GetAll().Select(u => new SelectListItem
        //    //    {
        //    //        Text = u.Name,
        //    //        Value = u.Id.ToString()
        //    //    });                //will store Category List  //fectch data from DB Use Projections EF core

        //    // ViewBag.CategoryList = CategoryList;         //one way to select data from another table

        //    //ViewData["CategoryList"] = CategoryList;         //another way 

        //    ProductVM productVM = new()
        //    {
        //        CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        }),
        //        Product = new Product()
        //    };
        //    return View(productVM);
        //}


        //[HttpPost]
        ////public IActionResult Create(Product obj)
        //public IActionResult Create(ProductVM obj)
        //{
        //    //if (obj.Name == obj.DisplayOrder.ToString())
        //    //{
        //    //    ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Product Name.");
        //    //    //Custom Validation
        //    //}
        //    //if (obj.Name != null && obj.Name.ToLower() == "test")
        //    //{
        //    //    ModelState.AddModelError("", "Test is an invalid Value.");
        //    //    //Custom Validation , To Show Validsummary Types
        //    //}
        //    if (ModelState.IsValid)
        //    {
        //        //_db.Categories.Add(obj);
        //        //_db.SaveChanges();                                          // once we created repository we don't need it

        //        //_unitofwork.Product.Add(obj);
        //        _unitofwork.Product.Add(obj.Product);
        //        _unitofwork.Save();
        //        TempData["success"] = "Product Created SuccessFully";       //To Show Notification
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {

        //        obj.CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        });

        //        return View(obj);
        //    }
        //}






        public IActionResult Upsert(int? id)             //We are combining Create and Edit page , So we don't need edit page 
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if(id== null || id==0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product=_unitofwork.Product.Get(u=>u.Id==id);
                return View(productVM);
            }
           
        }




        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.Product.Add(obj.Product);
                _unitofwork.Save();
                TempData["success"] = "Product Created SuccessFully";       //To Show Notification
                return RedirectToAction("Index");
            }
            else
            {

                obj.CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                return View(obj);
            }
        }






        //----------------------------------------------- Edit Section--------------------

        //We are combining Create and Edit page , So we don't need edit page 
       // public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? productFromDb = _unitofwork.Product.Get(u => u.Id == id);
        //    //   Product? ProductFromDb = _db.Categories.Find(id); //Product obj , Find() is a method  // one way of Retrieving the Product
        //    // Product? ProductFromDb1 = _db.Categories.FirstOrDefault(u => u.Id==id); //it ll use link operation , // 2nd Way - Recommended
        //    //  Product? ProductFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault(); //3rd Way 
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb); // if data found we will pass it to our view
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    //when we are updating , we will do the client side validation
        //    if (ModelState.IsValid)
        //    {
        //        //_db.Categories.Update(obj);
        //        //_db.SaveChanges();

        //        _unitofwork.Product.update(obj);
        //        _unitofwork.Save();
        //        TempData["success"] = "Product Updated SuccessFully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}


        //----------------------------------------------- Delete Section
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //  Product? ProductFromDb = _db.Categories.Find(id); 

            Product? productFromDb = _unitofwork.Product.Get(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb); // if data found we will pass it to our view
        }

        [HttpPost, ActionName("Delete")]   //explicitly say that this endpoints action name is Delete
        public IActionResult DeletePOST(int? id)  //Do not Use Get & Post Method name Same when parameters are same
        {
            //1st we have to find that Product from data base
            // Product? obj = _db.Categories.Find(id);

            Product? obj = _unitofwork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitofwork.Product.Remove(obj);   //remove method, which expects the Product obj .
            _unitofwork.Save();
            TempData["success"] = "Product Deleted SuccessFully";
            return RedirectToAction("Index");

        }
    }
}
