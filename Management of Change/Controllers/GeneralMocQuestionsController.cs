using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Management_of_Change.Data;
using Management_of_Change.Models;
using Management_of_Change.Utilities;
//using Management_of_Change.Migrations;

namespace Management_of_Change.Controllers
{
    public class GeneralMocQuestionsController : BaseController
    {
        private readonly Management_of_ChangeContext _context;

        public GeneralMocQuestionsController(Management_of_ChangeContext context) : base(context)
        {
            _context = context;
        }

        // GET: GeneralMocQuestions
        public async Task<IActionResult> Index()
        {
              return _context.GeneralMocQuestions != null ? 
                          View(await _context.GeneralMocQuestions.OrderBy(m => m.Order).ThenBy(m => m.Question).ToListAsync()) :
                          Problem("Entity set 'Management_of_ChangeContext.GeneralMocQuestions'  is null.");
        }

        // GET: GeneralMocQuestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GeneralMocQuestions == null)
                return NotFound();

            var generalMocQuestions = await _context.GeneralMocQuestions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (generalMocQuestions == null)
                return NotFound();

            return View(generalMocQuestions);
        }

        // GET: GeneralMocQuestions/Create
        public IActionResult Create()
        {
            GeneralMocQuestions generalMocQuestions = new GeneralMocQuestions
            {
                CreatedUser = _username,
                CreatedDate = DateTime.UtcNow
            };

            return View(generalMocQuestions);
        }

        // POST: GeneralMocQuestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] GeneralMocQuestions generalMocQuestions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(generalMocQuestions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(generalMocQuestions);
        }

        // GET: GeneralMocQuestions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GeneralMocQuestions == null)
                return NotFound();

            var generalMocQuestions = await _context.GeneralMocQuestions.FindAsync(id);

            if (generalMocQuestions == null)
                return NotFound();

            return View(generalMocQuestions);
        }

        // POST: GeneralMocQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,Order,CreatedUser,CreatedDate,ModifiedUser,ModifiedDate,DeletedUser,DeletedDate")] GeneralMocQuestions generalMocQuestions)
        {
            if (id != generalMocQuestions.Id)
                return NotFound();

            generalMocQuestions.ModifiedUser = _username;
            generalMocQuestions.ModifiedDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(generalMocQuestions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneralMocQuestionsExists(generalMocQuestions.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(generalMocQuestions);
        }

        // GET: GeneralMocQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GeneralMocQuestions == null)
                return NotFound();

            var generalMocQuestions = await _context.GeneralMocQuestions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (generalMocQuestions == null)
                return NotFound();

            return View(generalMocQuestions);
        }

        // POST: GeneralMocQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GeneralMocQuestions == null)
                return Problem("Entity set 'Management_of_ChangeContext.GeneralMocQuestions'  is null.");

            var generalMocQuestions = await _context.GeneralMocQuestions.FindAsync(id);
            if (generalMocQuestions != null)
                 _context.GeneralMocQuestions.Remove(generalMocQuestions);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneralMocQuestionsExists(int id)
        {
          return (_context.GeneralMocQuestions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
