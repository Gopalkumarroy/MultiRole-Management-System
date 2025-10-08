using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sewa360.DBContext;
using Sewa360.Models;

namespace Sewa360.Controllers
{
    public class BranchController : Controller
    {
        private readonly BranchDb _BranchDb;
        private readonly CompanyDb _companyDb;


        //public BranchController(BranchDb Branchdb, CompanyDb companyDb)
        //{
        //    _BranchDb = Branchdb;
        //    _companyDb = companyDb;
        //}
        public BranchController(IConfiguration configuration)
        {
            _BranchDb = new BranchDb(configuration);
            _companyDb = new CompanyDb(configuration);
        }

        [HttpGet]
        public IActionResult CreateBranch()
        {
            ViewBag.CompanyList = new SelectList(_companyDb.GetCompanyList(null), "Company_Id", "Company_Name");

            return View();
        }


        [HttpPost]
        public IActionResult CreateBranch(Branch model)
        {
            if (ModelState.IsValid)
            {
                _BranchDb.InsertBranch(model);
                ModelState.Clear();
                return RedirectToAction("CreateBranch");
            }
            ViewBag.CompanyList = new SelectList(_companyDb.GetCompanyList(null), "Company_Id", "Company_Name");
            return View(model);
        }


        // GET: Company List
        public IActionResult BranchListPage(int? id)
        {
            var branchList = _BranchDb.GetBranchList(id);
            return View(branchList);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                _BranchDb.DeleteBranch(id);
                TempData["SuccessMessage"] = "Branch deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "Error deleting branch: " + ex.Message;
            }

            return RedirectToAction("BranchListPage");
        }

        // GET: Edit Branch
        [HttpGet]
        public IActionResult EditBranch(int id)
        {
            var branch = _BranchDb.GetBranchList(id).FirstOrDefault();
            if (branch == null)
            {
                return NotFound(); // Optional: return 404 if not found
            }

            // Populate Company dropdown with selected value
            var companies = _companyDb.GetCompanyList(null);
            ViewBag.CompanyList = new SelectList(companies, "Company_Id", "Company_Name", branch.Company_Id);

            return View(branch);
        }

        // POST: Edit Branch

        [HttpPost]
        public IActionResult EditBranch(Branch model)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _BranchDb.UpdateBranch(model);

                TempData["SuccessMessage"] = "Branch successfully updated.";
                return RedirectToAction("BranchListPage");
            }
            var companies = _companyDb.GetCompanyList(null);
            ViewBag.CompanyList = new SelectList(companies, "Company_Id", "Company_Name", model.Company_Id);

            return View(model);
        }

        [HttpGet]
        public IActionResult BranchDetails(int id)
        {
            var branch = _BranchDb.GetBranchList(id).FirstOrDefault();

            if (branch == null)
            {
                return NotFound(); // or redirect to an error page
            }

            var company = _companyDb.GetCompanyList(branch.Company_Id).FirstOrDefault();
            ViewBag.CompanyName = company?.Company_Name;

            return View(branch);
        }


    }
}
