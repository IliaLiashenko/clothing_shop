using Shop_DataAccess;
using Shop_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace clothing_shop.Controllers
{
    [Authorize(Roles = Shop_Utility.WC.AdminRole)]
    public class ColorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColorsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return _context.Colors != null ?
                        View(await _context.Colors.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Colors'  is null.");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Colors colors)
        {
            if (ModelState.IsValid)
            {
                _context.Add(colors);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(colors);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var colors = await _context.Colors.FindAsync(id);
            if (colors == null)
            {
                return NotFound();
            }
            return View(colors);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Colors colors)
        {
            if (id != colors.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(colors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorsExists(colors.Id))
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
            return View(colors);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                return NotFound();
            }

            var colors = await _context.Colors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (colors == null)
            {
                return NotFound();
            }

            return View(colors);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Colors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Colors'  is null.");
            }
            var colors = await _context.Colors.FindAsync(id);
            if (colors != null)
            {
                _context.Colors.Remove(colors);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ColorsExists(int id)
        {
            return (_context.Colors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
