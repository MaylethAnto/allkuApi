using AllkuApi.DataTransferObjects_DTO_;


namespace AllkuApi.DataTransferObjects_DTO_
{
    public class RecetaDto
    {
        public int id_receta { get; set; }
        public string nombre_receta { get; set; }
        public string descripcion_receta { get; set; }
        public byte[] foto_receta { get; set; }
        public int id_canino { get; set; }
    }
}