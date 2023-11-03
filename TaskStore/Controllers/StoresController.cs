using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskStore.Data;
using TaskStore.Models;
using Newtonsoft.Json;
using System.Text;

namespace TaskStore.Controllers
{
    public class StoresController : Controller
    {
        private readonly TaskStoreContext _context;

        public StoresController(TaskStoreContext context)
        {
            _context = context;
        }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
              return _context.Store != null ? 
                          View(await _context.Store.ToListAsync()) :
                          Problem("Entity set 'TaskStoreContext.Store'  is null.");
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Store == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,WorkingTime,Products")] Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
            }
            return View(store);
        }

        public async Task<IActionResult> Upload()
        {
            return _context.Store != null ?
                        View(await _context.Store.ToListAsync()) :
                        Problem("Entity set 'TaskStoreContext.Store'  is null.");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileName = Path.GetFileName(file.FileName);

                    if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {

                        var xmlString = reader.ReadToEnd();
                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xmlString);

                        var storeNodes = xmlDoc.SelectNodes("//store");

                        foreach (XmlNode storeNode in storeNodes)
                        {
                            var store = new Store
                            {
                                Name = storeNode.Attributes["name"].Value,
                                Address = storeNode.Attributes["address"].Value,
                                WorkingTime = storeNode.Attributes["workingTime"].Value
                            };

                            _context.Store.Add(store);
                        }

                        _context.SaveChanges();
                    }
                    else if (fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        // Обработка JSON
                        var jsonString = reader.ReadToEnd();
                        var stores = JsonConvert.DeserializeObject<List<Store>>(jsonString);

                        _context.Store.AddRange(stores);
                    }
                }

                _context.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportToCsv()
        {
            var stores = await _context.Store.ToListAsync();

            var builder = new StringBuilder();
            builder.AppendLine("Name,Address,WorkingTime");

            foreach (var store in stores)
            {
                builder.AppendLine($"{store.Name},{store.Address},{store.WorkingTime}");
            }

            var csvData = builder.ToString();

            byte[] buffer = Encoding.UTF8.GetBytes(csvData);
            return File(buffer, "text/csv", "Stores.csv");
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Store == null)
            {
                return NotFound();
            }

            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,WorkingTime")] Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Store == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Store == null)
            {
                return Problem("Entity set 'TaskStoreContext.Store'  is null.");
            }
            var store = await _context.Store.FindAsync(id);
            if (store != null)
            {
                _context.Store.Remove(store);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id)
        {
          return (_context.Store?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
