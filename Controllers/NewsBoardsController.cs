using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;

// Controller object and methods specific to the newsboard class
namespace Lab4.Controllers
{
    public class NewsBoardsController : Controller
    {
        private readonly NewsDbContext _context;

        public NewsBoardsController(NewsDbContext context)
        {
            _context = context;
        }

        // GET: NewsBoards
        public async Task<IActionResult> Index(string ID)
        {
            // build a new NewBoardViewModel with the context of the db 
            var viewModel = new NewsBoardViewModel
            {
                // sets the context for the newsboard object and then includes other 
                // list structures as sub objects, leaving the parents null for selection 
                // logic to display information
                NewsBoards = await _context.NewsBoards
                .Include(i => i.Subscriptions)
                .ThenInclude(t => t.Client)
                .ToListAsync(),
            };

            // if the ID does not exist, the index view is returned with no listing details displayed for 
            // newsboard customer. If Id is passed into method, logic for specific customer information is 
            // returned in viewmodel for user display on index page
            if (ID != null)
            {
                ViewData["NewsBoardId"] = ID;
                // include specific object from id ref to new variable
                var newsBoard = viewModel.NewsBoards.SingleOrDefault(i => i.Id == ID);
                if (newsBoard != null)
                {
                    // set the clients list field to the new object where the fields are found
                    viewModel.Clients = newsBoard.Subscriptions.Select(t => t.Client);
                }
            }

            return View(viewModel);
        }

        // GET: NewsBoards/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.NewsBoards == null)
            {
                return NotFound();
            }

            var newsBoard = await _context.NewsBoards
                .FirstOrDefaultAsync(m => m.Id == id);
                

            if (newsBoard == null)
            {
                return NotFound();
            }

            return View(newsBoard);

        }

        /// <summary>
        /// Redirect to News controller Index page using newsboard Id
        /// </summary>
        /// <param name="id"> Id value of Newsboard being navigated from</param>
        /// <returns></returns>
        public IActionResult News(string id)
        {
            return RedirectToAction("Index", "News", new { id = id });
        }

        // GET: NewsBoards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NewsBoards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] NewsBoard newsBoard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newsBoard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsBoard);
        }

        // GET: NewsBoards/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.NewsBoards == null)
            {
                return NotFound();
            }

            var newsBoard = await _context.NewsBoards.FindAsync(id);
            if (newsBoard == null)
            {
                return NotFound();
            }
            return View(newsBoard);
        }

        // POST: NewsBoards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] NewsBoard newsBoard)
        {
            if (id != newsBoard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newsBoard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsBoardExists(newsBoard.Id))
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
            return View(newsBoard);
        }

        // GET: NewsBoards/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.NewsBoards == null)
            {
                return NotFound();
            }

            var newsBoard = await _context.NewsBoards
                .Include(x => x.News)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (newsBoard == null)
            {
                return NotFound();
            }

            return View(newsBoard);
        }

        // POST: NewsBoards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.NewsBoards == null)
            {
                return Problem("Entity set 'NewsDbContext.NewsBoards'  is null.");
            }
            var newsBoard = await _context.NewsBoards.FindAsync(id);
            if (newsBoard != null)
            {
                _context.NewsBoards.Remove(newsBoard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsBoardExists(string id)
        {
          return _context.NewsBoards.Any(e => e.Id == id);
        }
    }
}
