using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MO.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly MODbContext dbContext;

        public OrdersController(MODbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return this.dbContext.Orders.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/orders
        [HttpPost]
        public void Post(string email, string clientName, string itemName, int itemPrice)
        {
            Order order = new Order();
            order.Client = new Client() {Email = email, NameSurname = clientName};
            order.Item = new Item() {Name = itemName, Price = itemPrice};

            this.dbContext.Orders.Add(order);
            this.dbContext.SaveChanges();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
