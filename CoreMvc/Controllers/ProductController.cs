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
    public class ProductController : BaseController<ProductController>
    {
        public ProductController(CoreMvcDbContext context, ILogger<ProductController> log) : base(context, log)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(MessageType messageType, int p_brand_Id, int p_brandModel_Id, int p = 1)
        {
            ViewBag.BrandList = new SelectList(_context.Brands, "Id", "Name");

            //With this code, we get the message from Create,Edit,Delete functions and then if message is not empty, we show the message with Modal Dialog Popup
            //This Modal Dialog is in _Layout_Core_Mvc.cshtml Page. So i can use this Modal Dialog from all Pages.
            string message = Utils.GetMessage(messageType, "Product");
            if (message != "")
                ViewData["ModalMessage"] = message;

            ViewData["CurrentFilterBrandId"] = p_brand_Id;
            ViewData["CurrentFilterBrandModelId"] = p_brandModel_Id;

            PaginatedList<Product> list = new PaginatedList<Product>();
            list._items = await _context.Products.Where(m => (m.Brand_Id == p_brand_Id || p_brand_Id == 0) && (m.BrandModel_Id == p_brandModel_Id || p_brandModel_Id == 0)).ToListAsync();

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
            ViewBag.BrandList = new SelectList(_context.Brands, "Id", "Name");

            return View("Create", new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Products.Add(product);

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

            ViewBag.BrandList = new SelectList(_context.Brands, "Id", "Name");
            product.Brand_Id = 0;
            product.BrandModel_Id = 0;

            return View(product);
        }

        /// <summary>
        /// This method is called by Ajax. Returns Model List according to Brand
        /// </summary>
        /// <param name="brand_id"></param>
        /// <returns></returns>
        public JsonResult GetModelList(int brand_id)
        {
            var result = new SelectList(_context.BrandModels.Where(m => m.Brand_Id == brand_id), "Id", "Name");
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                Product product = await _context.Products.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (product == null)
                    return NotFound();

                ViewBag.BrandList = new SelectList(_context.Brands, "Id", "Name");

                return View("Edit", product);
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
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Attach(product).State = EntityState.Modified;

                    //With this code, we save the entity history.
                    _context.EnsureAutoHistory();
                    await _context.SaveChangesAsync();

                    //With this code, we save in log files what we want.
                    _log.LogInformation("ID = {0}, Record Updated", product.Id);

                    return RedirectToAction("Index", new { messageType = MessageType.Update });
                }
                catch (Exception ex)
                {
                    //With this code, we save in log files what we want.
                    _log.LogError(ex.Message + ". ID : {0}", product.Id);
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
                Product product = await _context.Products.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (product == null)
                    return NotFound();

                _context.Products.Remove(product);

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