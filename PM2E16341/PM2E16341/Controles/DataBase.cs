using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PM2E16341.Models;

namespace PM2E16341.Controles
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection db;

        // Constructor de clase vacío
        public DataBase() { }

        // Constructor con parámetros
        public DataBase(string pathbasedatos)
        {
            bool databaseExists = File.Exists(pathbasedatos);

            db = new SQLiteAsyncConnection(pathbasedatos);

            if (!databaseExists)
            {
                // La base de datos no existe, se crea la tabla Ubicaciones
                db.CreateTableAsync<Ubicaciones>().Wait();
            }
        }

        // Función para retornar la tabla como lista
        public Task<List<Ubicaciones>> ListaUbicaciones()
        {
            return db.Table<Ubicaciones>().ToListAsync();
        }

        // Obtener ubicación por ID
        public Task<Ubicaciones> ObtenerUbicacion(int uId)
        {
            return db.Table<Ubicaciones>()
                .Where(i => i.Id == uId)
                .FirstOrDefaultAsync();
        }

        // Obtener ubicación por longitud
        public Task<Ubicaciones> ObtenerLongitud(float uLongitud)
        {
            return db.Table<Ubicaciones>().Where(i => i.Longitud == uLongitud).FirstOrDefaultAsync();
        }

        // Obtener ubicación por latitud
        public Task<Ubicaciones> ObtenerLatitud(float uLatitud)
        {
            return db.Table<Ubicaciones>().Where(i => i.Latitud == uLatitud).FirstOrDefaultAsync();
        }

        // Obtener ubicación por descripción
        public Task<Ubicaciones> ObtenerDescripcion(string uDescripcion)
        {
            return db.Table<Ubicaciones>().Where(i => i.Descripcion == uDescripcion).FirstOrDefaultAsync();
        }

        // INSERT-UPDATE Ubicaciones
        public Task<int> GuardarUbicacion(Ubicaciones ubicacion)
        {
            if (ubicacion.Id != 0)
            {
                return db.UpdateAsync(ubicacion);
            }
            else
            {
                return db.InsertAsync(ubicacion);
            }
        }

        // Eliminar ubicación
        public Task<int> EliminarUbicacion(Ubicaciones ubicacion)
        {
            return db.DeleteAsync(ubicacion);
        }

        // Pasar imagen de byte a Stream
        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
