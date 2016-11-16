using ProyectoInge1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoInge1.Controllers
{
    public class ModuloCambiosController : Controller{

        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        /*
        public ActionResult Edit(ModeloProyecto modelo)
        {
            if (ModelState.IsValid)
            {
                baseDatos.Entry(modelo.modeloCambio).State = EntityState.Modified;
                baseDatos.SaveChanges();
            }
            else
            {

            }
        } */

    }
}