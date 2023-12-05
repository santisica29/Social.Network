using Dominio;
using Dominio.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	public class UsuarioController : Controller
	{
		Sistema s = Sistema.GetInstancia();
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]

		public IActionResult Login(string email, string password)
		{
			Usuario user = s.BuscarUsuario(email, password);
			if (user != null)
			{
				HttpContext.Session.SetInt32("LogueadoId", user.Id);
				HttpContext.Session.SetString("LogueadoRol", user.GetType().Name);
				if (user is Miembro)
				{
					Miembro miembro = (Miembro)user;
					HttpContext.Session.SetString("LogueadoNombre", miembro.Nombre);
					HttpContext.Session.SetString("LogueadoApellido", miembro.Apellido);
				}

				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.msg = "Error en los datos";
			}
			return View();
		}

		public IActionResult Registro()
		{
			return View();
		}

		[HttpPost]

		public IActionResult Registro(Miembro m)
		{
			try
			{
				s.AltaUsuario(m);
				HttpContext.Session.SetInt32("LogueadoId", m.Id);
				HttpContext.Session.SetString("LogueadoRol", m.GetType().Name);
				if (m is Miembro)
				{
					HttpContext.Session.SetString("LogueadoNombre", m.Nombre);
					HttpContext.Session.SetString("LogueadoApellido", m.Apellido);
				}
				ViewBag.msg = "Alta correcta";
			}
			catch (Exception e)
			{
				ViewBag.msg = $"ERROR: {e.Message}";
			}
			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult ListarMiembros()
		{
			int? lid = HttpContext.Session.GetInt32("LogueadoId");
			string? lrol = HttpContext.Session.GetString("LogueadoRol");

			if (lid == null || lrol != "Administrador")
			{
				return RedirectToAction("Index", "Home");
			}

			return View(s.GetMiembros());
		}

		public IActionResult BloquearMiembro(int id)
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");
			if (lrol == "Administrador")
			{
				Usuario u = s.GetUsuario(id);
				try
				{
					s.BloquearUsuario(u);
				}
				catch (Exception e)
				{
					TempData["msg"] = $"Error: {e.Message}";
				}
				return RedirectToAction("ListarMiembros");
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		public IActionResult DesbloquearMiembro(int id)
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");
			if (lrol == "Administrador")
			{
				Usuario u = s.GetUsuario(id);
				try
				{
					s.DesbloquearUsuario(u);
				}
				catch (Exception e)
				{
					TempData["msg"] = $"Error: {e.Message}";
				}
				return RedirectToAction("ListarMiembros");
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}
		public IActionResult Details()
		{
			int? lid = HttpContext.Session.GetInt32("LogueadoId");
			string? lrol = HttpContext.Session.GetString("LogueadoRol");

			if (lrol == "Miembro")
			{
				Miembro m = s.GetUsuario((int)lid) as Miembro;

				return View(m);
			}
			return RedirectToAction("Index", "Home");
		}
	}
}
