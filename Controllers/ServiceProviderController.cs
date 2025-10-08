using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Configuration;
using Sewa360.Areas.Admin.Models;
using Sewa360.DBContext;
using Sewa360.Filters;
using Sewa360.Models;

namespace Sewa360.Controllers
{
   
    public class ServiceProviderController : Controller
    {
        private readonly ServiceProviderDb _serviceproviderDb;
        private readonly ManagerDb _managerDb;

        public ServiceProviderController(ServiceProviderDb serviceproviderDb, ManagerDb managerDb)
        {
            _serviceproviderDb = serviceproviderDb;
            _managerDb = managerDb; 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ServiceProviderRegister()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ServiceProviderRegister(SericeProvider model)
        {
            if (string.IsNullOrWhiteSpace(model.Manager_Code))
            {
                ModelState.AddModelError("Manager_Code", "Referral code is required.");
                return View(model);
            }

            var manager = _managerDb.GetManagerByCode(model.Manager_Code);
            if (manager == null)
            {
                ModelState.AddModelError("Manager_Code", "Invalid referral code.");
                return View(model);
            }

            model.Manager_Id = manager.Manager_Id;
            if (!ModelState.IsValid)
            {
                bool isInserted = _serviceproviderDb.InsertServiceProvider(model);

                TempData["msg"] = "Registration successful! Please login.";
               
                return RedirectToAction("ServiceProviderLogin", "ServiceProvider"); // 👈 Redirect to Partner Login

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
            return View(model);
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

        [HttpGet]
        public IActionResult ServiceProviderLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ServiceProviderLogin(IFormCollection form)
        {
            string serviceProvEmail = form["ServiceProv_Email"];
            string password = form["ServiceProv_Password"];

            if (string.IsNullOrWhiteSpace(serviceProvEmail) && string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter Service Provider Email and Password.";
                return View();
            }
            else if (string.IsNullOrWhiteSpace(serviceProvEmail))
            {
                ViewBag.Error = "Please enter your Service Provider Email.";
                return View();
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter your Password.";
                return View();
            }

            var serviceProvider = _serviceproviderDb.ValidateServiceProviderLogin(serviceProvEmail, password);

            if (serviceProvider != null)
            {
                HttpContext.Session.SetString("FirstName", serviceProvider.First_Name ?? "");
                HttpContext.Session.SetString("ServiceProvEmail", serviceProvider.ServiceProv_Email ?? "");
                HttpContext.Session.SetInt32("ServiceProviderId", serviceProvider.ServiceProvider_Id);

                return RedirectToAction("Index", "ServiceProvider");
            }
            else
            {
                ViewBag.Error = "Invalid Service Provider Email or Password.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("ServiceProviderLogin", "ServiceProvider");
        }

        public IActionResult ServiceProviderList(int? id)
        {
            var serviceProviderList = _serviceproviderDb.GetServiceProviderList(id);
            return View(serviceProviderList);
        }

        [HttpGet]
        public IActionResult ServiceProviderKYC()
        {
            int? spId = HttpContext.Session.GetInt32("ServiceProviderId");
            if (spId == null)
                return RedirectToAction("Login", "ServiceProvider");

            var existingKyc = _serviceproviderDb.GetServiceProviderKYCById(spId.Value);
            if (existingKyc != null && existingKyc.KycStatus != "Rejected")
            {
                TempData["Info"] = $"You have already submitted KYC. Status: {existingKyc.KycStatus}";
                return RedirectToAction("KycStatus");
            }

            var model = new ServiceProviderKYC
            {
                ServiceProvider_Id = spId.Value
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult ServiceProviderKYC(ServiceProviderKYC model, IFormFile AadhaarDoc, IFormFile PanCardDoc)
        {
            int? spId = HttpContext.Session.GetInt32("ServiceProviderId");
            if (spId == null)
                return RedirectToAction("Login", "ServiceProvider");

            model.ServiceProvider_Id = spId.Value;

            var existingKyc = _serviceproviderDb.GetServiceProviderKYCById(spId.Value);
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
                string aadhaarFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/SP_Aadhaar");
                string panFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/SP_PanCard");
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

                    string fileName = $"AAD_SP_{spId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(aadhaarFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    AadhaarDoc.CopyTo(stream);
                    aadhaarPath = "/KYCDocs/SP_Aadhaar/" + fileName;
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

                    string fileName = $"PAN_SP_{spId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(panFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    PanCardDoc.CopyTo(stream);
                    panPath = "/KYCDocs/SP_PanCard/" + fileName;
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
                    model.ModifiedBy = "ServiceProvider";
                    model.ModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    success = _serviceproviderDb.UpdateServiceProviderKYC(model);
                }
                else
                {
                    model.CreatedBy = "ServiceProvider";
                    model.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    success = _serviceproviderDb.SaveServiceProviderKYC(model);
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
        public IActionResult ServiceProviderEditKYC(int id)
        {
            // Get KYC record by its primary key (KYC_Id)
            var kyc = _serviceproviderDb.GetServiceProviderKYCById(id);

            if (kyc == null)
            {
                TempData["Error"] = "KYC record not found.";
                return RedirectToAction("KycStatus"); // Redirect if not found
            }

            // Optional: check if the logged-in service provider is the owner of this KYC record
            int? serviceProviderId = HttpContext.Session.GetInt32("ServiceProviderId");
            if (serviceProviderId == null || kyc.ServiceProvider_Id != serviceProviderId.Value)
            {
                TempData["Error"] = "Unauthorized access.";
                return RedirectToAction("Login", "ServiceProvider");
            }

            return View(kyc); // Load the existing KYC data into the form
        }

        [HttpPost]
        public IActionResult ServiceProviderEditKYC(ServiceProviderKYC model, IFormFile AadhaarDoc, IFormFile PanCardDoc)
        {
            int? serviceProviderId = HttpContext.Session.GetInt32("ServiceProviderId");
            if (serviceProviderId == null)
                return RedirectToAction("Login", "ServiceProvider");

            model.ServiceProvider_Id = serviceProviderId.Value;

            if (ModelState.IsValid)
                return View(model); // If model is invalid, re-render form

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            long maxSize = 5 * 1024 * 1024;

            string aadhaarPath = model.AadhaarDocumentPath;
            string panPath = model.PanCardDocumentPath;

            try
            {
                string aadhaarFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/ServiceProvider/Aadhaar");
                string panFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KYCDocs/ServiceProvider/PanCard");
                Directory.CreateDirectory(aadhaarFolder);
                Directory.CreateDirectory(panFolder);

                // Aadhaar upload
                if (AadhaarDoc != null && AadhaarDoc.Length > 0)
                {
                    var ext = Path.GetExtension(AadhaarDoc.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                        ModelState.AddModelError("AadhaarDoc", "Invalid Aadhaar file type.");
                    if (AadhaarDoc.Length > maxSize)
                        ModelState.AddModelError("AadhaarDoc", "Aadhaar file size must be under 5MB.");

                    if (ModelState.IsValid)
                        return View(model);

                    string fileName = $"AAD_{serviceProviderId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(aadhaarFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    AadhaarDoc.CopyTo(stream);
                    aadhaarPath = "/KYCDocs/ServiceProvider/Aadhaar/" + fileName;
                }

                // PAN upload
                if (PanCardDoc != null && PanCardDoc.Length > 0)
                {
                    var ext = Path.GetExtension(PanCardDoc.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext))
                        ModelState.AddModelError("PanCardDoc", "Invalid PAN file type.");
                    if (PanCardDoc.Length > maxSize)
                        ModelState.AddModelError("PanCardDoc", "PAN file size must be under 5MB.");

                    if (ModelState.IsValid)
                        return View(model);

                    string fileName = $"PAN_{serviceProviderId}_{Guid.NewGuid()}{ext}";
                    string savePath = Path.Combine(panFolder, fileName);
                    using var stream = new FileStream(savePath, FileMode.Create);
                    PanCardDoc.CopyTo(stream);
                    panPath = "/KYCDocs/ServiceProvider/PanCard/" + fileName;
                }

                // Final assignment
                model.AadhaarDocumentPath = aadhaarPath;
                model.PanCardDocumentPath = panPath;
                model.ModifiedBy = "ServiceProvider";
                model.ModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                model.KycStatus = "Pending";
                model.IsActive = "1";
                model.IsDeleted = "0";

                bool updated = _serviceproviderDb.UpdateServiceProviderKYC(model);

                
                
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

        [HttpGet]
        public IActionResult KycStatus()
        {
            int? serviceProviderId = HttpContext.Session.GetInt32("ServiceProviderId");
            if (serviceProviderId == null)
                return RedirectToAction("Login", "ServiceProvider");

            var model = _serviceproviderDb.GetServiceProviderKYCById(serviceProviderId.Value);
            if (model == null)
            {
                TempData["Info"] = "No KYC record found. Please submit your KYC.";
                return RedirectToAction("ServiceProviderKYC");
            }

            return View(model);
        }


        public IActionResult ServiceProviderProfile()
        {
            int? serviceProviderId = HttpContext.Session.GetInt32("ServiceProviderId");
            if (serviceProviderId == null)
            {
                return RedirectToAction("ServiceProviderProfile", "ServiceProvider"); // or your login page
            }

            var partner = _serviceproviderDb.GetServiceProviderById(serviceProviderId.Value); // Use your DB method
            if (partner == null)
            {
                return NotFound();
            }

            return View(partner); // Pass data to view
        }

    }
}
