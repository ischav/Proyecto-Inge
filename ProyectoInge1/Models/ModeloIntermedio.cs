using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProyectoInge1.Models
{
	public class ModeloIntermedio
	{
		public Usuario modeloUsuario { get; set; }
        public Privilegio modeloPrivilegio { get; set; }
		public Privilegios_asociados_roles modeloPrivilegios_asociados_roles { get; set; }        
		public List<Usuario> listaPersonas = new List<Usuario>();
		public List<Privilegio> listaPrivilegios = new List<Privilegio>();
		public List<Privilegios_asociados_roles> listaPrivilegios_asociados_roles = new List<Privilegios_asociados_roles>();
        public IdentityRole modeloRol { get; set; }
        public List<IdentityRole> listaRoles = new List<IdentityRole>();
        public int cambiosGuardados = 0;

        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public DateTime? FechaNac { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Id { get; set; }

        // Rol del usuario loggeado al sistemma
        public string currentUserRole { get; set; }
        public string usuarioActualId { get; set; }
        public string rolActualId { get; set; }

        //Email Asp
        public string aspUserEmail { get; set; }

        //Objeto que tiene los atributos de las tablas de Asp
        public modeloCrear modCrear { get; set; } 
	}

    public class Gate
    {
        public string PreprationRequired { get; set; }
        public List<CheckBoxes> lstPreprationRequired { get; set; }
        public string[] CategoryIds { get; set; }
    }

    public class CheckBoxes
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
    }
}