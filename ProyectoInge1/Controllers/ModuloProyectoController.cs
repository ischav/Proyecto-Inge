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
	public class ModuloProyectoController : Controller {
		Entities baseDatos = new Entities();
		ApplicationDbContext context = new ApplicationDbContext();

		/* Método que carga el modelo para la vista Index
         * Requiere: no aplica
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
		[Authorize]
		[Permisos("PRO-I")]
        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {

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
            String id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            String rol = context.Users.Find(id).Roles.First().RoleId;
            var proyecto = from Proyecto p in baseDatos.Proyecto
                           select p;

            if (!(rol == "01Admin"))
            {
                String id_usuario = User.Identity.GetUserId();
                proyecto = from Proyecto p in baseDatos.Proyecto
                           join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on
                            p.Id equals USP.IdProyecto
                           where USP.IdUsuario == id_usuario
                           select p;
            }

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


		/*
		*Metodo GET para la pantalla unificada. Corresponde a consultar	
		*Requiere: Que el Id del proyecto sea enviado como parámetro
		*Modifica: no aplica
		*Retorna: el modelo de proyecto cargado
		*/
		[Authorize]
		[Permisos("PRO-M","PRO-E", "PRO-C")]
		public ActionResult MEC_Unificado(string id)
        {
            ModeloProyecto modeloPr = new ModeloProyecto();
            ModeloIntermedio modelo = new ModeloIntermedio();

            modeloPr.modeloProyecto = baseDatos.Proyecto.Find(id);
            /*
	    *Se cargan los modelos de usuarios asociados al proyecto
	    */
            obtenerUsuariosModificar(modeloPr);
            obtenerDesarrolladores(modeloPr);
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
        /*
            *Metodo POST para la pantalla unificada. Corresponde a modificar	
        *Requiere: que se envíe el modelo de proyecto con los campos modificados, además que teng el equipo de desarrollo y los recuros
        *Modifica: la base de datos, y actualiza los datos que han sido cambiados. Asocia y desasocia miembors del equipo
        *Retorna: el modelo con los datos actualizados
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_Unificado(ModeloProyecto modelo, string lider, string[] equipoDesarrollo, string[] recursos,string cliente)
        {
            if (ModelState.IsValid)
            {
                /*
            *Se carga el modelo con de usuarios asociados con la lista
            *de equipo de desarollo y con los de recursos
            */

                /*for (int i = 0; i < equipoDesarrollo.Count(); i++)
                {
                    modelo.listaUsuarios_asociados_proyecto.Add(
                        new Usuarios_asociados_proyecto
                        {
                            IdUsuario = equipoDesarrollo[i],
                            IdProyecto = modelo.modeloProyecto.Id,
                            RolProyecto = "Desarrollador"
                        });
                }*/

                for (int i = 0; i < equipoDesarrollo.Count(); i++)
                {
                    //se evita agregar al lider dos veces al proyecto
                    if (equipoDesarrollo[i] != lider)
                    {
                        modelo.listaUsuarios_asociados_proyecto.Add(
                            new Usuarios_asociados_proyecto
                            {
                                IdUsuario = equipoDesarrollo[i],
                                IdProyecto = modelo.modeloProyecto.Id,
                                RolProyecto = "Desarrollador"
                            });
                    }
                }

                Usuarios_asociados_proyecto usrLider = obtenerLiderClienteAsociado(modelo.modeloProyecto.Id, "Líder");

                ModeloProyecto modeloPr = new ModeloProyecto();

                if (recursos != null)
                {
                    for (int i = 0; i < recursos.Count(); i++)
                    {

                        modeloPr.listaUsuarios_asociados_proyecto.Add(
                            new Usuarios_asociados_proyecto
                            {
                                IdUsuario = recursos[i],
                                IdProyecto = modelo.modeloProyecto.Id,
                                RolProyecto = "Desarrollador"
                            });
                    }

                }

                //si no es igual al lider, es porque lo cambiaron                
                if (!lider.Equals(usrLider.IdUsuario))
                {
                    baseDatos.Usuarios_asociados_proyecto.Remove(usrLider); //borra al actual de la base
                    Usuarios_asociados_proyecto usuarioLider = new Usuarios_asociados_proyecto();
                    usuarioLider.IdProyecto = modelo.modeloProyecto.Id;
                    usuarioLider.IdUsuario = lider;
                    usuarioLider.RolProyecto = "Líder";
                    baseDatos.Usuarios_asociados_proyecto.Add(usuarioLider);
                }

                Usuarios_asociados_proyecto usrCliente = obtenerLiderClienteAsociado(modelo.modeloProyecto.Id, "Cliente");
                //si no es igual al lider, es porque lo cambiaron                
                if (!cliente.Equals(usrCliente.IdUsuario))
                {
                    baseDatos.Usuarios_asociados_proyecto.Remove(usrCliente); //borra al actual de la base
                    Usuarios_asociados_proyecto usuarioCliente = new Usuarios_asociados_proyecto();
                    usuarioCliente.IdProyecto = modelo.modeloProyecto.Id;
                    usuarioCliente.IdUsuario = cliente;
                    usuarioCliente.RolProyecto = "Cliente";
                    baseDatos.Usuarios_asociados_proyecto.Add(usuarioCliente);
                }

                /*
                *Se trae de la base de datos los usuarios asociados al proyecto
                */
                var listaUsuarios = (from usuario in baseDatos.Usuario
                                     join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                     where usrProy.IdProyecto == modelo.modeloProyecto.Id
                                     select new { usuario });

                var us = new List<Usuarios_asociados_proyecto>();

                /*
                *Si el usuarios está asociado al proyecto pero no está en el equipo de 
                *desarrollo modificado, se quita de la base de datos
                */
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

                Usuarios_asociados_proyecto usuarioBorrar = new Usuarios_asociados_proyecto();

                if (recursos != null)
                {
                    for (int i = 0; i < recursos.Count(); i++)
                    {
                        foreach (var usr in listaUsuarios)
                        {

                            if (usr.usuario.Id == modeloPr.listaUsuarios_asociados_proyecto[i].IdUsuario)
                            {
                                usuarioBorrar = baseDatos.Usuarios_asociados_proyecto.Find(usr.usuario.Id, modelo.modeloProyecto.Id);
                                //baseDatos.Usuarios_asociados_proyecto.Remove(modeloPr.listaUsuarios_asociados_proyecto[i]);
                                if (usuarioBorrar != null)
                                {
                                    baseDatos.Usuarios_asociados_proyecto.Remove(usuarioBorrar);
                                }
                            }
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

        /*
        *Requiere: el id del proyecto
        *Modifica: la base de datos eliminando el proyecto asociado
        *Retorna: el modelo en la pantalla del index
        */
        public ActionResult eliminarProyecto(string Id)
        {

            ModeloProyecto modelo = new ModeloProyecto();
            modelo.modeloProyecto = baseDatos.Proyecto.Find(Id);
	        /*
	        *Revisa si el estado del proyecto está finalizado, en caso  contrario
	        *muestra una alerta
	        */
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

       	/* Método que carga el modelo con los datos para desplegar en la vista de crear proyecto
	 * Requiere: no aplica
	 * Modifica: no aplica
	 * Retorna: el modelo con los datos cargados de la base
	 */
        public ActionResult Create() {
			ModeloProyecto modelo = new ModeloProyecto();
			obtenerUsuarios(modelo);

			return View(modelo);
		}
		
		/* Método que guarda en la base los datos de un nuevo proyecto y sus usuarios asociados
		 * Requiere: El modelo, el lider de proyecto, la lista de desarrolladores y una indicacion de que se quiere guardar los datos
		 * Modifica: la base de datos y el modelo
		 * Retorna: no aplica
		 */
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(ModeloProyecto modelo, string aceptar, string lider, string[] equipoDesarrollo) {

			if(!string.IsNullOrEmpty(aceptar)) {
				if(ModelState.IsValid) {
				
				/*
				 * se obtienen los desarrolladores que se asociaran al proyecto
				 */
				for(int i = 0; i < equipoDesarrollo.Count(); i++) {
					//se evita agregar al lider dos veces al proyecto
					if(equipoDesarrollo[i] != lider) {
						modelo.listaUsuarios_asociados_proyecto.Add(
							new Usuarios_asociados_proyecto {
								IdUsuario = equipoDesarrollo[i],
								IdProyecto = modelo.modeloProyecto.Id,
								RolProyecto = "Desarrollador"
							});
					}
				}
				
				/*
				 * se obtiene el lider del proyecto
				 */
				modelo.listaUsuarios_asociados_proyecto.Add(
				new Usuarios_asociados_proyecto {
					IdUsuario = lider,
					IdProyecto = modelo.modeloProyecto.Id,
					RolProyecto = "Líder"
				});
				
				/*
				 * se obtiene el cliente aosciado al proyecto
				 */
				modelo.listaUsuarios_asociados_proyecto.Add(
				new Usuarios_asociados_proyecto {
					IdUsuario = modelo.cliente,
					IdProyecto = modelo.modeloProyecto.Id,
					RolProyecto = "Cliente"
				});
				
				/*
				* se guardan los datos del proyecto junto con los desarrolladores asociados obtenidos anteriormente
				*/ 
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
			return RedirectToAction("Create");
		}
		
		/* Método que carga listas con clientes y recursos para desplegar en la vista
		 * Requiere: un objeto del modelo
		 * Modifica: el modelo que se envia como parametro a la vista
		 * Retorna: no aplica
		 */
		private void obtenerUsuarios(ModeloProyecto modelo) {
			var listaUsuarios = baseDatos.Usuario.ToList();// se cargan todos los usuarios de la base
			var clientes = new List<Usuario>();
			var recursos = new List<Usuario>();
			var lideres = new List<Usuario>();
			
			/*
			 * se clasifican los usuarios segun rol
			 */
			foreach(var usr in listaUsuarios) {
				if(context.Users.Find(usr.Id).Roles.First().RoleId == "03User") {
					clientes.Add(usr);
				} else if(context.Users.Find(usr.Id).Roles.First().RoleId == "02Develop") {
					recursos.Add(usr);
					lideres.Add(usr);
				}
			}
			
			/*
			 * se pasan las listas de usuarios clasificados segun rol a la vista
			 */
			ViewBag.listaClientes = clientes;
			ViewBag.listaRecursos = recursos;
			ViewBag.listaDesarrolladores = new List<Usuario>();
		}

	/* Método que carga listas con Desarroladores
	 * Requiere: un objeto del modelo
	 * Modifica: el modelo que se envia como parametro a la vista
	 * Retorna: no aplica
	 */
        private void obtenerDesarrolladores(ModeloProyecto modelo)
        {

            var usuariosAsociadosProyectos = new List<Usuario>();

            var listaUsuarios = (from usuario in baseDatos.Usuario
                                 join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                 where usrProy.IdProyecto == modelo.modeloProyecto.Id && usrProy.RolProyecto.Equals("Desarrollador")
                                 select new { usuario });
	    
	    /*
	    *Agrega los desarrolladores asociados al proyecto
	    */
            foreach (var usr in listaUsuarios)
            {
                usuariosAsociadosProyectos.Add(usr.usuario);
            }
            ViewBag.listaDesarrolladores = usuariosAsociadosProyectos;
        }

	/* Método que carga los desarrolladoresmodificados y los desasocia
	 * Requiere: un objeto del modelo
	 * Modifica: el modelo que se envia como parametro a la vista
	 * Retorna: no aplica
	 */
        private void obtenerUsuariosModificar(ModeloProyecto modelo)
        {

            var listaUsuarios = baseDatos.Usuario.ToList();
            var clientes = new List<Usuario>();
            var recursos = new List<Usuario>();
            var lideres = new List<Usuario>();

            var listaRecursos = (from usuario in baseDatos.Usuario
                                 join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                 where usrProy.IdProyecto == modelo.modeloProyecto.Id
                                 select new { usuario });

            var lid = (from usuario in baseDatos.Usuario
                       join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                       where usrProy.IdProyecto == modelo.modeloProyecto.Id && usrProy.RolProyecto == "Líder"
                       select new { usuario });


            var cli = (from usuario in baseDatos.Usuario
                       join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                       where usrProy.IdProyecto == modelo.modeloProyecto.Id && usrProy.RolProyecto == "Cliente"
                       select new { usuario });
            /*
            * Agrega cada usuario al modelo
            */

            if (lid.Count() > 0)
            {
                lideres.Add(lid.First().usuario);
                modelo.liderId = lid.First().usuario.Id;
            }
            if (cli.Count() > 0)
            {
                clientes.Add(cli.First().usuario);
            }

            foreach (var usr in listaUsuarios)
            {
                if (context.Users.Find(usr.Id).Roles.First().RoleId == "03User")
                {
                    if (usr.Id != cli.First().usuario.Id)
                    {
                        clientes.Add(usr);
                    }
                }
                else if (context.Users.Find(usr.Id).Roles.First().RoleId == "02Develop")
                {
                    if (usr.Id != lid.First().usuario.Id)
                    {
                        lideres.Add(usr);
                    }
                    recursos.Add(usr);

                }
            }

            foreach (var usr in listaRecursos)
            {
                if (usr.usuario.Id != lid.First().usuario.Id) { //que no borre el lider de la lista de recursos
                    recursos.Remove(usr.usuario);
                }
            }


            /*
             * se pasan las listas de usuarios clasificados segun rol a la vista
            */

            ViewBag.listaClientes = clientes;
            ViewBag.listaRecursos = recursos;
            ViewBag.listaLideres = lideres;

        }

        private Usuarios_asociados_proyecto obtenerLiderClienteAsociado(string proyectoId, string rol)
        {
            var lid = (from usuario in baseDatos.Usuario
                       join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                       where usrProy.IdProyecto == proyectoId && usrProy.RolProyecto == rol
                       select new { usrProy });

            return lid.First().usrProy;
        }

    }
}
