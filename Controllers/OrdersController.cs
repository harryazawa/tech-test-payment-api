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

    public class OrdersController : ControllerBase
    {
        private readonly DbOrdersContext _context;

        public OrdersController(DbOrdersContext context)
        {
            _context = context;
        }

        // "GET" METHOD FOR LISTING ALL ORDERS FROM "ORDERS TABLE"
        [HttpGet("ListAll")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersTable()
        {
            return await _context.OrdersTable.ToListAsync();
        }

        // "GET" METHOD FOR SEARCHING ORDER BY ID
        [HttpGet("SearchBy{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.OrdersTable.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // "PUT" METHOD FOR STATUS CHANGE 

        [HttpPut("ChangeStatus{id}")]
        public async Task<IActionResult> PutOrder(int id, string status, Order order)
        {

            if (id == 0)
                return BadRequest("ID is necessary for this search!");
            if (status == null)
                return BadRequest("Please inform new Order Status!");
            string[] statusDb1 = new string[2];

            //SEARCH FOR AN ORDER TO MODIFY
            Order orderDb = await _context.OrdersTable.FindAsync(id);

            //IF ORDER STATUS IS "SENT TO COURIER" OR "CANCELLED", MODIFICATION WON'T BE POSSIBLE

            if (orderDb.AvailableStatus.ToUpper() == "SENT TO COURIER" ||
                orderDb.AvailableStatus.ToUpper() == "CANCELLED")
            {
                return BadRequest($"Status of order on screen can't be changed: {orderDb.OrderStatus}");
            }

            statusDb1 = orderDb.AvailableStatus.Split(",");
            statusDb1[0].Trim();
            statusDb1[1].Trim();


            /* 
                THE FOLLOWING CODE MODIFIES STATUS ACCORDING TO THE FOLLOWING PREMISE:
            
                IF STATUS IS "AWAITING PAYMENT" IT CAN CHANGE TO EITHER "PAYMENT APPROVED" OR "ORDER CANCELLED"
                IF STATUS IS "PAYMENT APPROVED" IT CAN CHANGE TO EITHER "SENT TO COURIER" OR "ORDER CANCELLED"
                IF STATUS IS "SENT TO COURIER" IT CAN CHANGE TO "ARRIVED AT DESTINATION"
            */

            if (status.ToUpper() != statusDb1[0].ToUpper() && status.ToUpper() != statusDb1[1].ToUpper())
            {
                return BadRequest(
                    $"Only the following status is accepted by the Order on screen: {statusDb1[0]} or {statusDb1[1]}");
            }

            DateTime date = DateTime.Now;


            date.ToShortDateString();
            orderDb.OrderStatus = status;
            orderDb.LastUpdate = date;
            if (status.ToUpper() == "PAYMENT APPROVED")
            {
                orderDb.AvailableStatus = "Sent to Courier, Order Cancelled";
            }
            else if (status.ToUpper() == "SENT TO COURIER")
            {
                orderDb.AvailableStatus = "Arrived at Destination";
            }

            _context.Entry(orderDb).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetOrder", new { id = order.Id }, order);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(ex.ToString());
                }
            }

            return NoContent();
        }
        // "POST" Method for "New Order"

        [HttpPost("NewOrder")]
        public async Task<ActionResult<Order>> PostOrder(Order order, int sellerId, string items)
        {
            if (sellerId == 0)
            {
                return BadRequest("You need to input Seller's ID");
            }

            DateTime date = DateTime.Now;
            date.ToShortDateString();
            order.SellerId = sellerId;
            order.OrderStatus = "Awaiting Payment";
            order.AvailableStatus = "Payment Approved, Order Cancelled";
            order.OrderDate = date;
            order.LastUpdate = date;
            order.Items = items;

            try
            {
                _context.OrdersTable.Add(order);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetOrder", new { id = order.Id }, order);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex.ToString());
            }

        }
        
        private bool OrderExists(int id)
        {
            return _context.OrdersTable.Any(e => e.Id == id);
        }
    }
}