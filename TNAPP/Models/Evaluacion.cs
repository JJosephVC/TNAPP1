using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProTCE.Models
{
    public class Evaluacion
    {
        public string Nombre { get; set; }
        public string TipoEvaluacion { get; set; }
        public DateTime Fecha { get; set; }
        public int CalificacionMaxima { get; set; }
        public string CorteEvaluativo { get; set; }
        public string Semestre { get; set; }
        public Dictionary<string, Nota> Notas { get; set; } = new Dictionary<string, Nota>();
    }
}
