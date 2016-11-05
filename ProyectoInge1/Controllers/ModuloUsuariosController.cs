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

        /* Método que carga el modelo para la vista Index
         * Requiere: no aplica
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
        public ActionResult Index(string sortOrder, string tipo, string currentFilter, string searchString, int? page)
        {
            /*
             * Se crea el modelo:
             * Se obtiene la llave primaria del usuario actual (tabla generada por ASP) y se busca al usuario correspondiente 
             * en la base de datos (tabla de la base de datos)
             * Se verifica si el usuario actual cuenta con permisos para realizar la acción
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;
            Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-I", modelo.rolActualId);
            if (privilegio == null)
                modelo.agregar = false;
            else
                modelo.agregar = true;

            /* 
             * Se asignan los valores necesarios para la paginación y el filtrado 
             */ 
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            /*
             * Se asigna el valor de la primer página a la paginación, en el caso de que no se aplique un filtro y un valor
             * correspondiente al filtro en el caso de que se aplique
             */
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;

            /*
             * Se asigna a la variable usuarios, el valor de todos los usuarios de la base de datos
             */
            var usuarios = from s in baseDatos.Usuario
                           select s;

            /* 
             * Si existe una hilera con la cual filtrar, entonces se devuelven las tuplas de la tabla Usuario de la base
             * de datos, que cumplen con tener el valor de ese filtro en alguno de los atributos
             */
            if (!String.IsNullOrEmpty(searchString))
            {
                usuarios = usuarios.Where(s => s.Cedula.Contains(searchString) || s.Apellido1.Contains(searchString)
                                       || s.Apellido2.Contains(searchString) || s.Nombre.Contains(searchString) ||
                                        s.Telefono1.Contains(searchString) || s.Telefono2.Contains(searchString));
            }

            /*
             * De acuerdo al atributo con el que se desea ordenar, se ordena ya sea ascendente o descendentemente
             */
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

            /*
             * Se asigna un valor a la paginación y un número de página, en caso de que no sea 1
             */
            int pageSize = 7;
            int pageNumber = (page ?? 1);

            /*
             * Se retorna a la vista, el modelo con el usuarios con los cambios de paginación y filtrado
             */
            return View(usuarios.ToPagedList(pageNumber, pageSize));
        }

        /* Método elimina un elemento en la vista Index
         * Requiere: parámetro recibido válido en la base de datos
         * Modifica: la tabla Usuario de la base de datos
         * Retorna: redirección a la vista Index
         */
        public ActionResult eliminarUsuario(string Id)
        {
            /*
             * Se crea un modelo para la vista, se encuentra la tupla de la tabla Usuario en la base de datos, con id igual
             * al parámetro recibido y se elimina esa tupla de la base de datos, tanto en la tabla de usuarios de ASP, como
             * en la de la base de datos
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.modeloUsuario = baseDatos.Usuario.Find(Id);
            baseDatos.Usuario.Remove(modelo.modeloUsuario);
            baseDatos.SaveChanges();
            ApplicationUser user = context.Users.Find(Id);
            context.Users.Remove(user);
            context.SaveChanges();

            /*
             * Se retorna a la vista Index
             */
            return RedirectToAction("Index");
        }

        /* Método para consultar un usuario para la vista MEC_Unificado
         * Requiere: parámetro recibido válido en la base de datos
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
        public ActionResult MEC_Unificado(string id)
        {
            /*
             * Se crea el modelo:
             * Se obtiene la llave primaria del usuario actual (tabla generada por ASP) y se busca al usuario correspondiente 
             * en la base de datos (tabla de la base de datos)
             * Se verifica si el usuario actual cuenta con permisos para realizar las acciones
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;
            Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-M", modelo.rolActualId);
            if (privilegio == null)
                modelo.modificar = false;
            else
                modelo.modificar = true;
            privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-C", modelo.rolActualId);
            if (privilegio == null)
                modelo.consultar = false;
            else
                modelo.consultar = true;

            privilegio = baseDatos.Privilegios_asociados_roles.Find("GUS-E", modelo.rolActualId);
            if (privilegio == null)
                modelo.eliminar = false;
            else
                modelo.eliminar = true;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /*
             * Si el usuario existe:
             *   - Se asigna al atributo modeloUsuario el valor del usuario correspondiente al id
             *   - Se asigna el email a ese usuario
             *   - Se asigna un 0 a la variable de cambios guardados
             */
            modelo.modeloUsuario = baseDatos.Usuario.Find(id);
            if (modelo.modeloUsuario == null)
            {
                return HttpNotFound();
            }
            else
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var usr = manager.FindById(modelo.modeloUsuario.Id);
                if (usr != null)
                    modelo.aspUserEmail = usr.Email;
                else
                    modelo.aspUserEmail = " ";
            }

            /*
            * Se asigna el valor de 0 a la variable de cambios guardados, indicando que no se ha modificado la vista
            */
            modelo.cambiosGuardados = 0;

            /*
             * Se retorna el modelo a la vista
             */
            return View(modelo);
        }

        /* Método que guarda los cambios en la vista de MEC_Unificado
         * Requiere: no aplica
         * Modifica: la tabla de Usuario
         * Retorna: el modelo actualizado
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MEC_Unificado(ModeloIntermedio modelo, string aceptar, string cancelar)
        {
            /*
             * Si el estado del modelo es válido y se desea modificar el elemento:
             *   - Se busca el usuario al que permitenece el id correspondiente al usuario del modelo y se modifica el elemento
             *   - Se busca el email relacionado a ese elemento en las tablas de ASP
             *   - Se guardan los cambios
             */
            if (ModelState.IsValid && !string.IsNullOrEmpty(aceptar))
            {
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var manager = new UserManager<ApplicationUser>(store);
                var usr = manager.FindById(modelo.modeloUsuario.Id);

                if (usr != null)
                {
                    usr.Email = modelo.aspUserEmail;
                    var context = store.Context as ApplicationDbContext;
                    context.SaveChangesAsync();
                }

                baseDatos.Entry(modelo.modeloUsuario).State = EntityState.Modified;
                baseDatos.SaveChanges();

                modelo.errorValidacion = false;

                /*
                 * Se asigna el valor de 1 a la variable de cambios guardados, indicando que se ha modificado la vista
                 */
                modelo.cambiosGuardados = 1; 
            }
            else { 
                /*
                 * Si se desea cancelar la vista:
                 *   - Se reestablecen los campos de cada uno de los elementos en la vista
                 *   - Se envía el modelo con los datos actuales en la base de datos
                 */
                if (!string.IsNullOrEmpty(cancelar))
                {
                    ModelState.Clear();
                    return View(limpiarCampos(modelo.modeloUsuario.Id));
                }

                modelo.errorValidacion = true;
                
                /*
                 * Se asigna el valor de 2 a la variable de cambios guardados, indicando que se han descartado los cambios en la vista
                 */
                modelo.cambiosGuardados = 2; 
            }
            return View(modelo);
        }

        /* Método que obtiene los valores actuales sobre el usuario en la base de datos, en la vista de MEC_Unificado
         * Requiere: no aplica
         * Modifica: la tabla de Usuario
         * Retorna: el modelo actualizado
         */
        private ModeloIntermedio limpiarCampos(string id)
        {
            /*
             * Se obtiene la información de la tupla de la tabla Usuario en la base de datos, correspondiente al id que se
             * recibe como parámetro y se asigna el email 
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.modeloUsuario = baseDatos.Usuario.Find(id);
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var usr = manager.FindById(id);
            if (usr != null)
            {
                modelo.aspUserEmail = usr.Email;
            }

            /*
             * Se asigna el valor de 3 a la variable de cambios guardados, indicando que se han descartado los cambios (se limpió
             * el modelo)
             */
            modelo.cambiosGuardados = 3;

            /*
             * Se retorna el modelo recargado
             */
            return modelo;
        }

        //Metodo GET pantalla Crear
        public ActionResult Create()
        {
            return View();
        }
        
        /* Método que crea elementos en la vista de MEC_Unificado
         * Requiere: datos ingresados válidos
         * Modifica: la tabla de Usuario
         * Retorna: el modelo actualizado
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModeloIntermedio modelo)
        {
            /*
             * Si el estado del modelo es válido:
             *   - Se crea la tupla de la tabla Usuario de la base de datos, con los valores ingresados y se le envía un correo al 
             *     correo especificado, al usuario para darle a conocer su contraseña
             */
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

                /*
                 * Si no se logra ingresar la tupla, se indica que el elemento ya ha sido ingresado con anterioridad
                 */
                ModelState.AddModelError("", "El correo indicado ya fue registrado.");

                /*
                 * Se retorna a la vista el modelo actual
                 */
                return View(modelo);
            }
            else
            {
                /*
                 * Si el estado del modelo no es válido, se indica que se debe completar toda la información necesaria
                 */
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");

                /*
                 * Se retorna a la vista el modelo actual
                 */
                return View(modelo);
            }
        }

        /* Método para enviar un correo con la contraseña a un usuario
         * Requiere: no aplica
         * Modifica: no aplica
         * Retorna: mo aplica
         */
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
                ("SistemaRequerimientosSoporte@gmail.com", "Adrian96!");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }

        /* Método para consultar un elemento en la vista Informacion
         * Requiere: datos ingresados válidos
         * Modifica: la tabla de Usuario
         * Retorna: el modelo actualizado
         */
        public ActionResult Informacion(int? message)
        {
            /*
             * Se asigna a la variable StatusMessage el valor correspondiente a si se descartaron o no los cambios realizados
             */
            ViewBag.StatusMessage = message.Equals(1) ? "Su contraseña ha sido cambiada exitosamente" :
                 message.Equals(2) ? "Cambios descartados" : "";

            /*
             * Se verifica que el usuario logeado tenga una tupla en la tabla de Usuario de la base de datos y se le asigna el 
             * email al modelo
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.modeloUsuario = baseDatos.Usuario.Find(User.Identity.GetUserId());
            if (modelo.modeloUsuario == null)
                return HttpNotFound();
            else
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var usr = manager.FindById(User.Identity.GetUserId());
                if (usr != null)
                    modelo.aspUserEmail = usr.Email;
                else
                    modelo.aspUserEmail = " ";
            }

            /*
             * Se asigna el valor de 0 a la variable de cambios guardados, indicando que se no se han modificado los datos
             */
            modelo.cambiosGuardados = 0;

            /*
             * Se retorna la vista al modelo
             */
            return View(modelo);
        }

        /* Método para modificar un elemento en la vista Informacion
         * Requiere: datos ingresados válidos
         * Modifica: la tabla de Usuario
         * Retorna: el modelo actualizado
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Informacion(ModeloIntermedio modelo, string aceptar, string cancelar)
        {
            /*
             * Si el estado del modelo es válido y no se descartó la información:
             *   - Si encuentra la tupla de la tabla Usuario de la base de datos, del usuario logeado
             *   - Se modifica la tupla en la tabla Usuario y se modifica el correo
             */
            if (ModelState.IsValid && !string.IsNullOrEmpty(aceptar))
            {
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var manager = new UserManager<ApplicationUser>(store);
                var usr = manager.FindById(modelo.modeloUsuario.Id);

                if (usr != null)
                {
                    usr.Email = modelo.aspUserEmail;
                    var context = store.Context as ApplicationDbContext;
                    context.SaveChangesAsync();
                }
                
                /*
                 * Se guardan los cambios realizados en la base de datos
                 */
                baseDatos.Entry(modelo.modeloUsuario).State = EntityState.Modified;
                baseDatos.SaveChanges();

                /*
                 * Se asigna el valor de 0 a la variable de cambios guardados, indicando que se no se han modificado los datos
                 */
                modelo.cambiosGuardados = 1;
            }
            else
            {
                /*
                 * Si se descartó la información, se limpian los campos con la información actual en la base de datos, sino se asigna
                 * el valor de true a la variabel de error de validación, indicando que el estado del modelo no es válido
                 */
                if (!string.IsNullOrEmpty(cancelar))
                {
                    ModelState.Clear();
                    return View(limpiarCampos(modelo.modeloUsuario.Id));
                }
                modelo.errorValidacion = true;

            }

            /*
             * Se retorna el modelo actualizado a la vista
             */
            return View(modelo);
        }
    }
}
