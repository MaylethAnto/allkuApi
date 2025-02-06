using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllkuApi.Models
{
    public class GPS
    {
        [Key]
        [Column("id_gps")]
        public int IdGps { get; set; }

        [Column("id_canino")]
        public int IdCanino { get; set; }

        [Column("fecha_gps")]
        public DateTime? FechaGps { get; set; }

        [Column("distancia_km")]
        public decimal? DistanciaKm { get; set; }

        [Column("inicio_latitud", TypeName = "decimal(10, 7)")]
        public decimal? InicioLatitud { get; set; }

        [Column("inicio_longitud", TypeName = "decimal(10, 7)")]
        public decimal? InicioLongitud { get; set; }

        [Column("fin_latitud", TypeName = "decimal(10, 7)")]
        public decimal? FinLatitud { get; set; }

        [Column("fin_longitud", TypeName = "decimal(10, 7)")]
        public decimal? FinLongitud { get; set; }

        [ForeignKey(nameof(IdCanino))]
        public virtual Canino? Canino { get; set; }
    }
}