﻿using System;
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
         * Requiere: no aplica
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page, string Proyecto)
        {
            ViewBag.pro = new SelectList(baseDatos.Proyecto, "Id", "Nombre", Proyecto);
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

        public ActionResult Create()
        {
            ModeloProyecto modelo = new ModeloProyecto();
            modelo.listaProyectos = baseDatos.Proyecto.ToList();
            modelo.listaUsuariosCliente = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                     "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                     "USP.IdProyecto = P.Id " + 
                                                                     "WHERE USP.RolProyecto = 'Cliente';").ToList();
            modelo.listaUsuariosDesarrolladores = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                             "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                             "USP.IdProyecto = P.Id " +
                                                                             "WHERE USP.RolProyecto = 'Desarrollador';").ToList();

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModeloProyecto modelo)
        {
            if (ModelState.IsValid)
            {
                modelo.modeloRequerimiento.Imagen = Encoding.ASCII.GetBytes(modelo.rutaImagen);
                baseDatos.Requerimiento.Add(modelo.modeloRequerimiento);
                baseDatos.SaveChanges();
                ModeloProyecto nuevoModelo = new ModeloProyecto();
                modelo.listaProyectos = baseDatos.Proyecto.ToList();
                modelo.listaUsuariosCliente = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                         "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                         "USP.IdProyecto = P.Id " +
                                                                         "WHERE USP.RolProyecto = 'Cliente';").ToList();
                modelo.listaUsuariosDesarrolladores = baseDatos.Usuario.SqlQuery("SELECT * FROM Usuario U JOIN Usuarios_asociados_proyecto USP ON " +
                                                                                 "U.Id = USP.IdUsuario JOIN Proyecto P ON " +
                                                                                 "USP.IdProyecto = P.Id " +
                                                                                 "WHERE USP.RolProyecto = 'Desarrollador';").ToList();
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
        public ActionResult eliminarRequerimiento(string Id)
        {
            //Borra al requerimiento de la tabla Requerimientos
            ModeloProyecto modelo = new ModeloProyecto();
            modelo.modeloRequerimiento = baseDatos.Requerimiento.Find(Id);
            baseDatos.Requerimiento.Remove(modelo.modeloRequerimiento);
            baseDatos.SaveChanges();

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
    }
}
