using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrackMe.Areas.Admin.Controllers
{
    [Area("Admin")]          //Attribute
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult CompaniesList()
        {
            List<Company> objCompanies = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanies);
        }

        public IActionResult Upsert_Company(int? id)
        {
            if (id == 0 || id == null)
            {
                return View( new Company());
            }
            else
            {
                var Company = _unitOfWork.Company.Get(u => u.id == id);
                if (Company == null)
                {
                    return NotFound();
                }
                return View(Company);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert_Company(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company Created SuccessFully";
                }
                else
                {
                    _unitOfWork.Company.update(obj);
                    TempData["success"] = "Company Updated SuccessFully";
                }
                _unitOfWork.Save();
                return RedirectToAction("CompaniesList");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if(id== 0 )
            {
                return NotFound();
            }

            var CompanyId =  _unitOfWork.Company.Get(u => u.id == id);
            if (CompanyId == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Remove(CompanyId);
            _unitOfWork.Save();
            TempData["success"] = "Company Deleted SuccessFully";
            return RedirectToAction("CompaniesList");
        }
    }
}
