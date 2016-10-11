using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoInge1.Models;
using Microsoft.AspNet.Identity;

namespace ProyectoInge1.Controllers
{
    public class HomeController : Controller
    {
        Entities baseDatos = new Entities();
        ApplicationDbContext context = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Descripción";

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }
    }
}