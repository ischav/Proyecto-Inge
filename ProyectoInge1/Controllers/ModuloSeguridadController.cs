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

        //Verifica si un permiso X lo tiene asignado un rol Y
        private bool verificaPrivilegios(string privilegio)
        {
            string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userId).Roles.First();
            var privilegioId = baseDatos.Privilegio.Where(m => m.Descripcion == privilegio).First().Id;
            var listaRoles = baseDatos.Privilegios_asociados_roles.Where(m => m.Id_Privilegio == privilegioId).ToList().Select(n => n.Id_Rol);
            bool userRol = listaRoles.Contains(rol.RoleId);

            return userRol;
        }

        // GET: ModuloSeguridad
        public ActionResult Index()
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaRoles = context.Roles.ToList();
            modelo.listaPrivilegios = baseDatos.Privilegio.ToList();
            modelo.listaPrivilegios_asociados_roles = baseDatos.Privilegios_asociados_roles.ToList();
            modelo.cambiosGuardados = 0;

            //Obtener el usuario actual
            modelo.usuarioActualId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            modelo.rolActualId = context.Users.Find(modelo.usuarioActualId).Roles.First().RoleId;

            return View(modelo);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Guardar()
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaRoles = context.Roles.ToList();
            modelo.listaPrivilegios = baseDatos.Privilegio.ToList();
            modelo.listaPrivilegios_asociados_roles = baseDatos.Privilegios_asociados_roles.ToList();

            try
            {
                Gate gate = new Gate();
                TryUpdateModel(gate);

                if (ModelState.IsValid)
                {
                    for (int i = 0; i < modelo.listaPrivilegios_asociados_roles.Count; i++)
                    {
                        baseDatos.Privilegios_asociados_roles.Remove(modelo.listaPrivilegios_asociados_roles.ElementAt(i));
                        baseDatos.SaveChanges();
                    }
                    gate.PreprationRequired = Request.Form["pr"];
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
                modelo.listaPrivilegios_asociados_roles = baseDatos.Privilegios_asociados_roles.ToList();
                modelo.cambiosGuardados = 1;
            }
            catch
            {
                modelo.cambiosGuardados = 2;
            }

            return View(modelo);
        }
    }
}