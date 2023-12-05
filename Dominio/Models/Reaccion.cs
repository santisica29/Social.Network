using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Models
{
    public class Reaccion
    {
        private static int UltimoId { get; set; } = 1;
        public int Id { get; set; }
        public Miembro Autor { get; set; }
        public bool MeGusta { get; set; }

        public Reaccion()
        {
            Id = UltimoId++;
        }

        public Reaccion(Miembro autor, bool meGusta)
        {
            Id = UltimoId++;
            Autor = autor;
            MeGusta = meGusta;
        }
    }
}
