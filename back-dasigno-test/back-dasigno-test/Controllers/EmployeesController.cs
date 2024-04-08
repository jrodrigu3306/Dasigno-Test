using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using back_dasigno_test.Data;
using back_dasigno_test.Model;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace back_dasigno_test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly DataBaseContext _context;
        private readonly IConfiguration _configuration;

        public EmployeesController(DataBaseContext context , IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("getEmployees")]
        public async Task<ActionResult<ResutModel<Employee>>> GetEmployees(string? filter, string? filterType, int? page, int? itemsPerPage)
        {
            ResutModel<Employee> response = new ResutModel<Employee>();
            try
            {
                IQueryable<Employee> query = _context.Employees.OrderBy(e => e.EmployeeId);
                var employees = new List<Employee>();

                if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(filterType))
                {
                    if (filterType.ToLower() == "id") {
                        employees = new List<Employee>() { query.FirstOrDefault(e => e.EmployeeId == int.Parse(filter)) };
                        if (employees[0] == null) 
                        {
                            response = response.getResultModel(true);
                            response.Message = "El empleado no existe, no se encuentra el ID.";
                            Exception ex = new Exception(response.Message);
                            await saveLogAsync(ex);
                            return response;
                        } else {
                            return response.getResultModel(false, employees);
                        }
                    }
                    if (filterType.ToLower() == "firstname") query = query.Where(e => e.FirstName.Contains(filter)).OrderBy(e => e.FirstName);
                    else if (filterType.ToLower() == "surname") query = query.Where(e => e.Surname.ToString().Contains(filter)).OrderBy(e => e.Surname);
                    else
                    {
                        response = response.getResultModel(true);
                        response.Message = "No se puede filtrar por el tipo de filtro '"+filterType+ "', intenta filtrar por 'id' 'firstname' o 'surname'";
                        Exception ex = new Exception(response.Message);
                        await saveLogAsync(ex);
                        return response;
                    }
                }

                if(employees.Count == 0) employees = await query.ToListAsync();

                var TotalPages = 1;
                if (page != null && itemsPerPage != null)
                {
                    TotalPages = (int)((employees.Count % itemsPerPage == 0) ? employees.Count / itemsPerPage : (employees.Count / itemsPerPage) + 1);
                    if (page > TotalPages)
                    {
                        response = response.getResultModel(true);
                        response.Message = "La página " + page + " se encuentra fuera del rango de páginas.";                     
                        Exception ex = new Exception(response.Message);
                        await saveLogAsync(ex);
                    }
                    else
                    {
                        employees = employees.Skip((int)((page - 1) * itemsPerPage)).Take((int)itemsPerPage).ToList();
                        response = response.getResultModel(false, employees);                       
                    }
                    response.Page = page;
                    response.ItemsPerPage = itemsPerPage;
                    response.TotalPages = TotalPages;
                    return response;
                }

                response = response.getResultModel(false, employees);
                response.ItemsPerPage= response.TotalItems;
                return response;
            }
            catch (Exception ex)
            {
                response = response.getResultModel(true);
                response.Message = ex.Message;
                await saveLogAsync(ex);
                return response;
            }
        }

        [HttpGet("getEmployee/{id}")]
        public async Task<ActionResult<ResutModel<Employee>>> GetEmployees(int? id)
        {
            ResutModel<Employee> response = new ResutModel<Employee>();

            if (id == null)
            {
                response = response.getResultModel(true);
                response.Message = "El 'id' is nulo";
                return response;
            }

            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeId == id);
                if (employee == null)
                {
                    response = response.getResultModel(true);
                    response.Message = "No se encontro el empleado";
                    return response;
                }
                return response.getResultModel(false, new List<Employee> { employee });
            }
            catch (Exception ex)
            {
                response = response.getResultModel(true);
                response.Message = ex.Message;
                await saveLogAsync(ex);
                return response;
            }
        }


        [HttpPost("setEmployee")]
        public async Task<ActionResult<ResutModel<Employee>>> SetEmployee(Employee employee)
        {
            ResutModel<Employee> response = new ResutModel<Employee>();
            try
            {
                var res = CheckModel(employee);
                if (res == null)
                {
                    employee.SecondName = (employee.SecondName == "")? null: employee.SecondName;
                    employee.SecondSurname = (employee.SecondSurname == "") ? null : employee.SecondSurname;
                    employee.InsertDate = DateTime.Now;
                    employee.UpdateDate = DateTime.Now;
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return response.getResultModel(false, new List<Employee> { employee });
                }
                else
                {
                    response = response.getResultModel(true);
                    response.Message = res;
                    Exception ex = new Exception(response.Message);
                    await saveLogAsync(ex);
                    return response;
                }
            }
            catch (Exception ex)
            {
                response = response.getResultModel(true);
                response.Message = ex.Message;
                await saveLogAsync(ex);
                return response;
            }
        }

        [HttpPut("putEmployee")]
        public async Task<ActionResult<ResutModel<Employee>>> PutEmployee(EmployeeUpdateModel employeeUpdate)
        {
            ResutModel<Employee> response = new ResutModel<Employee>();

            try
            {
                Employee employee = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeId == employeeUpdate.EmployeeId);

                if (employee != null)
                {                    
                    employee.FirstName = (!string.IsNullOrEmpty(employeeUpdate.FirstName)) ? employeeUpdate.FirstName : employee.FirstName;
                    employee.Surname = (!string.IsNullOrEmpty(employeeUpdate.Surname)) ? employeeUpdate.Surname : employee.Surname;
                    employee.SecondName = (employeeUpdate.SecondName!=null) ? ((employeeUpdate.SecondName == "") ? null: employeeUpdate.SecondName) : employee.SecondName;
                    employee.SecondSurname = (employeeUpdate.SecondSurname != null) ? ((employeeUpdate.SecondSurname == "") ? null : employeeUpdate.SecondSurname) : employee.SecondSurname;                    
                    employee.Birthdate = (DateOnly)((!string.IsNullOrEmpty(employeeUpdate.Birthdate.ToString())) ? employeeUpdate.Birthdate : employee.Birthdate);
                    employee.Salary = (double)((!string.IsNullOrEmpty(employeeUpdate.Salary.ToString())) ? employeeUpdate.Salary : employee.Salary);
                    employee.InsertDate = (!string.IsNullOrEmpty(employeeUpdate.InsertDate.ToString())) ? employeeUpdate.InsertDate : employee.InsertDate;
                    employee.UpdateDate = DateTime.Now;

                    var res = CheckModel(employee);
                    if (res == null)
                    {
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                        return response.getResultModel(false, new List<Employee> { employee });
                    } else
                    {
                        response = response.getResultModel(true);
                        response.Message = res;
                        Exception ex = new Exception(response.Message);
                        await saveLogAsync(ex);
                        return response;
                    }
                } else
                {
                    response = response.getResultModel(true);
                    response.Message = "El empleado no existe, no se encuentra el ID.";
                    Exception ex = new Exception(response.Message);
                    await saveLogAsync(ex);
                    return response;
                }

            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!EmployeeExists(employeeUpdate.EmployeeId))
                {
                    response = response.getResultModel(true);
                    response.Message = "El empleado no existe";
                    await saveLogAsync(ex);
                    return response;
                }
                else
                {
                    response = response.getResultModel(true);
                    response.Message = ex.Message;
                    await saveLogAsync(ex);
                    return response;
                }
            }
        }

        [HttpDelete("deleteEmployee/{id}")]
        public async Task<ActionResult<ResutModel<Employee>>> DeleteEmployee(int id)
        {
            ResutModel<Employee> response = new ResutModel<Employee>();

            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                    response = response.getResultModel(false, new List<Employee>());
                    response.ItemsPerPage = 0;
                    response.TotalItems = 0;
                    response.TotalPages = 0;
                    response.Page = 0;
                    return response;
                } 
                else
                {
                    response = response.getResultModel(true);
                    response.Message = "El empleado no existe, no se encuentra el ID.";
                    Exception ex = new Exception(response.Message);
                    await saveLogAsync(ex);
                    return response;
                }
            }
            catch (Exception ex)
            {
                response = response.getResultModel(true);
                response.Message = ex.Message;
                await saveLogAsync(ex);
                return response;
            }
        }

        private async Task saveLogAsync(Exception ex)
        {
            ErrorLog err = new ErrorLog().CreateErrorLogFromException(ex);
            string connectionString = _configuration.GetConnectionString("SQLConnection");

            var options = new DbContextOptionsBuilder<DataBaseContext>()
                        .UseSqlServer(new SqlConnection(connectionString))
                        .Options;

            using (var dbContext = new DataBaseContext(options))
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        dbContext.ErrorLogs.Add(err);
                        await dbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

        }

        private bool HasNumber(string str)
        {
            //return (int.TryParse(str, out _));
            if (str != null)
            {
                Regex regex = new Regex(@"\d");
                return regex.IsMatch(str);
            } else
            {
                return false;
            }
        }

        private string CheckModel(Employee model)
        {
            string message = null;
            if (model.Salary <= 0) message = "El salario es igual o menor a cero.";
            else if (HasNumber(model.FirstName)) message = "El primer nombre es un número o tiene números.";
            else if (HasNumber(model.Surname)) message = "El primer apellido es un número o tiene números.";
            else if (HasNumber(model.SecondName)) message = "El segundo nombre es un número o tiene números.";
            else if (HasNumber(model.SecondSurname)) message = "El segundo apellido es un número o tiene números.";
            else if (model.FirstName == "") message = "El primer nombre no puede estar vacio.";
            else if (model.Surname == "") message = "El primer apellido no puede estar vacio.";
            else if (model.Birthdate.Year == 1) message = "La fecha es obligatoria.";
            return message;
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

    }
}
