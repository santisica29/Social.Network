using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Interfaces;

namespace Dominio.Models
{
    public class Administrador : Usuario, IValidacion
    {

        public Administrador() : base()
        {

        }
        public Administrador(string email, string contrasenia) : base(email, contrasenia)
        {
        }

        public override string ToString()
        {
            return $"Admin: {Email}";
        }
        public override void EsValido()
        {
            base.EsValido();
        }
    }
}
