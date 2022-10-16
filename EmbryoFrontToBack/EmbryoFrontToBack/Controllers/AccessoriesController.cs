using EmbryoFrontToBack.Data;
using EmbryoFrontToBack.Models;
using EmbryoFrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmbryoFrontToBack.Controllers
{
    public class AccessoriesController : Controller
    {
        private readonly AppDbContext _context;

        public AccessoriesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

                        
            ViewBag.count = await _context.Accessories.Where(m=>!m.IsDeleted).CountAsync();

            IEnumerable<Accessories> accessories = await _context.Accessories.Where(m=>!m.IsDeleted).Take(3).OrderBy(m=>m.Id).ToListAsync();

            AccessoriesVM accessoriesVM = new AccessoriesVM
            {
               Accessories = accessories
            };
            return View(accessoriesVM);
        }

        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Accessories> accessories = await _context.Accessories.Where(m=>!m.IsDeleted).Skip(skip).Take(3).ToListAsync();
            return PartialView("_ProductPartial", accessories);
        }

      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBasket(int? id)
         {
            if (id is null) return BadRequest();
            
            var dbAccesories = await GetAccessoriesById(id);

            if (dbAccesories == null) return NotFound();
                          
            List<BasketVM> basket = GetBasket();
            UpdateBasket(basket, dbAccesories.Id);

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            //return RedirectToAction(nameof(Index));
            return Json(new { success = true, responseText = "The attached file is not supported." });

        }


        private void UpdateBasket(List<BasketVM> basket, int id)
        {
            BasketVM existProduct = basket.FirstOrDefault(m => m.Id == id);

            if (existProduct == null)
            {
                basket.Add(new BasketVM
                {
                    Id = id,
                    Count = 1
                });
            }
            else
            {
                existProduct.Count++;
            }

        }

        private async Task<Accessories> GetAccessoriesById(int? id)
        {
            return await _context.Accessories.FindAsync(id);
        }

        private List<BasketVM> GetBasket()
        {

            List<BasketVM> basket;

            if (Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }
            return basket;
        }
    }
}
