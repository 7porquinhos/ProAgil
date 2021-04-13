using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;
using ProAgil.WebAPI.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProAgil.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProAgilRepository _proAgil;
        private readonly IEventoRepository _eventoRepository;

        public EventoController(IMapper mapper, IEventoRepository eventoRepository, IProAgilRepository proAgilRepository)
        {
            _mapper = mapper;
            _proAgil = proAgilRepository;
            _eventoRepository = eventoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var eventos = await _eventoRepository.GetAllEventoAsync(true);

                var results = _mapper.Map<EventoDto[]>(eventos);

                return Ok(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados Falhou");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources","Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                    var fullPath = Path.Combine(pathToSave, filename.Replace("\"", " ").Trim());

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados Falhou");
            }

            return BadRequest("Erro ao tentar realizar upload");
        }

        [HttpGet("{EventoId}")]
        public async Task<IActionResult> Get(int EventoId)
        {
            try
            {
                var evento = await _eventoRepository.GetEventoAsyncById(EventoId, true);

                var results = _mapper.Map<EventoDto>(evento);

                return Ok(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados Falhou");
            }
        }

        [HttpGet("getByTema{Tema}")]
        public async Task<IActionResult> Get(string Tema)
        {
            try
            {
                var eventos = await _eventoRepository.GetAllEventoAsyncByTema(Tema, true);

                var results = _mapper.Map<EventoDto[]>(eventos);
                return Ok(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados Falhou");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);
                _proAgil.Add(evento);

                if (await _proAgil.SaveChangesAsync())
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados Falhou");
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Put(int EventoId, EventoDto model)
        {
            try
            {
                var evento = await _eventoRepository.GetEventoAsyncById(EventoId, false);
                if (evento == null) return NotFound();

                var idLotes = new List<int>();
                var idRedesSociais = new List<int>();

                if(model.Lotes != null)
                {
                    model.Lotes.ForEach(item => idLotes.Add(item.Id));
                    model.RedesSociais.ForEach(item => idRedesSociais.Add(item.Id));

                    var lotesDelete = evento.Lotes
                        .Where(lote => !idLotes
                        .Contains(lote.Id))
                        .ToArray();
                    var RedesSociaisDelete = evento.RedesSociais
                        .Where(redeSocial => !idRedesSociais
                        .Contains(redeSocial.Id))
                        .ToArray();

                    if (lotesDelete.Length > 0) _proAgil.DeleteRange(lotesDelete);
                    if (RedesSociaisDelete.Length > 0) _proAgil.DeleteRange(RedesSociaisDelete);
                }
                

                _mapper.Map(model, evento);

                _proAgil.Update(evento);

                if (await _proAgil.SaveChangesAsync())
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento)); ;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados Falhou");
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int EventoId)
        {
            try
            {
                var evento = await _eventoRepository.GetEventoAsyncById(EventoId, false);
                if (evento == null) return NotFound();

                _proAgil.Delete(evento);

                if (await _proAgil.SaveChangesAsync())
                    return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados Falhou");
            }

            return BadRequest();
        }
    }
}
