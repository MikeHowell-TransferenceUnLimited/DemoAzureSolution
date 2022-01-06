using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBasicSite.Models;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using System.Data.SqlClient;

namespace MyBasicSite.Controllers
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

        public IActionResult Security()
        {
            string secretValue;

            try
            {
                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                   {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                  }
                };

            

                var client = new SecretClient(new Uri("https://mchdemokeyvault.vault.azure.net/"), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("DemoConnectionString");
                secretValue = secret.Value;
            }
            catch (Exception ex)
            {
                secretValue = ex.ToString();
            }


            return View(model:secretValue);
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













