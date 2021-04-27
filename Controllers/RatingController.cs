using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantRaterAPI.Models;

namespace RestaurantRaterAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatingController : Controller
    {
        private ApplicationDbContext _db;
        public RatingController(ApplicationDbContext db) {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> PostRestaurant([FromForm] RatingEdit model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _db.Ratings.Add(new Rating() {
                Score = model.Score,
                RestaurantId = model.RestaurantId,
            });

            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}