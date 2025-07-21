using System.Xml.XPath;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using ModularDieselApplication.Domain.Objects;



namespace ModularDieselApplication.Api.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;
    

        public UserController(IUserService service )
        {
            _service = service;
        }

        // ----------------------------------------
        // Render the main view for Users.
        // ----------------------------------------
        public IActionResult Index()
        {
            return View();
        }

        // ----------------------------------------
        // Fetch user details as JSON.
        // ----------------------------------------
        public async Task<IActionResult> DetailUserJson(string id)
        {
            var detailUser = await _service.DetailUserJsonAsync(id);
            return Json(new { data = detailUser });
        }

        // ----------------------------------------
        // Fetch user relationships as JSON.
        // ----------------------------------------
        public async Task<IActionResult> VazbyJson(string id)
        {
            var vazby = await _service.VazbyJsonAsync(id);
            return Json(new { data = vazby });
        }

        // ----------------------------------------
        // Change the password for a user.
        // ----------------------------------------
        public async Task<IActionResult> ChangePassword(string UserId, string newpasssword)
        {
            var result = await _service.ChangePasswordAsync(UserId, newpasssword);
            return JsonResult(result);
        }

        // ----------------------------------------
        // Add a new user with a role.
        // ----------------------------------------
        public async Task<IActionResult> AddUser(string email, string password, string role, string username, string Jmeno, string Prijmeni)
        {
            var result = await _service.AddUserAsync(username, password, email, role, Jmeno, Prijmeni);
            return JsonResult(result);


        }
        private JsonResult JsonResult(HandleResult result)
        {
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message });
            }
        }

    }
}
