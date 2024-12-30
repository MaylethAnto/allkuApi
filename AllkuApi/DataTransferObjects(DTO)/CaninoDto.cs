namespace AllkuApi.DataTransferObjects_DTO_
{
    public class CaninoDto
    {
        public int IdCanino { get; set; }
        public string NombreCanino { get; set; }
        public int EdadCanino { get; set; }
        public string RazaCanino { get; set; }
        public decimal PesoCanino { get; set; }
        public byte[] FotoCanino { get; set; }
        public string NombreDueno { get; set; }
    }
}
