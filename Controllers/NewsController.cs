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
using Azure.Storage.Blobs;
using Azure;

namespace Lab4.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "news";

        public NewsController(NewsDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: News
        public async Task<IActionResult> Index(string id, string title)
        {

            if (id == null)
            {
                return View("Error");
            }

            var newsBoard = await _context.NewsBoards
                                    .Where(nb => nb.Id == id)
                                    .Include(x => x.News)
                                    .SingleOrDefaultAsync();


            if (newsBoard == null)
            {
                return View("Error");
            }

            var newsViewModel = new NewsViewModel
            {
                NewsBoard = newsBoard,
                News = newsBoard.News
            };


            return View(newsViewModel);                  
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.newsboard)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        /// <summary>
        /// Get the create page with Newsboard data
        /// </summary>
        /// <param name="id"> the parent newsboard id to create a news item under</param>
        /// <param name="title">the parent newsboard title</param>
        /// <returns></returns>
        public IActionResult Create(string id, string title)
        {
            var fileInputViewModel = new FileInputViewModel
            {
                NewsBoardId = id,
                NewsBoardTitle = title
            };

            return View(fileInputViewModel);
        }

        // POST: News/Create
        /// <summary>
        /// Create a new blob image from upload file and save a new news item to DB
        /// </summary>
        /// <param name="file"> the file uploaded by user</param>
        /// <param name="newsBoardId"> the id of the newsboard that this news item will belong too</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, string newsBoardId)
        {

            BlobContainerClient containerClient;
            News news = new News();

            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                string randomFileName = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();

                    news.FileName = blockBlob.Name;
                    news.ImageUrl = blockBlob.Uri.AbsoluteUri;
                }
            }
            catch (RequestFailedException)
            {
                View("Error");
            }

            news.NewsBoardId = newsBoardId;
            _context.Add(news);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "News", new {id = news.NewsBoardId});
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["NewsBoardId"] = new SelectList(_context.NewsBoards, "Id", "Id", news.NewsBoardId);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NewsBoardId,FileName,ImageUrl")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            ViewData["NewsBoardId"] = new SelectList(_context.NewsBoards, "Id", "Id", news.NewsBoardId);
            return View(news);
        }

        // GET: News/Delete/5
        /// <summary>
        /// Retrieve news content from db and return the confirm delete view
        /// </summary>
        /// <param name="id"> news content id value</param>
        /// <returns> the confirm delete view of the news item</returns>
        public async Task<IActionResult> Delete(int id)
        {   

            var news = await _context.News
                .Include(n => n.newsboard)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        /// <summary>
        /// Confirm and remove News item from database and Blob storage
        /// </summary>
        /// <param name="id"> the news item id delete has been called on</param>
        /// <returns> News Index based on parent newsboard id </returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var news = await _context.News.FindAsync(id);

            if (news == null)
            {
                return NotFound();
            }

            BlobContainerClient containerClient;

            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            try
            {
                var blockBlob = containerClient.GetBlobClient(news.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }
            if (_context.News == null)
            {
                return Problem("Entity set 'NewsDbContext.News'  is null.");
            }

            _context.News.Remove(news);
  
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "News", new { id = news.NewsBoardId });
        }

        private bool NewsExists(int id)
        {
          return _context.News.Any(e => e.Id == id);
        }
    }
}
