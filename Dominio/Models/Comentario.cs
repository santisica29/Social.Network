using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Models
{
    public class Comentario : Publicacion
    {
        public Comentario() : base()
        {

        }
        public Comentario(string titulo, Miembro autor, string contenido) : base(titulo, autor, contenido)
        {

        }

        public override double CalcValorDeAceptacion()
        {
            return base.CalcValorDeAceptacion();
        }

        public override string ToString()
        {
            return "COMENTARIO: " + base.ToString();
        }

        public override void EsValido()
        {
            base.EsValido();
        }
    }
}
