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
        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page, string Proyecto)
        {

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

        public ActionResult Create(String proyecto)
        {
            ModeloProyecto modelo = new ModeloProyecto();
            modelo.listaProyectos = baseDatos.Proyecto.ToList();
            modelo.proyectoRequerimiento = proyecto;
            modelo.listaUsuariosCliente = baseDatos.Usuario.SqlQuery("SELECT DISTINCT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                     "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                     "USP.IdProyecto = P.Id " +
                                                                     "WHERE USP.RolProyecto = 'Cliente' AND USP.IdProyecto ='"+proyecto+"';").ToList();
            modelo.listaUsuariosDesarrolladores = baseDatos.Usuario.SqlQuery("SELECT DISTINCT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                             "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                             "USP.IdProyecto = P.Id " +
                                                                             "WHERE USP.RolProyecto = 'Desarrollador' AND USP.IdProyecto ='" + proyecto + "';").ToList();

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModeloProyecto modelo)
        {
            if (ModelState.IsValid && modelo.proyectoRequerimiento != "Seleccione un proyecto")
            {
				if(!string.IsNullOrEmpty(modelo.rutaImagen)) {
					modelo.modeloRequerimiento.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
				}
                modelo.modeloRequerimiento.IdProyecto = modelo.proyectoRequerimiento;
                modelo.modeloRequerimiento.Version = 1;
                baseDatos.Requerimiento.Add(modelo.modeloRequerimiento);

                /* Se guarda en el historial, ese nuevo requerimiento */
                Cambio cambio = new Cambio();
                cambio.EstadoSolicitud = "Aprobado";
                cambio.IdRequerimiento = modelo.modeloRequerimiento.Id;
                cambio.IdProyecto = modelo.modeloRequerimiento.IdProyecto;
                cambio.Nombre = modelo.modeloRequerimiento.Nombre;
                cambio.Prioridad = modelo.modeloRequerimiento.Prioridad;
                cambio.Esfuerzo = modelo.modeloRequerimiento.Esfuerzo;
                cambio.Estado = modelo.modeloRequerimiento.Estado;
                cambio.Descripcion = modelo.modeloRequerimiento.Descripcion;
                cambio.FechaInicio = modelo.modeloRequerimiento.FechaInicio;
                cambio.FechaFinal = modelo.modeloRequerimiento.FechaFinal;
                cambio.FechaCambio = modelo.modeloRequerimiento.FechaInicio;
                cambio.Sprint = modelo.modeloRequerimiento.Sprint;
                cambio.Modulo = modelo.modeloRequerimiento.Modulo;
                cambio.Observaciones = modelo.modeloRequerimiento.Observaciones;
                cambio.Imagen = modelo.modeloRequerimiento.Imagen;
                cambio.IdResponsable = modelo.modeloRequerimiento.IdResponsable;
                cambio.IdSolicitante = modelo.modeloRequerimiento.IdSolicitante;
                cambio.Version = 1;
                cambio.SolicitanteCambio = System.Web.HttpContext.Current.User.Identity.GetUserId();
                cambio.ObservacionesSolicitud = "Cambio inicial";
                cambio.DescripcionCambio = "Cambio inicial";
                cambio.JustificacionCambio = "Cambio inicial";
                baseDatos.Cambio.Add(cambio);
                baseDatos.SaveChanges();
                ModeloProyecto nuevoModelo = new ModeloProyecto();
                modelo.listaProyectos = baseDatos.Proyecto.ToList();
                modelo.listaUsuariosCliente = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                         "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                         "USP.IdProyecto = P.Id " +
                                                                         "WHERE USP.RolProyecto = 'Cliente' AND USP.IdProyecto ='" + modelo.proyectoRequerimiento + "';").ToList();
                modelo.listaUsuariosDesarrolladores = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                                 "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                                 "USP.IdProyecto = P.Id " +
                                                                                 "WHERE USP.RolProyecto = 'Desarrollador' AND USP.IdProyecto ='" + modelo.proyectoRequerimiento + "';").ToList();
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

        /* Método elimina un elemento en la vista Index
         * Requiere: parámetro recibido válido en la base de datos
         * Modifica: la tabla Requerimientos de la base de datos
         * Retorna: redirección a la vista Index
         */
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
                modelo.solicitante = solicitante.Nombre + " " + solicitante.Apellido1 + " " + solicitante.Apellido2;
                Usuario responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable);
                modelo.responsable = responsable.Nombre + " " + responsable.Apellido1 + " " + responsable.Apellido2;
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
        public ActionResult IndexHistorialCambios(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {
            var historialCambios = from s in baseDatos.Cambio
                                   select s;

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(historialCambios.ToPagedList(pageNumber, pageSize));
        }

        /* Método que carga el modelo para la vista Index de Solicitudes de Cambios
         * Requiere: Tipo de ordenamiento, tipo de filtrado, patron de búsqueda, número de página y id del proyecto
         * Modifica: el modelo
         * Retorna: el modelo de páginación cargado
         */
        public ActionResult IndexSolicitud(string sortOrder, string tipo, string currentFilter, string searchString, int? page, string Proyecto, string Categ)
        {
            if (TempData["mensaje"] != null)
            {
                ViewBag.Msj = TempData["mensaje"].ToString();
            }
            String id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            String rol = context.Users.Find(id).Roles.First().RoleId;

            var cambios = from Cambio c in baseDatos.Cambio
                          where c.IdProyecto == Proyecto
                          select c;

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


            foreach (var i in cambios)
            {
                String id_usuario = User.Identity.GetUserId();
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

            List<SelectListItem> categoria = new List<SelectListItem>();
            categoria.Add(new SelectListItem() { Text = "Mis solicitudes", Value = "1" });
            categoria.Add(new SelectListItem() { Text = "Realizadas por el equipo", Value = "2" });

            if (!String.IsNullOrEmpty(Categ))
            {
                var seleccionar = 1;
                foreach (var k in categoria)
                {
                    if (Int32.Parse(Categ) == seleccionar)
                        k.Selected = true;
                    seleccionar++;
                }
            }

            ViewBag.categoria = categoria;

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
                cambios = cambios.Where(s => s.IdSolicitante.Contains(searchString) || s.Nombre.Contains(searchString)
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
                            cambios = cambios.OrderByDescending(s => s.IdSolicitud);
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
                case "Date":
                    switch (tipo)
                    {
                        case "FechaInicio":
                            cambios = cambios.OrderBy(s => s.FechaInicio);
                            break;
                        case "FechaFinal":
                            cambios = cambios.OrderBy(s => s.FechaFinal);
                            break;
                        default:
                            cambios = cambios.OrderBy(s => s.FechaInicio);
                            break;
                    }
                    break;
                case "date_desc":
                    switch (tipo)
                    {
                        case "FechaInicio":
                            cambios = cambios.OrderByDescending(s => s.FechaInicio);
                            break;
                        case "FechaFinal":
                            cambios = cambios.OrderByDescending(s => s.FechaFinal);
                            break;
                        default:
                            cambios = cambios.OrderByDescending(s => s.FechaInicio);
                            break;
                    }
                    break;
                default:
                    switch (tipo)
                    {
                        case "IdSolicitud":
                            cambios = cambios.OrderBy(s => s.IdSolicitud);
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
        public ActionResult Edit(int IdSolicitud, string IdRequerimiento, string IdProyecto)
        {
            if (TempData["mensaje"] != null)
            {
                ViewBag.Msj = TempData["mensaje"].ToString();
            }
            ModeloProyecto modelo = new ModeloProyecto();
            modelo.modeloCambio = baseDatos.Cambio.Find(IdSolicitud, IdRequerimiento, IdProyecto);
            modelo.modeloRequerimiento = baseDatos.Requerimiento.Find(IdRequerimiento, IdProyecto);

            modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido2;
            modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido2;
            modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido2;
            modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido2;
            modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido2;

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
                    modelo.listaCambios = baseDatos.Cambio.ToList();
                    for (int i = 0; i < modelo.listaCambios.Count; i++)
                    {
                        if (modelo.listaCambios.ElementAt(i).Version > version)
                            version = (int)modelo.listaCambios.ElementAt(i).Version;
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
                else if(modelo.modeloCambio.EstadoSolicitud == "Rechazado")
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
                else if(modelo.modeloCambio.EstadoSolicitud == "Modificar")
                {
                    if(modelo.modeloCambio.ObservacionesSolicitud == null)
                    {
		    	modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido2;
                        modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido2;
                        modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido2;
                        modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido2;
                        modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido2;

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
                        
			modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido2;
                        modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido2;
                        modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido2;
                        modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido2;
                        modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido2;

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
	    
	    modelo.solicitanteCambio = baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.SolicitanteCambio).Apellido2;
                        modelo.solicitante = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdSolicitante).Apellido2;
                        modelo.responsable = baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloRequerimiento.IdResponsable).Apellido2;
                        modelo.solicitanteC = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante).Apellido2;
                        modelo.responsableC = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Nombre + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido1 + " " + baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable).Apellido2;

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
            ModeloProyecto modelo = new ModeloProyecto();

            var cambios = (from cambio in baseDatos.Cambio
                           where cambio.IdProyecto == idProyecto && cambio.IdRequerimiento == idRequerimiento && cambio.Version == version
                           select new { cambio });

            modelo.modeloCambio = cambios.First().cambio;
            modelo.proyectoRequerimiento = idProyecto;

            // Solicitante del requerimiento
            // este no cambia, porque fue quien generó inicialmente el requerimiento
            Usuario solicitante = baseDatos.Usuario.Find(modelo.modeloCambio.IdSolicitante);
            modelo.solicitante = solicitante.Nombre + " " + solicitante.Apellido1 + " " + solicitante.Apellido2; ;

            // Responsable del requerimiento
            // es el desaarrollador que está a cargo de la funcionalidad
            Usuario responsable = baseDatos.Usuario.Find(modelo.modeloCambio.IdResponsable);
            var responsables = new List<Usuario>();
            var desarrolladores = (from usuario in baseDatos.Usuario
                                   join usrProy in baseDatos.Usuarios_asociados_proyecto on usuario.Id equals usrProy.IdUsuario
                                   where usrProy.IdProyecto == idProyecto && usrProy.RolProyecto == "Desarrollador"
                                   select new { usuario });

            foreach (var i in desarrolladores)
            {
                responsables.Add(i.usuario);
            }


            // El solicitante del cambio es la persona que está "loggeada"
            // en el sistema actualmente.
            var soliCambio = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Usuario solicitanteCambio = baseDatos.Usuario.Find(soliCambio);
            modelo.solicitanteCambio = solicitanteCambio.Nombre + " " + solicitanteCambio.Apellido1 + " " + solicitanteCambio.Apellido2;
            
            ViewBag.listaResponsables = responsables;

            modelo.modeloCambio.FechaCambio = DateTime.Today;
            modelo.modeloCambio.DescripcionCambio = "";
            modelo.modeloCambio.JustificacionCambio = "";
            modelo.modeloCambio.EstadoSolicitud = "Pendiente";

            if (modelo.modeloCambio.Imagen != null)
            {
                modelo.rutaImagen = Encoding.ASCII.GetString(modelo.modeloCambio.Imagen);
            }

            return View(modelo);
        }

        /* Método que guarda una solicitud de cambios
         * Requiere: no aplica
         * Modifica: la tabla de Requerimiento
         * Retorna: el modelo actualizado
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearSolicitud(ModeloProyecto modelo)
        {

            if (!string.IsNullOrEmpty(modelo.rutaImagen))
            {
                modelo.modeloCambio.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
            }
            modelo.modeloCambio.IdProyecto = modelo.proyectoRequerimiento;
            modelo.modeloCambio.EstadoSolicitud = "Pendiente";
            modelo.modeloCambio.FechaCambio = DateTime.Today;

            if (ModelState.IsValid)
            {
                baseDatos.Cambio.Add(modelo.modeloCambio);
                baseDatos.SaveChanges();

                ModeloProyecto nuevoModelo = new ModeloProyecto();

                modelo.listaProyectos = baseDatos.Proyecto.ToList();

                modelo.listaUsuariosCliente = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                         "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                         "USP.IdProyecto = P.Id " +
                                                                         "WHERE USP.RolProyecto = 'Cliente' AND USP.IdProyecto ='" + modelo.proyectoRequerimiento + "';").ToList();

                modelo.listaUsuariosDesarrolladores = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                                 "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                                 "USP.IdProyecto = P.Id " +
                                                                                 "WHERE USP.RolProyecto = 'Desarrollador' AND USP.IdProyecto ='" + modelo.proyectoRequerimiento + "';").ToList();

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

        [Authorize]
        public ActionResult MEC_Solicitud(string idRequerimiento, string idProyecto, int version = 1)
        {
            idProyecto = "PRO-II";
            idRequerimiento = "RF-FQS-01";
            version = 1;
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

            return View(modelo);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_Solicitud(ModeloProyecto modelo55)
        {
            string idProyecto = "PRO-II";
            string idRequerimiento = "RF-FQS-01";
            int version = 1;
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

            return View(modelo);
        }
	
	    [Authorize]
        [HttpGet]
        public ActionResult DetallesVersion(string idRequerimiento, string idProyecto, int version=1)
        {
            idProyecto = "PRO-II";
            idRequerimiento = "RF-FQS-01";
            version = 1;
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

            return View(modelo);
        }
    }
}
