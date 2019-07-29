using CoreMvc.Data;
using CoreMvc.Models;
using CoreMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvc.Controllers
{
    [Authorize(Roles = Constants.StockRole)]
    public class BrandModelController : BaseController<BrandModelController>
    {
        public BrandModelController(CoreMvcDbContext context, ILogger<BrandModelController> log) : base(context, log)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(MessageType messageType, string name, int p = 1)
        {
            //With this code, we get the message from Create,Edit,Delete functions and then if message is not empty, we show the message with Modal Dialog Popup
            //This Modal Dialog is in _Layout_Core_Mvc.cshtml Page. So i can use this Modal Dialog from all Pages.
            string message = Utils.GetMessage(messageType, "Model");
            if (message != "")
                ViewData["ModalMessage"] = message;

            name = name == null ? "" : name;
            ViewData["CurrentFilter"] = name;

            PaginatedList<BrandModel> list = new PaginatedList<BrandModel>();
            list._items = await _context.BrandModels.Where(m => m.Name.Contains(name)).ToListAsync();

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
            ViewData["BrandList"] = new SelectList(_context.Brands, "Id", "Name");

            return View("Create", new BrandModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brandModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.BrandModels.Add(brandModel);

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

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                BrandModel brandModel = await _context.BrandModels.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (brandModel == null)
                    return NotFound();

                ViewData["BrandList"] = new SelectList(_context.Brands, "Id", "Name");

                return View("Edit", brandModel);
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
        public async Task<IActionResult> Edit(BrandModel brandModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Attach(brandModel).State = EntityState.Modified;

                    //With this code, we save the entity history.
                    _context.EnsureAutoHistory();
                    await _context.SaveChangesAsync();

                    //With this code, we save in log files what we want.
                    _log.LogInformation("ID = {0}, Record Updated", brandModel.Id);

                    return RedirectToAction("Index", new { messageType = MessageType.Update });
                }
                catch (Exception ex)
                {
                    //With this code, we save in log files what we want.
                    _log.LogError(ex.Message + ". ID : {0}", brandModel.Id);
                    TempData["ErrorMessage"] = ex.Message;

                    return RedirectToAction("Error");
                }
            }

            return View();
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
                BrandModel brandModel = await _context.BrandModels.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (brandModel == null)
                    return NotFound();

                if (brandModel.Products != null && brandModel.Products.Count != 0)
                    throw new Exception("There are related Records with this Brand. First You have to delete them!");

                _context.BrandModels.Remove(brandModel);

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