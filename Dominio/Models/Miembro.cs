using Dominio.Interfaces;

namespace Dominio.Models
{
    public class Miembro : Usuario, IValidacion, IComparable<Miembro>
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaDeNacimiento { get; set; }

        private List<Miembro> _amigos = new List<Miembro>();

        public Miembro() : base()
        {
        }

        public Miembro(string email, string contrasenia, string nombre, string apellido, DateTime fechaDeNacimiento) : base(email, contrasenia)
        {
            Nombre = nombre;
            Apellido = apellido;
            FechaDeNacimiento = fechaDeNacimiento;
        }

        public bool AgregarAmigo(Miembro m)
        {
            bool ret = false;

            if (m != null)
            {
                _amigos.Add(m);
                ret = true;
            }

            return ret;
        }

        public List<Miembro> GetAmigos()
        {
            return _amigos;
        }

        public bool EsAmigo(int id)
        {
            foreach(Miembro amigo in _amigos)
            {
                if (amigo.Id.Equals(id))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"{Nombre} {Apellido} {FechaDeNacimiento.ToShortDateString()}";
        }

        public override void EsValido()
        {
            base.EsValido();
            if (string.IsNullOrEmpty(Nombre))
            {
                throw new Exception("El nombre es incorrecto");
            }

            if (string.IsNullOrEmpty(Apellido))
            {
                throw new Exception("El apellido es incorrecto");
            }
        }

		public int CompareTo(Miembro? other)
		{
			if(Apellido.CompareTo(other.Apellido) > 0)
            {
                return 1;
            }
            else if(Apellido.CompareTo(other.Apellido) < 0)
			{
                return -1;
            }
            else
            {
                if(Nombre.CompareTo(other.Nombre) > 0)
                {
                    return 1;
                }
                else if (Nombre.CompareTo(other.Nombre) < 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
		}
	}
}