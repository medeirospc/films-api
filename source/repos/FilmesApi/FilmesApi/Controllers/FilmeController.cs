using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{

    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados.
    /// </summary>
    /// <param name="filmeDto">Objeto com campos necessários para criação de
    /// um filme.</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme(
        [FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);  
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperaFilmePorId), 
            new { id = filme.Id },
            filme);
    }

    /// <summary>
    /// Recupera um filme por paginação.
    /// </summary>
    /// <param name="skip">Parâmetros de pular e pegar filme por index. 
    /// </param>
    /// <returns>IEnumerable</returns>
    /// <response code="200">Caso recuperação seja feita com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0, 
        [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip
            (skip).Take(take));
    }

    /// <summary>
    /// Recupera um filme do banco de dados pelo ID.
    /// </summary>
    /// <param name="id">ID para ser recuperado pelo usuário. 
    /// </param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso recuperação seja feita com sucesso.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes
            .FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza um filme do banco de dados pelo ID com JSON inteiro.
    /// </summary>
    /// <param name="id">ID para ser atualizado pelo usuário.
    /// </param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso atualização seja feita com sucesso.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult AtualizaFilme(int id, 
        [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Atualiza um filme do banco de dados pelo ID com JSON parcial.
    /// </summary>
    /// <param name="id">ID para ser atualizado pelo usuário com JSON parcial.
    /// </param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso atualização seja feita com sucesso.</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult AtualizaFilmeParcial(int id, 
        JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if(!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Remove um filme do banco de dados pelo ID.
    /// </summary>
    /// <param name="id">ID para ser removido pelo usuário.
    /// </param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso remoção seja feita com sucesso.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if (filme == null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
