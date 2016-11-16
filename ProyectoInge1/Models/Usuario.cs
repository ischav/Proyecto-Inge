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

    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.Cambio = new HashSet<Cambio>();
            this.Requerimiento = new HashSet<Requerimiento>();
            this.Usuarios_asociados_proyecto = new HashSet<Usuarios_asociados_proyecto>();
        }

        [Required(ErrorMessage = "La c�dula es un campo requerido.")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "C�dula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El nombre es un campo requerido.")]
        [RegularExpression(@"[a-zA-Z������\s]+", ErrorMessage = "Solo se pueden ingresar letras")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido 1 es un campo requerido.")]
        [RegularExpression(@"[a-zA-Z������\s]+", ErrorMessage = "Solo se pueden ingresar letras")]
        [Display(Name = "Primer apellido")]
        public string Apellido1 { get; set; }

        [RegularExpression(@"[a-zA-Z������\s]+", ErrorMessage = "Solo se pueden ingresar letras")]
        [Display(Name = "Segundo apellido")]
        public string Apellido2 { get; set; }

        public string NombreCompleto
        {
            get
            {
                return Nombre + " " + Apellido1 + " " + Apellido2;
            }
        }

        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FechaNac { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "Primer tel�fono")]
        public string Telefono1 { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "Solo se pueden ingresar n�meros")]
        [Display(Name = "Segundo tel�fono")]
        public string Telefono2 { get; set; }

        public string Id { get; set; }
        public string Sexo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cambio> Cambio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Requerimiento> Requerimiento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuarios_asociados_proyecto> Usuarios_asociados_proyecto { get; set; }
    }
}
