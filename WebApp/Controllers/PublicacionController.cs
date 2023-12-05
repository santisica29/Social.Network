using Dominio;
using Dominio.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;

namespace WebApp.Controllers
{
    public class PublicacionController : Controller
    {
        Sistema s = Sistema.GetInstancia();
        private IWebHostEnvironment Environment;
        public PublicacionController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }
        public IActionResult Index()
        {
            int? lid = HttpContext.Session.GetInt32("LogueadoId");
            string? lrol = HttpContext.Session.GetString("LogueadoRol");
            ViewBag.rol = lrol;
            if(lrol == "Miembro")
            {
				Miembro logueado = s.GetUsuario((int)lid) as Miembro;
				ViewBag.logueado = logueado;
				List<Post> listaPosts = s.ListarPostsParaUsuario((int)lid);

				return View(listaPosts);
			}
            else if (lrol == "Administrador")
            {
                return View(s.GetPosts());
            }

            return RedirectToAction("Index","Home");
        }

		public IActionResult Buscar()
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");

			if (lrol != "Miembro")
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public IActionResult Buscar(string criterio, int num)
		{
			try
			{
				List<Publicacion> lista = s.BuscarPublicacion(criterio, num);
                return View(lista);
            }
			catch (Exception e)
			{
				ViewBag.msg = e.Message;
			}
			return View();
		}

		public IActionResult CrearPost()
        {
            string? lrol = HttpContext.Session.GetString("LogueadoRol");
            if (lrol == "Miembro")
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

		[HttpPost]
        public IActionResult CrearPost(Post p, bool visibilidad, IFormFile archivo)
        {
            try
            {
                if (archivo != null)
                {
                    string ruta = Environment.WebRootPath + "//img//";
                    string extension = Path.GetExtension(archivo.FileName);
                    string nombreArchivo = p.Id.ToString() + extension;
                    FileStream stream = new FileStream(ruta + nombreArchivo, FileMode.Create);
                    archivo.CopyTo(stream);
                    stream.Close();

                    int? lid = HttpContext.Session.GetInt32("LogueadoId");
                    Usuario autor = s.GetUsuario((int)lid);

                    p.Autor = (Miembro)autor;
                    p.EsPublico = visibilidad;
                    p.NombreImagen = nombreArchivo;

                    s.AltaPublicacion(p);
                    ViewBag.msg = "Post creado";
                }
                else
                {
                    throw new Exception("Falta imagen");
                }
            }
            catch (Exception e)
            {

                ViewBag.msg = e.Message;
            }
            return View();
        }

		public IActionResult Comentar(int pid, string titulo, string contenido)
        {
			string? lrol = HttpContext.Session.GetString("LogueadoRol");
            if(lrol == "Miembro")
            {
				int? lid = HttpContext.Session.GetInt32("LogueadoId");
				Miembro autor = s.GetUsuario((int)lid) as Miembro;
				Post p = s.BuscarPost(pid);
				Comentario nuevo = new Comentario(titulo, autor, contenido);
				try
				{
					s.AgregarComentarioAPost(p, nuevo);
				}
				catch (Exception e)
				{
					TempData["msg"] = e.Message;
				}
				return RedirectToAction("Index", "Publicacion");
			}

            return RedirectToAction("Index", "Home");
		}

        public IActionResult Reaccionar(int id, bool valor)
        {
			string? lrol = HttpContext.Session.GetString("LogueadoRol");

            if(lrol == "Miembro")
            {
				int? lid = HttpContext.Session.GetInt32("LogueadoId");
				Miembro autor = (Miembro)s.GetUsuario((int)lid);
				Reaccion nueva = new Reaccion(autor, valor);
				Publicacion p = s.BuscarPublicacion(id);

				try
				{
					s.Reaccionar(p, nueva);
				}
				catch (Exception e)
				{
					TempData["msg"] = e.Message;
				}
				return RedirectToAction("Index", "Publicacion");
			}

			return RedirectToAction("Index", "Home");	
        }

		public IActionResult BanearPost(int id)
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");

			if (lrol == "Administrador")
			{
				return View(s.BuscarPost(id));
			}
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult BanearPost(int id, bool isChecked)
		{
			if (isChecked)
			{
				s.BanearPost(id);
				TempData["msg"] = "Post baneado";
				return RedirectToAction("Index", "Publicacion");
			}
			else
			{
				ViewBag.msg = "Debe seleccionar el checkbox";
			}
			return View(s.BuscarPost(id));
		}
	}     
}
