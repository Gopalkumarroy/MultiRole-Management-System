using Microsoft.AspNetCore.Mvc;
using Sewa360.Areas.Admin.Models;
using Sewa360.DBContext;
using System.IO.Compression;

namespace Sewa360.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CertificateController : Controller
    {
        private readonly CertificateDb _certificateDb;

        public CertificateController(CertificateDb certificateDb)
        {
            _certificateDb = certificateDb;
        }

        // List all certificates in a batch
        public IActionResult BatchCertificates(int batchId)
        {
            var certs = _certificateDb.GetCertificatesByBatch(batchId);
            return View(certs); // Razor view will show download buttons
        }

        // Download single certificate
        public IActionResult Download(int id)
        {
            var cert = _certificateDb.GetCertificateById(id);
            if (cert == null || !System.IO.File.Exists(cert.PdfPath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(cert.PdfPath);
            return File(bytes, "application/pdf", Path.GetFileName(cert.PdfPath));
        }

        // Download all certificates from a batch as ZIP
        public IActionResult DownloadAll(int batchId)
        {
            var certs = _certificateDb.GetCertificatesByBatch(batchId);
            if (certs.Count == 0) return NotFound();

            string tempZip = Path.Combine(Path.GetTempPath(), $"Batch_{batchId}_Certificates.zip");

            using (var zip = ZipFile.Open(tempZip, ZipArchiveMode.Create))
            {
                foreach (var c in certs)
                {
                    if (System.IO.File.Exists(c.PdfPath))
                        zip.CreateEntryFromFile(c.PdfPath, Path.GetFileName(c.PdfPath));
                }
            }

            var zipBytes = System.IO.File.ReadAllBytes(tempZip);
            System.IO.File.Delete(tempZip); // cleanup
            return File(zipBytes, "application/zip", $"Batch_{batchId}_Certificates.zip");
        }


        // Show form
        public IActionResult CreateCertificate()
        {
            return View();
        }

        // Save certificate
        [HttpPost]
        public IActionResult CreateCertificate(Certificates model)
        {
            if (ModelState.IsValid)
            {
                _certificateDb.AddStudent(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // List certificates
        public IActionResult ShowCertificates()
        {
            var list = _certificateDb.GetAllCertificates();
            return View(list);
            //return View();
        }
    }
}
