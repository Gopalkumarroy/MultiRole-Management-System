using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sewa360.DBContext;
using Sewa360.Filters;
using Sewa360.Models;

namespace Sewa360.Controllers
{
   
    public class CompanyController : Controller
    {
        private readonly CompanyDb _companyDb;
        public CompanyController(CompanyDb companyDb) 
        {
            _companyDb = companyDb;
        }

        [AdminAuthorization]
        [HttpGet]
        public IActionResult CreateCompany()
        {
            ViewBag.Countries = GetCountries();
            return View();
        }

        [HttpGet]
        public JsonResult GetStatesByCountry(string country)
        {
            var states = GetStates(country);
            return Json(states);
        }

        private List<SelectListItem> GetCountries()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Text = "India", Value = "India" },
            //new SelectListItem { Text = "USA", Value = "USA" },
            //new SelectListItem { Text = "Canada", Value = "Canada" },
            //new SelectListItem { Text = "Australia", Value = "Australia" }
        };
        }

        private List<SelectListItem> GetStates(string country)
        {
            var states = new List<SelectListItem>();
            switch (country)
            {
                case "India":
                    states.Add(new SelectListItem { Text = "Andhra Pradesh", Value = "Andhra Pradesh" });
                    states.Add(new SelectListItem { Text = "Arunachal Pradesh", Value = "Arunachal Pradesh" });
                    states.Add(new SelectListItem { Text = "Assam", Value = "Assam" });
                    states.Add(new SelectListItem { Text = "Bihar", Value = "Bihar" });
                    states.Add(new SelectListItem { Text = "Chhattisgarh", Value = "Chhattisgarh" });
                    states.Add(new SelectListItem { Text = "Goa", Value = "Goa" });
                    states.Add(new SelectListItem { Text = "Gujarat", Value = "Gujarat" });
                    states.Add(new SelectListItem { Text = "Haryana", Value = "Haryana" });
                    states.Add(new SelectListItem { Text = "Himachal Pradesh", Value = "Himachal Pradesh" });
                    states.Add(new SelectListItem { Text = "Jharkhand", Value = "Jharkhand" });
                    states.Add(new SelectListItem { Text = "Karnataka", Value = "Karnataka" });
                    states.Add(new SelectListItem { Text = "Kerala", Value = "Kerala" });
                    states.Add(new SelectListItem { Text = "Madhya Pradesh", Value = "Madhya Pradesh" });
                    states.Add(new SelectListItem { Text = "Maharashtra", Value = "Maharashtra" });
                    states.Add(new SelectListItem { Text = "Manipur", Value = "Manipur" });
                    states.Add(new SelectListItem { Text = "Meghalaya", Value = "Meghalaya" });
                    states.Add(new SelectListItem { Text = "Mizoram", Value = "Mizoram" });
                    states.Add(new SelectListItem { Text = "Nagaland", Value = "Nagaland" });
                    states.Add(new SelectListItem { Text = "Odisha", Value = "Odisha" });
                    states.Add(new SelectListItem { Text = "Punjab", Value = "Punjab" });
                    states.Add(new SelectListItem { Text = "Rajasthan", Value = "Rajasthan" });
                    states.Add(new SelectListItem { Text = "Sikkim", Value = "Sikkim" });
                    states.Add(new SelectListItem { Text = "Tamil Nadu", Value = "Tamil Nadu" });
                    states.Add(new SelectListItem { Text = "Telangana", Value = "Telangana" });
                    states.Add(new SelectListItem { Text = "Tripura", Value = "Tripura" });
                    states.Add(new SelectListItem { Text = "Uttar Pradesh", Value = "Uttar Pradesh" });
                    states.Add(new SelectListItem { Text = "Uttarakhand", Value = "Uttarakhand" });
                    states.Add(new SelectListItem { Text = "West Bengal", Value = "West Bengal" });
                    break;

                //case "USA":
                //    states.Add(new SelectListItem { Text = "California", Value = "California" });
                //    states.Add(new SelectListItem { Text = "Texas", Value = "Texas" });
                //    states.Add(new SelectListItem { Text = "Florida", Value = "Florida" });
                //    break;
                //case "Canada":
                //    states.Add(new SelectListItem { Text = "Ontario", Value = "Ontario" });
                //    states.Add(new SelectListItem { Text = "Quebec", Value = "Quebec" });
                //    break;
                //case "Australia":
                //    states.Add(new SelectListItem { Text = "New South Wales", Value = "New South Wales" });
                //    states.Add(new SelectListItem { Text = "Victoria", Value = "Victoria" });
                //    break;
            }
            return states;
        }

        [HttpPost]
        public IActionResult CreateCompany(Company model)
        {
            if (ModelState.IsValid)
            {
                _companyDb.InsertCompany(model);
                ModelState.Clear();
                return RedirectToAction("CompanyListPage");
            }
            return View(model);
        }

        // GET: Company List
        //[AdminAuthorization]
        public IActionResult CompanyListPage(int? id)
        {
            var companyList = _companyDb.GetCompanyList(id);
            return View(companyList);
        }


        // GET: Edit Company
        public IActionResult CompanyEdit(int id)
        {
            var model = _companyDb.EditCompanyById(id);

            ViewBag.Countries = GetCountries();
            ViewBag.States = GetStates(model?.Company_Country);
            return View(model);
        }

        [HttpPost]
        public IActionResult CompanyEdit(Company model)
        {
            if (ModelState.IsValid)
            {
                _companyDb.UpdateCompany(model);
                TempData["SuccessMessage"] = "Company updated successfully";
                return RedirectToAction("CompanyListPage");
            }

            ViewBag.Countries = GetCountries();
            ViewBag.States = GetStates(model.Company_Country);
            return View(model);
        }

        // GET: Delete Company
        public IActionResult Delete(int id)
        {
            _companyDb.DeleteCompany(id, "Admin");
            TempData["SuccessMessage"] = "Company Deleted Successfully";
            return RedirectToAction("CompanyListPage");
        }

        public IActionResult CompanyDetails(int id)
        {
            var model = _companyDb.EditCompanyById(id);
            if (model == null)
            {
                return HttpNotFound(); // Optional: custom 404 view
            }
            return View(model);
        }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }
    }
}
