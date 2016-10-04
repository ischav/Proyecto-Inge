using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoInge1.Models;
using PagedList;

namespace ProyectoInge1.Controllers
{
    public class ModuloUsuariosController : Controller
    {
        Entities baseDatos = new Entities();

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

            var usuarios = from s in baseDatos.Usuario
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                usuarios = usuarios.Where(s => s.Cedula.Contains(searchString) || s.Apellido1.Contains(searchString)
                                       || s.Apellido2.Contains(searchString) || s.Nombre.Contains(searchString) ||
                                        s.Telefono1.Contains(searchString) || s.Telefono2.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    switch (tipo)
                    {
                        case "Cedula":
                            usuarios = usuarios.OrderByDescending(s => s.Cedula);
                            break;
                        case "Nombre":
                            usuarios = usuarios.OrderByDescending(s => s.Nombre);
                            break;
                        case "Apellido1":
                            usuarios = usuarios.OrderByDescending(s => s.Apellido1);
                            break;
                        case "Apellido2":
                            usuarios = usuarios.OrderByDescending(s => s.Apellido2);
                            break;
                        case "Telefono1":
                            usuarios = usuarios.OrderByDescending(s => s.Telefono1);
                            break;
                        case "Telefono2":
                            usuarios = usuarios.OrderByDescending(s => s.Telefono2);
                            break;
                        default:
                            usuarios = usuarios.OrderByDescending(s => s.Cedula);
                            break;
                    }
                    break;
                case "Date":
                    usuarios = usuarios.OrderBy(s => s.FechaNac);
                    break;
                case "date_desc":
                    usuarios = usuarios.OrderByDescending(s => s.FechaNac);
                    break;
                default:
                    switch (tipo)
                    {
                        case "Cedula":
                            usuarios = usuarios.OrderBy(s => s.Cedula);
                            break;
                        case "Nombre":
                            usuarios = usuarios.OrderBy(s => s.Nombre);
                            break;
                        case "Apellido1":
                            usuarios = usuarios.OrderBy(s => s.Apellido1);
                            break;
                        case "Apellido2":
                            usuarios = usuarios.OrderBy(s => s.Apellido2);
                            break;
                        case "Telefono1":
                            usuarios = usuarios.OrderBy(s => s.Telefono1);
                            break;
                        case "Telefono2":
                            usuarios = usuarios.OrderBy(s => s.Telefono2);
                            break;
                        default:
                            usuarios = usuarios.OrderBy(s => s.Cedula);
                            break;
                    }
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(usuarios.ToPagedList(pageNumber, pageSize));
        }

    }
}