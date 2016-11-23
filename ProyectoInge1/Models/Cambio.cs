//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProyectoInge1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Cambio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cambio()
        {
            this.CriterioAceptacionHistorial = new HashSet<CriterioAceptacionHistorial>();
        }

        public int IdSolicitud { get; set; }
        public string IdRequerimiento { get; set; }

        [Display(Name = "Proyecto")]
        public string IdProyecto { get; set; }

        [Required(ErrorMessage = "El nombre es un campo requerido.")]
        [RegularExpression(@"[0-9a-zA-Z\-_������\s]+", ErrorMessage = "Solo se pueden ingresar letras, n�meros y guiones")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "Esfuerzo")]
        public string Esfuerzo { get; set; }

        [Required(ErrorMessage = "El estado es un campo requerido.")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "La descripci�n es un campo requerido.")]
        [Display(Name = "Descripci�n")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FechaInicio { get; set; }

        [Display(Name = "Fecha final")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FechaFinal { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "Sprint")]
        public string Sprint { get; set; }

        [RegularExpression(@"[0-9a-zA-Z\-_������\s]+", ErrorMessage = "Solo se pueden ingresar letras, n�meros y guiones")]
        [Display(Name = "M�dulo")]
        public string Modulo { get; set; }


        public string Observaciones { get; set; }
        public byte[] Imagen { get; set; }

        [Display(Name = "Responsable")]
        public string IdResponsable { get; set; }

        [Display(Name = "Solicitante")]
        public string IdSolicitante { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "Versi�n")]
        public Nullable<int> Version { get; set; }

        [Display(Name = "Descripci�n de los cambios")]
        [DataType(DataType.MultilineText)]
        public string DescripcionCambio { get; set; }

        [Display(Name = "Justificaci�n de los cambios")]
        [DataType(DataType.MultilineText)]
        public string JustificacionCambio { get; set; }

        public string SolicitanteCambio { get; set; }

        [Display(Name = "Fecha de solicitud")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FechaCambio { get; set; }
        public string EstadoSolicitud { get; set; }

        [Display(Name = "Observaciones Solicitud")]
        [DataType(DataType.MultilineText)]
        public string ObservacionesSolicitud { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "Versi�n del Cambio")]
        public Nullable<int> VersionCambio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CriterioAceptacionHistorial> CriterioAceptacionHistorial { get; set; }
        public virtual Requerimiento Requerimiento { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}

