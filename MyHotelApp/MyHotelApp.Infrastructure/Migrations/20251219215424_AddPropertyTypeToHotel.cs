using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyTypeToHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "Hotels");
        }
    }
}
