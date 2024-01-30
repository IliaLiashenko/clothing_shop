using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_DataAccess;
using Shop_Models;
using Microsoft.AspNetCore.Authorization;
using Shop_DataAccess.Repository.IRepository;
using dotless.Core.Parser.Infrastructure;

namespace clothing_shop.Controllers
{
    [Authorize(Roles = Shop_Utility.WC.AdminRole)]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _catRepo;

        public CategoriesController(ICategoryRepository catRepo)
        {
            _catRepo = catRepo;
        }

        // GET: Categories

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> objList = _catRepo.GetAll();
            return View(objList);
        }

        //// GET: Categories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Category == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Category
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Add(obj);
                await _catRepo.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _catRepo == null)
            {
                return NotFound();
            }

            var obj = _catRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category obj)
        {
            if (id != obj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _catRepo.Update(obj);
                    await _catRepo.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CategoryExistsAsync(obj.Id))
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
            return View(obj);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _catRepo == null)
            {
                return NotFound();
            }

            var obj = _catRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_catRepo == null)
            {
                return NotFound();
            }
            var obj = _catRepo.Find(id);
            if (obj != null)
            {
                _catRepo.Remove(obj);
            }
            
            await _catRepo.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExistsAsync(int id)
        {
            return await _catRepo.AnyAsync(e => e.Id == id);
        }
    }
}
