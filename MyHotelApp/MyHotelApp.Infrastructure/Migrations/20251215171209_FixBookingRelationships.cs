using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AdultsCapacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChildrenCapacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "HotelId",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RoomNumber",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CityId",
                table: "Hotels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Hotels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StarRating",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostOffice",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "HotelId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CityId",
                table: "Hotels",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_HotelId",
                table: "Bookings",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Hotels_HotelId",
                table: "Bookings",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Cities_CityId",
                table: "Hotels",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Hotels_HotelId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Cities_CityId",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_CityId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_HotelId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdultsCapacity",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ChildrenCapacity",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "StarRating",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "PostOffice",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
