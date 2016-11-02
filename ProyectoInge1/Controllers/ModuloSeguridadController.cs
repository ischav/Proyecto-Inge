using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoInge1.Models;
using Microsoft.AspNet.Identity;

namespace ProyectoInge1.Controllers
{
    public class ModuloSeguridadController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        /* Método que carga el modelo para la vista Index
         * Requiere: no aplica
         * Modifica: el modelo
         * Retorna: el modelo cargado
         */
        public ActionResult Index()
        {
            /*
             * Se crea el modelo:
             *   - Se agregan los roles a la lista de roles (tabla generada por ASP)
             *   - Se agregan los privilegios a la lista de privilegios (tabla de la base de datos)
             *   - Se agregan los privilegios asociados a roles a la lista de privilegios asociados a roles (tabla de la base de datos)
             *   - Se asigna el valor de 0 a la variable de cambios guardados, indicando que no se ha modificado la vista
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaRoles = context.Roles.ToList();
            modelo.listaPrivilegios = baseDatos.Privilegio.ToList();
            modelo.listaPrivilegios_asociados_roles = baseDatos.Privilegios_asociados_roles.ToList();
            modelo.cambiosGuardados = 0;

            /*
             * Se obtiene la llave primaria del usuario actual (tabla generada por ASP) y se busca al usuario correspondiente 
             * en la base de datos (tabla de la base de datos)
             * Se verifica si el usuario actual cuenta con permisos para realizar la acción
             */
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;
            Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("SEG-I", modelo.rolActualId);
            if (privilegio == null)
                modelo.privilegios = false;
            else
                modelo.privilegios = true;
            
            /*
             * Se retorna el modelo a la vista
             */
            return View(modelo);
        }

        /* Método que guarda los cambios en la vista de Index
         * Requiere: usuario logeado tenga permisos para modificar los privilegios según el rol
         * Modifica: la tabla de privilegios asociados a roles
         * Retorna: el modelo actualizado
         */
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Guardar()
        {
            /*
             * Se crea el modelo:
             *   - Se agregan los roles a la lista de roles (tabla generada por ASP)
             *   - Se agregan los privilegios a la lista de privilegios (tabla de la base de datos)
             *   - Se agregan los privilegios asociados a roles a la lista de privilegios asociados a roles (tabla de la base de datos)
             */
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaRoles = context.Roles.ToList();
            modelo.listaPrivilegios = baseDatos.Privilegio.ToList();
            modelo.listaPrivilegios_asociados_roles = baseDatos.Privilegios_asociados_roles.ToList();

            try
            {
                /*
                 * Se crea un objeto de tipo Gate y se actualiza de acuerdo a los valores actuales del controlador
                 */
                Gate gate = new Gate();
                TryUpdateModel(gate);

                /*
                 * Si el estado del modelo es válido, entonces se eliminan todas tuplas de la tabla de privilegios
                 * asociados a roles. 
                 *   - Se accede a todas las "forms" con el atributo name igual a pr, y se guarda en el atributo 
                 *     PreparationRequired, un string con los valores id (formados por el atributo llave de la tabla privilegio
                 *     más, el atributo llave de la tabla de rol) de cada una de las "forms", separados por un espacio
                 *   - Si existe al menos un "form" con nombre igual a pr, entonces se separa cada id del atributo 
                 *     PreparationRequired
                 *   - Para cada id, se separa el atributo llave de cada tabla y se agrega la tupla en la tabla de privilegios
                 *     asociados a roles
                 */
                if (ModelState.IsValid)
                {
                    for (int i = 0; i < modelo.listaPrivilegios_asociados_roles.Count; i++)
                    {
                        baseDatos.Privilegios_asociados_roles.Remove(modelo.listaPrivilegios_asociados_roles.ElementAt(i));
                        baseDatos.SaveChanges();
                    }
                    gate.PreprationRequired = Request.Form["pr"];
                    if (gate.PreprationRequired != null)
                    {
                        string[] cat = gate.PreprationRequired.Split(',');
                        for (int i = 0; i < cat.Length; i++)
                        {
                            string[] v = cat[i].Split('*');

                            Privilegios_asociados_roles pari = new Privilegios_asociados_roles();
                            pari.Id_Privilegio = v[0];
                            pari.Id_Rol = v[1];
                            baseDatos.Privilegios_asociados_roles.Add(pari);

                            baseDatos.SaveChanges();
                        }
                    }
                }
                /*
                 * Se agrega al modelo los privilegios asociados a roles agregados anteriormente
                 */
                modelo.listaPrivilegios_asociados_roles = baseDatos.Privilegios_asociados_roles.ToList();

                /*
                 * Se obtiene la llave primaria del usuario actual (tabla generada por ASP) y se busca al usuario correspondiente 
                 * en la base de datos (tabla de la base de datos)
                 * Se verifica si el usuario actual cuenta con permisos para realizar la acción
                 */
                modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;
                Privilegios_asociados_roles privilegio = baseDatos.Privilegios_asociados_roles.Find("SEG-I", modelo.rolActualId);
                if (privilegio == null)
                    modelo.privilegios = false;
                else
                    modelo.privilegios = true;

                /*
                 * Se asigna el valor de 1 a la variable de cambios guardados, indicando que se ha modificado la vista
                 */
                modelo.cambiosGuardados = 1;
            }
            catch
            {
                /*
                 * Se asigna el valor de 2 a la variable de cambios guardados, indicando que ha ocurrido un error al tratar
                 * de modificar la vista
                 */
                modelo.cambiosGuardados = 2;
            }

            /*
             * Se retorna el modelo actualizado a la vista
             */
            return View(modelo);
        }
    }
}