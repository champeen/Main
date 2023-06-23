using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class ProductLinesController : Controller
    {
        private readonly Management_of_ChangeContext _context;

        public ProductLinesController(Management_of_ChangeContext context)
        {
            _context = context;
        }

        // GET: ProductLines
        public async Task<IActionResult> Index()
        {
              return _context.ProductLine != null ? 
                          View(await _context.ProductLine.OrderBy(m => m.Order).ThenBy(m => m.Description).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.ProductLine'  is null.");
        }

        // GET: ProductLines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductLine == null)
                return NotFound();

            var productLine = await _context.ProductLine
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productLine == null)
                return NotFound();

            return View(productLine);
        }

        // GET: ProductLines/Create
        public IActionResult Create()
        {
            ProductLine productLine = new ProductLine
            {
                CreatedUser = "Michael Wilson",
                CreatedDate = DateTime.Now
            };

            return View(productLine);
        }

        // POST: ProductLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ProductLine productLine)
        {
            // Make sure duplicates are not entered...
            List<ProductLine> checkDupes = await _context.ProductLine
                .Where(m => m.Description == productLine.Description)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Description", "Product Line already exists.");
                return View(productLine);
            }

            if (ModelState.IsValid)
            {
                _context.Add(productLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productLine);
        }

        // GET: ProductLines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductLine == null)
                return NotFound();

            var productLine = await _context.ProductLine.FindAsync(id);

            if (productLine == null)
                return NotFound();
            return View(productLine);
        }

        // POST: ProductLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] ProductLine productLine)
        {
            if (id != productLine.Id)
                return NotFound();

            // Make sure duplicates are not entered...
            List<ProductLine> checkDupes = await _context.ProductLine
                .Where(m => m.Description == productLine.Description)
                .ToListAsync();
            if (checkDupes.Count > 0)
            {
                ModelState.AddModelError("Description", "Product Line already exists.");
                return View(productLine);
            }

            productLine.ModifiedUser = "Michael Wilson";
            productLine.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productLine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductLineExists(productLine.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productLine);
        }

        // GET: ProductLines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductLine == null)
                return NotFound();

            var productLine = await _context.ProductLine
                .FirstOrDefaultAsync(m => m.Id == id);

            if (productLine == null)
                return NotFound();

            return View(productLine);
        }

        // POST: ProductLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductLine == null)
                return Problem("Entity set 'Management_of_ChangeContext.ProductLine'  is null.");

            var productLine = await _context.ProductLine.FindAsync(id);

            if (productLine != null)
                _context.ProductLine.Remove(productLine);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductLineExists(int id)
        {
          return (_context.ProductLine?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
