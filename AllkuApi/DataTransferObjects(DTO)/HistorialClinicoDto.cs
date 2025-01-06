namespace AllkuApi.DataTransferObjects_DTO_
{
    public class HistorialClinicoDto
    {
        public int id_historial { get; set; } // Cambiar a int?
        public int id_canino { get; set; } // Cambiar a int?
        public DateTime fecha_historial { get; set; } // Cambiar a DateTime?
        public string tipo_historial { get; set; }
        public string descripcion_historial { get; set; }
        public bool notificacion_historial { get; set; } // Cambiar a bool?
    }

}