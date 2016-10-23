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
    public class ModuloRequerimientosController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {
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

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModeloProyecto modelo)
        {
            if (ModelState.IsValid)
            {
                Proyecto proyectoRequerimiento = baseDatos.Proyecto.Find(modelo.proyectoRequerimiento.ToString());
                modelo.modeloRequerimiento.Proyecto = proyectoRequerimiento;
                baseDatos.Requerimiento.Add(modelo.modeloRequerimiento);
                baseDatos.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                modelo.listaProyectos = baseDatos.Proyecto.ToList();
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return View(modelo);
            }
        }

        public ActionResult eliminarRequerimiento(string Id)
        {
            //Borra al requerimiento de la tabla Requerimientos
            ModeloProyecto modelo = new ModeloProyecto();
            modelo.modeloRequerimiento = baseDatos.Requerimiento.Find(Id);
            baseDatos.Requerimiento.Remove(modelo.modeloRequerimiento);
            baseDatos.SaveChanges();

            return RedirectToAction("Index");
        }

        //Metodo POST para la pantalla unificada. Corresponde a modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_UnificadoRequerimientos(ModeloProyecto modelo)
        {
            if (ModelState.IsValid)
            {
                //Para guardar en tabla usuarios
                baseDatos.Entry(modelo.modeloRequerimiento).State = EntityState.Modified;
                baseDatos.SaveChanges();

                modelo.errorValidacion = false;
                modelo.cambiosGuardados = 0; //cambios guardados
            }

            return View(modelo);
        }
    }
}