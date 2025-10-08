using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sewa360.Areas.Admin.Models;
using Sewa360.DBContext;
using Sewa360.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sewa360.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class ManagerController : Controller
    {
        private readonly ManagerDb _managerDb;
        private readonly PartnerDb _partnerDb;
        private readonly AdminDb _adminDb;
        private readonly ServiceProviderDb _serviceProviderDb;

        public ManagerController(ManagerDb managerDb, PartnerDb partnerDb, AdminDb adminDb, ServiceProviderDb serviceProviderDb)
        {
            _managerDb = managerDb;
            _partnerDb = partnerDb;
            _adminDb = adminDb;
            _serviceProviderDb = serviceProviderDb;
        }

        public IActionResult Index()
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");
            if (managerId == null)
            {
                return RedirectToAction("ManagerLogin", "Manager");
            }

            var counts = _adminDb.GetKYCCountsByManager(managerId.Value);

            var model = new ManagerDashboardViewModel
            {
                TotalPartners = _adminDb.GetPartnerCountByManager(managerId.Value),
                KYCSubmitted = counts.Approved,
                KYCPending = counts.Pending,
                KYCRejected = counts.Rejected,
                KYCNotDone = counts.NotDone,
                TotalServiceProviders = _serviceProviderDb.GetServiceProviderCountByManager(managerId.Value),

                 TotalSales = 120000, // ₹1,20,000
                MonthlySales = new List<decimal> { 20000, 30000, 25000, 40000, 30000, 45000 }

            };

            return View(model);
        }



        public IActionResult TeamList()
        {
            var managerCode = HttpContext.Session.GetString("ManagerCode");

            if (string.IsNullOrEmpty(managerCode))
            {
                return RedirectToAction("ManagerLogin", "Manager");
            }

            var partners = _partnerDb.GetPartnersByManagerCode(managerCode);
            return View(partners);
        }

        [HttpGet]
        public IActionResult GetManagerNameByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json("");
            }

            var allManagers = _managerDb.GetAllManagers();

            var managerName = allManagers
                .FirstOrDefault(m => m.Manager_Code != null && m.Manager_Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                ?.Manager_Name;

            return Json(managerName ?? "");
        }

        [HttpGet]
        public IActionResult ManagerLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ManagerLogin(IFormCollection form)
        {
            string managerEmail = form["Manager_Email"];
            string password = form["Manager_Password"];
            string managerCode = form["Manager_Code"];

            // Case 1: Both missing
            if (string.IsNullOrWhiteSpace(managerEmail) && string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both Manager Email and Password.";
                return View();
            }

            //  Case 2: Only email is provided
            if (!string.IsNullOrWhiteSpace(managerEmail) && string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter your password.";
                return View();
            }

            //  Case 3: Only password is provided
            if (string.IsNullOrWhiteSpace(managerEmail) && !string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter your email.";
                return View();
            }

            // 🔸 Case 4: Validate credentials
            var manager = _managerDb.ValidateManagerLogin(managerEmail, password);

            if (manager != null)
            {
                HttpContext.Session.SetString("ManagerCode", manager.Manager_Code ?? "");
                HttpContext.Session.SetString("ManagerName", manager.Manager_Name ?? "");
                HttpContext.Session.SetString("ManagerEmail", manager.Manager_Email ?? "");
                HttpContext.Session.SetInt32("ManagerId", manager.Manager_Id);

                return RedirectToAction("Index", "Manager", new { area = "Admin" });
            }
            else
            {
                ViewBag.Error = "Invalid Manager Email or Password.";
                return View();
            }
        }


        [HttpPost]
       // [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("ManagerLogin", "Manager", new { area = "Admin" });
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Manager", new { area = "Admin" });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var manager = _managerDb.GetManagerByEmail(email); // 🔹 Create/Get method in DB
                if (manager != null)
                {
                    HttpContext.Session.SetString("ManagerEmail", manager.Manager_Email);
                    HttpContext.Session.SetString("ManagerName", manager.Manager_Name);
                    HttpContext.Session.SetInt32("ManagerId", manager.Manager_Id);

                    return RedirectToAction("Index", "Manager", new { area = "Admin" });
                }
                TempData["Error"] = "No account found for this Google email.";
            }

            //return RedirectToAction("ManagerLogin");

            return RedirectToAction("Index", "Manager", new { area = "Admin" });
        }




        [HttpGet]
        public IActionResult CreatePartner()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePartner(Partner partner)
        {
            var managerId = HttpContext.Session.GetInt32("ManagerId");
            var managerCode = HttpContext.Session.GetString("ManagerCode");

            if (managerId == null || string.IsNullOrEmpty(managerCode))
            {
                TempData["msg"] = "Session expired. Please log in again.";
                return RedirectToAction("ManagerLogin");
            }

            // ✅ Set both Manager_Id and Manager_Code
            partner.Manager_Id = managerId.Value;
            partner.Manager_Code = managerCode;

            if (ModelState.IsValid)
            {
                _partnerDb.InsertPartner(partner);
                TempData["msg"] = "Partner created successfully!";
                return RedirectToAction("CreatePartner");
            }

            return View(partner);
        }

        [HttpPost]
        public IActionResult EditPartner(Partner partner)
        {
            if (ModelState.IsValid)
            {
                _partnerDb.UpdatePartner(partner);
                TempData["msg"] = "Partner updated successfully!";
                return RedirectToAction("Index");
            }
            return View(partner);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = _partnerDb.DeletePartner(id);
                return Json(new { success = isDeleted });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult MyPartners()
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");

            if (managerId == null)
            {
                return RedirectToAction("ManagerLogin");
            }

            List<Partner> partners = _partnerDb.GetPartnersByManagerId(managerId.Value);
            return View(partners);
        }

        public IActionResult ManagerProfile()
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId"); // ✅ Correct key

            if (managerId == null)
            {
                // Not logged in, redirect to login page
                return RedirectToAction("ManagerLogin", "Manager");
            }

            var manager = _managerDb.GetManagerById(managerId.Value);

            if (manager == null)
            {
                TempData["Error"] = "Manager profile not found.";
                return RedirectToAction("Index", "Admin");
            }

            return View(manager);
        }

        [HttpGet]
        public IActionResult ManagerKYC()
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");
            if (managerId == null)
                return RedirectToAction("Login", "Manager");

            var existingKyc = _managerDb.GetManagerKYCByManagerId(managerId.Value);

            if (existingKyc != null && existingKyc.KycStatus != "Rejected")
            {
                TempData["Info"] = $"You have already submitted KYC. Status: {existingKyc.KycStatus}";
                return RedirectToAction("KycStatus");
            }

            var model = new ManagerKYC
            {
                Manager_Id = managerId.Value
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult ManagerKYC(ManagerKYC model, IFormFile AadhaarDoc, IFormFile PanCardDoc)
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");
            if (managerId == null)
                return RedirectToAction("ManagerLogin", "Manager");

            model.Manager_Id = managerId.Value;

            var existingKyc = _managerDb.GetManagerKYCByManagerId(managerId.Value);
            if (existingKyc != null && existingKyc.KycStatus != "Rejected")
            {
                TempData["Error"] = $"KYC already submitted and is currently {existingKyc.KycStatus}.";
                return RedirectToAction("KycStatus");
            }

            if (ModelState.IsValid)
                return View(model);

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            long maxSize = 5 * 1024 * 1024;
            string aadhaarPath = null, panPath = null;

            try
            {
                string aadhaarFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/ManagerAadhaar");
                string panFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/ManagerPanCard");
                Directory.CreateDirectory(aadhaarFolder);
                Directory.CreateDirectory(panFolder);

                if (AadhaarDoc != null && AadhaarDoc.Length > 0)
                {
                    var ext = Path.GetExtension(AadhaarDoc.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                        ModelState.AddModelError("AadhaarDoc", "Invalid Aadhaar file type.");
                    if (AadhaarDoc.Length > maxSize)
                        ModelState.AddModelError("AadhaarDoc", "Aadhaar file size must be under 5MB.");

                    if (ModelState.IsValid)
                        return View(model);

                    string fileName = $"MAN_AAD_{managerId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(aadhaarFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    AadhaarDoc.CopyTo(stream);
                    aadhaarPath = "/KYCDocs/ManagerAadhaar/" + fileName;
                }

                if (PanCardDoc != null && PanCardDoc.Length > 0)
                {
                    var ext = Path.GetExtension(PanCardDoc.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                        ModelState.AddModelError("PanCardDoc", "Invalid PAN file type.");
                    if (PanCardDoc.Length > maxSize)
                        ModelState.AddModelError("PanCardDoc", "PAN file size must be under 5MB.");

                    if (ModelState.IsValid)
                        return View(model);

                    string fileName = $"MAN_PAN_{managerId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(panFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    PanCardDoc.CopyTo(stream);
                    panPath = "/KYCDocs/ManagerPanCard/" + fileName;
                }

                model.AadhaarDocumentPath = aadhaarPath ?? existingKyc?.AadhaarDocumentPath;
                model.PanCardDocumentPath = panPath ?? existingKyc?.PanCardDocumentPath;
                model.KycStatus = "Pending";
                model.IsActive = "1";
                model.IsDeleted = "0";

                bool success;

                if (existingKyc != null)
                {
                    model.KYC_Id = existingKyc.KYC_Id;
                    model.ModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    model.ModifiedBy = "Manager";
                    success = _managerDb.UpdateManagerKYC(model);
                }
                else
                {
                    model.CreatedBy = "Manager";
                    model.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    success = _managerDb.SaveManagerKYC(model);
                }

                
                    TempData["Success"] = "KYC submitted successfully.";
                    return RedirectToAction("KycStatus");
                

                ModelState.AddModelError("", "Failed to save KYC.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult KycStatus()
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");
            if (managerId == null)
                return RedirectToAction("ManagerLogin", "Manager");

            var model = _managerDb.GetManagerKYCByManagerId(managerId.Value);
            if (model == null)
            {
                TempData["Info"] = "No KYC record found. Please submit your KYC.";
                return RedirectToAction("ManagerKYC");
            }

            return View(model);
        }

        [Area("Admin")]
        [HttpGet]
        public IActionResult EditKYC()
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");
            if (managerId == null)
                return RedirectToAction("Login", "Manager");

            var model = _managerDb.GetManagerKYCByManagerId(managerId.Value);
            if (model == null)
            {
                TempData["Error"] = "No KYC record found to edit.";
                return RedirectToAction("ManagerKYC");
            }

            if (model.KycStatus == "Approved")
            {
                TempData["Error"] = "Your KYC is already approved. You cannot edit it.";
                return RedirectToAction("KycStatus");
            }

            return View(model); // Goes to Views/Manager/EditKYC.cshtml
        }

        [HttpPost]
        public IActionResult EditKYC(ManagerKYC model, IFormFile AadhaarDoc, IFormFile PanCardDoc)
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");
            if (managerId == null)
                return RedirectToAction("Login", "Manager");

            model.Manager_Id = managerId.Value;

            string aadhaarPath = model.AadhaarDocumentPath;
            string panPath = model.PanCardDocumentPath;
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            long maxSize = 5 * 1024 * 1024;

            try
            {
                string aadhaarFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/Aadhaar");
                string panFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/PanCard");
                Directory.CreateDirectory(aadhaarFolder);
                Directory.CreateDirectory(panFolder);

                if (AadhaarDoc != null && AadhaarDoc.Length > 0)
                {
                    var ext = Path.GetExtension(AadhaarDoc.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                        ModelState.AddModelError("AadhaarDoc", "Invalid Aadhaar file type.");
                    if (AadhaarDoc.Length > maxSize)
                        ModelState.AddModelError("AadhaarDoc", "Aadhaar file size too large.");

                    string fileName = $"AAD_MGR_{managerId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(aadhaarFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    AadhaarDoc.CopyTo(stream);
                    aadhaarPath = "/KYCDocs/Aadhaar/" + fileName;
                }

                if (PanCardDoc != null && PanCardDoc.Length > 0)
                {
                    var ext = Path.GetExtension(PanCardDoc.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                        ModelState.AddModelError("PanCardDoc", "Invalid PAN file type.");
                    if (PanCardDoc.Length > maxSize)
                        ModelState.AddModelError("PanCardDoc", "PAN file size too large.");

                    string fileName = $"PAN_MGR_{managerId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(panFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    PanCardDoc.CopyTo(stream);
                    panPath = "/KYCDocs/PanCard/" + fileName;
                }

                model.AadhaarDocumentPath = aadhaarPath;
                model.PanCardDocumentPath = panPath;
                model.ModifiedBy = "Manager";
                model.ModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                model.KycStatus = "Pending";

                bool result = _managerDb.UpdateManagerKYC(model);
                if (result)
                {
                    TempData["Success"] = "KYC updated successfully.";
                    return RedirectToAction("KycStatus");
                }

                ModelState.AddModelError("", "Failed to update KYC.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(model);
        }

        public IActionResult PartnerKYCList()
        {
            int? managerId = HttpContext.Session.GetInt32("ManagerId");

            if (managerId == null)
            {
                return RedirectToAction("Login", "Manager"); // Session expired or not logged in
            }

            var kycList = _adminDb.GetPartnerKYCList(managerId.Value);
            return View(kycList);
        }



    }
}
