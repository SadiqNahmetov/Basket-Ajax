using EmbryoFrontToBack.Data;
using EmbryoFrontToBack.Models;
using EmbryoFrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmbryoFrontToBack.Controllers
{

    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketVM> basketItems = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            List<BasketDetailVM> basketDetail = new List<BasketDetailVM>();
            
            foreach (var item in basketItems)
            {
                Accessories accessories = await _context.Accessories
                    .Where(m => m.Id == item.Id && m.IsDeleted == false).FirstOrDefaultAsync();


                BasketDetailVM newBasket = new BasketDetailVM
                {
                    Title = accessories.Title,
                    Image = accessories.Image,
                    Price = accessories.Price,
                    Count = item.Count,
                    Total =accessories.Price * item.Count
                };

                basketDetail.Add(newBasket);
            }
            return View(basketDetail);
        }
    }
}
