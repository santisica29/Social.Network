using Dominio.Interfaces;

namespace Dominio.Models
{
    public abstract class Publicacion : IValidacion
    {
        private static int UltimoId { get; set; } = 1;
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public Miembro Autor { get; set; }
        public string Contenido { get; set; }

        private List<Reaccion> _reacciones = new List<Reaccion>();

        public Publicacion()
        {
            Id = UltimoId++;
			Fecha = DateTime.Now;
		}

        public Publicacion(string titulo, Miembro autor, string contenido)
        {
            Id = UltimoId++;
            Titulo = titulo;
            Fecha = DateTime.Now;
            Autor = autor;
            Contenido = contenido;
        }
        public List<Reaccion> GetReacciones()
        {
            return _reacciones;
        }
        public bool AgregarReaccion(Reaccion r)
        {
            bool ret = false;
            if (r != null)
            {
                _reacciones.Add(r);
                ret = true;
            }
            return ret;
        }
        public bool MiembroYaReacciono(Miembro m)
        {
            foreach (Reaccion r in _reacciones)
            {
                if (r.Autor.Id.Equals(m.Id))
                {
                    return true;
                }
            }
            return false;
        }
        public List<Reaccion> GetMeGustas()
        {
            List<Reaccion> ret = new List<Reaccion>();

            foreach (Reaccion r in _reacciones)
            {
                if (r.MeGusta)
                {
                    ret.Add(r);
                }
            }
            return ret;
        }
        public List<Reaccion> GetNoMeGustas()
        {
            List<Reaccion> ret = new List<Reaccion>();

            foreach (Reaccion r in _reacciones)
            {
                if (!r.MeGusta)
                {
                    ret.Add(r);
                }
            }
            return ret;
        }
        public virtual double CalcValorDeAceptacion()
        {
            int cantMeGusta = GetMeGustas().Count;
            int cantNoMeGusta = GetNoMeGustas().Count;

            double total = (cantMeGusta * 5) + (cantNoMeGusta * -2);

            return total;
        }

        public virtual void EsValido()
        {
            if (string.IsNullOrEmpty(Contenido) || string.IsNullOrEmpty(Titulo))
            {
                throw new Exception("Uno de los campos está vacío");
            }
			if (Titulo.Length < 3)
			{
				throw new Exception("El título no es válido. Debe ser de al menos 3 caracteres.");
			}
		}

        public override string ToString()
        {
            string contenido = Contenido;
            if (Contenido.Length > 50)
            {
                contenido = Contenido.Substring(0, 50) + "...";
            }
            return $"Título: {Titulo}, Fecha: {Fecha}, Autor: {Autor.Nombre} {Autor.Apellido} \n" +
                    $"Contenido: {contenido}" +
                    $"\n---------------------------------------------";
        }
    }
}
