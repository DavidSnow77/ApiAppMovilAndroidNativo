using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abarrotes.Migrations
{
    /// <inheritdoc />
    public partial class CrearUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ PRIMERO: Crear las funciones SQL
            migrationBuilder.Sql(@"
                -- ============================================
                -- FUNCIÓN PARA USUARIOS (USER-######)
                -- ============================================
                CREATE SEQUENCE IF NOT EXISTS usuarios_seq START 1;
                
                CREATE OR REPLACE FUNCTION generar_clave_usuario()
                RETURNS varchar AS $$
                BEGIN
                    RETURN 'USER-' || LPAD(nextval('usuarios_seq')::text, 6, '0');
                END;
                $$ LANGUAGE plpgsql;
                
                -- ============================================
                -- FUNCIÓN PARA PRODUCTOS (PROD-######)
                -- ============================================
                CREATE SEQUENCE IF NOT EXISTS productos_seq START 1;
                
                CREATE OR REPLACE FUNCTION generar_clave_producto()
                RETURNS varchar AS $$
                BEGIN
                    RETURN 'PROD-' || LPAD(nextval('productos_seq')::text, 6, '0');
                END;
                $$ LANGUAGE plpgsql;
                
                -- ============================================
                -- FUNCIÓN PARA VENTAS (VENT-######)
                -- ============================================
                CREATE SEQUENCE IF NOT EXISTS ventas_seq START 1;
                
                CREATE OR REPLACE FUNCTION generar_clave_venta()
                RETURNS varchar AS $$
                BEGIN
                    RETURN 'VENT-' || LPAD(nextval('ventas_seq')::text, 6, '0');
                END;
                $$ LANGUAGE plpgsql;
            ");

            // ✅ LUEGO: Crear la tabla
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(type: "varchar", nullable: false, defaultValueSql: "generar_clave_usuario()"),
                    Usuario = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Clave = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioId", x => x.UsuarioId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar tabla
            migrationBuilder.DropTable(
                name: "Usuarios");

            // Eliminar funciones y secuencias
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS generar_clave_usuario();
                DROP FUNCTION IF EXISTS generar_clave_producto();
                DROP FUNCTION IF EXISTS generar_clave_venta();
                DROP SEQUENCE IF EXISTS usuarios_seq;
                DROP SEQUENCE IF EXISTS productos_seq;
                DROP SEQUENCE IF EXISTS ventas_seq;
            ");
        }
    }
}