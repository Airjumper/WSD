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

namespace BadgerysCreekHotel.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MyDetails()
        {
            // retrieve the logged-in user's email
            // need to add "using System.Security.Claims;"
            string _email = User.FindFirst(ClaimTypes.Name).Value;
            // check whether the user already exists in database
            var customer = await _context.Customer.FindAsync(_email);

            if (customer == null)
            {
                // if the moviegoer's record doesn't exist yet:
                // validation will not be checked when creating an object, but in web form
                customer = new Customer { Email = _email };
                return View("~/Views/Customers/MyDetailsCreate.cshtml", customer);
            }
            else
            {
                // if the moviegoer's record already exists
                return View("~/Views/Customers/MyDetailsUpdate.cshtml", customer);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsCreate([Bind("Email,Surname,GivenName,PostNumber")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();

                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyDetailsUpdate([Bind("Email,Surname,GivenName,PostNumber")]  Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                return View("~/Views/Customers/MyDetailsSuccess.cshtml", customer);
            }
            return View(customer);
        }
    }
}
