using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BadgerysCreekHotel.Data;
using BadgerysCreekHotel.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BadgerysCreekHotel.Models.ViewModels;
using Microsoft.Data.Sqlite;

namespace BadgerysCreekHotel.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string sortOrder)
        {
            var bookings = (IQueryable<Booking>)_context.Booking.Include(s => s.TheRoom).Include(s => s.TheCustomer);


            if (User.IsInRole("Customers"))
            {
                string _email = User.FindFirst(ClaimTypes.Name).Value;

                bookings = bookings.Where(s => s.CustomerEmail.Contains(_email));
            }
            else
            {
                bookings = bookings.Where(s => s.CustomerEmail.Contains(s.TheCustomer.Email));

            }

            if (String.IsNullOrEmpty(sortOrder))
            {
                // When the Index page is loaded for the first time, the sortOrder is empty.
                // By default, the movies should be displayed in the order of title_asc.
                sortOrder = "checkInDate_asc";
            }

            // Prepare the query for getting the entire list of movies.
            // Convert the data type from DbSet<Movie> to IQueryable<Movie>


            // Sort the movies by specified order
            switch (sortOrder)
            {
                case "checkInDate_asc":
                    bookings = bookings.OrderBy(m => m.CheckIn);
                    break;
                case "checkInDate_desc":
                    bookings = bookings.OrderByDescending(m => m.CheckIn);
                    break;
                case "price_asc":
                    bookings = bookings.OrderBy(m => (m.CheckOut.Ticks - m.CheckIn.Ticks) * m.TheRoom.Price);
                    break;
                case "price_desc":
                    bookings = bookings.OrderByDescending(m => (m.CheckOut.Ticks - m.CheckIn.Ticks) * m.TheRoom.Price);
                    break;
            }

            // Deciding the query string (sortOrder=xxx) to include in the heading links
            // for Title and Price respectively.
            // They specify the next display order if a heading link is clicked. 
            // Store them in ViewData dictionary to pass them to View.
            ViewData["NextNameOrder"] = sortOrder != "checkInDate_asc" ? "checkInDate_asc" : "checkInDate_desc";
            ViewData["NextPriceOrder"] = sortOrder != "price_asc" ? "price_asc" : "price_desc";


            // Access database to execute the query prepared above
            // Pass the returned movie list to View
            return View(await bookings.AsNoTracking().ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email");
            ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomID");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingsCreateView bookings)
        {
            Booking b = new Booking();

            if (ModelState.IsValid)
            {
                var theRoom = await _context.Room.FindAsync(bookings.RoomID);

                var theCustomer = await _context.Customer.FindAsync(bookings.CustomerEmail);
                ViewData["OrderedRoom"] = theRoom.RoomID;


                b.RoomID = bookings.RoomID;

                if (User.IsInRole("Customers"))
                {
                    string _email = User.FindFirst(ClaimTypes.Name).Value;
                    b.CustomerEmail = _email;
                }
                else
                {
                    b.CustomerEmail = bookings.CustomerEmail;
                }

                b.CheckIn = bookings.CheckIn;
                b.CheckOut = bookings.CheckOut;
                b.Cost = (bookings.CheckOut.Day - bookings.CheckIn.Day) * theRoom.Price;

                _context.Booking.Add(b);
                await _context.SaveChangesAsync();


            }


            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", b.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomID", b.RoomID);
            return View(b);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomID", booking.RoomID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomID,CustomerEmail,CheckIn,CheckOut,Cost")] Booking booking)
        {
            if (id != booking.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.ID))
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
            ViewData["CustomerEmail"] = new SelectList(_context.Customer, "Email", "Email", booking.CustomerEmail);
            ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomID", booking.RoomID);
            return View(booking);
        }

        private bool BookingExists(int iD)
        {
            throw new NotImplementedException();
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.TheCustomer)
                .Include(b => b.TheRoom)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> Statistics()
        {

            var subGroups1 = _context.Booking.Where(s => s.CustomerEmail.Contains(s.TheCustomer.Email)).GroupBy(m => m.TheCustomer.PostNumber)
                .Select(g => new Statistics { PostCode = g.Key, NumOfCusotmer = g.Count() });

            var subGroups2 = _context.Booking.Where(s => s.RoomID == s.TheRoom.RoomID).GroupBy(m => m.TheRoom.RoomID)
                .Select(g => new Statistics { RoomID = g.Key, NumOfBooking = g.Count() });
           
            ViewBag.Group1 = await subGroups1.ToListAsync();
           
            ViewBag.Group2 = await subGroups2.ToListAsync();
            
            
            return View();
        }
    }
}
