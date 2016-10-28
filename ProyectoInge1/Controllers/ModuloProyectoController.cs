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

namespace ProyectoInge1.Controllers
{
    public class ModuloProyectoController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            //---------------------
            // Obtener el usuario actual
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;

            // Verificación de los privilegios disponibles en el modulo de proyecto y
            // asociadoos al rol del usuario loggeado en el sistema.
            Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-I", modelo.rolActualId);
            if (privilegio == null)
            {
                modelo.agregar = false;
            }
            else
            {
                modelo.agregar = true;
            }
            //---------------------

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

            var proyecto = from s in baseDatos.Proyecto
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                proyecto = proyecto.Where(s => s.Id.Contains(searchString) || s.Nombre.Contains(searchString)
                                       || s.Estado.Contains(searchString) || s.Duracion.Contains(searchString));
            }

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

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(proyecto.ToPagedList(pageNumber, pageSize));
        }

        //Metodo GET pantalla Crear Proyecto
        public ActionResult Create()
        {
            return View();
        }


        /**************Cambios Adrián****************************/

        //Metodo GET para la pantalla unificada. Corresponde a consultar
        public ActionResult MEC_Unificado(string id)
        {
            ModeloProyecto modeloPr = new ModeloProyecto();
            ModeloIntermedio modelo = new ModeloIntermedio();

            modeloPr.modeloProyecto = baseDatos.Proyecto.Find(id);

            // Verificación de los privilegios disponibles en el modulo de usuarios y
            // asociadoos al rol del usuario loggeado en el sistema.
            //Cambiar por los de este modulo
            Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-M", modelo.rolActualId);
            if (privilegio == null)
            {
                modelo.modificar = false;
            }
            else
            {
                modelo.modificar = true;
            }

            privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-C", modelo.rolActualId);
            if (privilegio == null)
            {
                modelo.consultar = false;
            }
            else
            {
                modelo.consultar = true;
            }

            privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-E", modelo.rolActualId);
            if (privilegio == null)
            {
                modelo.eliminar = false;
            }
            else
            {
                modelo.eliminar = true;
            }
            //------------------

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
        public ActionResult MEC_Unificado(ModeloProyecto modelo, string aceptar, string cancelar)
        {
            if (ModelState.IsValid)
            {
                baseDatos.Entry(modelo.modeloProyecto).State = EntityState.Modified;
                baseDatos.SaveChanges();
            }
            else
            {
                modelo.cambiosGuardados = 1;
            }


            return View(modelo);
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

    }
}