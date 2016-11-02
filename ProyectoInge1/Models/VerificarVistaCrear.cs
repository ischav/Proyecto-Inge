using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoInge1.Models
{
    public class modeloCrear
    {
        /*
         * Validaciones para el objeto string, correspondiente al valor del email del usuario logeado
         */
        [Required(ErrorMessage = "El email es un campo requerido.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /*
         * Validaciones para el objeto string, correspondiente al valor del password del usuario logeado
         */
        [Required(ErrorMessage = "La contraseña es un campo requerido.")]
        [StringLength(100, ErrorMessage = "La {0} debe ser de al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{6,}$", ErrorMessage = "La contraseña debe contener al menos un símbolo, número y letra mayúscula. Debe ser de al menos 6 caracteres")]
        public string Password { get; set; }

        /*
         * Validaciones para el objeto string, correspondiente al valor de confirmación del password del usuario logeado
         */
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        /*
         * Validaciones para el objeto string, correspondiente al valor del rol del usuario logeado
         */
        [Required]
        [Display(Name = "Rol")]
        public string Rol { get; set; }
    }
}