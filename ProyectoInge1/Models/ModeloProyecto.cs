using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoInge1.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

/*Modelo intermedio para proyectos, requerimientos y cambios*/

namespace ProyectoInge1.Models
{
    public class ModeloProyecto
    {
        public string proyectoRequerimiento { get; set; }
        public Proyecto modeloProyecto { get; set; }
        public Cambio modeloCambio { get; set; }
        public Requerimiento modeloRequerimiento { get; set; }
        public CriterioAceptacion modeloCriterioAceptacion { get; set; }
        public Usuarios_asociados_proyecto modeloUsuarios_asociados_proyecto { get; set; }
		public Usuario modeloUsuario { get; set; }
        public int accion = 0;
        public int cambiosGuardados = 0;

        //Listas
        public List<Proyecto> listaProyectos = new List<Proyecto>();
        public List<Cambio> listaCambios = new List<Cambio>();
        public List<Requerimiento> listaRequerimientos = new List<Requerimiento>();
        public List<CriterioAceptacion> listaCriteriosAceptacion = new List<CriterioAceptacion>();
        public List<Usuarios_asociados_proyecto> listaUsuarios_asociados_proyecto = new List<Usuarios_asociados_proyecto>();
        public bool errorValidacion { get; set; }
    }
}