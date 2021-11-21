using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogCore.Models
{
    public class Categoria
    {
        [key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingresa un Nombre Para la categoria")]
        [Display(Name = "Nombre Categoria")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Orden de Visualizacion")]
        public string Orden { get; set; }
    }
}
