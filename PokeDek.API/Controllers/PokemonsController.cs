using Microsoft.AspNetCore.Mvc;
using PokeDek.API.Models;
using PokeDek.API.Services;
using System.Security.Cryptography.X509Certificates;

namespace PokeDek.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonsController : ControllerBase
    {
        // GET: api/Pokemons
        [HttpGet]
        public IEnumerable<Pokemon> GetPokemons()
        {
            return PokemonsMockDatabase.GetPokemons();
        }
    }
}
