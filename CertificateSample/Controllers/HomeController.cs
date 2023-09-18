using CertificateSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Diagnostics;
using System.Reflection;

namespace CertificateSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetFile(CertificateViewModel model)
        {
            var result = await GetReport(model);
            return Json(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> ExportReportToPdf(CertificateViewModel model)
        {
            var result = await GetReport(model);
            var fileName = $"شهادة_{model.Name}.pdf";
            return File(result, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        }

        public async Task<byte[]> GetReport(CertificateViewModel model)
        {
            using var report = new LocalReport();
            using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream("CertificateSample.wwwroot.samples.rptCertificate.rdlc");
            report.LoadReportDefinition(rs);
            report.DataSources.Add(new ReportDataSource("dsCertificate",
                new List<CertificateViewModel>() { model }));
            var bytes = report.Render("PDF");
            return bytes;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}