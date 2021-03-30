using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProAgil.Repository
{
    public class PalestranteRepository : IPalestranteRepository
    {
        private readonly ProAgilContext _context;

        public PalestranteRepository(ProAgilContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<Palestrante[]> GetAllPalestranteAsyncByName(string name,bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                 .Include(c => c.RedesSociais);

            if (includeEventos)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query
                .AsNoTracking()
                .Where(p => p.Nome.ToLower().Contains(name.ToLower()));

            return await query.ToArrayAsync();
        }

        public async Task<Palestrante> GetPalestranteAsync(int PalestranteId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
               .Include(c => c.RedesSociais);

            if (includeEventos)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .Where(p => p.Id == PalestranteId);

            return await query.FirstOrDefaultAsync();
        }
    }
}
