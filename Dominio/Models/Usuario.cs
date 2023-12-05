using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Interfaces;

namespace Dominio.Models
{
	public abstract class Usuario : IValidacion
	{
		private static int UltimoId { get; set; } = 1;
		public int Id { get; set; }
		public string Email { get; set; }
		public string Contrasenia { get; set; }
		public bool EstaBloqueado { get; set; }

		public Usuario()
		{
			Id = UltimoId++;
		}

		public Usuario(string email, string contrasenia)
		{
			Id = UltimoId++;
			Email = email;
			Contrasenia = contrasenia;
			EstaBloqueado = false;
		}
		public void Bloquear()
		{
			EstaBloqueado = true;
		}
		public override string ToString()
		{
			return base.ToString();
		}

		public virtual void EsValido()
		{
			if (!Email.Contains('@') || Email[0] == '@' || Email[Email.Length - 1] == '@')
			{
				throw new Exception("Email inválido");
			}
			if (string.IsNullOrEmpty(Contrasenia))
			{
				throw new Exception("Ingrese una contraseña.");
			}

		}
	}
}
