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

// controller object specifc for the client class
namespace Lab4.Controllers
{
    public class ClientsController : Controller
    {
        private readonly NewsDbContext _context;

        public ClientsController(NewsDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? id)
        {

            var viewModel = new NewsBoardViewModel
            {
                Clients = await _context.Clients.ToListAsync()
            };

            if (id != null)
            {
                var subList = _context.Subscriptions.Where(x => x.ClientId == id);
                viewModel.Subscriptions = subList;


                // add all the news boards then iterate over them and only output where it matches? 

                viewModel.NewsBoards = await _context.NewsBoards.ToListAsync();

            }

            return View(viewModel);

        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // method to deregister client from newsboard using subscription associative element
        public async Task<IActionResult> Deregister(int clientId, String newsBoardId)
        {

            // create a subscription object where the id's match for the passed params
            var subscription = await _context.Subscriptions.FindAsync(clientId, newsBoardId);
            
            // remove from context and save
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            // pass a new viewmodel into the edit subscriptions view for further program continuation
            var viewModel = new ClientSubscriptionsViewModel
            {
                Client = await _context.Clients.FindAsync(clientId),

                NewsBoards = await _context.NewsBoards
                .Include(i => i.Subscriptions)
                .ThenInclude(s => s.Client)
                .ToListAsync(),
            };

            return View("EditSubscriptions", viewModel);

        }

        // registers client based on creating new subscription associative element
        public async Task<IActionResult> Register(int clientId, String newsBoardId)
        {

            // create a new subscription object and assign the params from tag helper
            var sub = new Subscription();
            sub.ClientId = clientId;
            sub.NewsBoardId = newsBoardId;

            // add this new object to the db
            _context.Subscriptions.Add(sub);
            await _context.SaveChangesAsync();

            // create new viewModel for editSubscriptions view 
            var viewModel = new ClientSubscriptionsViewModel
            {
                Client = await _context.Clients.FindAsync(clientId),

                NewsBoards = await _context.NewsBoards
                .Include(i => i.Subscriptions)
                .ThenInclude(s => s.Client)
                .ToListAsync()
                ,

            };

            return View("EditSubscriptions", viewModel);
        }

        // controller function for initial call of edit subscription with no register / unregister action
        public async Task<IActionResult> EditSubscriptions(int? id)
        {

            var viewModel = new ClientSubscriptionsViewModel
            {
                Client = await _context.Clients.FindAsync(id),

                NewsBoards = await _context.NewsBoards
                .Include(i => i.Subscriptions)
                .ThenInclude(s => s.Client)
                .ToListAsync()
                ,

            };

            return View(viewModel);

        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'NewsDbContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
          return _context.Clients.Any(e => e.Id == id);
        }
    }
}
