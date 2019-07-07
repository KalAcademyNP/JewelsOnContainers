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

        // GET api/Catalog/Items/type/1/brand/null[?pageSize=4&pageIndex=0]
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> Items(int? catalogTypeId,
            int? catalogBrandId,
            [FromQuery] int pageSize = 6,
            [FromQuery] int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>)_context.CatalogItems;

            if (catalogTypeId.HasValue)
            {
                root = root.Where(c => c.CatalogTypeId == catalogTypeId);
            }
            if (catalogBrandId.HasValue)
            {
                root = root.Where(c => c.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root
                              .LongCountAsync();
            var itemsOnPage = await root
                              .OrderBy(c => c.Name)
                              .Skip(pageSize * pageIndex)
                              .Take(pageSize)
                              .ToListAsync();
            itemsOnPage = ChangePictureUrl(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Count = totalItems,
                Data = itemsOnPage
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

        //GET api/Catalog/items/withname/Wonder?pageSize=2&pageIndex=0
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items(string name,
            [FromQuery] int pageSize = 6,
            [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _context.CatalogItems
                               .Where(c => c.Name.StartsWith(name))
                              .LongCountAsync();
            var itemsOnPage = await _context.CatalogItems
                              .Where(c => c.Name.StartsWith(name))
                              .OrderBy(c => c.Name)
                              .Skip(pageSize * pageIndex)
                              .Take(pageSize)
                              .ToListAsync();
            itemsOnPage = ChangePictureUrl(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItem>
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                Count = totalItems,
                Data = itemsOnPage
            };

            return Ok(model);

        }

        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CatalogItem product)
        {
            var item = new CatalogItem
            {
                CatalogBrandId = product.CatalogBrandId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                Name = product.Name,
                PictureUrl = product.PictureUrl,
                Price = product.Price
            };
            _context.CatalogItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemsById), new { id = item.Id });
        }


        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateProduct(
            [FromBody] CatalogItem productToUpdate)
        {
            var catalogItem = await _context.CatalogItems
                              .SingleOrDefaultAsync
                              (i => i.Id == productToUpdate.Id);
            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }
            catalogItem = productToUpdate;
            _context.CatalogItems.Update(catalogItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemsById), new { id = productToUpdate.Id });
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.CatalogItems
                .SingleOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();

            }
            _context.CatalogItems.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();

        }


    }
}