using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppOwnsData.Models;
using AppOwnsData.Services;

namespace AppOwnsData.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private PowerBiServiceApi powerBiServiceApi;

        public HomeController(PowerBiServiceApi powerBiServiceApi)
        {
            this.powerBiServiceApi = powerBiServiceApi;
        }

       // [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Embed() {

            // Replace these two GUIDs with the workspace ID and report ID you recorded earlier.
            Guid workspaceId = new Guid("f91c9eb1-77c1-4e6f-a351-dc2af802f18d");
            Guid reportId = new Guid("5e2c954e-c6cf-495e-add1-bd61389afa8b");

            var viewModel = await powerBiServiceApi.GetReport(workspaceId, reportId);

            return View(viewModel);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}