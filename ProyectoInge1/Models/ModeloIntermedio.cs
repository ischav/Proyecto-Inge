﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoInge1.Models
{
	public class ModeloIntermedio
	{
        // Gestión de usuarios
		public Usuario modeloUsuario { get; set; }
        public List<Usuario> listaPersonas = new List<Usuario>();

        // Seguridad
        public Privilegio modeloPrivilegio { get; set; }
		public Privilegios_asociados_roles modeloPrivilegios_asociados_roles { get; set; }
		public List<Privilegio> listaPrivilegios = new List<Privilegio>();
		public List<Privilegios_asociados_roles> listaPrivilegios_asociados_roles = new List<Privilegios_asociados_roles>();
        public IdentityRole modeloRol { get; set; }
        public List<IdentityRole> listaRoles = new List<IdentityRole>();
    }
}