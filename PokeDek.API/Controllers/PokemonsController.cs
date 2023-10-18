using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeDek.API.Helpers;
using PokeDek.API.Models;
using PokeDek.API.Services;
using System.Security.Cryptography.X509Certificates;

namespace PokeDek.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PokemonsController : ControllerBase
    {
        // GET: api/Pokemons
        [HttpGet]
        public IEnumerable<Pokemon> GetPokemons([FromQuery]string? searchString, string? sortBy, int? pageIndex)
        {
            //If we were pointing to a DB using EF this should be an IQueryable
            var pokemonsQueryResult = PokemonsMockDatabase.GetPokemons().AsQueryable();
            //Searching (non case-sensitive)
            if (!String.IsNullOrEmpty(searchString))
            {
                pokemonsQueryResult = pokemonsQueryResult.Where(s => s.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)
                                    || s.Code.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));
            }

            //Sorting
            pokemonsQueryResult = sortBy switch
            {
                "name" => pokemonsQueryResult.OrderBy(s => s.Name),
                "name_desc" => pokemonsQueryResult.OrderByDescending(s => s.Name),
                "code" => pokemonsQueryResult.OrderBy(s => s.Code),
                "code_desc" => pokemonsQueryResult.OrderByDescending(s => s.Code),
                _ => pokemonsQueryResult.OrderBy(s => s.Code),
            };

            //Paging
            int pageSize = 10;
            var paginatedResult = PaginatedList<Pokemon>.Create(
                pokemonsQueryResult, pageIndex ?? 1, pageSize);
            return paginatedResult;
        }

        // GET: api/Pokemons/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pokemon))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPokemon([FromRoute] string id)
        {
   
            var pokemon = PokemonsMockDatabase.GetPokemons()
                                              .FirstOrDefault(p => p.Code == id);

            return pokemon == null? NotFound() : Ok(pokemon);

        }

        // POST: api/Pokemons
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Pokemon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModelResponse))]
        public IActionResult PostPokemon([FromBody] Pokemon pokemon)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PokemonsMockDatabase.CreatePokemon(pokemon);

            return CreatedAtAction("GetPokemon", new { id = pokemon.Code }, pokemon);
        }

        // PUT: api/Pokemons/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PutPokemon([FromRoute] string id, [FromBody] Pokemon pokemon)
        {
            if (!ModelState.IsValid)
            {
                return Conflict(ModelState);
            }

            if (id != pokemon.Code)
            {
                return BadRequest();
            }

            if (!PokemonsMockDatabase.PokemonExists(pokemon.Code))
            {
                return NotFound();
            }

            PokemonsMockDatabase.UpdatePokemon(pokemon);

            return NoContent();
        }

        // DELETE: api/Pokemons/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pokemon))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePokemon([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!PokemonsMockDatabase.PokemonExists(id))
            {
                return NotFound();
            }

            var pokemon = PokemonsMockDatabase.GetPokemons()
                                              .ToList()
                                              .FirstOrDefault(p => p.Code == id);


            PokemonsMockDatabase.Remove(pokemon);

            return Ok(pokemon);
        }
    }
}
