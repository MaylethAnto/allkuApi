using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using AllkuApi.Models;

namespace AllkuApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GpsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GpsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("distancia")]
        public async Task<ActionResult<DistanciaRecorrida>> GetDistanciaRecorrida(int id_canino, DateTime fecha_inicio, DateTime fecha_fin)
        {
            var distancia = new DistanciaRecorrida
            {
                FechaInicio = fecha_inicio,
                FechaFin = fecha_fin
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("ObtenerDistanciaRecorrida", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_canino", id_canino);
                        cmd.Parameters.AddWithValue("@fecha_inicio", fecha_inicio);
                        cmd.Parameters.AddWithValue("@fecha_fin", fecha_fin);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                distancia.DistanciaTotal = reader["DistanciaTotal"] != DBNull.Value ? (decimal)reader["DistanciaTotal"] : 0;
                            }
                        }
                    }
                }

                return Ok(distancia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}