using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tech_test_payment_api.Context;
using tech_test_payment_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace tech_test_payment_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SellersController : ControllerBase
    {
        private readonly DbOrdersContext _context;

        public SellersController(DbOrdersContext context)
        {
            _context = context;
        }

        // "GET" METHOD TO LIST ALL SELLERS
        [HttpGet("ListAll")]
        public async Task<ActionResult<IEnumerable<Seller>>> GetOrderSellers()
        {
            return await _context.OrderSellers.ToListAsync();
        }

        // "GET" METHOD TO SEARCH SELLER BY ID
        [HttpGet("SearchBy{id}")]
        public async Task<ActionResult<Seller>> GetSeller(int id)
        {
            var seller = await _context.OrderSellers.FindAsync(id);

            if (seller == null)
            {
                return NotFound();
            }

            return seller;
        }
        // "POST" METHOD FOR INSERTING NEW SELLER ON DATABASE
       
        [HttpPost("NewSeller")]
        public async Task<ActionResult<Seller>> PostSelle(Seller seller, string name, string cpf, string phoneNumber)
        {
            if(name==null) return BadRequest("Insert Seller's Name.");
            if(cpf==null) return BadRequest("Insert Seller's CPF.");
            if(phoneNumber==null) return BadRequest("Insert Seller's Phone Number");
            seller.CPF=cpf;
            seller.Name=name;
            seller.PhoneNumber=phoneNumber;
            
            try{
                    _context.OrderSellers.Add(seller);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetSeller", new { id = seller.Id }, seller);

                }catch (DbUpdateConcurrencyException ex)
                {
                        return BadRequest(ex.ToString());
                }
        }
        
        private bool SellerExists(int id)
        {
            return _context.OrderSellers.Any(e => e.Id == id);
        }
    }
}