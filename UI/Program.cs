using Dominio;
using Dominio.Models;

namespace UI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sistema s = Sistema.GetInstancia();

            int op = -1;

            while (op != 0)
            {
                Console.Clear();
                Console.WriteLine("    Bienvenido!");
                Console.WriteLine("    Seleccione una opción por favor.");
                Console.WriteLine("1 - Alta de miembro.");
                Console.WriteLine("2 - Listar todas las publicaciones que ha realizado un miembro.");
                Console.WriteLine("3 - Listar posts donde un miembro haya comentado.");
                Console.WriteLine("4 - Listar posts realizados entre dos fechas.");
                Console.WriteLine("5 - Obtener los miembros que hayan hecho más publicaciones.");
                Console.WriteLine("0 - Salir.");
                

                op = int.Parse(Console.ReadLine());

                if (op.Equals(1))
                {
                    try
                    {
                        Console.WriteLine("Ingrese un email");
                        string email = Console.ReadLine();
                        Console.WriteLine("Ingrese una contraseña");
                        string contrasenia = Console.ReadLine();
                        Console.WriteLine("Ingrese un nombre");
                        string nombre = Console.ReadLine();
                        Console.WriteLine("Ingrese un apellido");
                        string apellido = Console.ReadLine();
                        Console.WriteLine("Ingrese una fecha de nacimiento");
                        DateTime fechaNac = DateTime.Parse(Console.ReadLine());

                        Miembro nuevoMiembro = new Miembro(email, contrasenia, nombre, apellido, fechaNac);
                        s.AltaUsuario(nuevoMiembro);
                        Console.WriteLine("Alta de miembro correcta.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
                else if (op.Equals(2))
                {
                    try
                    {
                        Console.WriteLine("Ingrese el mail del miembro");
                        string email = Console.ReadLine();
                        List<Publicacion> listaPublicacionDeMiembro = s.ListarPublicacionesDeMiembro(email);
                        if (listaPublicacionDeMiembro.Count > 0)
                        {
                            foreach (Publicacion p in listaPublicacionDeMiembro)
                            {
                                Console.WriteLine(p);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No existen publicaciones de ese miembro.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                else if (op.Equals(3))
                {
                    try
                    {
                        Console.WriteLine("Ingrese el mail del miembro");
                        string email = Console.ReadLine();
                        List<Post> listaPostsDondeMiembroComento = s.ListarPostsDondeHayaComentado(email);
                        if (listaPostsDondeMiembroComento.Count > 0)
                        {
                            foreach (Publicacion p in listaPostsDondeMiembroComento)
                            {
                                Console.WriteLine(p);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No existen posts donde haya comentado ese miembro.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (op.Equals(4))
                {
                    try
                    {
                        Console.WriteLine("Ingrese la fecha 1");
                        DateTime f1 = DateTime.Parse(Console.ReadLine());
                        Console.WriteLine("Ingrese la fecha 2");
                        DateTime f2 = DateTime.Parse(Console.ReadLine());

                        List<Post> postsRealizadosEntre2Fechas = s.ListarPostsRealizadosEntreDosFechas(f1, f2);

                        if (postsRealizadosEntre2Fechas.Count > 0)
                        {
                            foreach (Post p in postsRealizadosEntre2Fechas)
                            {
                                Console.WriteLine(p);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No hay posts realizados entre esas fechas.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (op.Equals(5))
                {
                    try
                    {
                        List<Miembro> listaDeMaximos = s.ObtenerMiembrosConMasPublicaciones();

                        if (listaDeMaximos.Count > 0)
                        {
                            foreach (Miembro m in listaDeMaximos)
                            {
                                Console.WriteLine(m);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No hay publicaciones.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (op.Equals(6))
                {
                    List<Publicacion> lista = s.GetPublicaciones();

                    foreach(Publicacion pub in lista)
                    {
                        if (pub is Post)
                        {
                            Console.WriteLine($"Post: VA:{pub.CalcValorDeAceptacion()}");
                        }
                        else if (pub is Comentario)
                        {
                            Console.WriteLine($"Com: Va - {pub.CalcValorDeAceptacion()}");
                        }
                    }
                }
                else if(op != 0)
                {
                    Console.WriteLine("Opción inválida");
                }
                Console.ReadKey();
            }

        }
    }
}
