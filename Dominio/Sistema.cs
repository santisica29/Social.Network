using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Models;

namespace Dominio
{
	public class Sistema
	{
		#region SINGLETON
		private Sistema()
		{
			Precarga();
		}

		private static Sistema _instancia = null;

		public static Sistema GetInstancia()
		{
			if (_instancia == null)
			{
				_instancia = new Sistema();
			}

			return _instancia;
		}
		#endregion

		#region Listas
		private List<Usuario> _usuarios = new List<Usuario>();
		private List<Publicacion> _publicaciones = new List<Publicacion>();
		private List<Invitacion> _invitaciones = new List<Invitacion>();
		#endregion

		#region Altas
		public void AltaUsuario(Usuario u)
		{
			if (string.IsNullOrEmpty(u.Email))
			{
				throw new Exception("Ingrese un mail.");
			}
			if (ExisteMailEnSistema(u.Email))
			{
				throw new Exception("El mail ya existe en el sistema. Elija otro.");
			}

			u.EsValido();
			_usuarios.Add(u);
		}
		public void AltaPublicacion(Publicacion p)
		{
			if (p.Autor.EstaBloqueado)
			{
				throw new Exception("El miembro esta bloqueado no puede realizar publicaciones.");
			}

			p.EsValido();
			_publicaciones.Add(p);


		}
		public void AltaInvitacion(Invitacion i)
		{
			if (SonAmigos(i.MiembroSolicitante, i.MiembroSolicitado))
			{
				throw new Exception("Los miembros ya son amigos");
			}
			if (i.MiembroSolicitado.EstaBloqueado || i.MiembroSolicitante.EstaBloqueado)
			{
				throw new Exception("Uno de los miembros está bloqueado no puede realizar invitaciones.");
			}
			if (LaMismaInvitacionYaExiste(i))
			{
				throw new Exception("Ya existe esa misma invitación.");
			}

			if (ExisteInvitacionPendienteEntreSolicitantes(i))
			{
				i.Aceptar();
			}
			else
			{
				i.EsValido();
				_invitaciones.Add(i);
			}
		}
		#endregion

		#region Getters
		public List<Usuario> GetUsuarios() { return _usuarios; }
		public List<Publicacion> GetPublicaciones() { return _publicaciones; }
		public List<Invitacion> GetInvitaciones() { return _invitaciones; }
		public List<Invitacion> GetInvitacionesPendientesRecibidas(Usuario u)
		{
			List<Invitacion> ret = new List<Invitacion>();

			foreach (Invitacion i in _invitaciones)
			{
				if (i.MiembroSolicitado == u && i.EstadoSolicitud == Estado.PENDIENTE_APROBACION)
				{
					ret.Add(i);
				}
			}
			return ret;
		}
		public Invitacion GetInvitacionPendiente(int idSolicitante, int idSolicitado)
		{
			foreach (Invitacion i in _invitaciones)
			{
				if (i.MiembroSolicitante.Id.Equals(idSolicitante) && i.MiembroSolicitado.Id.Equals(idSolicitado))
				{
					return i;
				}
			}
			return null;
		}
		public List<Miembro> GetMiembros()
		{
			List<Miembro> ret = new List<Miembro>();

			foreach (Usuario u in _usuarios)
			{
				if (u is Miembro)
				{
					ret.Add((Miembro)u);
				}
			}
			ret.Sort();
			return ret;
		}
		public List<Administrador> GetAdministradores()
		{
			List<Administrador> ret = new List<Administrador>();

			foreach (Usuario u in _usuarios)
			{
				if (u is Administrador)
				{
					ret.Add(u as Administrador);
				}
			}
			return ret;
		}
		public List<Post> GetPosts()
		{
			List<Post> ret = new List<Post>();

			foreach (Publicacion p in _publicaciones)
			{
				if (p is Post)
				{
					ret.Add(p as Post);
				}
			}

			return ret;
		}
		public List<Comentario> GetComentarios()
		{
			List<Comentario> ret = new List<Comentario>();

			foreach (Publicacion p in _publicaciones)
			{
				if (p is Comentario)
				{
					ret.Add((Comentario)p);
				}
			}
			return ret;
		}
		public Usuario GetUsuario(int id)
		{
			foreach (Usuario u in _usuarios)
			{
				if (u.Id.Equals(id))
				{
					return u;
				}
			}
			return null;
		}
		#endregion

		#region Listar
		public List<Publicacion> ListarPublicacionesDeMiembro(string email)
		{
			if (!ExisteMailEnSistema(email))
			{
				throw new Exception("No existe ese mail en el sistema");
			}
			List<Publicacion> lista = new List<Publicacion>();

			foreach (Publicacion p in _publicaciones)
			{
				if (p.Autor.Email == email)
				{
					lista.Add(p);
				}
			}

			return lista;
		}
		public List<Post> ListarPostsDondeHayaComentado(string email)
		{
			if (!ExisteMailEnSistema(email))
			{
				throw new Exception("No existe ese mail en el sistema");
			}
			List<Post> ret = new List<Post>();

			foreach (Post p in GetPosts())
			{
				foreach (Comentario c in p.GetComentarios())
				{
					if (c.Autor.Email == email && !ret.Contains(p))
					{
						ret.Add(p);
					}
				}
			}
			return ret;
		}
		public List<Post> ListarPostsRealizadosEntreDosFechas(DateTime f1, DateTime f2)
		{
			List<Post> ret = new List<Post>();

			foreach (Post p in GetPosts())
			{
				if (p.Fecha >= f1 && p.Fecha <= f2)
				{
					ret.Add(p);
				}
			}
			ret.Sort();
			return ret;
		}
		public List<Miembro> ObtenerMiembrosConMasPublicaciones()
		{
			List<Miembro> miembrosConMaximasPublicaciones = new List<Miembro>();
			int max = 0;

			foreach (Miembro m in GetMiembros())
			{
				if (miembrosConMaximasPublicaciones.Count == 0)
				{
					max = CantidadDePublicacionesDeUnMiembro(m);
					miembrosConMaximasPublicaciones.Add(m);
				}
				else if (CantidadDePublicacionesDeUnMiembro(m) > max)
				{
					max = CantidadDePublicacionesDeUnMiembro(m);
					miembrosConMaximasPublicaciones.Clear();
					miembrosConMaximasPublicaciones.Add(m);
				}
				else if (CantidadDePublicacionesDeUnMiembro(m) == max)
				{
					miembrosConMaximasPublicaciones.Add(m);
				}
			}
			return miembrosConMaximasPublicaciones;
		}
		public List<Post> ListarPostsParaUsuario(int id)
		{
			List<Post> ret = new List<Post>();
			Usuario u = GetUsuario(id);

			foreach (Post p in GetPosts())
			{
				if (!p.EstaCensurado)
				{
					if (p.EsPublico || SonAmigos((Miembro)u, p.Autor) || p.Autor.Equals(u))
					{
						ret.Add(p);
					}
				}
			}

			return ret;
		}

		public List<Publicacion> BuscarPublicacion(string criterio, int num)
		{
			if(criterio == null)
			{
				throw new Exception("Debe ingresar un criterio");
			}
			List<Publicacion> ret = new List<Publicacion>();

			foreach (Publicacion p in _publicaciones)
			{
				if(p is Post)
				{
					Post post = p as Post;
					if (!post.EstaCensurado)
					{
						if (p.Contenido.ToLower().Contains(criterio.ToLower()) && p.CalcValorDeAceptacion() > num)
						{
							ret.Add(p);
						}
					}
				}
				else if (p.Contenido.ToLower().Contains(criterio.ToLower()) && p.CalcValorDeAceptacion() > num)
				{
					ret.Add(p);
				}
			}
			return ret;
		}
		public Publicacion BuscarPublicacion(int id)
		{
			foreach (Publicacion p in _publicaciones)
			{
				if (p.Id.Equals(id))
				{
					return p;
				}
			}
			return null;
		}
		public Post BuscarPost(int pid)
		{
			foreach (Post p in GetPosts())
			{
				if (p.Id.Equals(pid))
				{
					return p;
				}
			}
			return null;
		}
		public Usuario BuscarUsuario(string email, string pw)
		{
			foreach (Usuario u in _usuarios)
			{
				if (u.Email.Equals(email) && u.Contrasenia.Equals(pw))
				{
					return u;
				}
			}
			return null;
		}
		#endregion

		#region Metodos
		public void AceptarInvitacion(Invitacion i)
		{
			i.Aceptar();
		}
		public void RechazarInvitacion(Invitacion i)
		{
			i.Rechazar();
		}
		public void Reaccionar(Publicacion p, Reaccion r)
		{
			if (p.MiembroYaReacciono(r.Autor))
			{
				throw new Exception("El miembro ya reaccionó en esta publicación");
			}

			p.AgregarReaccion(r);
		}
		public void AgregarComentarioAPost(Post p, Comentario c)
		{
			if (ComentarioYaExisteEnPost(p, c))
			{
				throw new Exception("El comentario ya existe en el post");
			}
			else if (!p.EsPublico && !SonAmigos(p.Autor, c.Autor) && !p.Autor.Id.Equals(c.Autor.Id))
			{
				throw new Exception("No puede comentar en un post privado a menos que seas amigo del autor o el post sea tuyo");
			}
			AltaPublicacion(c);
			p.AgregarComentario(c);

		}
		public int CantidadDePublicacionesDeUnMiembro(Miembro m)
		{
			int contador = 0;
			foreach (Publicacion p in _publicaciones)
			{
				if (p.Autor == m)
				{
					contador++;
				}
			}
			return contador;
		}
		public void BloquearUsuario(Usuario u)
		{
			if (!u.EstaBloqueado)
			{
				u.EstaBloqueado = true;
			}
			else
			{
				throw new Exception("Usuario ya está bloqueado");
			}
		}
		public void DesbloquearUsuario(Usuario u)
		{
			if (u.EstaBloqueado)
			{
				u.EstaBloqueado = false;
			}
			else
			{
				throw new Exception("Usuario ya está desbloqueado");
			}
		}
		public void BanearPost(int id)
		{
			foreach (Post p in GetPosts())
			{
				if (p.Id.Equals(id))
				{
					p.EstaCensurado = true;
				}
			}
		}
		#endregion

		#region Validaciones 
		public bool SonAmigos(Miembro m1, Miembro m2)
		{
			foreach (Miembro m in m1.GetAmigos())
			{
				if (m.Id == m2.Id)
				{
					return true;
				}
			}
			return false;
		}
		public bool ComentarioYaExisteEnPost(Post p, Comentario c)
		{
			return p.GetComentarios().Contains(c);
		}
		public bool ExisteMailEnSistema(string email)
		{
			foreach (Usuario u in _usuarios)
			{
				if (u.Email == email)
				{
					return true;
				}
			}
			return false;
		}
		public bool LaMismaInvitacionYaExiste(Invitacion i)
		{
			foreach (Invitacion inv in _invitaciones)
			{
				bool YaExisteInvitacion = inv.MiembroSolicitado == i.MiembroSolicitado && inv.MiembroSolicitante == i.MiembroSolicitante;

				if (YaExisteInvitacion)
				{
					return true;
				}
			}
			return false;
		}
		public bool ExisteInvitacionPendienteEntreSolicitantes(Invitacion i)
		{
			foreach (Invitacion inv in _invitaciones)
			{
				if (inv.MiembroSolicitado == i.MiembroSolicitante
					&& inv.MiembroSolicitante == i.MiembroSolicitado
					&& inv.EstadoSolicitud == Estado.PENDIENTE_APROBACION)
				{
					return true;
				}
			}
			return false;
		}
		
		#endregion

		public void Precarga()
		{
			try
			{
				Usuario m1 = new Miembro("santisica29@gmail.com", "santisica", "Santiago", "Sica", new DateTime(1998, 01, 16));
				Usuario m2 = new Miembro("katebush@gmail.com", "kate1", "Kate", "Bush", new DateTime(1958, 05, 30));
				Usuario m3 = new Miembro("jeanne@gmail.com", "jeanne", "Jeanne", "D'Arc", new DateTime(1412, 01, 16));
				Usuario m4 = new Miembro("paramore@gmail.com", "paramore", "Hayley", "Williams", new DateTime(1988, 12, 02));
				Usuario m5 = new Miembro("weyesblood@gmail.com", "weyes", "Natalie", "Mering", new DateTime(1985, 09, 01));
				Usuario m6 = new Miembro("lonzo@gmail.com", "lonzo", "Lonzo", "Sica", new DateTime(2020, 02, 10));
				Usuario m7 = new Miembro("mia@gmail.com", "mia", "Mia", "Sica", new DateTime(2020, 03, 4));
				Usuario m8 = new Miembro("jh@gmail.com", "jh", "James", "Harden", new DateTime(1988, 06, 15));
				Usuario m9 = new Miembro("napo@gmail.com", "napo", "Napoleon", "Bonaparte", new DateTime(1850, 11, 24));
				Usuario m10 = new Miembro("joni@gmail.com", "joni", "Joni", "Mitchell", new DateTime(1950, 03, 22));
				Usuario m11 = new Miembro("bloqueado@gmail.com", "bloqueado", "El", "Bloqueado", new DateTime(1980, 03, 22));
				Usuario m12 = new Miembro("laura@gmail.com", "laura", "Laura", "Nyro", new DateTime(1960, 05, 25));
				m11.EstaBloqueado = true;
				AltaUsuario(m1);
				AltaUsuario(m2);
				AltaUsuario(m3);
				AltaUsuario(m4);
				AltaUsuario(m5);
				AltaUsuario(m6);
				AltaUsuario(m7);
				AltaUsuario(m8);
				AltaUsuario(m9);
				AltaUsuario(m10);
				AltaUsuario(m11);
				AltaUsuario(m12);
				Usuario a1 = new Administrador("admin@gmail.com", "admin");
				Usuario a2 = new Administrador("adminning@outlook.com", "TheBigA");
				AltaUsuario(a1);
				AltaUsuario(a2);

				Invitacion i1 = new Invitacion((Miembro)m1, (Miembro)m2);
				Invitacion i2 = new Invitacion((Miembro)m3, (Miembro)m4);
				Invitacion i3 = new Invitacion((Miembro)m5, (Miembro)m1);
				Invitacion i4 = new Invitacion((Miembro)m7, (Miembro)m8);
				Invitacion i5 = new Invitacion((Miembro)m9, (Miembro)m10);
				Invitacion i6 = new Invitacion((Miembro)m10, (Miembro)m1);
				Invitacion i7 = new Invitacion((Miembro)m4, (Miembro)m8);
				Invitacion i8 = new Invitacion((Miembro)m9, (Miembro)m2);
				Invitacion i9 = new Invitacion((Miembro)m2, (Miembro)m5);
				Invitacion i10 = new Invitacion((Miembro)m2, (Miembro)m7);
				Invitacion i11 = new Invitacion((Miembro)m6, (Miembro)m2);
				Invitacion i12 = new Invitacion((Miembro)m4, (Miembro)m2);
				Invitacion i13 = new Invitacion((Miembro)m2, (Miembro)m3);
				Invitacion i14 = new Invitacion((Miembro)m2, (Miembro)m8);
				Invitacion i15 = new Invitacion((Miembro)m10, (Miembro)m2);
				Invitacion i16 = new Invitacion((Miembro)m4, (Miembro)m1);
				Invitacion i17 = new Invitacion((Miembro)m4, (Miembro)m5);
				Invitacion i18 = new Invitacion((Miembro)m6, (Miembro)m4);
				Invitacion i19 = new Invitacion((Miembro)m4, (Miembro)m7);
				Invitacion i20 = new Invitacion((Miembro)m9, (Miembro)m4);
				Invitacion i21 = new Invitacion((Miembro)m10, (Miembro)m3);
				Invitacion i22 = new Invitacion((Miembro)m1, (Miembro)m3);
				Invitacion i23 = new Invitacion((Miembro)m9, (Miembro)m1);
				Invitacion i24 = new Invitacion((Miembro)m6, (Miembro)m1);
				Invitacion i25 = new Invitacion((Miembro)m12, (Miembro)m1);

				AltaInvitacion(i1);
				AltaInvitacion(i2);
				AltaInvitacion(i3);
				AltaInvitacion(i4);
				AltaInvitacion(i5);
				AltaInvitacion(i6);
				AltaInvitacion(i7);
				AltaInvitacion(i8);
				AltaInvitacion(i9);
				AltaInvitacion(i10);
				AltaInvitacion(i11);
				AltaInvitacion(i12);
				AltaInvitacion(i13);
				AltaInvitacion(i14);
				AltaInvitacion(i15);
				AltaInvitacion(i16);
				AltaInvitacion(i17);
				AltaInvitacion(i18);
				AltaInvitacion(i19);
				AltaInvitacion(i20);
				AltaInvitacion(i21);
				AltaInvitacion(i22);
				AltaInvitacion(i23);
				AltaInvitacion(i24);
				AltaInvitacion(i25);

				AceptarInvitacion(i1);
				AceptarInvitacion(i2);
				RechazarInvitacion(i3);
				AceptarInvitacion(i4);
				RechazarInvitacion(i5);
				AceptarInvitacion(i6);
				AceptarInvitacion(i7);
				AceptarInvitacion(i8);
				AceptarInvitacion(i9);
				AceptarInvitacion(i10);
				RechazarInvitacion(i11);
				RechazarInvitacion(i13);
				AceptarInvitacion(i15);
				AceptarInvitacion(i16);
				AceptarInvitacion(i17);
				RechazarInvitacion(i18);
				RechazarInvitacion(i19);
				AceptarInvitacion(i20);
				RechazarInvitacion(i21);
				AceptarInvitacion(i23);

				Post p1 = new Post("Saludando", (Miembro)m1, "Buen dia amigos", "greeting.jpg", true);
				Post p2 = new Post("Opinando", (Miembro)m2, "Yo no opino lo mismo", "opinion.jpg", true);
				Post p3 = new Post("Mi película", (Miembro)m3, "Juana de Arco del 1948 donde me interpretó Ingrid Bergman", "juana.png", false);
				Post p4 = new Post("Futbol", (Miembro)m8, "GOOOOL", "realmadrid.png", true);
				Post p5 = new Post("Fausto", (Miembro)m9, "Librazo escrito en rima, cuenta la lucha entre el hombre y el diablo. Escrita por Goethe", "fausto.jpg", false);
				Post p6 = new Post("Gym", (Miembro)m10, "Dia de pecho arranco con press de banca 2 sets de 6-8 reps", "mikementzer.jpg", true);
				Post p7 = new Post("Napoleon", (Miembro)m7, "Este Jueves se estrena 'Napoleon' la película a las 8:40 pm", "napoleon.jpg", false);
				Post p8 = new Post("Selfie", (Miembro)m5, "Soy como un oso pero chiquito", "lonzo.jpg", false);
				Post p9 = new Post("Album favorito", (Miembro)m12, "The Kick Inside by Kate Bush", "kick.png", true);
				p2.Fecha = new DateTime(2011, 03, 14);
				AltaPublicacion(p1);
				AltaPublicacion(p2);
				AltaPublicacion(p3);
				AltaPublicacion(p4);
				AltaPublicacion(p5);
				AltaPublicacion(p6);
				AltaPublicacion(p7);
				AltaPublicacion(p8);
				AltaPublicacion(p9);

				Comentario c1 = new Comentario("Respondiendo", (Miembro)m5, "Todo bien?");
				Comentario c2 = new Comentario("Concierto Weyes Blood", (Miembro)m6, "Alguien tiene tickets? porfa");
				Comentario c3 = new Comentario("Yo tmb opino", (Miembro)m2, "En mi opinion no");
				Comentario c4 = new Comentario("Mi pelicula favorita", (Miembro)m8, "La La Land");
				Comentario c5 = new Comentario("Claro", (Miembro)m2, "Yo comparto mi opinión con él");
				Comentario c6 = new Comentario("Mi cancion fav", (Miembro)m1, "Wuthering Heights y Blow Away");
				Comentario c7 = new Comentario("Rezar", (Miembro)m4, "Lo hago todos los días");
				Comentario c8 = new Comentario("Laptop Nueva", (Miembro)m4, "Anda medio lenta pero me gusta");
				Comentario c9 = new Comentario("Equipo nuevo", (Miembro)m4, "Con este plantel bajamos a segunda");
				Comentario c10 = new Comentario("Sube el boleto", (Miembro)m4, "Otra vez subió el boleto");
				Comentario c11 = new Comentario("VAR", (Miembro)m7, "Fue offside");
				Comentario c12 = new Comentario("Milei", (Miembro)m2, "La casta tiene miedo");
				Comentario c13 = new Comentario("Blow Away Lyrics", (Miembro)m7, "Our engineer had a different idea, from people who nearly died but survived");
				Comentario c14 = new Comentario("Mi lenguage favorito", (Miembro)m1, "Es C#");
				Comentario c15 = new Comentario("Mis perros", (Miembro)m2, "Tengo un boxer y un caniche grande");
				Comentario c16 = new Comentario("Lonzo", (Miembro)m1, "Es mi perro");
				Comentario c17 = new Comentario("Mia", (Miembro)m1, "Es mi perra");
				Comentario c18 = new Comentario("Karma", (Miembro)m10, "Karma is a God");
				Comentario c19 = new Comentario("Buena Rutina", (Miembro)m8, "Yo hago menos sets que eso #mikementzer");
				Comentario c20 = new Comentario("Emocionado", (Miembro)m8, "Ya compré entrada je");
				Comentario c21 = new Comentario("Concuerdo", (Miembro)m1, "Uno de los mejores albumes de debut que alguien pueda pedir.");

				AgregarComentarioAPost(p1, c1);
				AgregarComentarioAPost(p1, c2);
				AgregarComentarioAPost(p1, c3);
				AgregarComentarioAPost(p2, c4);
				AgregarComentarioAPost(p2, c5);
				AgregarComentarioAPost(p2, c6);
				AgregarComentarioAPost(p3, c7);
				AgregarComentarioAPost(p3, c8);
				AgregarComentarioAPost(p3, c9);
				AgregarComentarioAPost(p4, c10);
				AgregarComentarioAPost(p4, c11);
				AgregarComentarioAPost(p4, c12);
				AgregarComentarioAPost(p4, c13);
				AgregarComentarioAPost(p5, c14);
				AgregarComentarioAPost(p5, c15);
				AgregarComentarioAPost(p5, c16);
				AgregarComentarioAPost(p6, c17);
				AgregarComentarioAPost(p6, c18);
				AgregarComentarioAPost(p6, c19);
				AgregarComentarioAPost(p7, c20);
				AgregarComentarioAPost(p9, c21);

				Reaccion r1 = new Reaccion((Miembro)m1, true);
				Reaccion r2 = new Reaccion((Miembro)m3, true);
				Reaccion r3 = new Reaccion((Miembro)m5, false);
				Reaccion r4 = new Reaccion((Miembro)m7, true);
				Reaccion r5 = new Reaccion((Miembro)m9, true);
				Reaccion r6 = new Reaccion((Miembro)m10, false);
				Reaccion r7 = new Reaccion((Miembro)m2, true);
				Reaccion r8 = new Reaccion((Miembro)m6, true);
				Reaccion r9 = new Reaccion((Miembro)m3, false);
				Reaccion r10 = new Reaccion((Miembro)m12, true);
				Reaccionar(p1, r1);
				Reaccionar(p4, r5);
				Reaccionar(p6, r6);
				Reaccionar(p6, r7);
				Reaccionar(c1, r3);
				Reaccionar(c1, r8);
				Reaccionar(c2, r4);
				Reaccionar(c8, r3);
				Reaccionar(p7, r9);
				Reaccionar(c21, r10);

			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}
