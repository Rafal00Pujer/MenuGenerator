using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuGenerator.Models.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDishTypeToEntityAndAddedDishAttributeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludesFirstSupplement",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "IncludesSecondSupplement",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Dishes");

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "Dishes",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "DishAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishAttributeEntityDishEntity",
                columns: table => new
                {
                    AttributeListId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DishListId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishAttributeEntityDishEntity", x => new { x.AttributeListId, x.DishListId });
                    table.ForeignKey(
                        name: "FK_DishAttributeEntityDishEntity_DishAttributes_AttributeListId",
                        column: x => x.AttributeListId,
                        principalTable: "DishAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishAttributeEntityDishEntity_Dishes_DishListId",
                        column: x => x.DishListId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_TypeId",
                table: "Dishes",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DishAttributeEntityDishEntity_DishListId",
                table: "DishAttributeEntityDishEntity",
                column: "DishListId");

            migrationBuilder.CreateIndex(
                name: "IX_DishAttributes_Name",
                table: "DishAttributes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishTypes_Name",
                table: "DishTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_DishTypes_TypeId",
                table: "Dishes",
                column: "TypeId",
                principalTable: "DishTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_DishTypes_TypeId",
                table: "Dishes");

            migrationBuilder.DropTable(
                name: "DishAttributeEntityDishEntity");

            migrationBuilder.DropTable(
                name: "DishTypes");

            migrationBuilder.DropTable(
                name: "DishAttributes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_TypeId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Dishes");

            migrationBuilder.AddColumn<bool>(
                name: "IncludesFirstSupplement",
                table: "Dishes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludesSecondSupplement",
                table: "Dishes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Dishes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
