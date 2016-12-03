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
using System.Text;

namespace ProyectoInge1.Controllers
{
    public class ModuloRequerimientosController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        /* Método que carga el modelo para la vista Index
         * Requiere: Tipo de ordenamiento, tipo de filtrado, patron de búsqueda, número de página y id del proyecto
         * Modifica: el modelo
         * Retorna: el modelo de páginación cargado
         */
		 [Authorize]
		 [Permisos("GRF-I")]
        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page, string Proyecto)
        {
			String id;
			String rol;
			IQueryable<Proyecto> proyecto;

			try {
				id = System.Web.HttpContext.Current.User.Identity.GetUserId();
				rol = context.Users.Find(id).Roles.First().RoleId;
				proyecto = from Proyecto p in baseDatos.Proyecto
						   select p;

				if(!(rol == "01Admin")) {
					String id_usuario = User.Identity.GetUserId();
					proyecto = from Proyecto p in baseDatos.Proyecto
							   join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on
								p.Id equals USP.IdProyecto
							   where USP.IdUsuario == id_usuario
							   select p;
				}
			}
			catch {
				return mostrarError("Error al intentar acceder a la base de datos");
			}

            ViewBag.pro = new SelectList(proyecto, "Id", "Nombre", Proyecto);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var requerimiento = from s in baseDatos.Requerimiento
                                select s;

            requerimiento = requerimiento.Where(s => s.IdProyecto.Equals(Proyecto));

            if (!String.IsNullOrEmpty(searchString))
            {
                requerimiento = requerimiento.Where(s => s.Id.Contains(searchString) || s.Nombre.Contains(searchString)
                                             || s.Prioridad.Contains(searchString) || s.Esfuerzo.Contains(searchString)
                                             || s.Estado.Contains(searchString) || s.Sprint.Contains(searchString)
                                             || s.Modulo.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    switch (tipo)
                    {
                        case "Id":
                            requerimiento = requerimiento.OrderByDescending(s => s.Id);
                            break;
                        case "Nombre":
                            requerimiento = requerimiento.OrderByDescending(s => s.Nombre);
                            break;
                        case "Prioridad":
                            requerimiento = requerimiento.OrderByDescending(s => s.Prioridad);
                            break;
                        case "Esfuerzo":
                            requerimiento = requerimiento.OrderByDescending(s => s.Esfuerzo);
                            break;
                        case "Sprint":
                            requerimiento = requerimiento.OrderByDescending(s => s.Sprint);
                            break;
                        case "Modulo":
                            requerimiento = requerimiento.OrderByDescending(s => s.Modulo);
                            break;
                        default:
                            requerimiento = requerimiento.OrderByDescending(s => s.Id);
                            break;
                    }
                    break;
                case "Date":
                    switch (tipo)
                    {
                        case "FechaInicio":
                            requerimiento = requerimiento.OrderBy(s => s.FechaInicio);
                            break;
                        case "FechaFinal":
                            requerimiento = requerimiento.OrderBy(s => s.FechaFinal);
                            break;
                        default:
                            requerimiento = requerimiento.OrderBy(s => s.FechaInicio);
                            break;
                    }
                    break;
                case "date_desc":
                    switch (tipo)
                    {
                        case "FechaInicio":
                            requerimiento = requerimiento.OrderByDescending(s => s.FechaInicio);
                            break;
                        case "FechaFinal":
                            requerimiento = requerimiento.OrderByDescending(s => s.FechaFinal);
                            break;
                        default:
                            requerimiento = requerimiento.OrderByDescending(s => s.FechaInicio);
                            break;
                    }
                    break;
                default:
                    switch (tipo)
                    {
                        case "Id":
                            requerimiento = requerimiento.OrderBy(s => s.Id);
                            break;
                        case "Nombre":
                            requerimiento = requerimiento.OrderBy(s => s.Nombre);
                            break;
                        case "Prioridad":
                            requerimiento = requerimiento.OrderBy(s => s.Prioridad);
                            break;
                        case "Esfuerzo":
                            requerimiento = requerimiento.OrderBy(s => s.Esfuerzo);
                            break;
                        case "Sprint":
                            requerimiento = requerimiento.OrderBy(s => s.Sprint);
                            break;
                        case "Modulo":
                            requerimiento = requerimiento.OrderBy(s => s.Modulo);
                            break;
                        default:
                            requerimiento = requerimiento.OrderBy(s => s.Id);
                            break;
                    }
                    break;

            }

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(requerimiento.ToPagedList(pageNumber, pageSize));
        }

		[Authorize]
        public ActionResult Create(String proyecto)
        {
            List<SelectListItem> clientesDropDown = new List<SelectListItem>();
            List<SelectListItem> responsablesDropDown = new List<SelectListItem>();

			IQueryable<Usuario> clientes;
			IQueryable<Usuario> desarrolladores;
			try {
				clientes = from Usuario U in baseDatos.Usuario
							   join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
							   where USP.IdProyecto == proyecto && USP.RolProyecto == "Cliente"
							   select U;

				desarrolladores = from Usuario U in baseDatos.Usuario
									  join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
									  where USP.IdProyecto == proyecto && USP.RolProyecto == "Desarrollador"
									  select U;
			}catch{
				return mostrarError("Error al intentar acceder a la base de datos");
			}


            foreach (var cl in clientes)
                clientesDropDown.Add(new SelectListItem() { Text = cl.NombreCompleto, Value = cl.Id });

            foreach (var ds in desarrolladores)
                responsablesDropDown.Add(new SelectListItem() { Text = ds.NombreCompleto, Value = ds.Id });

            ViewBag.Solicitante = clientesDropDown;
            ViewBag.Responsable = responsablesDropDown;

            var requerimientos = new Requerimiento();
            requerimientos.IdProyecto = proyecto;
            requerimientos.agregarCriterios(0);
            requerimientos.cambiosGuardados = 0;
            return View(requerimientos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Prioridad,Esfuerzo,Estado,Descripcion,FechaInicio,FechaFinal,Sprint,Modulo,Observaciones,Imagen,IdProyecto,IdResponsable,IdSolicitante,Version,rutaImagen,CriterioAceptacion")] Requerimiento modelo)
        {
            var num = 1;
            foreach (CriterioAceptacion criterio in modelo.CriterioAceptacion.ToList())
            {
                if (criterio.Borrar == true)
                    modelo.CriterioAceptacion.Remove(criterio);
                else
                {
                    // Le da un valor generado al Escenario de Criterio de Aceptación
                    criterio.Escenario = num.ToString();
                    num++;
                    criterio.IdProyecto = modelo.IdProyecto;
                    criterio.IdRequerimiento = modelo.Id;
                }
            }

            if (!string.IsNullOrEmpty(modelo.rutaImagen))
                modelo.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);

            modelo.Version = 1;

            List<SelectListItem> clientesDropDown = new List<SelectListItem>();
            List<SelectListItem> responsablesDropDown = new List<SelectListItem>();

            var clientes = from Usuario U in baseDatos.Usuario
                           join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
                           where USP.IdProyecto == modelo.IdProyecto && USP.RolProyecto == "Cliente"
                           select U;

            var desarrolladores = from Usuario U in baseDatos.Usuario
                                  join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
                                  where USP.IdProyecto == modelo.IdProyecto && USP.RolProyecto == "Desarrollador"
                                  select U;


            foreach (var cl in clientes)
                clientesDropDown.Add(new SelectListItem() { Text = cl.NombreCompleto, Value = cl.Id });

            foreach (var ds in desarrolladores)
                responsablesDropDown.Add(new SelectListItem() { Text = ds.NombreCompleto, Value = ds.Id });

            ViewBag.Solicitante = clientesDropDown;
            ViewBag.Responsable = responsablesDropDown;

            if (ModelState.IsValid)
            {
                String id = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var cambio_historial = new Cambio();
                cambio_historial.IdRequerimiento = modelo.Id;
                cambio_historial.IdProyecto = modelo.IdProyecto;
                cambio_historial.Nombre = modelo.Nombre;
                cambio_historial.Prioridad = modelo.Prioridad;
                cambio_historial.Esfuerzo = modelo.Esfuerzo;
                cambio_historial.Estado = modelo.Estado;
                cambio_historial.Descripcion = modelo.Descripcion;
                cambio_historial.FechaInicio = modelo.FechaInicio;
                cambio_historial.FechaFinal = modelo.FechaFinal;
                cambio_historial.Sprint = modelo.Sprint;
                cambio_historial.Modulo = modelo.Modulo;
                cambio_historial.Observaciones = modelo.Observaciones;
                if (!string.IsNullOrEmpty(modelo.rutaImagen))
                    cambio_historial.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
                cambio_historial.IdResponsable = modelo.IdResponsable;
                cambio_historial.IdSolicitante = modelo.IdSolicitante;
                cambio_historial.Version = modelo.Version;
                cambio_historial.DescripcionCambio = "Cambio inicial";
                cambio_historial.JustificacionCambio = "Cambio inicial";
                cambio_historial.SolicitanteCambio = id;
                cambio_historial.FechaCambio = DateTime.Today;
                cambio_historial.EstadoSolicitud = "Aprobado";
                cambio_historial.ObservacionesSolicitud = "Cambio inicial";
                cambio_historial.VersionCambio = 1;

                baseDatos.Requerimiento.Add(modelo);
                baseDatos.SaveChanges();
                baseDatos.Cambio.Add(cambio_historial);
                baseDatos.SaveChanges();

                foreach (CriterioAceptacion criterio in modelo.CriterioAceptacion.ToList())
                {
                    var c = new CriterioAceptacionHistorial();

                    c.IdSolicitud = cambio_historial.IdSolicitud;
                    c.IdProyecto = criterio.IdProyecto;
                    c.IdRequerimiento = criterio.IdRequerimiento;
                    c.Escenario = criterio.Escenario;
                    c.Descripcion = criterio.Descripcion;
                    cambio_historial.CriterioAceptacionHistorial.Add(c);
                }

                baseDatos.Entry(cambio_historial).State = EntityState.Modified;
                baseDatos.SaveChanges();
                modelo.cambiosGuardados = 1;
                return View(modelo);
            }

            modelo.cambiosGuardados = 1;
            return View(modelo);
        }

        /* Método elimina un elemento en la vista Index
         * Requiere: parámetro recibido válido en la base de datos
         * Modifica: la tabla Requerimientos de la base de datos
         * Retorna: redirección a la vista Index
         */
		 [Authorize]
        public ActionResult eliminarRequerimiento(string Id, string IdProyecto)
        {
            //Borra al requerimiento de la tabla Requerimientos
            ModeloProyecto modelo = new ModeloProyecto();
            modelo.modeloRequerimiento = baseDatos.Requerimiento.Find(Id, IdProyecto);
            if (modelo.modeloRequerimiento.Estado.Equals("Pendiente de asignar"))
            {
                baseDatos.Requerimiento.Remove(modelo.modeloRequerimiento);
                baseDatos.SaveChanges();
            }
            else
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        /* Método para consultar un requerimiento para la vista MEC_UnificadoRequerimientos
         * Requiere: parámetro recibido válido en la base de datos
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
		 [Authorize]
		 [Permisos("GRF-M", "GRF-E", "GCS-CS")]
        public ActionResult MEC_UnificadoRequerimientos(string Id, string IdProyecto)
        {
            /*
             * Se crea el modelo:
             * Se obtiene la llave primaria del usuario actual (tabla generada por ASP) y se busca al usuario correspondiente 
             * en la base de datos (tabla de la base de datos)
             * Se verifica si el usuario actual cuenta con permisos para realizar las acciones
             */

            ModeloProyecto modelo = new ModeloProyecto();
            try
            {
                modelo.modeloRequerimiento = baseDatos.Requerimiento.Find(Id, IdProyecto);
                Usuario solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante);
                modelo.solicitante = solicitante.NombreCompleto;
                Usuario responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable);
                modelo.responsable = responsable.NombreCompleto;
                modelo.listaUsuariosCliente = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                         "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                         "USP.IdProyecto = P.Id JOIN Requerimiento R " +
                                                                         "ON P.Id = R.IdProyecto " +
                                                                         "WHERE USP.RolProyecto = 'Cliente' AND " +
                                                                         "R.Id = '" + modelo.modeloRequerimiento.Id + "';").ToList();
                modelo.listaUsuariosDesarrolladores = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                         "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                         "USP.IdProyecto = P.Id JOIN Requerimiento R " +
                                                                         "ON P.Id = R.IdProyecto " +
                                                                         "WHERE USP.RolProyecto = 'Desarrollador' AND " +
                                                                         "R.Id = '" + modelo.modeloRequerimiento.Id + "';").ToList();
            }
            catch
            {
				return mostrarError("Error al intentar acceder a la base de datos");
            }

            if (modelo.modeloRequerimiento.Imagen != null)
            {
                modelo.rutaImagen = Encoding.ASCII.GetString(modelo.modeloRequerimiento.Imagen);
            }
            modelo.accion = 0;

            return View(modelo);
        }

        /* Método que guarda los cambios en la vista de MEC_UnificadoRequerimientos
         * Requiere: no aplica
         * Modifica: la tabla de Requerimiento
         * Retorna: el modelo actualizado
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_UnificadoRequerimientos(ModeloProyecto modelo)
        {
            if (ModelState.IsValid)
            {
                modelo.modeloRequerimiento.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
                baseDatos.Entry(modelo.modeloRequerimiento).State = EntityState.Modified;
                baseDatos.SaveChanges();
                ModeloProyecto nuevoModelo = new ModeloProyecto();
                nuevoModelo.modeloRequerimiento = baseDatos.Requerimiento.Find(modelo.modeloRequerimiento.Id, modelo.modeloRequerimiento.IdProyecto);
                if (modelo.modeloRequerimiento.Imagen != null)
                {
                    nuevoModelo.rutaImagen = Encoding.ASCII.GetString(nuevoModelo.modeloRequerimiento.Imagen);
                }
                nuevoModelo.listaUsuariosCliente = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                              "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                              "USP.IdProyecto = P.Id JOIN Requerimiento R " +
                                                                              "ON P.Id = R.IdProyecto " +
                                                                              "WHERE USP.RolProyecto = 'Cliente' AND " +
                                                                              "R.Id = '" + modelo.modeloRequerimiento.Id + "';").ToList();
                nuevoModelo.listaUsuariosDesarrolladores = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                              "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                              "USP.IdProyecto = P.Id JOIN Requerimiento R " +
                                                                              "ON P.Id = R.IdProyecto " +
                                                                              "WHERE USP.RolProyecto = 'Desarrollador' AND " +
                                                                              "R.Id = '" + modelo.modeloRequerimiento.Id + "';").ToList();
                nuevoModelo.accion = 0;
                nuevoModelo.cambiosGuardados = 1;

                return View(nuevoModelo);
            }
            else
            {
                modelo.listaProyectos = baseDatos.Proyecto.ToList();
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                modelo.cambiosGuardados = 2;

                return View(modelo);
            }
        }

        /* Método que al cambiar una acción en la vista, actualiza los campos de un requerimiento.
         * Requiere: no aplica
         * Modifica: no aplica
         * Retorna: el modelo actualizado
         */
        public ActionResult cambiarAccion(string Id, int Accion)
        {
            try
            {
                ModeloProyecto modelo = new ModeloProyecto();
                modelo.modeloRequerimiento = baseDatos.Requerimiento.Find(Id);
                modelo.accion = Accion;
                return View(modelo);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return View("Index");
            }
        }

        /* Método que obtiene los usuarios del sistema con un rol específico.
         * Requiere: no aplica
         * Modifica: la tabla de Usuario
         * Retorna: el modelo actualizado
         */
        private void obtenerUsuarios(ModeloProyecto modelo)
        {
            var listaUsuarios = baseDatos.Usuario.ToList();
            var clientes = new List<Usuario>();
            var recursos = new List<Usuario>();

            foreach (var usr in listaUsuarios)
            {
                if (context.Users.Find(usr.Id).Roles.First().RoleId == "03User")
                {
                    clientes.Add(usr);
                }
                else if (context.Users.Find(usr.Id).Roles.First().RoleId == "02Develop")
                {
                    recursos.Add(usr);
                }
            }

            ViewBag.listaClientes = clientes;
            ViewBag.listaRecursos = recursos;
            ViewBag.listaDesarrolladores = new List<Usuario>();
        }

        /*-------------------------------------------------------------------------------------------
                                            GESTION DE CAMBIOS
        -------------------------------------------------------------------------------------------*/


        /* Método que carga el modelo para la vista Index de Historial de Cambios
		* Requiere: Tipo de ordenamiento, tipo de filtrado, patron de búsqueda, número de página y id del proyecto
		* Modifica: el modelo
		* Retorna: el modelo de páginación cargado
		*/
		[Authorize]
        public ActionResult IndexHistorialCambios(string sortOrder, string tipo, string currentFilter, string searchString, int? page, string Requerimiento, string Proyecto)
        {

			String id;
			String rol;
			IQueryable<Proyecto> proyecto;
			IQueryable<Cambio> cambios;

			try {
				// Id del usuario en sessión 
				id = System.Web.HttpContext.Current.User.Identity.GetUserId();
				// Rol del usuario en sessión
				rol = context.Users.Find(id).Roles.First().RoleId;

				// Se filtran todos los proyectos
				proyecto = from Proyecto p in baseDatos.Proyecto
						   select p;

				// Si no es administrador se filtran solo los proyectos a los cuales pertecene
				if(!(rol == "01Admin")) {
					proyecto = from Proyecto p in baseDatos.Proyecto
							   join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on
								p.Id equals USP.IdProyecto
							   where USP.IdUsuario == id
							   select p;

				}

				// Se filtran los cambios asociados a un proyectos
				cambios = from Cambio c in baseDatos.Cambio
						  where c.IdRequerimiento == Requerimiento && c.IdProyecto == Proyecto && (c.EstadoSolicitud == "Aprobado" || c.EstadoSolicitud == "Rechazado")
						  select c;
			}
			catch {
				return mostrarError("Error al intentar acceder a la base de datos");
			}

            // Modificaciones a los atributos de los cambios
            foreach (var i in cambios)
            {
                var nombre = from Usuario u in baseDatos.Usuario
                             join Cambio c in baseDatos.Cambio on
                             u.Id equals c.SolicitanteCambio
                             where u.Id == i.SolicitanteCambio
                             select u;

                foreach (var j in nombre)
                {
                    i.SolicitanteCambio = j.NombreCompleto;
                }
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                cambios = cambios.Where(s => s.Nombre.Contains(searchString) || s.IdRequerimiento.Contains(searchString)
                                              || s.Prioridad.Contains(searchString) || s.Esfuerzo.Contains(searchString)
                                              || s.EstadoSolicitud.Contains(searchString) || s.Sprint.Contains(searchString)
                                              || s.Modulo.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    switch (tipo)
                    {
                        case "IdSolicitud":
                            cambios = cambios.OrderByDescending(s => s.IdSolicitud);
                            break;
                        case "Id":
                            cambios = cambios.OrderByDescending(s => s.IdRequerimiento);
                            break;
                        case "Nombre":
                            cambios = cambios.OrderByDescending(s => s.Nombre);
                            break;
                        case "Prioridad":
                            cambios = cambios.OrderByDescending(s => s.Prioridad);
                            break;
                        case "Esfuerzo":
                            cambios = cambios.OrderByDescending(s => s.Esfuerzo);
                            break;
                        case "Sprint":
                            cambios = cambios.OrderByDescending(s => s.Sprint);
                            break;
                        case "Modulo":
                            cambios = cambios.OrderByDescending(s => s.Modulo);
                            break;
                        case "EstadoCambio":
                            cambios = cambios.OrderByDescending(s => s.EstadoSolicitud);
                            break;
                        case "VersionCambio":
                            cambios = cambios.OrderByDescending(s => s.Version);
                            break;
                        case "VersionFinal":
                            cambios = cambios.OrderByDescending(s => s.VersionCambio);
                            break;
                        case "Solicitante":
                            cambios = cambios.OrderByDescending(s => s.SolicitanteCambio);
                            break;
                        default:
                            cambios = cambios.OrderByDescending(s => s.IdSolicitud);
                            break;
                    }
                    break;
                case "date_desc":
                    cambios = cambios.OrderByDescending(s => s.FechaCambio);
                    break;
                case "Date":
                    cambios = cambios.OrderBy(s => s.FechaCambio);
                    break;
                default:
                    switch (tipo)
                    {
                        case "IdSolicitud":
                            cambios = cambios.OrderBy(s => s.IdSolicitud);
                            break;
                        case "Id":
                            cambios = cambios.OrderByDescending(s => s.IdRequerimiento);
                            break;
                        case "Nombre":
                            cambios = cambios.OrderBy(s => s.Nombre);
                            break;
                        case "Prioridad":
                            cambios = cambios.OrderBy(s => s.Prioridad);
                            break;
                        case "Esfuerzo":
                            cambios = cambios.OrderBy(s => s.Esfuerzo);
                            break;
                        case "Sprint":
                            cambios = cambios.OrderBy(s => s.Sprint);
                            break;
                        case "Modulo":
                            cambios = cambios.OrderBy(s => s.Modulo);
                            break;
                        case "EstadoCambio":
                            cambios = cambios.OrderBy(s => s.EstadoSolicitud);
                            break;
                        case "VersionCambio":
                            cambios = cambios.OrderBy(s => s.Version);
                            break;
                        case "VersionFinal":
                            cambios = cambios.OrderBy(s => s.VersionCambio);
                            break;
                        case "Solicitante":
                            cambios = cambios.OrderBy(s => s.SolicitanteCambio);
                            break;
                        default:
                            cambios = cambios.OrderBy(s => s.IdSolicitud);
                            break;
                    }
                    break;

            }

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(cambios.ToPagedList(pageNumber, pageSize));
        }

        /* Método que carga el modelo para la vista Index de Solicitudes de Cambios
                 * Requiere: Tipo de ordenamiento, tipo de filtrado, patron de búsqueda, número de página y id del proyecto
                 * Modifica: el modelo
                 * Retorna: el modelo de páginación cargado
                 */
		[Authorize]
        public ActionResult IndexSolicitud(string sortOrder, string tipo, string currentFilter, string searchString, int? page, string Proyecto, string Categ)
        {
            if (TempData["mensaje"] != null)
            {
                ViewBag.Msj = TempData["mensaje"].ToString();
            }

            // Id del usuario en sessión 
            String id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            // Rol del usuario en sessión

            String rol = context.Users.Find(id).Roles.First().RoleId;

            // Se filtran todos los proyectos
            var proyecto = from Proyecto p in baseDatos.Proyecto
                           select p;

            // Se filtran los cambios asociados a un proyectos
            var cambios = from Cambio c in baseDatos.Cambio
                          where c.IdProyecto == Proyecto && (c.EstadoSolicitud == "Pendiente" || c.EstadoSolicitud == "Modificar") && c.SolicitanteCambio == id
                          select c;

            // Si no es administrador se filtran solo los proyectos a los cuales pertecene
            if (!(rol == "01Admin"))
            {
                proyecto = from Proyecto p in baseDatos.Proyecto
                           join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on
                            p.Id equals USP.IdProyecto
                           where USP.IdUsuario == id
                           select p;
            }

            // Si no es administrador se filtran solo las solicitudes asociadas al usuario
            var asociado = from Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto
                           where USP.IdProyecto == Proyecto && USP.IdUsuario == id && USP.RolProyecto == "Líder"
                           select USP;

            if (asociado.Count() != 0 || rol == "01Admin")
            {
                cambios = from Cambio c in baseDatos.Cambio
                          where c.IdProyecto == Proyecto && (c.EstadoSolicitud == "Pendiente" || c.EstadoSolicitud == "Modificar")
                          select c;

                var usuarioAprobador = from Usuario u in baseDatos.Usuario
                                       where u.Id == id
                                       select u;

                ViewBag.aprobador = usuarioAprobador.First().NombreCompleto;
                ViewBag.rol = "Evaluar";
            }
            else
                ViewBag.rol = "Ver";


            // Modificaciones a los atributos de los cambios
            foreach (var i in cambios)
            {
                var nombre = from Usuario u in baseDatos.Usuario
                             join Cambio c in baseDatos.Cambio on
                             u.Id equals c.SolicitanteCambio
                             where u.Id == i.SolicitanteCambio
                             select u;

                foreach (var j in nombre)
                {
                    i.SolicitanteCambio = j.NombreCompleto;
                }
            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.pro = new SelectList(proyecto, "Id", "Nombre", Proyecto);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                cambios = cambios.Where(s => s.Nombre.Contains(searchString) || s.IdRequerimiento.Contains(searchString)
                                              || s.Prioridad.Contains(searchString) || s.Esfuerzo.Contains(searchString)
                                              || s.EstadoSolicitud.Contains(searchString) || s.Sprint.Contains(searchString)
                                              || s.Modulo.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    switch (tipo)
                    {
                        case "IdSolicitud":
                            cambios = cambios.OrderByDescending(s => s.IdSolicitud);
                            break;
                        case "Id":
                            cambios = cambios.OrderByDescending(s => s.IdRequerimiento);
                            break;
                        case "Nombre":
                            cambios = cambios.OrderByDescending(s => s.Nombre);
                            break;
                        case "Prioridad":
                            cambios = cambios.OrderByDescending(s => s.Prioridad);
                            break;
                        case "Esfuerzo":
                            cambios = cambios.OrderByDescending(s => s.Esfuerzo);
                            break;
                        case "Sprint":
                            cambios = cambios.OrderByDescending(s => s.Sprint);
                            break;
                        case "Modulo":
                            cambios = cambios.OrderByDescending(s => s.Modulo);
                            break;
                        case "Version":
                            cambios = cambios.OrderByDescending(s => s.Version);
                            break;
                        default:
                            cambios = cambios.OrderByDescending(s => s.IdSolicitud);
                            break;
                    }
                    break;
                case "date_desc":
                    cambios = cambios.OrderByDescending(s => s.FechaCambio);
                    break;
                case "Date":
                    cambios = cambios.OrderBy(s => s.FechaCambio);
                    break;
                default:
                    switch (tipo)
                    {
                        case "IdSolicitud":
                            cambios = cambios.OrderBy(s => s.IdSolicitud);
                            break;
                        case "Id":
                            cambios = cambios.OrderBy(s => s.IdRequerimiento);
                            break;
                        case "Nombre":
                            cambios = cambios.OrderBy(s => s.Nombre);
                            break;
                        case "Prioridad":
                            cambios = cambios.OrderBy(s => s.Prioridad);
                            break;
                        case "Esfuerzo":
                            cambios = cambios.OrderBy(s => s.Esfuerzo);
                            break;
                        case "Sprint":
                            cambios = cambios.OrderBy(s => s.Sprint);
                            break;
                        case "Modulo":
                            cambios = cambios.OrderBy(s => s.Modulo);
                            break;
                        case "Version":
                            cambios = cambios.OrderBy(s => s.Version);
                            break;
                        default:
                            cambios = cambios.OrderBy(s => s.IdSolicitud);
                            break;
                    }
                    break;

            }

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(cambios.ToPagedList(pageNumber, pageSize));
        }


        /* Método para aprobar o rechazar solicitudes de cambios a un requerimiento
         * Requiere: 
         * Modifica:
         * Retorna: 
         */
		 [Authorize]
        public ActionResult Edit(int IdSolicitud, string IdRequerimiento, string IdProyecto)
        {
            if (TempData["mensaje"] != null)
            {
                ViewBag.Msj = TempData["mensaje"].ToString();
            }
            ModeloProyecto modelo = new ModeloProyecto();
			try {
				modelo.modeloCambio = baseDatos.Cambio.Find(IdSolicitud, IdRequerimiento, IdProyecto);
				modelo.modeloRequerimiento = baseDatos.Requerimiento.Find(IdRequerimiento, IdProyecto);

				modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).NombreCompleto;
				modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).NombreCompleto;
				modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).NombreCompleto;
				modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).NombreCompleto;
				modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).NombreCompleto;
			}
			catch {
				return mostrarError("Error al intentar acceder a la base de datos");
			}
            if (modelo.modeloRequerimiento.Imagen != null)
            {
                modelo.rutaImagen = Encoding.ASCII.GetString(modelo.modeloRequerimiento.Imagen);
            }
            if (modelo.modeloCambio.Imagen != null)
            {
                modelo.rutaImagenCambio = Encoding.ASCII.GetString(modelo.modeloCambio.Imagen);
            }

            return View(modelo);
        }

        /* Método para aprobar, rechazar o pedir modificar; solicitudes de cambios a un requerimiento
         * Requiere: que el estado del cambio sea válido
         * Modifica: el cambio y el requerimiento; o el cambio
         * Retorna: la vista con un mensaje de éxito o fallo
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModeloProyecto modelo)
        {
            if (ModelState.IsValid)
            {
                /* 
                 * Si el estado es Aprobado, entonces se debe modificar el cambio y el requerimiento 
                 */
                if (modelo.modeloCambio.EstadoSolicitud == "Aprobado")
                {
                    /*
                     * Se debe buscar cuál es la última versión de cambio, la nueva versión será esa más 1
                     */
                    int version = 0;
                    modelo.listaCambios = baseDatos.Cambio.SqlQuery("SELECT * FROM Cambio WHERE " +
                                                                    "idRequerimiento = " + "'" + modelo.modeloCambio.IdRequerimiento + "' AND " +
                                                                    "idProyecto = " + "'" + modelo.modeloCambio.IdProyecto + "';").ToList();
                    for (int i = 0; i < modelo.listaCambios.Count; i++)
                    {
                        if (modelo.listaCambios.ElementAt(i).Version > version)
                            version = (int)modelo.listaCambios.ElementAt(i).VersionCambio;
                    }

                    modelo.modeloCambio.VersionCambio = version + 1;

                    /* 
                     * Se modifica el requerimiento correspondiente al cambio, para que se le apliquen los cambios
                     * que dice el requerimiento 
                     */
                    modelo.modeloRequerimiento.Nombre = modelo.modeloCambio.Nombre;
                    modelo.modeloRequerimiento.Prioridad = modelo.modeloCambio.Prioridad;
                    modelo.modeloRequerimiento.Esfuerzo = modelo.modeloCambio.Esfuerzo;
                    modelo.modeloRequerimiento.Estado = modelo.modeloCambio.Estado;
                    modelo.modeloRequerimiento.Descripcion = modelo.modeloCambio.Descripcion;
                    modelo.modeloRequerimiento.FechaInicio = modelo.modeloCambio.FechaInicio;
                    modelo.modeloRequerimiento.FechaFinal = modelo.modeloCambio.FechaFinal;
                    modelo.modeloRequerimiento.Sprint = modelo.modeloCambio.Sprint;
                    modelo.modeloRequerimiento.Modulo = modelo.modeloCambio.Modulo;
                    modelo.modeloRequerimiento.Observaciones = modelo.modeloCambio.Observaciones;
                    modelo.modeloRequerimiento.IdResponsable = modelo.modeloCambio.IdResponsable;
                    modelo.modeloRequerimiento.IdSolicitante = modelo.modeloCambio.IdSolicitante;
                    modelo.modeloRequerimiento.Version = modelo.modeloCambio.VersionCambio;

                    /* 
                     * Si al cambio se le eliminó la imagen, entonces el requerimiento debe elimnarla 
                     */
                    if (modelo.modeloCambio.Imagen != null)
                    {
                        modelo.modeloRequerimiento.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagenCambio);
                    }
                    else
                    {
                        modelo.modeloRequerimiento.Imagen = null;
                    }

                    /* 
                     * Se modifica el cambio y el requerimiento 
                     */
                    Cambio cambio = baseDatos.Cambio.Find(modelo.modeloCambio.IdSolicitud, modelo.modeloCambio.IdRequerimiento, modelo.modeloCambio.IdProyecto);
                    Requerimiento requerimiento = baseDatos.Requerimiento.Find(modelo.modeloRequerimiento.Id, modelo.modeloRequerimiento.IdProyecto);
                    baseDatos.Entry(cambio).CurrentValues.SetValues(modelo.modeloCambio);
                    baseDatos.Entry(requerimiento).CurrentValues.SetValues(modelo.modeloRequerimiento);

                    baseDatos.SaveChanges();

                    TempData["mensaje"] = "exito";
                    return RedirectToAction("IndexSolicitud");
                }
                /* 
                 * Si se rechazó la solicitud, simplemente se le cambia el estado a rechazado
                 */
                else if (modelo.modeloCambio.EstadoSolicitud == "Rechazado")
                {
                    Cambio cambio = baseDatos.Cambio.Find(modelo.modeloCambio.IdSolicitud, modelo.modeloCambio.IdRequerimiento, modelo.modeloCambio.IdProyecto);
                    baseDatos.Entry(cambio).CurrentValues.SetValues(modelo.modeloCambio);
                    baseDatos.SaveChanges();

                    TempData["mensaje"] = "exito";
                    return RedirectToAction("IndexSolicitud");
                }

                /*
                 * Si se quiere que el solicitante haga un cambio, entonces debe indicarse cuál es el cambio que
                 * se desea, de lo contrario se indica un error
                 */
                else if (modelo.modeloCambio.EstadoSolicitud == "Modificar")
                {
                    if (modelo.modeloCambio.ObservacionesSolicitud == null)
                    {
                        modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).NombreCompleto;
						modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).NombreCompleto;
						modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).NombreCompleto;
						modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).NombreCompleto;
						modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).NombreCompleto;

                        if (modelo.modeloRequerimiento.Imagen != null)
                        {
                            modelo.rutaImagen = Encoding.ASCII.GetString(modelo.modeloRequerimiento.Imagen);
                        }
                        if (modelo.modeloCambio.Imagen != null)
                        {
                            modelo.rutaImagenCambio = Encoding.ASCII.GetString(modelo.modeloCambio.Imagen);
                        }

                        TempData["mensaje"] = "erroreem";
                        ViewBag.Msj = "erroreem";
                        return View(modelo);
                    }
                    else
                    {
                        Cambio cambio = baseDatos.Cambio.Find(modelo.modeloCambio.IdSolicitud, modelo.modeloCambio.IdRequerimiento, modelo.modeloCambio.IdProyecto);
                        baseDatos.Entry(cambio).CurrentValues.SetValues(modelo.modeloCambio);
                        baseDatos.SaveChanges();

                        modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).NombreCompleto;
						modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).NombreCompleto;
						modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).NombreCompleto;
						modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).NombreCompleto;
						modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).NombreCompleto;

                        if (modelo.modeloRequerimiento.Imagen != null)
                        {
                            modelo.rutaImagen = Encoding.ASCII.GetString(modelo.modeloRequerimiento.Imagen);
                        }
                        if (modelo.modeloCambio.Imagen != null)
                        {
                            modelo.rutaImagenCambio = Encoding.ASCII.GetString(modelo.modeloCambio.Imagen);
                        }

                        TempData["mensaje"] = "exito";
                        ViewBag.Msj = "exito";
                        return View(modelo);
                    }
                }
            }

			modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).NombreCompleto;
			modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).NombreCompleto;
			modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).NombreCompleto;
			modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).NombreCompleto;
			modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).NombreCompleto;

            if (modelo.modeloRequerimiento.Imagen != null)
            {
                modelo.rutaImagen = Encoding.ASCII.GetString(modelo.modeloRequerimiento.Imagen);
            }
            if (modelo.modeloCambio.Imagen != null)
            {
                modelo.rutaImagenCambio = Encoding.ASCII.GetString(modelo.modeloCambio.Imagen);
            }

            TempData["mensaje"] = "error";
            ViewBag.Msj = "error";
            return View(modelo);
        }


        /* Método para crear una solicitud de cambio de un requerimiento
         * Requiere: idRequerimiento, idProyecto y versión
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
        [Authorize]
        [Permisos("PRO-M", "PRO-E", "PRO-C")]
        public ActionResult CrearSolicitud(string idRequerimiento, string idProyecto, int version)
        {
            var modelo = new Cambio();

            var solicitantes = new List<Usuario>();
            var responsables = new List<Usuario>();
            var cambios = from Cambio C in baseDatos.Cambio
                          where C.IdProyecto == idProyecto && C.IdRequerimiento == idRequerimiento && C.VersionCambio == version
                          select C;

            modelo = cambios.First();
            modelo.IdProyecto = idProyecto;
            modelo.IdRequerimiento = idRequerimiento;
            modelo.DescripcionCambio = "";
            modelo.JustificacionCambio = "";
            modelo.EstadoSolicitud = "Pendiente";
            modelo.FechaCambio = DateTime.Today;
            var soliCambio = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Usuario solicitanteCambio = baseDatos.Usuario.Find(soliCambio);
            modelo.solicitanteCambioNombre = solicitanteCambio.NombreCompleto;
            modelo.SolicitanteCambio = soliCambio;

            if (modelo.Imagen != null)
            {
                modelo.rutaImagen = Encoding.ASCII.GetString(modelo.Imagen);
            }

            List<SelectListItem> estado = new List<SelectListItem>();
            List<SelectListItem> clientesDropDown = new List<SelectListItem>();
            List<SelectListItem> responsablesDropDown = new List<SelectListItem>();

            estado.Add(new SelectListItem() { Text = "Pendiente de asignar", Value = "Pendiente de asignar" });
            estado.Add(new SelectListItem() { Text = "Asignado", Value = "Asignado" });
            estado.Add(new SelectListItem() { Text = "En ejecución", Value = "En ejecución" });
            estado.Add(new SelectListItem() { Text = "Finalizado", Value = "Finalizado" });
            estado.Add(new SelectListItem() { Text = "Cerrado", Value = "Cerrado" });

            foreach (var est in estado)
            {
                if (est.Text.Equals(modelo.Estado))
                {
                    est.Selected = true;
                    break;
                }
            }

            var clientes = from Usuario U in baseDatos.Usuario
                           join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
                           where USP.IdProyecto == idProyecto && USP.RolProyecto == "Cliente"
                           select U;

            var desarrolladores = from Usuario U in baseDatos.Usuario
                                  join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
                                  where USP.IdProyecto == idProyecto && USP.RolProyecto == "Desarrollador"
                                  select U;


            foreach (var cl in clientes)
                clientesDropDown.Add(new SelectListItem() { Text = cl.NombreCompleto, Value = cl.Id });

            foreach (var ds in desarrolladores)
                responsablesDropDown.Add(new SelectListItem() { Text = ds.NombreCompleto, Value = ds.Id });

            foreach (var cl in clientesDropDown)
            {
                if (cl.Value.Equals(modelo.IdSolicitante))
                {
                    cl.Selected = true;
                    break;
                }
            }

            foreach (var ds in responsablesDropDown)
            {
                if (ds.Value.Equals(modelo.IdResponsable))
                {
                    ds.Selected = true;
                    break;
                }
            }

            ViewBag.Estado = estado;
            ViewBag.ListaSolicitante = clientesDropDown;
            ViewBag.ListaResponsable = responsablesDropDown;

            return View(modelo);
        }

        /* Método que guarda una solicitud de cambios
         * Requiere: no aplica
         * Modifica: la tabla de Requerimiento
         * Retorna: el modelo actualizado
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearSolicitud([Bind(Include = "IdSolicitud,IdRequerimiento,IdProyecto,Nombre,Prioridad,Esfuerzo,Estado,Descripcion,FechaInicio,FechaFinal,Sprint,Modulo,Observaciones,Imagen,IdResponsable,IdSolicitante,Version,DescripcionCambio,JustificacionCambio,SolicitanteCambio,FechaCambio,EstadoSolicitud,ObservacionesSolicitud,VersionCambio,rutaImagen,CriterioAceptacionHistorial")] Cambio modelo)
        {
            if (!string.IsNullOrEmpty(modelo.rutaImagen))
            {
                modelo.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
            }

            modelo.EstadoSolicitud = "Pendiente";

            if (ModelState.IsValid)
            {
                List<CriterioAceptacionHistorial> auxiliar_criterios = new List<CriterioAceptacionHistorial>();

                foreach (CriterioAceptacionHistorial criterio in modelo.CriterioAceptacionHistorial.ToList())
                {
                    auxiliar_criterios.Add(criterio);
                    modelo.CriterioAceptacionHistorial.Remove(criterio);
                }

                baseDatos.Cambio.Add(modelo);
                baseDatos.SaveChanges();

                List<SelectListItem> estado = new List<SelectListItem>();
                List<SelectListItem> clientesDropDown = new List<SelectListItem>();
                List<SelectListItem> responsablesDropDown = new List<SelectListItem>();

                estado.Add(new SelectListItem() { Text = "Pendiente de asignar", Value = "Pendiente de asignar" });
                estado.Add(new SelectListItem() { Text = "Asignado", Value = "Asignado" });
                estado.Add(new SelectListItem() { Text = "En ejecución", Value = "En ejecución" });
                estado.Add(new SelectListItem() { Text = "Finalizado", Value = "Finalizado" });
                estado.Add(new SelectListItem() { Text = "Cerrado", Value = "Cerrado" });

                foreach (var est in estado)
                {
                    if (est.Text.Equals(modelo.Estado))
                    {
                        est.Selected = true;
                        break;
                    }
                }

                var clientes = from Usuario U in baseDatos.Usuario
                               join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
                               where USP.IdProyecto == modelo.IdProyecto && USP.RolProyecto == "Cliente"
                               select U;

                var desarrolladores = from Usuario U in baseDatos.Usuario
                                      join Usuarios_asociados_proyecto USP in baseDatos.Usuarios_asociados_proyecto on U.Id equals USP.IdUsuario
                                      where USP.IdProyecto == modelo.IdProyecto && USP.RolProyecto == "Desarrollador"
                                      select U;


                foreach (var cl in clientes)
                    clientesDropDown.Add(new SelectListItem() { Text = cl.NombreCompleto, Value = cl.Id });

                foreach (var ds in desarrolladores)
                    responsablesDropDown.Add(new SelectListItem() { Text = ds.NombreCompleto, Value = ds.Id });

                foreach (var cl in clientesDropDown)
                {
                    if (cl.Value.Equals(modelo.IdSolicitante))
                    {
                        cl.Selected = true;
                        break;
                    }
                }

                foreach (var ds in responsablesDropDown)
                {
                    if (ds.Value.Equals(modelo.IdResponsable))
                    {
                        ds.Selected = true;
                        break;
                    }
                }

                ViewBag.Estado = estado;
                ViewBag.ListaSolicitante = clientesDropDown;
                ViewBag.ListaResponsable = responsablesDropDown;

                var i = 1;
                foreach (CriterioAceptacionHistorial criterio in auxiliar_criterios)
                {
                    if (criterio.Borrar != true)
                    {
                        criterio.Escenario = i.ToString();
                        criterio.IdSolicitud = modelo.IdSolicitud;
                        criterio.IdRequerimiento = modelo.IdRequerimiento;
                        criterio.IdProyecto = modelo.IdProyecto;
                        modelo.CriterioAceptacionHistorial.Add(criterio);
                        i++;
                    }
                }

                baseDatos.Entry(modelo).State = EntityState.Modified;
                baseDatos.SaveChanges();

                modelo.cambiosGuardados = 1;

                return View(modelo);
            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                modelo.cambiosGuardados = 2;
                return View(modelo);
            }
        }


        [Authorize]
        public ActionResult MEC_Solicitud(int idSolicitud, string idRequerimiento, string idProyecto)
        {
            ModeloProyecto modelo = new ModeloProyecto();
            var solicitantes = new List<Usuario>();
            var responsables = new List<Usuario>();

            var cambios = (from cambio in baseDatos.Cambio
                           where cambio.IdProyecto == idProyecto && cambio.IdRequerimiento == idRequerimiento && cambio.IdSolicitud == idSolicitud
                           select new { cambio });

            modelo.modeloCambio = cambios.First().cambio;
            Usuario solicitante = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante);
            Usuario responsable = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable);
            Usuario solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio);
            modelo.solicitanteCambio = solicitanteCambio.NombreCompleto;

            //no agrego el solicitante para que salga de primero
            var clientes = (from usuario in baseDatos.Usuario
                            join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                            where usrProy.IdProyecto == idProyecto && usrProy.RolProyecto == "Cliente" && usrProy.IdUsuario != solicitante.Id
                            select new { usuario });

            //no agrego el responsable para que salga de primero
            var desarrolladores = (from usuario in baseDatos.Usuario
                                   join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                   where usrProy.IdProyecto == idProyecto && usrProy.RolProyecto == "Desarrollador" && usrProy.IdUsuario != responsable.Id
                                   select new { usuario });


            solicitantes.Add(solicitante);
            foreach (var s in clientes)
            {
                solicitantes.Add(s.usuario);
            }

            responsables.Add(responsable);
            foreach (var d in desarrolladores)
            {
                solicitantes.Add(d.usuario);
                responsables.Add(d.usuario);
            }

            ViewBag.listaSolicitantes = solicitantes;
            ViewBag.listaResponsables = responsables;

            if (!string.IsNullOrEmpty(modelo.rutaImagen))
            {
                modelo.modeloCambio.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
            }

            ViewBag.msj = TempData["msj"] ?? "";
            return View(modelo);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_Solicitud(ModeloProyecto modelo)
        {
            modelo.modeloCambio.FechaCambio = DateTime.Now;
            if (ModelState.IsValid)
            {
				try {
					baseDatos.Entry(modelo.modeloCambio).State = EntityState.Modified;
					baseDatos.SaveChanges();
					TempData["msj"] = "exito";
					return RedirectToAction("MEC_Solicitud",
							new {
								idSolicitud = modelo.modeloCambio.IdSolicitud,
								idRequerimiento = modelo.modeloCambio.IdRequerimiento,
								idProyecto = modelo.modeloCambio.IdProyecto
							});
				}
				catch
                {
                    ViewBag.msj = "error";
                }
            }
            ViewBag.msj = "error";
            return View(modelo);
        }

		[Authorize]
        public ActionResult eliminarSolicitud(int idSolicitud, string idRequerimiento, string idProyecto)
        {
            try
            {
                var cambios = (from cambio in baseDatos.Cambio
                               where cambio.IdProyecto == idProyecto && cambio.IdRequerimiento == idRequerimiento && cambio.IdSolicitud == idSolicitud
                               select new { cambio });

                Cambio modelo = cambios.First().cambio;
                baseDatos.Cambio.Remove(modelo);
                baseDatos.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [Authorize]
		[Permisos("GCS-CS")]
        [HttpGet]
        public ActionResult DetallesVersion(int version, string idRequerimiento, string idProyecto)
        {
            ModeloProyecto modelo = new ModeloProyecto();
            var solicitantes = new List<Usuario>();
            var responsables = new List<Usuario>();

            var cambios = (from cambio in baseDatos.Cambio
                           where cambio.IdProyecto == idProyecto && cambio.IdRequerimiento == idRequerimiento && cambio.Version == version
                           select new { cambio });

            modelo.modeloCambio = cambios.First().cambio;
            Usuario solicitante = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante);
            Usuario responsable = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable);
            Usuario solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio);
            modelo.solicitanteCambio = solicitanteCambio.Nombre + " " + solicitanteCambio.Apellido1 + " " + solicitanteCambio.Apellido2;


            //no agrego el solicitante para que salga de primero
            var clientes = (from usuario in baseDatos.Usuario
                            join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                            where usrProy.IdProyecto == idProyecto && usrProy.RolProyecto == "Cliente" && usrProy.IdUsuario != solicitante.Id
                            select new { usuario });

            //no agrego el responsable para que salga de primero
            var desarrolladores = (from usuario in baseDatos.Usuario
                                   join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                   where usrProy.IdProyecto == idProyecto && usrProy.RolProyecto == "Desarrollador" && usrProy.IdUsuario != responsable.Id
                                   select new { usuario });


            solicitantes.Add(solicitante);
            foreach (var s in clientes)
            {
                solicitantes.Add(s.usuario);
            }

            responsables.Add(responsable);
            foreach (var d in desarrolladores)
            {
                solicitantes.Add(d.usuario);
                responsables.Add(d.usuario);
            }

            ViewBag.listaSolicitantes = solicitantes;
            ViewBag.listaResponsables = responsables;

            if (!string.IsNullOrEmpty(modelo.rutaImagen))
            {
                modelo.modeloCambio.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
            }

            return View(modelo);
        }

		private ActionResult mostrarError(string msj) {
			HttpContext.Response.StatusCode = 500;
			var view = new ViewResult { ViewName = "~/Views/Shared/Error.cshtml", ViewData = new ViewDataDictionary() };
			view.ViewData["errorBD"] = msj;
			return (view);
		}
	}
}
