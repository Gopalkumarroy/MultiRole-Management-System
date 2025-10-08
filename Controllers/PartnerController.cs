using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sewa360.Areas.Admin.Models;
using Sewa360.DBContext;
using Sewa360.Filters;
using Sewa360.Models;


using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Sewa360.Controllers
{

   // [Area("Admin")]
  
    public class PartnerController : Controller
    {
        private readonly PartnerDb _partnerDb;
        private readonly ManagerDb _managerDb;
        public PartnerController(PartnerDb partnerDb, ManagerDb managerDb)
        {
            _partnerDb = partnerDb;
            _managerDb = managerDb;
        }
        public IActionResult Index()
        {
            PartnerDashboardViewModel model = new PartnerDashboardViewModel
            {
                JobPostCount = 15,
                SalesAmount = 100000,
                NumberOfServices = 12,
                CommissionAmount = 8000,
                ListingCount = 9,
                Months = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                MonthlySales = new List<decimal> { 15000, 18000, 14000, 20000, 22000, 19000 }
            };

            return View(model);
            //return View();
        }

        [HttpGet]
        public IActionResult PartnerRegisterForm(int? id)
        {
            Partner partner = new Partner();

            if (id.HasValue)
            {
                partner = _partnerDb.GetPartnerById(id.Value); // Load data if editing
            }

            return View(partner);
        }

        // Previous code  

        //[HttpPost]
        //public IActionResult PartnerRegisterForm(Partner partner)
        //{
        //    if (string.IsNullOrWhiteSpace(partner.Manager_Code))
        //    {
        //        ModelState.AddModelError("Manager_Code", "Referral code is required.");
        //        return View(partner);
        //    }

        //    var manager = _managerDb.GetManagerByCode(partner.Manager_Code);
        //    if (manager == null)
        //    {
        //        ModelState.AddModelError("Manager_Code", "Invalid referral code.");
        //        return View(partner);
        //    }

        //    partner.Manager_Id = manager.Manager_Id;

        //    if (!ModelState.IsValid)
        //    {
        //        bool isInserted = _partnerDb.InsertPartner(partner);

        //        if (isInserted)
        //        {
        //            TempData["msg"] = "Registration successful!";
        //            return RedirectToAction("Success");
        //        }

        //        else
        //        {
        //            ModelState.AddModelError("", "Failed to register partner.");
        //        }
        //    }

        //    return View(partner);
        //}

        [HttpPost]
        public IActionResult PartnerRegisterForm(Partner partner)
        {
            if (string.IsNullOrWhiteSpace(partner.Manager_Code))
            {
                ModelState.AddModelError("Manager_Code", "Referral code is required.");
                return View(partner);
            }

            var manager = _managerDb.GetManagerByCode(partner.Manager_Code);
            if (manager == null)
            {
                ModelState.AddModelError("Manager_Code", "Invalid referral code.");
                return View(partner);
            }

            partner.Manager_Id = manager.Manager_Id;

            if (!ModelState.IsValid)
            {
                bool isInserted = _partnerDb.InsertPartner(partner);

                TempData["msg"] = "Registration successful! Please login.";
                return RedirectToAction("PartnerLogin", "Partner"); // 👈 Redirect to Partner Login

                //if (isInserted)
                //{
                //    TempData["msg"] = "Registration successful! Please login.";
                //    return RedirectToAction("Login", "Partner"); // 👈 Redirect to Partner Login
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Failed to register partner.");
                //}
            }
            return View(partner);
        }




        [HttpGet]
        public IActionResult PartnerLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PartnerLogin(IFormCollection form)
        {
            string partnerEmail = form["Partner_Email"];
            string password = form["Partner_Password"];

            // ✅ Input validations
            if (string.IsNullOrWhiteSpace(partnerEmail) && string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter Partner Email and Password.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(partnerEmail))
            {
                ViewBag.Error = "Please enter your Partner Email.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter your Password.";
                return View();
            }

            if (!partnerEmail.Contains("@") || !partnerEmail.Contains(".")) // Optional
            {
                ViewBag.Error = "Please enter a valid email address.";
                return View();
            }

            // ✅ Check credentials
            var partner = _partnerDb.ValidatePartnerLogin(partnerEmail, password);

            if (partner != null)
            {
                HttpContext.Session.SetString("FirstName", partner.First_Name ?? "");
                HttpContext.Session.SetString("PartnerEmail", partner.Partner_Email ?? "");
                HttpContext.Session.SetInt32("PartnerId", partner.Partner_Id);

                return RedirectToAction("Index", "Partner");
            }

            // ✅ If not found or invalid credentials
            ViewBag.Error = "Invalid Partner Email or Password.";
            return View();
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PartnerLogin", "Partner");
        }



        [HttpGet]
        public IActionResult GetManagerNameByCode(string managerCode)
        {
            if (string.IsNullOrEmpty(managerCode))
            {
                return Json("");
            }

            var manager = _managerDb.GetManagerByCode(managerCode);

            if (manager != null && manager.Manager_Code != null &&
                manager.Manager_Code.Equals(managerCode, StringComparison.OrdinalIgnoreCase))
            {
                return Json(manager.Manager_Name);
            }

            return Json("");
        }

        public IActionResult ListPartners()
        {
            // Get ManagerCode from Session
            string managerCode = HttpContext.Session.GetString("ManagerCode");

            if (string.IsNullOrEmpty(managerCode))
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("ManagerLogin", "Manager", new { area = "Admin" });
            }

            // Get list of partners associated with the logged-in mana
            var partnerList = _partnerDb.GetPartnersByManagerCode(managerCode);

            return View(partnerList); // Make sure you have a corresponding View
        }


        [Area("Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Delete the partner from DB by id
            _partnerDb.DeletePartner(id); // your delete logic here
            TempData["SuccessMessage"] = "Partner deleted successfully.";
            // Redirect back to list page
            return RedirectToAction("Index", "Admin", new { area = "Admin" });

        }


        [Area("Admin")]
        [HttpGet]
        public IActionResult PartnerDetails(int id)
        {
            var partner = _partnerDb.GetPartnerById(id);
            if (partner == null)
            {
                return HttpNotFound(); // Optional: custom 404 view
            }
             return View("~/Views/Partner/PartnerDetails.cshtml", partner);
        }
        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public IActionResult PartnerEdit()
        {

            return View();
        }

      

        [HttpGet]
        public IActionResult PartnerKYC()
        {
            int? partnerId = HttpContext.Session.GetInt32("PartnerId");
            if (partnerId == null)
                return RedirectToAction("Login", "Partner");

            // Call the method with SELECTBYPARTNERID
            var existingKyc = _partnerDb.GetPartnerKYCByPartnerId(partnerId.Value);

            // If KYC is already submitted and not rejected, redirect to KycStatus view
            if (existingKyc != null && existingKyc.KycStatus != "Rejected")
            {
                TempData["Info"] = $"You have already submitted KYC. Status: {existingKyc.KycStatus}";
                return RedirectToAction("KycStatus");
            }

            // If KYC is missing or rejected, open the KYC submission form
            var model = new PartnerKYC
            {
                Partner_Id = partnerId.Value
            };

            return View(model);
        }



        [HttpPost]
        public IActionResult PartnerKYC(PartnerKYC model, IFormFile AadhaarDoc, IFormFile PanCardDoc)
        {
            int? partnerId = HttpContext.Session.GetInt32("PartnerId");
            if (partnerId == null)
                return RedirectToAction("Login", "Partner");

            model.Partner_Id = partnerId.Value;

            var existingKyc = _partnerDb.GetPartnerKYCByPartnerId(partnerId.Value);
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
                        ModelState.AddModelError("AadhaarDoc", "Aadhaar file size must be under 5MB.");

                    if (ModelState.IsValid)
                        return View(model);

                    string fileName = $"AAD_{partnerId}_{Guid.NewGuid()}{ext}";
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
                        ModelState.AddModelError("PanCardDoc", "PAN file size must be under 5MB.");

                    if (ModelState.IsValid)
                        return View(model);

                    string fileName = $"PAN_{partnerId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(panFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    PanCardDoc.CopyTo(stream);
                    panPath = "/KYCDocs/PanCard/" + fileName;
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
                    model.ModifiedBy = "Partner";
                    success = _partnerDb.UpdateKYC(model);
                }
                else
                {
                    model.CreatedBy = "Partner";
                    model.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    success = _partnerDb.SaveKYC(model);
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


        public IActionResult PartnerProfile()
        {
            int? partnerId = HttpContext.Session.GetInt32("PartnerId");
            if (partnerId == null)
            {
                return RedirectToAction("PartnerProfile", "Partner"); // or your login page
            }

            var partner = _partnerDb.GetPartnerById(partnerId.Value); // Use your DB method
            if (partner == null)
            {
                return NotFound();
            }

            return View(partner); // Pass data to view
        }

        [HttpGet]
        public IActionResult KycStatus()
        {
            int? partnerId = HttpContext.Session.GetInt32("PartnerId");
            if (partnerId == null)
                return RedirectToAction("Login", "Partner");

            var model = _partnerDb.GetPartnerKYCByPartnerId(partnerId.Value);
            if (model == null)
            {
                TempData["Info"] = "No KYC record found. Please submit your KYC.";
                return RedirectToAction("PartnerKYC");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EditKYC()
        {
            int? partnerId = HttpContext.Session.GetInt32("PartnerId");
            if (partnerId == null)
                return RedirectToAction("Login", "Partner");

            var model = _partnerDb.GetPartnerKYCByPartnerId(partnerId.Value);
            if (model == null)
            {
                TempData["Error"] = "No KYC record found to edit.";
                return RedirectToAction("PartnerKYC");
            }

            if (model.KycStatus == "Approved")
            {
                TempData["Error"] = "Your KYC is already approved. You cannot edit it.";
                return RedirectToAction("KycStatus");
            }

            return View(model); // Goes to EditKYC.cshtml
        }


        [HttpPost]
        public IActionResult EditKYC(PartnerKYC model, IFormFile AadhaarDoc, IFormFile PanCardDoc)
        {
            int? partnerId = HttpContext.Session.GetInt32("PartnerId");
            if (partnerId == null)
                return RedirectToAction("Login", "Partner");

            model.Partner_Id = partnerId.Value;

            // File handling (reuse logic from your PartnerKYC POST)
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

                    string fileName = $"AAD_{partnerId}_{Guid.NewGuid()}{ext}";
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

                    string fileName = $"PAN_{partnerId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(panFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    PanCardDoc.CopyTo(stream);
                    panPath = "/KYCDocs/PanCard/" + fileName;
                }

                // Assign new paths if updated
                model.AadhaarDocumentPath = aadhaarPath;
                model.PanCardDocumentPath = panPath;
                model.ModifiedBy = "Partner";
                //model.ModifiedOn = DateTime.Now;
                model.KycStatus = "Pending"; // reset status to Pending after edit

                bool result = _partnerDb.UpdateKYC(model);
                
                
                    TempData["Success"] = "KYC updated successfully.";
                    return RedirectToAction("KycStatus");
                

                ModelState.AddModelError("", "Failed to update KYC.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(model);
        }



    }
}

