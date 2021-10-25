using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamagotchiAPI.Models;

namespace TamagotchiAPI.Controllers
{
    [ApiController]
    [Route("/Pets")]
    public class PetController : ControllerBase
    {
        // DAO: Data Access Object
        private readonly DatabaseContext _context;

        public PetController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pet>>> GetAllPets()
        {
            return await _context.Pets.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPetById(int id)
        {
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null) return NotFound();

            return pet;
        }

        [HttpPost]
        public async Task<ActionResult<Pet>> CreatePet(Pet p)
        {
            if (p.Name.Length < 1) return BadRequest("Your pet needs a name!");
            Pet newPet = new Pet();
            newPet.Name = p.Name;
            newPet.Birthday = DateTime.Now;
            newPet.HungerLevel = 0;
            newPet.HappinessLevel = 0;


            _context.Pets.Add(p);
            await _context.SaveChangesAsync();

            return Created("Pets", p);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Pet>> DeletePet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null) return NotFound();

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return Ok(pet);
        }

        [HttpPost("{id}/Playtimes")]
        public async Task<ActionResult<Playtimes>> Play(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            Playtimes play = new Playtimes();
            play.When = DateTime.Now;
            play.PetId = id;
            pet.HappinessLevel += 5;
            pet.HungerLevel += 3;
            _context.Playtimes.Add(play);
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();

            return Created("{id}/Playtimes", play);
        }

        [HttpPost("{id}/Feedings")]
        public async Task<ActionResult<Feedings>> Feed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            Feedings feed = new Feedings();
            feed.When = DateTime.Now;
            feed.PetId = id;
            pet.HappinessLevel += 3;
            pet.HungerLevel = (pet.HungerLevel - 5 < 0 ? 0 : pet.HungerLevel - 5);
            _context.Feedings.Add(feed);
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();

            return Created("{id}/Feedings", feed);
        }

        [HttpPost("{id}/Scoldings")]
        public async Task<ActionResult<Scoldings>> Scold(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            Scoldings scold = new Scoldings();
            scold.When = DateTime.Now;
            scold.PetId = id;
            pet.HappinessLevel = (pet.HappinessLevel - 5 < 0 ? 0 : pet.HappinessLevel - 5);
            _context.Scoldings.Add(scold);
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();

            return Created("{id}/Scoldings", scold);
        }
    }
}