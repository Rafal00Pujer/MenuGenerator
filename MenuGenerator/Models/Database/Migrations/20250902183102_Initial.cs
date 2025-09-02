using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuGenerator.Models.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allergens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayId = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IncludeInNewMenus = table.Column<bool>(type: "INTEGER", nullable: false),
                    IncludesFirstSupplement = table.Column<bool>(type: "INTEGER", nullable: false),
                    IncludesSecondSupplement = table.Column<bool>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "AllergenEntityDishEntity",
                columns: table => new
                {
                    AllergenListId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DishListId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergenEntityDishEntity", x => new { x.AllergenListId, x.DishListId });
                    table.ForeignKey(
                        name: "FK_AllergenEntityDishEntity_Allergens_AllergenListId",
                        column: x => x.AllergenListId,
                        principalTable: "Allergens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllergenEntityDishEntity_Dishes_DishListId",
                        column: x => x.DishListId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OccurenceRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DishId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RuleType = table.Column<string>(type: "TEXT", maxLength: 34, nullable: false),
                    Days = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OccurenceRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OccurenceRules_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DishEntityMenuEntity",
                columns: table => new
                {
                    DishListId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MenuListDate = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishEntityMenuEntity", x => new { x.DishListId, x.MenuListDate });
                    table.ForeignKey(
                        name: "FK_DishEntityMenuEntity_Dishes_DishListId",
                        column: x => x.DishListId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishEntityMenuEntity_Menus_MenuListDate",
                        column: x => x.MenuListDate,
                        principalTable: "Menus",
                        principalColumn: "Date",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DependsOnRuleEntityDishEntity",
                columns: table => new
                {
                    DependsOnDishListId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DependsOnRuleEntityId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependsOnRuleEntityDishEntity", x => new { x.DependsOnDishListId, x.DependsOnRuleEntityId });
                    table.ForeignKey(
                        name: "FK_DependsOnRuleEntityDishEntity_Dishes_DependsOnDishListId",
                        column: x => x.DependsOnDishListId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DependsOnRuleEntityDishEntity_OccurenceRules_DependsOnRuleEntityId",
                        column: x => x.DependsOnRuleEntityId,
                        principalTable: "OccurenceRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllergenEntityDishEntity_DishListId",
                table: "AllergenEntityDishEntity",
                column: "DishListId");

            migrationBuilder.CreateIndex(
                name: "IX_Allergens_DisplayId",
                table: "Allergens",
                column: "DisplayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DependsOnRuleEntityDishEntity_DependsOnRuleEntityId",
                table: "DependsOnRuleEntityDishEntity",
                column: "DependsOnRuleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DishEntityMenuEntity_MenuListDate",
                table: "DishEntityMenuEntity",
                column: "MenuListDate");

            migrationBuilder.CreateIndex(
                name: "IX_OccurenceRules_DishId",
                table: "OccurenceRules",
                column: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllergenEntityDishEntity");

            migrationBuilder.DropTable(
                name: "DependsOnRuleEntityDishEntity");

            migrationBuilder.DropTable(
                name: "DishEntityMenuEntity");

            migrationBuilder.DropTable(
                name: "Allergens");

            migrationBuilder.DropTable(
                name: "OccurenceRules");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Dishes");
        }
    }
}
