﻿using Microsoft.AspNetCore.Mvc;
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

    }
}
