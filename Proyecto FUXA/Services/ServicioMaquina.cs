using Microsoft.EntityFrameworkCore;
using Proyecto_FUXA.Data;
using Proyecto_FUXA.Models;

namespace Proyecto_FUXA.Services
{
    public class ServicioMaquina
    {
        private readonly AppDbContext _db;

        public ServicioMaquina(AppDbContext db)
        {
            _db = db;
        }

        // Obtener todas las máquinas ordenadas por nombre
        public async Task<List<Maquina>> GetAllAsync()
        {
            return await _db.Maquinas
                .Include(m => m.Producciones)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        // Obtener una máquina por su ID
        public async Task<Maquina?> GetByIdAsync(int id)
        {
            return await _db.Maquinas
                .Include(m => m.Producciones)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Añadir una nueva máquina
        public async Task AddAsync(Maquina maquina)
        {
            maquina.Nombre = (maquina.Nombre ?? string.Empty).Trim();
            maquina.FechaCreacion = DateTime.UtcNow;
            maquina.FechaActualizacion = DateTime.UtcNow;
            _db.Maquinas.Add(maquina);
            await _db.SaveChangesAsync();

            var yaExisteObjetoVisual = await _db.ObjetosVisualesPlanta
                .AnyAsync(x => x.MaquinaId == maquina.Id);

            if (!yaExisteObjetoVisual)
            {
                var totalObjetos = await _db.ObjetosVisualesPlanta.CountAsync();
                var columna = totalObjetos % 5;
                var fila = totalObjetos / 5;

                _db.ObjetosVisualesPlanta.Add(new PlantaObjetoVisual
                {
                    Nombre = maquina.Nombre,
                    Tipo = "Rectangulo",
                    PosX = 20 + (columna * 150),
                    PosY = 20 + (fila * 110),
                    Width = 120,
                    Height = 80,
                    ColorHex = "#2f80ed",
                    MaquinaId = maquina.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }
        }

        // Actualizar una máquina
        public async Task UpdateAsync(Maquina maquina)
        {
            maquina.FechaActualizacion = DateTime.UtcNow;
            _db.Maquinas.Update(maquina);
            await _db.SaveChangesAsync();
        }

        // Registrar un ciclo de producción
        public async Task AddCycleAsync(int maquinaId, int ciclosReales)
        {
            var log = new MaquinaProduccion
            {
                MaquinaId = maquinaId,
                CiclosReales = ciclosReales,
                FechaRegistro = DateTime.UtcNow
            };

            _db.Producciones.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<Maquina?> ObtenerPorIdFuxa(string idFuxa)
        {
            return await _db.Maquinas
                .FirstOrDefaultAsync(m => m.FuxaDeviceId == idFuxa);
        }

        public async Task GuardarMaquina(Maquina maquina)
        {

            var existe = maquina.Id > 0 || await _db.Maquinas.AnyAsync(m => m.FuxaDeviceId == maquina.FuxaDeviceId);

            maquina.FechaActualizacion = DateTime.UtcNow;

                if (!existe)
                {
                    maquina.FechaCreacion = DateTime.UtcNow;

                    _db.Maquinas.Add(maquina);
                }
                else
                {
                    _db.Maquinas.Update(maquina);
                }

                await _db.SaveChangesAsync();
            }
        }
    }
