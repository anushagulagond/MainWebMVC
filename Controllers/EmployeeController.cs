using MainWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace MainWebMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;


        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await GetEmployeesAsync();

            return View("Index", employees);
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
            Employee employees = new Employee();
            return View("Create", employees);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {

            if (ModelState.IsValid)
            {
                var isSuccess = await PostEmployeesAsync(employee);

                if (isSuccess)
                {
                    return RedirectToAction("Index");  // Redirect to Index or success page
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to insert employee.");
                }
            }

            return View(employee);
        }

        private async Task<bool> PostEmployeesAsync(Employee employee)
        {
            HttpClient client = new HttpClient();
            // Set base address and headers
            client.BaseAddress = new Uri("https://localhost:7172/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Serialize the department object to JSON
            var json = JsonSerializer.Serialize(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the POST request to the Web API
            HttpResponseMessage response = await client.PostAsync("api/Employees", content);

            // Return true if the request was successful
            return response.IsSuccessStatusCode;
        }

        private static async Task<List<Employee>> GetEmployeesAsync()
        {
            HttpClient client = new HttpClient();
            // Set the base address of the API
            client.BaseAddress = new Uri("https://localhost:7172/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Call the API
            HttpResponseMessage response = await client.GetAsync("api/Employees");

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response to a List<Department>
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Employee>>(responseData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                throw new Exception("Failed to retrieve employees from API");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            HttpClient client = new HttpClient();

            int id = employee.EmpId;
            string apiUrl = $"https://localhost:7172/api/employees/{id}";
            var jsonContent = JsonSerializer.Serialize(employee);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // or appropriate action
            }
            else
            {
                ModelState.AddModelError("", "Error updating employee.");
                return View(employee);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            Employee employee = new Employee();
            employee = await GetEmployeeByIdAsync(id);
            return View("Edit", employee);
        }
        private async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7172/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/Employees/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Employee>(responseData,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    throw new Exception("Failed to retrieve employees from API");
                }
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            var employee = await GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee); // Display confirmation view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var isSuccess = await DeleteEmployeeAsync(id);
            if (isSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete Employee.");
                return RedirectToAction(nameof(Index)); // Optionally, you can return to an error view
            }
        }

        private async Task<bool> DeleteEmployeeAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7172/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"api/Employees/{id}");
                return response.IsSuccessStatusCode;
            }
        }
        }
    }
