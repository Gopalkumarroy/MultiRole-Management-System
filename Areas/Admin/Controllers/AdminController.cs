using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sewa360.Areas.Admin.Models;
using Sewa360.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sewa360.Filters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Policy;
using Microsoft.DotNet.Scaffolding.Shared;



namespace Sewa360.Areas.Admin.Controllers
{


    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AdminDb _adminDb;
        private readonly ManagerDb _managerDb;
        private readonly PartnerDb _partnerDb;


        private readonly ServiceProviderDb _serviceProviderDb;
        private readonly CompanyDb _companyDb;
        private readonly BranchDb _branchDb;


        public AdminController(PartnerDb partnerDb, ManagerDb managerDb, CompanyDb companyDb, BranchDb branchDb, AdminDb adminDb, ServiceProviderDb serviceProviderDb)
        {
            _adminDb = adminDb;
            _partnerDb = partnerDb;
            _managerDb = managerDb;
            _companyDb = companyDb;
            _branchDb = branchDb;
            _serviceProviderDb = serviceProviderDb;

        }

        public IActionResult ServiceList()
        {
            var services = new List<ServiceViewModel>
    {
        new ServiceViewModel
        {
            Id = 1,
            Name = "Technology & Development",
            SubServices = new List<SubServiceViewModel>
            {
                new SubServiceViewModel { Id = 1, Name = "Web Development" },
                new SubServiceViewModel { Id = 2, Name = "Mobile App Development" },
                new SubServiceViewModel { Id = 3, Name = "Software Engineering" },
                new SubServiceViewModel { Id = 4, Name = "AI & Machine Learning" },
                new SubServiceViewModel { Id = 5, Name = "Cloud Computing" }
            }
        },
        new ServiceViewModel
        {
            Id = 2,
            Name = "Business Marketing",
            SubServices = new List<SubServiceViewModel>
            {
                new SubServiceViewModel { Id = 6, Name = "Digital Marketing" },
                new SubServiceViewModel { Id = 7, Name = "Social Media Ads" },
                new SubServiceViewModel { Id = 8, Name = "Brand Strategy" },
                new SubServiceViewModel { Id = 9, Name = "SEO Services" },
                new SubServiceViewModel { Id = 10, Name = "Ad Campaigns" }
            }
        },
        new ServiceViewModel
        {
            Id = 3,
            Name = "Creative Design",
            SubServices = new List<SubServiceViewModel>
            {
                new SubServiceViewModel { Id = 11, Name = "Logo Design" },
                new SubServiceViewModel { Id = 12, Name = "UI/UX Design" },
                new SubServiceViewModel { Id = 13, Name = "Motion Graphics" },
                new SubServiceViewModel { Id = 14, Name = "Product Packaging" },
                new SubServiceViewModel { Id = 15, Name = "Brochure & Flyers" }
            }
        },
        new ServiceViewModel
        {
            Id = 4,
            Name = "Personal Development",
            SubServices = new List<SubServiceViewModel>
            {
                new SubServiceViewModel { Id = 16, Name = "Communication Skills" },
                new SubServiceViewModel { Id = 17, Name = "Leadership Training" },
                new SubServiceViewModel { Id = 18, Name = "Time Management" },
                new SubServiceViewModel { Id = 19, Name = "Public Speaking" },
                new SubServiceViewModel { Id = 20, Name = "Career Counselling" }
            }
        }
    };

            return View(services);
        }


        public IActionResult BookList()
        {
            var books = new List<BookViewModel>
    {
        new BookViewModel
        {
            Id = 1,
            Name = "Fiction",
            SubBooks = new List<SubBookViewModel>
            {
                new SubBookViewModel { Id = 1, Name = "Fantasy" },
                new SubBookViewModel { Id = 2, Name = "Mystery" },
                new SubBookViewModel { Id = 3, Name = "Romance" }
            }
        },
        new BookViewModel
        {
            Id = 2,
            Name = "Non-Fiction",
            SubBooks = new List<SubBookViewModel>
            {
                new SubBookViewModel { Id = 4, Name = "Biography" },
                new SubBookViewModel { Id = 5, Name = "Self-Help" },
                new SubBookViewModel { Id = 6, Name = "History" }
            }
        }
    };

            return View(books);
        }



        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalPartners = _partnerDb.GetAllPartners().Count(),
                TotalManagers = _managerDb.GetAllManagers().Count(),
                TotalCompanies = _companyDb.GetAllCompanies().Count(),
                TotalBranches = _branchDb.GetAllBranches().Count(),
                TotalServiceProviders = _serviceProviderDb.GetAllServiceProvider().Count(),
            };

            return View(model);

            // var partners = _partnerDb.GetAllPartners();
            //return View(partners);
            // return View();
        }

        public IActionResult PartnerList()
        {
            var partners = _partnerDb.GetAllPartners();
            return View(partners);
            // return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AdminLogin model)
        {
            if (ModelState.IsValid)
            {
                var user = _adminDb.AdminLogin(model.Admin_Name, model.Admin_Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("username", user.Admin_Name);
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }

                ViewBag.msg = "Invalid username or password";
            }

            return View(model);
        }




        //[HttpPost]
        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear(); // Clear session data
        //    return RedirectToAction("Login", "Admin");
        //}


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Admin", new { area = "Admin" });
        }


        [AdminAuthorization]
        [HttpGet]
        public IActionResult CreateManager()
        {
            // Fetch all active (non-deleted) managers to show in the UI
            List<Manager> managers = _managerDb.GetAllManagers();
            return View(managers);
        }

        [HttpPost]
        public IActionResult CreateManager(Manager model)
        {
            try
            {
                _managerDb.CreateManager(model);
                TempData["Message"] = "Manager created successfully.";
                return RedirectToAction("CreateManager");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [AdminAuthorization]
        public IActionResult ManagerList()
        {
            var managers = _managerDb.GetAllManagers();
            return View(managers);
        }

        [AdminAuthorization]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = _managerDb.DeleteManager(id); // Use your DB logic
                return Json(new { success = isDeleted });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        //--------------------------------------------------Start Partner KYC section implemented by Gopal Kumar Roy – includes submission, edit, approval, and listing functionality------------------------------------------
        public IActionResult ApproveKYC(int id)
        {
            bool success = _partnerDb.UpdateKycStatus(id, "Approved", "Admin");
            TempData["Message"] = success ? "Failed to approve KYC." : " KYC has been approved successfully.";
            return RedirectToAction("KYCList");
        }

        public IActionResult RejectKYC(int id)
        {
            bool success = _partnerDb.UpdateKycStatus(id, "Rejected", "Admin");
            TempData["Message"] = success ? "Failed to reject KYC." : " KYC has been rejected successfully.";
            return RedirectToAction("KYCList");
        }

        public IActionResult KYCList()
        {
            var kycList = _partnerDb.GetAllKycRecords(); // Fetch all KYC records from the database
            return View(kycList); // Pass the list to the view
        }

        //------------------------------------End Partner KYC section implemented by Gopal Kumar Roy – includes submission, edit, approval, and listing functionality------------------------------------------------------



        //------------------------------------Start Manager KYC section implemented by Gopal Kumar Roy – includes submission, edit, approval, and listing functionality----------------------------------------------------
        // For Admin approval/rejection of Manager KYC


        public IActionResult ApproveManagerKYC(int id)
        {
            bool success = _managerDb.UpdateManagerKycStatus(id, "Approved", "Admin");
            TempData["Message"] = success ? "Failed to approve Manager KYC." : "Manager KYC has been approved successfully.";
            return RedirectToAction("ManagerKYCList");
        }


        public IActionResult RejectManagerKYC(int id)
        {
            bool success = _managerDb.UpdateManagerKycStatus(id, "Rejected", "Admin");
            TempData["Message"] = success ? "Failed to reject Manager KYC." : "Manager KYC has been rejected successfully.\r\n    }";
            return RedirectToAction("ManagerKYCList");
        }


        public IActionResult ManagerKYCList()
        {
            var kycList = _managerDb.GetAllManagerKycRecords(); // You must define this method in DB layer
            return View(kycList);
        }
        //------------------------------------ End Manager KYC section implemented by Gopal Kumar Roy – includes submission, edit, approval, and listing functionality.------------------------------------------------------


        //--------------------------------------------------Start Service-Provider KYC section implemented by Gopal Kumar Roy – includes submission, edit, approval, and listing functionality------------------------------------------

        // Approve Service Provider KYC
        public IActionResult ApproveServiceProviderKYC(int id)
        {
            bool success = _serviceProviderDb.UpdateKycStatus(id, "Approved", "Admin");
            TempData["Message"] = success
                ? "Failed to approve Service Provider KYC."
                : "Service Provider KYC has been approved successfully.";
            return RedirectToAction("ServiceProviderKYCList");
        }
        
        // Reject Service Provider KYC
        public IActionResult RejectServiceProviderKYC(int id)
        {
            bool success = _serviceProviderDb.UpdateKycStatus(id, "Rejected", "Admin");
            TempData["Message"] = success
                ? "Failed to reject Service Provider KYC."
                : "Service Provider KYC has been rejected successfully.";
            return RedirectToAction("ServiceProviderKYCList");
        }
        
        // List all Service Provider KYC records
        public IActionResult ServiceProviderKYCList()
        {
            var kycList = _serviceProviderDb.GetAllServiceProviderKYCs();
            return View(kycList);
        }



        //------------------------------------ End Service-Provider KYC section implemented by Gopal Kumar Roy – includes submission, edit, approval, and listing functionality.------------------------------------------------------



    }
}
