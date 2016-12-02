using Microsoft.AspNet.Identity;
using ProyectoInge1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoInge1.Controllers
{

	public class PermisosAttribute : ActionFilterAttribute
	{
		ApplicationDbContext context = new ApplicationDbContext();
		private string [] privilegios;

		//Se recibe una lista de privilegios para verificar si el usuario actual los posee
		public PermisosAttribute(params string [] privilegios) {
			this.privilegios = privilegios;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			bool resultado = false;
			Entities bd = new Entities();
			for(int i = 0; i < privilegios.Length; i++) {
				try {
					//Verificacion de privilegios del usuario actual
					string idUsuario = HttpContext.Current.User.Identity.GetUserId();
					string rol = context.Users.Find(idUsuario).Roles.First().RoleId;
					string priv = bd.Privilegios_asociados_roles.Find(privilegios[i], rol)?.Id_Privilegio ?? "";

					if(!string.IsNullOrEmpty(priv)) {
						resultado = true;
					} else {
						resultado = false;
					}
					//Se asigna falso o verdadero si se cuenta con el/los  permiso(s) solicitado(s)
					//Estos atributos Session[privilegio] se consultan en las vistas
					filterContext.HttpContext.Session[privilegios[i]] = resultado;
				}
				catch {
					//En caso de errores al acceder a la base de datos se redirige a una pantalla de error
					filterContext.HttpContext.Response.StatusCode = 500;
					var view = new ViewResult { ViewName = "~/Views/Shared/Error.cshtml", ViewData = new ViewDataDictionary() };
					view.ViewData["errorBD"] = "Error al intentar validar los permisos asociados al rol.";
					filterContext.Result = view;
					break;
				}
			}
		}
	}
}