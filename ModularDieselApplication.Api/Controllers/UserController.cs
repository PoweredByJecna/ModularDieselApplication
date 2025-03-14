using System.Xml.XPath;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Api.Controllers
{
    public class UserController(IUserService service) : Controller
    {
        private readonly IUserService _service = service;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DetailUserJson(string id)
        {
            var detailUser = await _service.DetailUserJsonAsync(id);
            return Json(new
            {
                data=detailUser
            });
        }

        public async Task<IActionResult> VazbyJson(string id)
        {
            var vazby = await _service.VazbyJsonAsync(id);
            return Json(new
            {
                data=vazby
            });
        }
    }
}