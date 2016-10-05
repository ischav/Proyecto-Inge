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

namespace ProyectoInge1.Controllers
{
    public class ModuloUsuariosController : Controller
    {
        Entities baseDatos = new Entities();

	//Metodo GET index usuarios
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

public ActionResult eliminarUsuario(string cedula, string Id) {

            //Borra al usuario de la tabla Usuarios
            ModeloIntermedio modelo = new ModeloIntermedio();
			modelo.modeloUsuario = baseDatos.Usuario.Find(cedula, Id);
			baseDatos.Usuario.Remove(modelo.modeloUsuario);
			baseDatos.SaveChanges();

            //Borra al usuario de la tabla AspNetUsers
            var bd = new ApplicationDbContext();
            var user = bd.Users.Find(Id);
            bd.Users.Remove(user);
            bd.SaveChanges();

                return RedirectToAction("Index");
		}

		//Metodo GET para la pantalla unificada. Corresponde a consultar
		public ActionResult MEC_Unificado(string cedula, string id) {
			if(id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			ModeloIntermedio modelo = new ModeloIntermedio();
			modelo.modeloUsuario = baseDatos.Usuario.Find(cedula, id);
			if(modelo.modeloUsuario == null) {
				return HttpNotFound();
			}else {
				//Se obtiene el email de AspNetUsers
				var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
				var usr = manager.FindById(modelo.modeloUsuario.Id);
				if(usr != null) {
					modelo.aspUserEmail = usr.Email;
				} else {
					modelo.aspUserEmail = "Nulo";
				}
			}


			return View(modelo);
		}

		//Metodo POST para la pantalla unificada. Corresponde a modificar
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult MEC_Unificado(ModeloIntermedio modelo) {
			if(ModelState.IsValid) {
				
				//Para guardar en aspNetUsers
				var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
				var manager = new UserManager<ApplicationUser>(store);
				var usr = manager.FindById(modelo.modeloUsuario.Id);

				if(usr != null) {
					usr.Email = modelo.aspUserEmail;
					var context = store.Context as ApplicationDbContext;
					context.SaveChangesAsync();
				}

                //Para guardar en tabla usuarios
                baseDatos.Entry(modelo.modeloUsuario).State = EntityState.Modified;
				baseDatos.SaveChanges();

			} else {
				ModelState.AddModelError("", "Debe completar toda la información necesaria.");
			}
            return View(modelo);
		}

        //Metodo GET pantalla Crear
        public ActionResult Create()
        {
            return View();
        }


        //Metodo POST de pantalla Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModeloIntermedio modelo)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = modelo.modCrear.Email, Email = modelo.modCrear.Email };
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));            
                var result = await manager.CreateAsync(user, modelo.modCrear.Password);
                modelo.modeloUsuario.Id = user.Id;
                manager.AddToRole(user.Id, modelo.modCrear.Rol);

                baseDatos.Usuario.Add(modelo.modeloUsuario);
                baseDatos.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return View(modelo);
            }
        }


    }
}