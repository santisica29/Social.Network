using Dominio;
using Dominio.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	public class InvitacionController : Controller
	{
		Sistema s = Sistema.GetInstancia();
		public IActionResult Index()
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");

			if(lrol == "Miembro")
			{
				int? lid = HttpContext.Session.GetInt32("LogueadoId");
				Usuario u = s.GetUsuario((int)lid);

				Usuario logueado = s.GetUsuario((int)lid);
				ViewBag.logueado = logueado;

				return View(s.GetMiembros());
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult EnviarSolicitud(int id)
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");
			if(lrol == "Miembro")
			{
				int? lid = HttpContext.Session.GetInt32("LogueadoId");
				Usuario solicitante = s.GetUsuario((int)lid);
				Usuario solicitado = s.GetUsuario(id);
				Invitacion nueva = new Invitacion((Miembro)solicitante, (Miembro)solicitado);
				try
				{
					s.AltaInvitacion(nueva);
					TempData["msg"] = "Invitacion realizada/aceptada";
				}
				catch (Exception e)
				{
					TempData["msg"] = e.Message;
				}

				return RedirectToAction("Index");
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult VerSolicitudes()
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");
			if(lrol == "Miembro")
			{
				int? lid = HttpContext.Session.GetInt32("LogueadoId");
				Usuario u = s.GetUsuario((int)lid);
				List<Invitacion> solicitudes = s.GetInvitacionesPendientesRecibidas(u);

				return View(solicitudes);
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult ResponderSolicitud(int id, string tipo)
		{
			string? lrol = HttpContext.Session.GetString("LogueadoRol");

			if(lrol == "Miembro")
			{
				int? lid = HttpContext.Session.GetInt32("LogueadoId");
				Invitacion pendiente = s.GetInvitacionPendiente(id, (int)lid);

				if(tipo == "aceptar")
				{
					try
					{
						s.AceptarInvitacion(pendiente);
						TempData["msg"] = "Invitacion aceptada";
					}
					catch (Exception e)
					{
						TempData["msg"] = e.Message;
					}
					return RedirectToAction("VerSolicitudes");
				}
				else if (tipo == "rechazar")
				{
					try
					{
						s.RechazarInvitacion(pendiente);
						TempData["msg"] = "Invitacion rechazada";
					}
					catch (Exception e)
					{
						TempData["msg"] = e.Message;
					}
					return RedirectToAction("VerSolicitudes");
				}
			}
			return RedirectToAction("Index", "Home");
		}

	}
}
