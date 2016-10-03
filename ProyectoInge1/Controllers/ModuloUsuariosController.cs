using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoInge1.Models;

namespace ProyectoInge1.Controllers
{
    public class ModuloUsuariosController : Controller
    {
        Entities baseDatos = new Entities();

        public ActionResult Index()
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaPersonas = baseDatos.Usuario.ToList();
            return View(modelo);
        }

    }
}