using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    [Table("Notificacion")]
    public class Notificacion
    {
        [Key]
        [Column("id_notificacion")]
        public int IdNotificacion { get; set; }

        [Column("mensaje")]
        public string Mensaje { get; set; }

        [Column("numero_paseador")]
        public string NumeroPaseador { get; set; }

        [Column("cedula_dueno")]
        public string CedulaDueno { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("leida")]
        public bool Leida { get; set; }

        public Dueno Dueno { get; set; } // Relación con el dueño
    }
}