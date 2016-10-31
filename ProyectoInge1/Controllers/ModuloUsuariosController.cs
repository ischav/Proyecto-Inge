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
    public class ModuloUsuariosController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        //Metodo GET index usuarios
        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            //---------------------
            // Obtener el usuario actual
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;

            // Verificación de los privilegios disponibles en el modulo de usuarios y
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

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(usuarios.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult eliminarUsuario(string Id)
        {

            //Borra al usuario de la tabla Usuarios
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.modeloUsuario = baseDatos.Usuario.Find(Id);
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
        public ActionResult MEC_Unificado(string id)
        {
            ModeloIntermedio modelo = new ModeloIntermedio();

            // Obtener el usuario actual
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;

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

            modelo.modeloUsuario = baseDatos.Usuario.Find(id);
            if (modelo.modeloUsuario == null)
            {
                return HttpNotFound();
            }
            else
            {
                //Se obtiene el email de AspNetUsers
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var usr = manager.FindById(modelo.modeloUsuario.Id);
                if (usr != null)
                {
                    modelo.aspUserEmail = usr.Email;
                }
                else
                {
                    modelo.aspUserEmail = " ";
                }
            }

            modelo.cambiosGuardados = 0; //no se muestra mensaje
            return View(modelo);
        }

        //Metodo POST para la pantalla unificada. Corresponde a modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_Unificado(ModeloIntermedio modelo, string aceptar, string cancelar)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(aceptar))
            {

                //Para guardar en aspNetUsers
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
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
                modelo.cambiosGuardados = 1; //cambios guardados
            }
            else
            {

                if (!string.IsNullOrEmpty(cancelar))
                {
                    ModelState.Clear();
                    return View(limpiarCampos(modelo.modeloUsuario.Id));
                }
                modelo.errorValidacion = true;
                modelo.cambiosGuardados = 2; //cambios no guardados
            }
            return View(modelo);
        }

        private ModeloIntermedio limpiarCampos(string id)
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.modeloUsuario = baseDatos.Usuario.Find(id);
            //Se obtiene el email de AspNetUsers
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var usr = manager.FindById(id);
            if (usr != null)
            {
                modelo.aspUserEmail = usr.Email;
            }
            modelo.cambiosGuardados = 3; //cambios descartados
            return modelo;
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
                if (result.Succeeded)
                {
                    modelo.modeloUsuario.Id = user.Id;
                    manager.AddToRole(user.Id, modelo.modCrear.Rol);
                    baseDatos.Usuario.Add(modelo.modeloUsuario);
                    baseDatos.SaveChanges();
                    MailModel mailModel = new MailModel();
                    mailModel.Body = modelo.modCrear.Password;
                    mailModel.From = "SistemaRequerimientosSoporte@gmail.com";
                    mailModel.To = modelo.modCrear.Email;
                    mailModel.Subject = "Contraseña Sistema de Requerimientos";
                    EnviarCorreo(mailModel);
                    return RedirectToAction("Index");

                }

                ModelState.AddModelError("", "El correo indicado ya fue registrado.");
                return View(modelo);

            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return View(modelo);
            }
        }


        void EnviarCorreo(MailModel _objModelMail)
        {
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress(_objModelMail.From);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Body;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("SistemaRequerimientosSoporte@gmail.com", "Adrian96!");// Enter seders User name and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }
        // ___________________- Este metodo hay que cambiarlo Isa ___________________________
        public ActionResult Informacion(int? message)
        {
            ViewBag.StatusMessage = message.Equals(1) ? "Su contraseña ha sido cambiada exitosamente" :
                 message.Equals(2) ? "Cambios descartados" : "";

            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.modeloUsuario = baseDatos.Usuario.Find(User.Identity.GetUserId());

            if (modelo.modeloUsuario == null)
            {
                return HttpNotFound();
            }
            else
            {
                //Se obtiene el email de AspNetUsers
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var usr = manager.FindById(User.Identity.GetUserId());
                if (usr != null)
                {
                    modelo.aspUserEmail = usr.Email;
                }
                else
                {
                    modelo.aspUserEmail = " ";
                }

            }
            modelo.cambiosGuardados = 0;
            return View(modelo);
        }

        // ___________________- Este metodo hay que cambiarlo Isa ___________________________
        //Metodo POST para la pantalla unificada. Corresponde a modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Informacion(ModeloIntermedio modelo, string aceptar, string cancelar)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(aceptar))
            {

                //Para guardar en aspNetUsers
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
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
                modelo.cambiosGuardados = 1;
            }
            else
            {
                if (!string.IsNullOrEmpty(cancelar))
                {
                    ModelState.Clear();
                    return View(limpiarCampos(modelo.modeloUsuario.Id));
                }
                modelo.errorValidacion = true;

            }
            return View(modelo);
        }
    }
}