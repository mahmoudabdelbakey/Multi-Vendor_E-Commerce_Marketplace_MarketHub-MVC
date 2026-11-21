using MarketHub.BLL.Interfaces;
using MarketHub.BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketHub.MVC.Controllers
{
    [AllowAnonymous]
    [Route("Products")]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] ProductQuery query)
        {
            var result = await _catalogService.SearchAsync(query);
            ViewBag.Categories = await _catalogService.GetActiveCategoriesAsync();

            return View(result);
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _catalogService.GetDetailsAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}