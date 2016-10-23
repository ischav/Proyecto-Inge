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
                                       || s.Estado.Contains(searchString)  || s.Duracion.Contains(searchString));
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
    }
}