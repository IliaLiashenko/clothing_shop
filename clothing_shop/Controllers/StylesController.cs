using Shop_DataAccess;
using Shop_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_DataAccess.Repository.IRepository;

namespace clothing_shop.Controllers
{
    [Authorize(Roles = Shop_Utility.WC.AdminRole)]
    public class StylesController : Controller
    {
		private readonly ApplicationDbContext _context;

		public StylesController(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
        {
            return _context.Styles != null ?
                        View(await _context.Styles.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Style'  is null.");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Style style)
        {
            if (ModelState.IsValid)
            {
				_context.Add(style);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
            return View(style);
        }

        // GET: Styles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Styles == null)
            {
                return NotFound();
            }

            var style = await _context.Styles.FindAsync(id);
            if (style == null)
            {
                return NotFound();
            }
            return View(style);
        }

        // POST: Styles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Style style)
        {
            if (id != style.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					_context.Update(style);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StyleExists(style.Id))
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
            return View(style);
        }

        // GET: Styles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Styles == null)
            {
                return NotFound();
            }

            var style = await _context.Styles
				.FirstOrDefaultAsync(m => m.Id == id);
			if (style == null)
            {
                return NotFound();
            }

            return View(style);
        }

        // POST: Style/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Styles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Style'  is null.");
            }
            var style = await _context.Styles.FindAsync(id);
			if (style != null)
            {
				_context.Styles.Remove(style);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
        }

        private bool StyleExists(int id)
        {
			return (_context.Styles?.Any(e => e.Id == id)).GetValueOrDefault();
		}
    }
}
