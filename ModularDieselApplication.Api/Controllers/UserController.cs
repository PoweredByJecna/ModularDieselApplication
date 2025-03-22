using System.Xml.XPath;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Api.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;
        private readonly UserManager<TableUser> _userManager;
        public UserController(IUserService service, IMapper mapper, UserManager<TableUser> userManager)
        {
            _service = service;
            _mapper = mapper;
            _userManager = userManager;
        }        
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

        public async Task<IActionResult> ChangePassword(string UserId, string newpasssword)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newpasssword);
            if (result.Succeeded)
            {
                return Json(new
                {
                    success = true,
                    message = "Heslo bylo úspěšně změněno"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Něco se pokazilo"
                });
            }
        }

    }
}