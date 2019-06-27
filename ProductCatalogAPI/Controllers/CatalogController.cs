using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductCatalogAPI.Data;
using ProductCatalogAPI.Domain;
using ProductCatalogAPI.ViewModels;

namespace ProductCatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;
        private readonly IConfiguration _config;
        public CatalogController(CatalogContext context, 
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET api/catalog/items?pageSize=10&pageIndex=2
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items(
            [FromQuery]int pageSize = 6,
            [FromQuery]int pageIndex = 0)
        {
            var itemsCount = await _context.CatalogItems.LongCountAsync();

            var items = await _context.CatalogItems
                 .OrderBy(c => c.Name)
                 .Skip(pageSize * pageIndex)
                 .Take(pageSize)
                 .ToListAsync();
            items = ChangePictureUrl(items);
            var model = new PaginatedItemsViewModel<CatalogItem>
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Count = itemsCount,
                Data = items
            };
            return Ok(model);
        }

        private List<CatalogItem> ChangePictureUrl(
            List<CatalogItem> items)
        {
            items.ForEach(
                c =>
                c.PictureUrl =
                 c.PictureUrl
                 .Replace("http://externalcatalogbaseurltobereplaced"
                 , _config["ExternalCatalogBaseUrl"]));

            return items;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var items = await _context.CatalogTypes.ToListAsync();
            return Ok(items);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrands()
        {
            var items = await _context.CatalogBrands.ToListAsync();
            return Ok(items);
        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemsById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Incorrect Id!");
            }

            var item = await _context.CatalogItems
                            .SingleOrDefaultAsync(c => c.Id == id);


            if (item == null)
            {
                return NotFound("Catalog item not found");
            }

            item.PictureUrl = item.PictureUrl
                 .Replace("http://externalcatalogbaseurltobereplaced"
                 , _config["ExternalCatalogBaseUrl"]);
            return Ok(item);
        }
    }
}