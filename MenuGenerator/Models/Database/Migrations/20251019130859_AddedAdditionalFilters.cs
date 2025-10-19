using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuGenerator.Models.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedAdditionalFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayMenuDishEntity_DayMenuTemplateEntity_DayMenuTemplateEntityId",
                table: "DayMenuDishEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_DayMenuDishEntity_DishFilterEntity_DishFilterId",
                table: "DayMenuDishEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_DayMenuDishEntity_DishTypes_DishTypeId",
                table: "DayMenuDishEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedDayMenuTemplateEntity_DayMenuTemplateEntity_DayMenuTemplateId",
                table: "OrderedDayMenuTemplateEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishFilterEntity",
                table: "DishFilterEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DayMenuTemplateEntity",
                table: "DayMenuTemplateEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DayMenuDishEntity",
                table: "DayMenuDishEntity");

            migrationBuilder.DropIndex(
                name: "IX_DayMenuDishEntity_DishFilterId",
                table: "DayMenuDishEntity");

            migrationBuilder.RenameTable(
                name: "DishFilterEntity",
                newName: "DishFilters");

            migrationBuilder.RenameTable(
                name: "DayMenuTemplateEntity",
                newName: "DayMenuTemplates");

            migrationBuilder.RenameTable(
                name: "DayMenuDishEntity",
                newName: "DayMenuDishes");

            migrationBuilder.RenameIndex(
                name: "IX_DayMenuDishEntity_DishTypeId",
                table: "DayMenuDishes",
                newName: "IX_DayMenuDishes_DishTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DayMenuDishEntity_DayMenuTemplateEntityId",
                table: "DayMenuDishes",
                newName: "IX_DayMenuDishes_DayMenuTemplateEntityId");

            migrationBuilder.AddColumn<Guid>(
                name: "AttributeId",
                table: "DishFilters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DishFilterWithDependentsEntityId",
                table: "DishFilters",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Negate",
                table: "DishFilters",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishFilters",
                table: "DishFilters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DayMenuTemplates",
                table: "DayMenuTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DayMenuDishes",
                table: "DayMenuDishes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DishFilters_AttributeId",
                table: "DishFilters",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_DishFilters_DishFilterWithDependentsEntityId",
                table: "DishFilters",
                column: "DishFilterWithDependentsEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DayMenuDishes_DishFilterId",
                table: "DayMenuDishes",
                column: "DishFilterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DayMenuDishes_DayMenuTemplates_DayMenuTemplateEntityId",
                table: "DayMenuDishes",
                column: "DayMenuTemplateEntityId",
                principalTable: "DayMenuTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DayMenuDishes_DishFilters_DishFilterId",
                table: "DayMenuDishes",
                column: "DishFilterId",
                principalTable: "DishFilters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DayMenuDishes_DishTypes_DishTypeId",
                table: "DayMenuDishes",
                column: "DishTypeId",
                principalTable: "DishTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishFilters_DishAttributes_AttributeId",
                table: "DishFilters",
                column: "AttributeId",
                principalTable: "DishAttributes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishFilters_DishFilters_DishFilterWithDependentsEntityId",
                table: "DishFilters",
                column: "DishFilterWithDependentsEntityId",
                principalTable: "DishFilters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedDayMenuTemplateEntity_DayMenuTemplates_DayMenuTemplateId",
                table: "OrderedDayMenuTemplateEntity",
                column: "DayMenuTemplateId",
                principalTable: "DayMenuTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DayMenuDishes_DayMenuTemplates_DayMenuTemplateEntityId",
                table: "DayMenuDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DayMenuDishes_DishFilters_DishFilterId",
                table: "DayMenuDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DayMenuDishes_DishTypes_DishTypeId",
                table: "DayMenuDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DishFilters_DishAttributes_AttributeId",
                table: "DishFilters");

            migrationBuilder.DropForeignKey(
                name: "FK_DishFilters_DishFilters_DishFilterWithDependentsEntityId",
                table: "DishFilters");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedDayMenuTemplateEntity_DayMenuTemplates_DayMenuTemplateId",
                table: "OrderedDayMenuTemplateEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishFilters",
                table: "DishFilters");

            migrationBuilder.DropIndex(
                name: "IX_DishFilters_AttributeId",
                table: "DishFilters");

            migrationBuilder.DropIndex(
                name: "IX_DishFilters_DishFilterWithDependentsEntityId",
                table: "DishFilters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DayMenuTemplates",
                table: "DayMenuTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DayMenuDishes",
                table: "DayMenuDishes");

            migrationBuilder.DropIndex(
                name: "IX_DayMenuDishes_DishFilterId",
                table: "DayMenuDishes");

            migrationBuilder.DropColumn(
                name: "AttributeId",
                table: "DishFilters");

            migrationBuilder.DropColumn(
                name: "DishFilterWithDependentsEntityId",
                table: "DishFilters");

            migrationBuilder.DropColumn(
                name: "Negate",
                table: "DishFilters");

            migrationBuilder.RenameTable(
                name: "DishFilters",
                newName: "DishFilterEntity");

            migrationBuilder.RenameTable(
                name: "DayMenuTemplates",
                newName: "DayMenuTemplateEntity");

            migrationBuilder.RenameTable(
                name: "DayMenuDishes",
                newName: "DayMenuDishEntity");

            migrationBuilder.RenameIndex(
                name: "IX_DayMenuDishes_DishTypeId",
                table: "DayMenuDishEntity",
                newName: "IX_DayMenuDishEntity_DishTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DayMenuDishes_DayMenuTemplateEntityId",
                table: "DayMenuDishEntity",
                newName: "IX_DayMenuDishEntity_DayMenuTemplateEntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishFilterEntity",
                table: "DishFilterEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DayMenuTemplateEntity",
                table: "DayMenuTemplateEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DayMenuDishEntity",
                table: "DayMenuDishEntity",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DayMenuDishEntity_DishFilterId",
                table: "DayMenuDishEntity",
                column: "DishFilterId");

            migrationBuilder.AddForeignKey(
                name: "FK_DayMenuDishEntity_DayMenuTemplateEntity_DayMenuTemplateEntityId",
                table: "DayMenuDishEntity",
                column: "DayMenuTemplateEntityId",
                principalTable: "DayMenuTemplateEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DayMenuDishEntity_DishFilterEntity_DishFilterId",
                table: "DayMenuDishEntity",
                column: "DishFilterId",
                principalTable: "DishFilterEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DayMenuDishEntity_DishTypes_DishTypeId",
                table: "DayMenuDishEntity",
                column: "DishTypeId",
                principalTable: "DishTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedDayMenuTemplateEntity_DayMenuTemplateEntity_DayMenuTemplateId",
                table: "OrderedDayMenuTemplateEntity",
                column: "DayMenuTemplateId",
                principalTable: "DayMenuTemplateEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
