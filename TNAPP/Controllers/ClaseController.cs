using ProTCE.Database;
using ProTCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Controllers
{
    public class ClaseController
    {
        private readonly FirebaseConnection firebaseConnection;
        public ClaseController()
        {
            firebaseConnection = new FirebaseConnection();
        }

        public async Task<bool> CreateClass(Clase nuevaClase)
        {
            try
            {
                await firebaseConnection.CreateClase(nuevaClase);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la clase: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GenerarCodigoClase(string nombreClase, string grado, int anio)
        {
            // Eliminar sufijos como "mo", "no", "ro", "do", "er", "to" del grado
            grado = grado.Replace("mo", "").Replace("no", "").Replace("ro", "").Replace("do", "")
                .Replace("er", "").Replace("to", "").Replace("vo", "").Trim();
            var materiaAbreviada = nombreClase.Length > 3 ? nombreClase.Substring(0, 3).ToUpper() : nombreClase.ToUpper();
            var codigoGrado = grado;
            var codigoAnio = anio % 100;

            var  codigoBase = $"{materiaAbreviada}{codigoGrado}{codigoAnio}";

            //Verificar si el Codigo existe
            string codigoClaseFinal = codigoBase;
            int intentos = 0;
            while( await ExisteCodigoClase(codigoClaseFinal) && intentos < 100)
            {
                var numeroAleatorio = new Random().Next(100, 999);
                codigoClaseFinal = $"{codigoBase}-{numeroAleatorio}";
                intentos++;
            }

            return codigoClaseFinal;
        }

        public async Task<bool> UpdateClass(Clase clase)
        {
            try
            {
                return await firebaseConnection.UpdateClase(clase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la clase: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ExisteCodigoClase(string codigoClase)
        {
            var claseExistente = await firebaseConnection.GetClaseByCodigo(codigoClase);
            return claseExistente != null; // Devuelve true si la clase existe, false si no
        }

        public async Task<Clase> GetClaseByCodigo(string codigoClase)
        {
            return await firebaseConnection.GetClaseByCodigo(codigoClase);
        }
    }
}
