using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Book Book { get; set; }
        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? Id)
        {
            Book = new Book();

            if (Id ==null)
            {
                //create
                return View(Book);
            }
            Book = _db.Books.FirstOrDefault(u => u.Id == Id);
            if(Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            

            if (ModelState.IsValid)
            {
                //create
                if(Book.Id == 0)
                {
                    _db.Books.Add(Book);

                }
                else
                {
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");

                
            }
            return View(Book);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookEntity = await _db.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (bookEntity == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.Remove(bookEntity);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion
    }
}