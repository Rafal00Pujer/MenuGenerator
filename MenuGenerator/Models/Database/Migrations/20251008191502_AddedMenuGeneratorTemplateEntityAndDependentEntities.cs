using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuGenerator.Models.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedMenuGeneratorTemplateEntityAndDependentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DependsOnRuleEntityDishEntity");

            migrationBuilder.DropTable(
                name: "DishEntityMenuEntity");

            migrationBuilder.DropTable(
                name: "OccurenceRules");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.CreateTable(
                name: "DayMenuTemplateEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateDays = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayMenuTemplateEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishFilterEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishFilterEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuGeneratorTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuGeneratorTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DayMenuDishEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DishTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DishFilterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DayMenuTemplateEntityId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayMenuDishEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayMenuDishEntity_DayMenuTemplateEntity_DayMenuTemplateEntityId",
                        column: x => x.DayMenuTemplateEntityId,
                        principalTable: "DayMenuTemplateEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DayMenuDishEntity_DishFilterEntity_DishFilterId",
                        column: x => x.DishFilterId,
                        principalTable: "DishFilterEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DayMenuDishEntity_DishTypes_DishTypeId",
                        column: x => x.DishTypeId,
                        principalTable: "DishTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderedDayMenuTemplateEntity",
                columns: table => new
                {
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    MenuGeneratorTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DayMenuTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedDayMenuTemplateEntity", x => new { x.MenuGeneratorTemplateId, x.Order });
                    table.ForeignKey(
                        name: "FK_OrderedDayMenuTemplateEntity_DayMenuTemplateEntity_DayMenuTemplateId",
                        column: x => x.DayMenuTemplateId,
                        principalTable: "DayMenuTemplateEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderedDayMenuTemplateEntity_MenuGeneratorTemplates_MenuGeneratorTemplateId",
                        column: x => x.MenuGeneratorTemplateId,
                        principalTable: "MenuGeneratorTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayMenuDishEntity_DayMenuTemplateEntityId",
                table: "DayMenuDishEntity",
                column: "DayMenuTemplateEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DayMenuDishEntity_DishFilterId",
                table: "DayMenuDishEntity",
                column: "DishFilterId");

            migrationBuilder.CreateIndex(
                name: "IX_DayMenuDishEntity_DishTypeId",
                table: "DayMenuDishEntity",
                column: "DishTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedDayMenuTemplateEntity_DayMenuTemplateId",
                table: "OrderedDayMenuTemplateEntity",
                column: "DayMenuTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayMenuDishEntity");

            migrationBuilder.DropTable(
                name: "OrderedDayMenuTemplateEntity");

            migrationBuilder.DropTable(
                name: "DishFilterEntity");

            migrationBuilder.DropTable(
                name: "DayMenuTemplateEntity");

            migrationBuilder.DropTable(
                name: "MenuGeneratorTemplates");

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
    }
}
