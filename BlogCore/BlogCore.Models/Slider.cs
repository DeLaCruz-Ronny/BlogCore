using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogCore.Models
{
    public class Slider
    {
        [key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese un nombre para el Slider")]
        [Display(Name = "Nombre Slider")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imagen")]
        public string UrlImagen { get; set; }

    }
}
