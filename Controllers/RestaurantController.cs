using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantRaterAPI.Models;

namespace RestaurantRaterAPI.Controllers
{
    [ApiController]
    [Route("Restaurants")]
    public class RestaurantController : Controller
    {
        private ApplicationDbContext _db;
        public RestaurantController(ApplicationDbContext db) {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> PostRestaurant([FromForm] RestaurantEdit model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _db.Restaurants.Add(new Restaurant() {
                Name = model.Name,
                Location = model.Location,
            });

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRestaurants() {
            var restaurantsdata = await _db.Restaurants.ToListAsync();
            List<RestaurantDetail> restaurants = new List<RestaurantDetail>();

            foreach (Restaurant r in restaurantsdata) {
                double totalScore = 0.0;
                foreach (Rating rating in r.Ratings) {
                    totalScore += Convert.ToSingle(rating.Score);
                }
                restaurants.Add(new RestaurantDetail() {
                    Id = r.Id,
                    Name = r.Name,
                    Location = r.Location,
                    AverageScore = totalScore / r.Ratings.Count
                });
            }

            return Ok(restaurants);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetRestaurantById(int id) {
            var restaurant = await _db.Restaurants.FindAsync(id);
            if (restaurant == null) {
                return NotFound();
            }

            double totalScore = 0;
            foreach (Rating rating in restaurant.Ratings) {
                totalScore += rating.Score;
            }

            var restaurantDetail = new RestaurantDetail() {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Location = restaurant.Location,
                AverageScore = totalScore / restaurant.Ratings.Count
            };

            return Ok(restaurantDetail);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRestaurant([FromForm] RestaurantEdit model, [FromRoute] int id) {
            var oldRestaurant = await _db.Restaurants.FindAsync(id);
            if (oldRestaurant == null) {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            if (!string.IsNullOrEmpty(model.Name)) {
                oldRestaurant.Name = model.Name;
            }
            if (!string.IsNullOrEmpty(model.Location)) {
                oldRestaurant.Location  = model.Location;
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id) {
            var oldRestaurant = await _db.Restaurants.FindAsync(id);
            if (oldRestaurant == null) {
                return NotFound();
            }
            _db.Remove(id);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}