using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoInge1.Models;

namespace ProyectoInge1.Controllers
{
    public class ModuloSeguridadController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        // GET: ModuloSeguridad
        public ActionResult Index()
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaRoles = context.Roles.ToList();
            modelo.listaPrivilegios = baseDatos.Privilegio.ToList();
            modelo.listaPrivilegios_asociados_roles = baseDatos.Privilegios_asociados_roles.ToList();
            modelo.cambiosGuardados = 0;

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