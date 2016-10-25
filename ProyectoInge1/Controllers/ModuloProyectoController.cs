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

        private ModeloProyecto limpiarCampos(string id)
        {
            ModeloProyecto modelo = new ModeloProyecto();
            /*modelo.modeloUsuario = baseDatos.Usuario.Find(cedula, id);
            //Se obtiene el email de AspNetUsers
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var usr = manager.FindById(id);
            if (usr != null)
            {
                modelo.aspUserEmail = usr.Email;
            }*/
            modelo.cambiosGuardados = 3; //cambios descartados
            return modelo;
        }


        //Metodo GET para la pantalla unificada. Corresponde a consultar
        public ActionResult MEC_Unificado(string id)
        {
            ModeloProyecto modeloPr = new ModeloProyecto();
            ModeloIntermedio modelo = new ModeloIntermedio();
            //id = "Proy01";

            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;
            var usuarioAct = from m in baseDatos.Usuario
             select m;

            if (modelo.usuarioActualId != null)
            {
                usuarioAct = usuarioAct.Where(s => s.Id.Contains(modelo.usuarioActualId));
            }

            usuarioAct.ToList();

            Usuario user = usuarioAct.First();
            modeloPr.modeloProyecto = baseDatos.Proyecto.Find(id);

            // Verificación de los privilegios disponibles en el modulo de usuarios y
            // asociadoos al rol del usuario loggeado en el sistema.
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

            modelo.cambiosGuardados = 0; //no se muestra mensaje
            return View(modeloPr);
        }

        //Metodo POST para la pantalla unificada. Corresponde a modificar

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_Unificado(ModeloProyecto modelo, string aceptar, string cancelar)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(aceptar))
            {

                //Para guardar en aspNetUsers
                /*var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var manager = new UserManager<ApplicationUser>(store);
                var usr = manager.FindById(modelo.modeloUsuario.Id);

                if (usr != null)
                {
                    usr.Email = modelo.aspUserEmail;
                    var context = store.Context as ApplicationDbContext;
                    context.SaveChangesAsync();
                }

                //Para guardar en tabla usuarios
                baseDatos.Entry(modelo.modeloUsuario).State = EntityState.Modified;
                baseDatos.SaveChanges();

                modelo.errorValidacion = false;
                */
                modelo.cambiosGuardados = 1; //cambios guardados
            }
            else
            {

                /*if (!string.IsNullOrEmpty(cancelar))
                {
                    ModelState.Clear();
                    return View(limpiarCampos(modelo.modeloUsuario.Cedula, modelo.modeloUsuario.Id));
                }
                modelo.errorValidacion = true;*/
                modelo.cambiosGuardados = 2; //cambios no guardados
            }
            return View(modelo);
        }

        public ActionResult eliminarProyecto(string Id)
        {

            //Borra el proyecto de la tabla Proyectos

            return RedirectToAction("Index");
        }
    }
}