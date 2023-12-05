using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Models
{
    public class Post : Publicacion, IComparable<Post>
    {
        public string NombreImagen { get; set; }
        public bool EstaCensurado { get; set; }
        public bool EsPublico { get; set; }

        private List<Comentario> _comentarios = new List<Comentario>();

        public Post() : base()
        {

        }

        public Post(string titulo, Miembro autor, string contenido, string nombreImagen, bool esPublico) : base(titulo, autor, contenido)
        {
            NombreImagen = nombreImagen;
            EstaCensurado = false;
            EsPublico = esPublico;
        }

        public bool AgregarComentario(Comentario c)
        {
            bool ret = false;
            if (c != null)
            {
                ret = true;
                _comentarios.Add(c);
            }
            return ret;
        }

        public List<Comentario> GetComentarios()
        {
            return _comentarios;
        }
        public void Censurar()
        {
            EstaCensurado = true;
        }
        public int CompareTo(Post? other)
        {
            if (Titulo.CompareTo(other.Titulo) > 0)
            {
                return -1;
            }
            else if (Titulo.CompareTo(other.Titulo) < 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public override double CalcValorDeAceptacion()
        {
            if (EsPublico)
            {
                return base.CalcValorDeAceptacion() + 10;
            }
            else
            {
                return base.CalcValorDeAceptacion();
            }
        }
        public override string ToString()
        {
            return "POST: " + base.ToString();
        }
        public override void EsValido()
        {
            base.EsValido();
            if (string.IsNullOrEmpty(NombreImagen))
            {
                throw new Exception("El nombre de la imágen no puede estar vacío");
            }
            if (!NombreImagen.EndsWith(".jpg") && !NombreImagen.EndsWith(".png"))
            {
                throw new Exception("El nombre de la imágen debe terminar con .jpg o .png");
            }
        }
    }
}
