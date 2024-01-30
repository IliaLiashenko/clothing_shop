using Shop_DataAccess;
using Shop_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace clothing_shop.Controllers
{
    [Authorize(Roles = Shop_Utility.WC.AdminRole)]
    public class SizesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SizesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return _context.Size != null ?
                        View(await _context.Size.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Size'  is null.");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Size size)
        {
            if (ModelState.IsValid)
            {
                _context.Add(size);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(size);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Size == null)
            {
                return NotFound();
            }

            var size = await _context.Size.FindAsync(id);
            if (size == null)
            {
                return NotFound();
            }
            return View(size);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Size size)
        {
            if (id != size.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(size);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SizeExists(size.Id))
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
            return View(size);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Size == null)
            {
                return NotFound();
            }

            var size = await _context.Size
                .FirstOrDefaultAsync(m => m.Id == id);
            if (size == null)
            {
                return NotFound();
            }

            return View(size);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Size == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Size'  is null.");
            }
            var size = await _context.Size.FindAsync(id);
            if (size != null)
            {
                _context.Size.Remove(size);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SizeExists(int id)
        {
            return (_context.Size?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
