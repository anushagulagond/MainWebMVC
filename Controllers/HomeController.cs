using MainWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
/*using Newtonsoft.Json;*/
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;



namespace MainWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await GetDepartmentsAsync();

            return View("DepartmentView",departments);
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
        public IActionResult Create()
        {
            Department department = new Department();
            return View( "Create", department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {

            if (ModelState.IsValid)
            {
                var isSuccess = await PostDepartmentAsync(department);

                if (isSuccess)
                {
                    return RedirectToAction("Index");  // Redirect to Index or success page
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to insert department.");
                }
            }

            return View(department);
        }

        private async Task<bool> PostDepartmentAsync(Department department)
        {
             HttpClient client = new HttpClient();
        // Set base address and headers
            client.BaseAddress = new Uri("https://localhost:7172/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serialize the department object to JSON
            var json = JsonSerializer.Serialize(department);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the POST request to the Web API
            HttpResponseMessage response = await client.PostAsync("api/Departments", content);

            // Return true if the request was successful
            return response.IsSuccessStatusCode;
        }
        
        private static async Task<List<Department>> GetDepartmentsAsync()
        {
            HttpClient client = new HttpClient();
            // Set the base address of the API
            client.BaseAddress = new Uri("https://localhost:7172/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Call the API
            HttpResponseMessage response = await client.GetAsync("api/Departments");

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response to a List<Department>
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Department>>(responseData, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                throw new Exception("Failed to retrieve departments from API");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDepartment(Department department)
        {
            HttpClient client = new HttpClient();
           
            int id = department.DeptId;
            string apiUrl = $"https://localhost:7172/api/departments/{id}";
            var jsonContent = JsonSerializer.Serialize(department);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // or appropriate action
            }
            else
            {
                ModelState.AddModelError("", "Error updating department.");
                return View(department);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            Department department = new Department();
            department = await GetDepartmentByIdAsync(id);
            return View("Edit", department);
        }
        private async Task<Department> GetDepartmentByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7172/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/Departments/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Department>(responseData,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    throw new Exception("Failed to retrieve departments from API");
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var department = await GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department); // Display confirmation view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isSuccess = await DeleteDepartmentAsync(id);
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete department.");
                return RedirectToAction(nameof(Index)); // Optionally, you can return to an error view
            }
        }

        private async Task<bool> DeleteDepartmentAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7172/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"api/Departments/{id}");
                return response.IsSuccessStatusCode;
            }
        }

        }
    }
