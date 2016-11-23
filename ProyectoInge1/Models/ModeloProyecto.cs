using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoInge1.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

/*
 * ModeloProyecto para el Sprint 2, incluye:
 *   - Tabla de Proyecto de la base de datos
 *   - Tabla de Requerimiento de la base de datos
 *   - Tabla de CriteriosAceptacion de la base de datos
 */
namespace ProyectoInge1.Models
{
    public class ModeloProyecto
    {
        /* Objeto que corresponde al id de las tuplas en la tabla Proyecto de la base de datos */
        [Display(Name = "Proyecto")]
        public string proyectoRequerimiento { get; set; }

        /* Objeto que corresponde a la tabla Proyecto de la base de datos */
        public Proyecto modeloProyecto { get; set; }

        /* Objeto que corresponde a la tabla Cambio de la base de datos */
        public Cambio modeloCambio { get; set; }

        /* Objeto que corresponde a la tabla Requerimiento de la base de datos */
        public Requerimiento modeloRequerimiento { get; set; }

        /* Objeto que corresponde a la tabla CriterioAceptacion de la base de datos */
        public CriterioAceptacion modeloCriterioAceptacion { get; set; }

        /* Objeto que corresponde a la tabla Usuarios_asociados_proyecto de la base de datos */
        public Usuarios_asociados_proyecto modeloUsuarios_asociados_proyecto { get; set; }

        /* Objeto que corresponde a la tabla Usuario de la base de datos */
        public Usuario modeloUsuario { get; set; }

        /* Objeto string que corresponde al nombre del solicitante del Requerimiento de la base de datos */
        [Display(Name = "Solicitante")]
        public string solicitante { get; set; }

        /* Objeto string que corresponde al nombre del responsable del Requerimiento de la base de datos */
        [Display(Name = "Responsable")]
        public string responsable { get; set; }

        public int accion = 0;

        /* Objeto int que corresponde al valor para controlar cuando el modelo ha cambiado */
        public int cambiosGuardados = 0;

        //[Required(ErrorMessage = "Este campo es obligatorio.")]
        [Display(Name = "Cliente")]
        public string cliente { get; set; }

        /* Objeto que corresponde a las tuplas de la tabla Proyecto en la base de datos */
        public List<Proyecto> listaProyectos = new List<Proyecto>();

        /* Objeto que corresponde a las tuplas de la tabla Cambio en la base de datos */
        public List<Cambio> listaCambios = new List<Cambio>();

        /* Objeto que corresponde a las tuplas de la tabla Requerimiento en la base de datos */
        public List<Requerimiento> listaRequerimientos = new List<Requerimiento>();

        /* Objeto que corresponde a las tuplas de la tabla CriterioAceptacion en la base de datos */
        public List<CriterioAceptacion> listaCriteriosAceptacion = new List<CriterioAceptacion>();

        /* Objeto que corresponde a las tuplas de la tabla Usuarios_asociados_proyecto en la base de datos */
        public List<Usuarios_asociados_proyecto> listaUsuarios_asociados_proyecto = new List<Usuarios_asociados_proyecto>();

        /* Objeto que corresponde a las tuplas de la tabla Usuarios en la base de datos */
        public List<Usuario> listaUsuariosCliente = new List<Usuario>();

        /* Objeto que corresponde a las tuplas de la tabla Usuarios en la base de datos */
        public List<Usuario> listaUsuariosDesarrolladores = new List<Usuario>();

        /* Objeto bool que corresponde al valor para verificar si ha ocurrido un error de validación */
        public bool errorValidacion { get; set; }

        /* Objeto string que corresponde al valor para especificar la ruta de la imagen del requerimiento */
        [Display(Name = "Imagen")]
        public string rutaImagen { get; set; }
		
		[Display(Name = "Imagen")]
        public string rutaImagenCambio { get; set; }

        /* Objetos string que correesponde a la específicación del rol de un usuario.*/
        public string rolActualId { get; set; }
        public string rolProyectoActual { get; set; }
        public string usuarioActualId { get; set; }
        public string liderId { get; set; }

        /* Objeto que corresponde a las tuplas de la tabla Usuarios en la base de datos */
        public List<Usuario> listaUsuariosSolicitantesCambios = new List<Usuario>();

        /* Objeto string que corresponde al nombre del responsable del Requerimiento de la base de datos */
        [Display(Name = "Solicitante Cambio")]
        public string solicitanteCambio { get; set; }
    }
}
