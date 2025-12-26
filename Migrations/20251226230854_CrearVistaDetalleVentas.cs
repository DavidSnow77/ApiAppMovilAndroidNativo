using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abarrotes.Migrations
{
    public partial class CrearVistaDetalleVentas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW vista_detalle_ventas AS
                SELECT 
                    v.""VentaId""        AS ""VentaId"",
                    v.""FechaVenta""     AS ""FechaVenta"",
                    p.""NombreProducto"" AS ""Producto"",
                    p.""Precio""         AS ""Precio"",
                    d.""Cantidad""       AS ""Cantidad"",
                    d.""Estado""         AS ""Estado"",
                    (p.""Precio"" * d.""Cantidad"") AS ""Total""
                FROM ""DetalleVentas"" d
                JOIN ""Ventas""     v ON d.""VentaId""    = v.""VentaId""
                JOIN ""Productos""  p ON d.""ProductoId"" = p.""ProductoId"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vista_detalle_ventas;");
        }
    }
}