using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InquiryChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_Vehicles_VehicleId",
                table: "Inquiries");

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "Inquiries",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "DealerMessage",
                table: "Inquiries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Inquiries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_Type",
                table: "Inquiries",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_Vehicles_VehicleId",
                table: "Inquiries",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_Vehicles_VehicleId",
                table: "Inquiries");

            migrationBuilder.DropIndex(
                name: "IX_Inquiries_Type",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "DealerMessage",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Inquiries");

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "Inquiries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_Vehicles_VehicleId",
                table: "Inquiries",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
