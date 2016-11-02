using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

/*
 * ModeloIntermedio para el Sprint 1, incluye:
 *   - Tabla de Usuario de la base de datos
 *   - Tabla de Privilegio de la base de datos
 *   - Tabla de Privilegios_asociados_roles de la base de datos
 */
namespace ProyectoInge1.Models
{
	public class ModeloIntermedio
	{
        /* Objeto que corresponde a la tabla Usuario de la base de datos */
		public Usuario modeloUsuario { get; set; }
        public string Cedula { get; set; } /* Atributo correspondiente al Cedula en la tabla Usuario */
        public string Nombre { get; set; } /* Atributo correspondiente al Nombre en la tabla Usuario */
        public string Apellido1 { get; set; } /* Atributo correspondiente al Apellido1 en la tabla Usuario */
        public string Apellido2 { get; set; } /* Atributo correspondiente al Apellido2 en la tabla Usuario */
        public DateTime? FechaNac { get; set; } /* Atributo correspondiente al FechaNac en la tabla Usuario */
        public string Telefono1 { get; set; } /* Atributo correspondiente al Telefono1 en la tabla Usuario */
        public string Telefono2 { get; set; } /* Atributo correspondiente al Telefono2 en la tabla Usuario */
        public string Id { get; set; } /* Atributo correspondiente al Id en la tabla Usuario */

        /* Objeto que corresponde a la tabla Privilegio de la base de datos */
        public Privilegio modeloPrivilegio { get; set; }

        /* Objeto que corresponde a la tabla Privilegios_asociados_roles de la base de datos */
        public Privilegios_asociados_roles modeloPrivilegios_asociados_roles { get; set; }

        /* Objeto que corresponde a la tabla IdentityRole de la base de datos */
        public IdentityRole modeloRol { get; set; }

        /* Objeto que corresponde a las tuplas de la tabla Usuario en la base de datos */
        public List<Usuario> listaPersonas = new List<Usuario>();

        /* Objeto que corresponde a las tuplas de la tabla Privilegio en la base de datos */
        public List<Privilegio> listaPrivilegios = new List<Privilegio>();

        /* Objeto que corresponde a las tuplas de la tabla Privilegios_asociados_roles en la base de datos */
        public List<Privilegios_asociados_roles> listaPrivilegios_asociados_roles = new List<Privilegios_asociados_roles>();

        /* Objeto que corresponde a las tuplas de la tabla IdentityRole en la base de datos */
        public List<IdentityRole> listaRoles = new List<IdentityRole>();

        /* Objeto int que corresponde al valor para controlar cuando el modelo ha cambiado */
        public int cambiosGuardados = 0;

        /* Objetos string que corresponden a los valores para almacenar el id de usuario y el id de rol del usuario logeado en el sistema */
        public string usuarioActualId { get; set; }
        public string rolActualId { get; set; } 

        /* Objeto bool que corresponde al valor para verificar si un usuario logeado tiene permiso o no para editar privilegios */
        public bool privilegios { get; set; }

        /* Variables para el control de los privilegios relacionados a roles en las funcionalidades de Usuario */
        public bool agregar { get; set; }
        public bool consultar { get; set; }
        public bool modificar { get; set; }
        public bool eliminar { get; set; }

        /* Objeto string que corresponde al valor para almacenar el correo del usuario logeado en el sistema */
        public string aspUserEmail { get; set; }

        /* 
         * Objeto modeloCrear que corresponde al modelo para acceder a las atributos email, password y rol del usuario
         * logeado
         */
        public modeloCrear modCrear { get; set; } 
		
        /* Objeto bool que corresponde al valor para verificar si ha ocurrido un error de validación */
		public bool errorValidacion { get; set; }
	}

    /* 
     * Clases Gate y CheckBoxes, para acceder a los valores id de los checkboxes de la vista Index del módulo de 
     * seguridad 
     */
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