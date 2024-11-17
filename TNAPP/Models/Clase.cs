using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Models
{
    public class Clase
    {
        public string NombreClase { get; set; }
        public string Grado { get; set; }
        public string CodigoClase { get; set; }
        public string IdMaestro { get; set; }
        public int Anio { get; set; }
        public string Materia { get; set; }
        public Dictionary<string, bool> estudiantes { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, Evaluacion> evaluaciones { get; set; } = new Dictionary<string, Evaluacion>();
    }
}
