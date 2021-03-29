using ProAgil.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProAgil.Repository
{
    public interface IEventoRepository
    {
        Task<Evento> GetEventoAsyncById(int EventoId, bool includePalestrantes);
        
        Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool includePalestrantes);

        Task<Evento[]> GetAllEventoAsync(bool includePalestrantes);
    }
}
