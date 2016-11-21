using Microsoft.AspNet.Identity;
using PagedList;
using ProyectoInge1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ProyectoInge1.Controllers
{
    public class ModuloCambiosController : Controller{

        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        public ActionResult IndexHistorialCambios(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {
            var historialCambios = from s in baseDatos.Cambio
                                select s;

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(historialCambios.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult IndexSolicitud(string sortOrder, string tipo, string currentFilter, string searchString, int? page, string Proyecto)
        {
            String id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            String rol = context.Users.Find(id).Roles.First().RoleId;

            var cambios = from Cambio c in baseDatos.Cambio
                          select c;

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

        public ActionResult Edit(ModeloProyecto modelo)
        {
            if (ModelState.IsValid)
            {
                baseDatos.Entry(modelo.modeloCambio).State = EntityState.Modified;
                baseDatos.SaveChanges();
            }
            else
            {

            }

            return View(modelo);
        }

        /* Método para crear una solicitud de cambio de un requerimiento
         * Requiere: parámetro recibido válido en la base de datos
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
        public ActionResult CrearSolicitud(string Id, string IdProyecto)
        {
            ModeloProyecto modelo = new ModeloProyecto();
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
            if (modelo.modeloRequerimiento.Imagen != null)
            {
                modelo.rutaImagen = Encoding.ASCII.GetString(modelo.modeloRequerimiento.Imagen);
            }
            modelo.accion = 0;

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
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(modelo.rutaImagen))
                {
                    modelo.modeloCambio.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
                }
                modelo.modeloCambio.IdProyecto = modelo.proyectoRequerimiento;
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

    }
}