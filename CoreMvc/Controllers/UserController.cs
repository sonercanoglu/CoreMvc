using CoreMvc.Data;
using CoreMvc.Models;
using CoreMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvc.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class UserController : BaseController<UserController>
    {
        public UserController(CoreMvcDbContext context, ILogger<UserController> log) : base(context, log)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(MessageType messageType, string name, int p = 1)
        {
            //With this code, we get the message from Create,Edit,Delete functions and then if message is not empty, we show the message with Modal Dialog Popup
            //This Modal Dialog is in _Layout_Core_Mvc.cshtml Page. So i can use this Modal Dialog from all Pages.
            string message = Utils.GetMessage(messageType, "User");
            if (message != "")
                ViewData["ModalMessage"] = message;

            name = name == null ? "" : name;
            ViewData["CurrentFilter"] = name;

            PaginatedList<User> list = new PaginatedList<User>();
            list._items = await _context.Users.Where(m => m.Username.Contains(name)).ToListAsync();
            list._TotalRecords = list._items.Count;

            if (list._TotalRecords >= ((p - 1) * list._PageSize))
                list._PageIndex = p;
            else
                list._PageIndex = 1;

            list._items = list._items.OrderBy(m => m.Id).Skip((list._PageIndex - 1) * list._PageSize).Take(list._PageSize).ToList();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            FillList();

            UserView userView = new UserView();
            userView = AddDefaultUserViewRoles(userView);
            return View("Create", userView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserView userView)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user = new User();
                    user.Employee_Id = userView.Employee_Id;
                    user.Password = userView.Password;
                    user.Username = userView.Username;

                    user.UserRoles = new List<UserRole>();

                    foreach (UserRoleView userRoleView in userView.UserRoleViews)
                    {
                        if (userRoleView.Selected)
                            user.UserRoles.Add(new UserRole { Role_Id = userRoleView.Id, User = user });
                    }

                    _context.Users.Add(user);

                    //With this code, we save the entity history.
                    _context.EnsureAutoHistory();
                    await _context.SaveChangesAsync();

                    //With this code, we save in log files what we want.
                    _log.LogInformation("New Record Added");
                    return RedirectToAction("Index", new { messageType = MessageType.Add });
                }
                catch (Exception ex)
                {
                    //With this code, we save in log files what we want.
                    _log.LogError(ex.Message);
                    TempData["ErrorMessage"] = ex.Message;

                    return RedirectToAction("Error");
                }
            }

            FillList();
            userView.Employee_Id = 0;

            return View(userView);
        }

        /// <summary>
        /// Get All Employee List, who is not an User
        /// Get Role List
        /// </summary>
        public void FillList()
        {
            var list = _context.Employees.Where(m => m.User == null).Select(m => new { Id = m.Id, Text = m.Name + " " + m.Surname }).ToList();

            ViewBag.EmployeeList = new SelectList(list, "Id", "Text");
            ViewBag.RoleList = _context.Roles.ToList();
        }

        /// <summary>
        /// Fill Role List in to the UserRoleViews
        /// </summary>
        /// <param name="userView"></param>
        /// <returns></returns>
        public UserView AddDefaultUserViewRoles(UserView userView)
        {
            userView.UserRoleViews = new List<UserRoleView>();
            foreach (Role r in ViewBag.RoleList)
                userView.UserRoleViews.Add(new UserRoleView { Id = r.Id, Name = r.Name, Selected = false, UserRole_Id = 0 });

            return userView;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                User user = await _context.Users.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound();

                ViewBag.RoleList = _context.Roles.ToList();

                UserView userView = new UserView();
                userView = AddDefaultUserViewRoles(userView);
                userView.Id = user.Id;
                userView.Username = user.Username;
                userView.Password = user.Password;
                userView.Employee_Id = user.Employee_Id;
                userView.EmployeeName = user.Employee.Name + " " + user.Employee.Surname;

                UserRoleView userRoleView;
                foreach (UserRole ur in user.UserRoles)
                {
                    userRoleView = userView.UserRoleViews.Where(m => m.Id == ur.Role_Id).FirstOrDefault();
                    if (userRoleView != null)
                    {
                        userRoleView.Selected = true;
                        userRoleView.UserRole_Id = ur.Id;
                    }
                }

                return View("Edit", userView);
            }
            catch (Exception ex)
            {
                //With this code, we save in log files what we want.
                _log.LogError(ex.Message + ". ID : {0}", id);
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserView userView)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user = await _context.Users.Where(m => m.Id == userView.Id).FirstOrDefaultAsync();
                    if (user == null)
                        return NotFound();

                    user.Password = userView.Password;
                    user.Username = userView.Username;

                    user.UserRoles.Where(m => userView.UserRoleViews.Any(o => o.UserRole_Id == m.Id && o.Selected == false)).ToList()
                        .ForEach(d => _context.UserRoles.Remove(d));

                    userView.UserRoleViews.Where(m => m.UserRole_Id == 0 && m.Selected).ToList()
                        .ForEach(a => user.UserRoles.Add(new UserRole { Role_Id = a.Id, User = user }));

                    _context.Attach(user).State = EntityState.Modified;

                    //With this code, we save the entity history.
                    _context.EnsureAutoHistory();
                    await _context.SaveChangesAsync();

                    //With this code, we save in log files what we want.
                    _log.LogInformation("ID = {0}, Record Updated", user.Id);

                    return RedirectToAction("Index", new { messageType = MessageType.Update });
                }
                catch (Exception ex)
                {
                    //With this code, we save in log files what we want.
                    _log.LogError(ex.Message + ". ID : {0}", userView.Id);
                    TempData["ErrorMessage"] = ex.Message;

                    return RedirectToAction("Error");
                }
            }

            return View(userView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string deleteId)
        {
            int id = 0;
            if (!int.TryParse(deleteId, out id))
                throw new Exception("Wrong Id Information.");

            try
            {
                User user = await _context.Users.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound();

                if (user.UserRoles != null)
                    user.UserRoles.ToList()
                        .ForEach(m => _context.UserRoles.Remove(m));

                _context.Users.Remove(user);

                //With this code, we save the entity history.
                _context.EnsureAutoHistory();
                await _context.SaveChangesAsync();

                //With this code, we save in log files what we want.
                _log.LogInformation("Record Deleted");

                return RedirectToAction("Index", new { messageType = MessageType.Delete });
            }
            catch (Exception ex)
            {
                //With this code, we save in log files what we want.
                _log.LogError(ex.Message + ". ID : {0}", id);
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}