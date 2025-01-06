using AllkuApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllkuApi.Models
{
    public class HistorialRequest
    {

        [Required]
        public int id_historial { get; set; }

        [Required]
        public DateTime fecha_historial { get; set; }

        [Required]
        public string tipo_historial { get; set; }

        [Required]
        public string descripcion_historial { get; set; }

        [Required]
        public bool notificacion_historial { get; set; }

        [Required]
        public int id_canino { get; set; }


        [Required]
        public string nombre_canino { get; set; }

    }
}