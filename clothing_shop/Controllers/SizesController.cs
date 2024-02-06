using Shop_DataAccess;
using Shop_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_DataAccess.Repository.IRepository;

namespace clothing_shop.Controllers
{
    [Authorize(Roles = Shop_Utility.WC.AdminRole)]
    public class SizesController : Controller
    {
        private readonly ISizeRepository _sizeRepo;

        public SizesController(ISizeRepository sizeRepo)
        {
           _sizeRepo = sizeRepo;
        }
        public async Task<IActionResult> Index()
        {
            return _sizeRepo != null ?
                        View(await _sizeRepo.GetAllSizes().ToListAsync()) :
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
                _sizeRepo.Add(size);
                await _sizeRepo.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(size);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _sizeRepo == null)
            {
                return NotFound();
            }

            var size = _sizeRepo.Find(id);
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
                    _sizeRepo.Update(size);
                    await _sizeRepo.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SizeExistsAsync(size.Id))
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
            if (id == null || _sizeRepo == null)
            {
                return NotFound();
            }

            var size = _sizeRepo
                .FirstOrDefault(m => m.Id == id);
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
            if (_sizeRepo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Size'  is null.");
            }
            var size = _sizeRepo.Find(id);
            if (size != null)
            {
                _sizeRepo.Remove(size);
            }

            await _sizeRepo.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> SizeExistsAsync(int id)
        {
            return await _sizeRepo.AnyAsync(e => e.Id == id);
        }
    }
}
