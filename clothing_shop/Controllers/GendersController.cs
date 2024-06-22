using Shop_DataAccess;
using Shop_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_DataAccess.Repository.IRepository;

namespace clothing_shop.Controllers
{
    [Authorize(Roles = Shop_Utility.WC.AdminRole)]
    public class GendersController : Controller
    {
		private readonly ApplicationDbContext _context;

		public GendersController(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
        {
            return _context.Genders != null ?
                        View(await _context.Genders.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Gender'  is null.");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Gender gender)
        {
            if (ModelState.IsValid)
            {
				_context.Add(gender);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
            return View(gender);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Genders == null)
            {
                return NotFound();
            }

            var gender = await _context.Genders.FindAsync(id);
            if (gender == null)
            {
                return NotFound();
            }
            return View(gender);
        }

        // POST: Genders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Gender gender)
        {
            if (id != gender.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					_context.Update(gender);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenderExists(gender.Id))
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
            return View(gender);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Genders == null)
            {
                return NotFound();
            }

            var gender = await _context.Genders
				.FirstOrDefaultAsync(m => m.Id == id);
			if (gender == null)
            {
                return NotFound();
            }

            return View(gender);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Genders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Gender'  is null.");
            }
            var gender = await _context.Genders.FindAsync(id);
			if (gender != null)
            {
				_context.Genders.Remove(gender);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
        }

        private bool GenderExists(int id)
        {
			return (_context.Genders?.Any(e => e.Id == id)).GetValueOrDefault();
		}
    }
}
