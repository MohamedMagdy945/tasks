using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherForecasts.API.Data;
using WeatherForecasts.API.Models;

namespace WeatherForecasts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<Employee>> Get()
        {
            return await _context.Employees.ToListAsync();
        }


    }
}
