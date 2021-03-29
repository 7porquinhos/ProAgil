using ProAgil.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProAgil.Repository
{
    public interface IPalestranteRepository
    {
        Task<Palestrante> GetPalestranteAsync(int PalestranteId, bool includeEventos);

        Task<Palestrante[]> GetAllPalestranteAsyncByName(string name, bool includeEventos);
    }
}
