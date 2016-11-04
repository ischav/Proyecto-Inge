using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoInge1.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;

namespace ProyectoInge1.Controllers
{
    public class ModuloProyectoController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        /* Método que carga el modelo para la vista Index
         * Requiere: no aplica
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {
            /*
             * Se crea el modelo:
             * Se obtiene la llave primaria del usuario actual (tabla generada por ASP) y se busca al usuario correspondiente 
             * en la base de datos (tabla de la base de datos)
             * Se verifica si el usuario actual cuenta con permisos para realizar la acción
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;
            Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-I", modelo.rolActualId);
            if (privilegio == null)
                modelo.agregar = false;
            else
                modelo.agregar = true;

            /* 
             * Se asignan los valores necesarios para la paginación y el filtrado 
             */
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            /*
             * Se asigna el valor de la primer página a la paginación, en el caso de que no se aplique un filtro y un valor
             * correspondiente al filtro en el caso de que se aplique
             */
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;

            /*
             * Se asigna a la variable proyecto, el valor de todos los proyectos de la base de datos
             */
            var proyecto = from s in baseDatos.Proyecto
                           select s;

            /* 
             * Si existe una hilera con la cual filtrar, entonces se devuelven las tuplas de la tabla Proyecto de la base
             * de datos, que cumplen con tener el valor de ese filtro en alguno de los atributos
             */
            if (!String.IsNullOrEmpty(searchString))
            {
                proyecto = proyecto.Where(s => s.Id.Contains(searchString) || s.Nombre.Contains(searchString)
                                       || s.Estado.Contains(searchString) || s.Duracion.Contains(searchString));
            }

            /*
             * De acuerdo al atributo con el que se desea ordenar, se ordena ya sea ascendente o descendentemente
             */
            switch (sortOrder)
            {
                case "name_desc":
                    switch (tipo)
                    {
                        case "Id":
                            proyecto = proyecto.OrderByDescending(s => s.Id);
                            break;
                        case "Nombre":
                            proyecto = proyecto.OrderByDescending(s => s.Nombre);
                            break;
                        case "Estado":
                            proyecto = proyecto.OrderByDescending(s => s.Estado);
                            break;
                        case "Duracion":
                            proyecto = proyecto.OrderByDescending(s => s.Duracion);
                            break;
                        default:
                            proyecto = proyecto.OrderByDescending(s => s.Id);
                            break;
                    }
                    break;
                case "Date":
                    switch (tipo)
                    {
                        case "FechaInicio":
                            proyecto = proyecto.OrderBy(s => s.FechaInicio);
                            break;
                        case "FechaFinal":
                            proyecto = proyecto.OrderBy(s => s.FechaFinal);
                            break;
                        default:
                            proyecto = proyecto.OrderBy(s => s.FechaInicio);
                            break;
                    }
                    break;
                case "date_desc":
                    switch (tipo)
                    {
                        case "FechaInicio":
                            proyecto = proyecto.OrderByDescending(s => s.FechaInicio);
                            break;
                        case "FechaFinal":
                            proyecto = proyecto.OrderByDescending(s => s.FechaFinal);
                            break;
                        default:
                            proyecto = proyecto.OrderByDescending(s => s.FechaInicio);
                            break;
                    }
                    break;
                default:
                    switch (tipo)
                    {
                        case "Id":
                            proyecto = proyecto.OrderBy(s => s.Id);
                            break;
                        case "Nombre":
                            proyecto = proyecto.OrderBy(s => s.Nombre);
                            break;
                        case "Estado":
                            proyecto = proyecto.OrderBy(s => s.Estado);
                            break;
                        case "Duracion":
                            proyecto = proyecto.OrderBy(s => s.Duracion);
                            break;
                        default:
                            proyecto = proyecto.OrderBy(s => s.Id);
                            break;
                    }
                    break;
            }

            /*
             * Se asigna un valor a la paginación y un número de página, en caso de que no sea 1
             */
            int pageSize = 7;
            int pageNumber = (page ?? 1);

            /*
             * Se retorna a la vista, el modelo con el usuarios con los cambios de paginación y filtrado
             */
            return View(proyecto.ToPagedList(pageNumber, pageSize));
        }

        //Metodo GET para la pantalla unificada. Corresponde a consultar
        public ActionResult MEC_Unificado(string id)
        {
            ModeloProyecto modeloPr = new ModeloProyecto();
            ModeloIntermedio modelo = new ModeloIntermedio();

            modeloPr.modeloProyecto = baseDatos.Proyecto.Find(id);
            obtenerUsuariosModificar(modeloPr);
            obtenerDesarrolladores(modeloPr);

            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;
            modeloPr.rolActualId = modelo.rolActualId;
            modeloPr.usuarioActualId = modelo.usuarioActualId;



            // Verificación de los privilegios disponibles en el modulo de usuarios y
            // asociadoos al rol del usuario loggeado en el sistema.
            //Cambiar por los de este modulo
            Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("PRO-M", modelo.rolActualId);
            if (privilegio == null)
            {
                modelo.modificar = false;
            }
            else
            {
                modelo.modificar = true;
            }

            privilegio = baseDatos.Privilegios_asociados_roles.Find("PRO-C", modelo.rolActualId);
            if (privilegio == null)
            {
                modelo.consultar = false;
            }
            else
            {
                modelo.consultar = true;
            }

            privilegio = baseDatos.Privilegios_asociados_roles.Find("PRO-E", modelo.rolActualId);
            if (privilegio == null)
            {
                modelo.eliminar = false;
            }
            else
            {
                modelo.eliminar = true;
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (modeloPr.modeloProyecto == null)
            {
                return HttpNotFound();
            }
            else
            {
                //Se obtiene el email de AspNetUsers

            }

            return View(modeloPr);
        }

        //Metodo POST para la pantalla unificada. Corresponde a modificar

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_Unificado(ModeloProyecto modelo, string lider, string[] equipoDesarrollo, string[] equipoDesarrolloNuevo)
        {
            if (ModelState.IsValid)
            {

                for (int i = 0; i < equipoDesarrollo.Count(); i++)
                {
                    modelo.listaUsuarios_asociados_proyecto.Add(
                        new Usuarios_asociados_proyecto
                        {
                            IdUsuario = equipoDesarrollo[i],
                            IdProyecto = modelo.modeloProyecto.Id,
                            RolProyecto = "Desarrollador"
                        });
                }

                modelo.listaUsuarios_asociados_proyecto.Add(
                new Usuarios_asociados_proyecto
                {
                    IdUsuario = lider,
                    IdProyecto = modelo.modeloProyecto.Id,
                    RolProyecto = "Líder"
                });



                var listaUsuarios = (from usuario in baseDatos.Usuario
                                     join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                     where usrProy.IdProyecto == modelo.modeloProyecto.Id
                                     select new { usuario });

                var us = new List<Usuarios_asociados_proyecto>();


                for (int i = 0; i < modelo.listaUsuarios_asociados_proyecto.Count(); i++)
                {
                    bool enBase = false;
                    foreach (var usr in listaUsuarios)
                    {

                        if (usr.usuario.Id == modelo.listaUsuarios_asociados_proyecto[i].IdUsuario)
                        {
                            enBase = true;
                        }
                    }

                    if (!enBase)
                    {
                        if (modelo.listaUsuarios_asociados_proyecto[i].IdUsuario != null)
                        {
                            baseDatos.Usuarios_asociados_proyecto.Add(modelo.listaUsuarios_asociados_proyecto[i]);
                            us.Add(modelo.listaUsuarios_asociados_proyecto[i]);
                        }
                    }
                }

                baseDatos.Entry(modelo.modeloProyecto).State = EntityState.Modified;
                baseDatos.SaveChanges();
            }
            else
            {
                modelo.cambiosGuardados = 1;
            }

            return RedirectToAction("MEC_Unificado");
        }

        public ActionResult eliminarProyecto(string Id)
        {

            //Borra el proyecto de la tabla Proyectos
            ModeloProyecto modelo = new ModeloProyecto();
            modelo.modeloProyecto = baseDatos.Proyecto.Find(Id);
            if (modelo.modeloProyecto.Estado.Equals("Finalizado"))
            {
                baseDatos.Proyecto.Remove(modelo.modeloProyecto);
                baseDatos.SaveChanges();
            }
            else
            {
                return HttpNotFound();
            }


            return RedirectToAction("Index");
        }

        //Metodo GET pantalla Crear Proyecto
        public ActionResult Create() {
			ModeloProyecto modelo = new ModeloProyecto();
			obtenerUsuarios(modelo);

			return View(modelo);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(ModeloProyecto modelo, string aceptar, string lider, string[] equipoDesarrollo) {

			if(!string.IsNullOrEmpty(aceptar)) {
				if(ModelState.IsValid) {
				for(int i = 0; i < equipoDesarrollo.Count(); i++) {
					modelo.listaUsuarios_asociados_proyecto.Add(
						new Usuarios_asociados_proyecto {
							IdUsuario = equipoDesarrollo[i],
							IdProyecto = modelo.modeloProyecto.Id,
							RolProyecto = "Desarrollador"
						});
				}

				modelo.listaUsuarios_asociados_proyecto.Add(
				new Usuarios_asociados_proyecto {
					IdUsuario = lider,
					IdProyecto = modelo.modeloProyecto.Id,
					RolProyecto = "Líder"
				});

				baseDatos.Usuarios_asociados_proyecto.AddRange(modelo.listaUsuarios_asociados_proyecto);
				baseDatos.Proyecto.Add(modelo.modeloProyecto);
				try {
					baseDatos.SaveChanges();
				}
				catch { }
			}
			}

			ModeloProyecto nuevoModelo = new ModeloProyecto();
			obtenerUsuarios(nuevoModelo);
			//return View(nuevoModelo);
			return RedirectToAction("Create");
		}

		private void obtenerUsuarios(ModeloProyecto modelo) {
			var listaUsuarios = baseDatos.Usuario.ToList();
			var clientes = new List<Usuario>();
			var recursos = new List<Usuario>();
			var lideres = new List<Usuario>();

			foreach(var usr in listaUsuarios) {
				if(context.Users.Find(usr.Id).Roles.First().RoleId == "03User") {
					clientes.Add(usr);
				} else if(context.Users.Find(usr.Id).Roles.First().RoleId == "02Develop") {
					recursos.Add(usr);
					lideres.Add(usr);
				}
			}

			ViewBag.listaClientes = clientes;
			ViewBag.listaRecursos = recursos;
			ViewBag.listaDesarrolladores = new List<Usuario>();
		}

        private void obtenerDesarrolladores(ModeloProyecto modelo)
        {

            var usuariosAsociadosProyectos = new List<Usuario>();

            var listaUsuarios = (from usuario in baseDatos.Usuario
                                 join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                 where usrProy.IdProyecto == modelo.modeloProyecto.Id && usrProy.RolProyecto.Equals("Desarrollador")
                                 select new { usuario });

            foreach (var usr in listaUsuarios)
            {
                usuariosAsociadosProyectos.Add(usr.usuario);
            }
            ViewBag.listaDesarrolladores = usuariosAsociadosProyectos;
        }

        private void obtenerUsuariosModificar(ModeloProyecto modelo)
        {

            var listaUsuarios = baseDatos.Usuario.ToList();
            var clientes = new List<Usuario>();
            var recursos = new List<Usuario>();
            var lideres = new List<Usuario>();
            var lider = new List<Usuario>();
            var cliente = new List<Usuario>();

            var listaRecursos = (from usuario in baseDatos.Usuario
                                 join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                 where usrProy.IdProyecto == modelo.modeloProyecto.Id
                                 select new { usuario });

            foreach (var usr in listaUsuarios)
            {
                if (context.Users.Find(usr.Id).Roles.First().RoleId == "03User")
                {
                    clientes.Add(usr);
                }
                else if (context.Users.Find(usr.Id).Roles.First().RoleId == "02Develop")
                {
                    lideres.Add(usr);
                    recursos.Add(usr);

                }
            }

            foreach (var usr in listaRecursos)
            {
                recursos.Remove(usr.usuario);
            }

            var lid = (from usuario in baseDatos.Usuario
                       join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                       where usrProy.IdProyecto == modelo.modeloProyecto.Id && usrProy.RolProyecto == "Líder"
                       select new { usuario });

            var cli = (from usuario in baseDatos.Usuario
                       join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                       where usrProy.IdProyecto == modelo.modeloProyecto.Id && usrProy.RolProyecto == "Cliente"
                       select new { usuario });

            if (lid.Count() > 0)
            {
                lider.Add(lid.First().usuario);
                modelo.liderId = lid.First().usuario.Id;
            }
            if (cli.Count() > 0)
            {
                cliente.Add(cli.First().usuario);
            }
            ViewBag.listaClientes = cliente;
            ViewBag.listaRecursos = recursos;
            ViewBag.listaDesarrolladores = new List<Usuario>();
            ViewBag.listaLideres = lider;
        }



    }
}