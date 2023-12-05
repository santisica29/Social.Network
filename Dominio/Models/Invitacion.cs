using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Interfaces;

namespace Dominio.Models
{
    public class Invitacion : IValidacion
    {
        private static int UltimoId { get; set; } = 1;
        public int Id { get; set; }
        public Miembro MiembroSolicitante { get; set; }
        public Miembro MiembroSolicitado { get; set; }
        public Estado EstadoSolicitud { get; set; }
        public DateTime FechaDeSolicitud { get; set; }

        public Invitacion()
        {
            Id = UltimoId++;
			EstadoSolicitud = Estado.PENDIENTE_APROBACION;
			FechaDeSolicitud = DateTime.Now;
		}

        public Invitacion(Miembro miembroSolicitante, Miembro miembroSolicitado)
        {
            Id = UltimoId++;
            MiembroSolicitante = miembroSolicitante;
            MiembroSolicitado = miembroSolicitado;
            EstadoSolicitud = Estado.PENDIENTE_APROBACION;
            FechaDeSolicitud = DateTime.Now;
        }

        public void Aceptar()
        {
            EstadoSolicitud = Estado.APROBADA;
            MiembroSolicitado.AgregarAmigo(MiembroSolicitante);
            MiembroSolicitante.AgregarAmigo(MiembroSolicitado);
        }

        public void Rechazar()
        {
            EstadoSolicitud = Estado.RECHAZADA;
        }
        public void EsValido()
        {
            if (MiembroSolicitado == null || MiembroSolicitante == null)
            {
                throw new Exception("Los miembros no pueden ser nulos");
            }
        }
        public override string ToString()
        {
            return $"Invitacion hecha por: {MiembroSolicitante.Nombre} {MiembroSolicitante.Apellido} " +
                $"a {MiembroSolicitado.Nombre} {MiembroSolicitado.Apellido} " +
                $"el {FechaDeSolicitud}";
        }
    }
}
