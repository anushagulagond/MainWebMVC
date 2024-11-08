using MainWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MainWebMVC.Controllers
{
    public class EmpDeptController : Controller
    {
        private readonly ILogger<EmpDeptController> _logger;


        public EmpDeptController(ILogger<EmpDeptController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var empDept = await GetEmpDeptAsync();

            return View("EmpDeptView", empDept);
        }

        private static async Task<List<EmpDept>> GetEmpDeptAsync()
        {
            HttpClient client = new HttpClient();
            // Set the base address of the API
            client.BaseAddress = new Uri("https://localhost:7172/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Call the API
            HttpResponseMessage response = await client.GetAsync("api/Join");

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response to a List<EmpDept>
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<EmpDept>>(responseData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                throw new Exception("Failed to retrieve EmpDept from API");
            }
        }
    }
}
