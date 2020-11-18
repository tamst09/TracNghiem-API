using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TN.Business.Catalog.Interface;
using TN.Data.DataContext;
using TN.Data.Entities;

namespace TN.App.Controllers
{
    public class ExamsController : Controller
    {
        private readonly TNDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IManageExamService _examManager;

        public ExamsController(TNDbContext context,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IManageExamService examManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _examManager = examManager;
        }

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            var r = await _examManager.GetAll();
            
            //var tNDbContext = _context.Exams.Include(e => e.Category).Include(e => e.Owner).OrderBy(e=>e.ExamName);
            return View(r);
        }

        // GET: Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // GET: Exams/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "CategoryName");
            ViewData["OwnerID"] = _userManager.GetUserId(_signInManager.Context.User);
            ViewData["OwnerUsername"] = _userManager.GetUserName(_signInManager.Context.User);
            return View();
        }

        // POST: Exams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExamName,isPrivate,Time,ImageURL,CategoryID,OwnerID")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                //exam.OwnerID = Convert.ToInt32(_userManager.GetUserId(_signInManager.Context.User));
                //_context.Add(exam);
                //await _context.SaveChangesAsync();
                await _examManager.Create(exam, Convert.ToInt32(_userManager.GetUserId(_signInManager.Context.User)));
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "CategoryName", exam.CategoryID);
            ViewData["OwnerID"] = _userManager.GetUserId(_signInManager.Context.User);
            return View(exam);
        }

        // GET: Exams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }
            else
            {
                if(exam.OwnerID.ToString() != _userManager.GetUserId(_signInManager.Context.User))
                {
                    return Redirect("/Identity/Account/AccessDenied");
                }
                else
                {
                    ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "CategoryName", exam.CategoryID);
                    ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", exam.OwnerID);
                    return View(exam);
                }
            }
        }

        // POST: Exams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ExamName,isPrivate,Time,ImageURL,TimeCreated,NumOfAttemps,CategoryID,OwnerID")] Exam exam)
        {
            if (id != exam.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.ID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "CategoryName", exam.CategoryID);
            ViewData["OwnerID"] = new SelectList(_context.Users, "Id", "Id", exam.OwnerID);
            return View(exam);
        }
        // GET: Exams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.ID == id);
        }
    }
}
