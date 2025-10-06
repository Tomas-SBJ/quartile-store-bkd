using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuartileStore.Commons.Migrations
{
    /// <inheritdoc />
    public partial class RenameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_stores_StoreId",
                schema: "quartile",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_stores_companies_CompanyId",
                schema: "quartile",
                table: "stores");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                schema: "quartile",
                table: "stores",
                newName: "company_id");

            migrationBuilder.RenameIndex(
                name: "IX_stores_CompanyId",
                schema: "quartile",
                table: "stores",
                newName: "IX_stores_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_stores_code_CompanyId",
                schema: "quartile",
                table: "stores",
                newName: "IX_stores_code_company_id");

            migrationBuilder.RenameColumn(
                name: "descriptions",
                schema: "quartile",
                table: "products",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                schema: "quartile",
                table: "products",
                newName: "store_id");

            migrationBuilder.RenameIndex(
                name: "IX_products_StoreId",
                schema: "quartile",
                table: "products",
                newName: "IX_products_store_id");

            migrationBuilder.RenameIndex(
                name: "IX_products_code_StoreId",
                schema: "quartile",
                table: "products",
                newName: "IX_products_code_store_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_stores_store_id",
                schema: "quartile",
                table: "products",
                column: "store_id",
                principalSchema: "quartile",
                principalTable: "stores",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stores_companies_company_id",
                schema: "quartile",
                table: "stores",
                column: "company_id",
                principalSchema: "quartile",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_stores_store_id",
                schema: "quartile",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_stores_companies_company_id",
                schema: "quartile",
                table: "stores");

            migrationBuilder.RenameColumn(
                name: "company_id",
                schema: "quartile",
                table: "stores",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_stores_company_id",
                schema: "quartile",
                table: "stores",
                newName: "IX_stores_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_stores_code_company_id",
                schema: "quartile",
                table: "stores",
                newName: "IX_stores_code_CompanyId");

            migrationBuilder.RenameColumn(
                name: "store_id",
                schema: "quartile",
                table: "products",
                newName: "StoreId");

            migrationBuilder.RenameColumn(
                name: "description",
                schema: "quartile",
                table: "products",
                newName: "descriptions");

            migrationBuilder.RenameIndex(
                name: "IX_products_store_id",
                schema: "quartile",
                table: "products",
                newName: "IX_products_StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_products_code_store_id",
                schema: "quartile",
                table: "products",
                newName: "IX_products_code_StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_products_stores_StoreId",
                schema: "quartile",
                table: "products",
                column: "StoreId",
                principalSchema: "quartile",
                principalTable: "stores",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stores_companies_CompanyId",
                schema: "quartile",
                table: "stores",
                column: "CompanyId",
                principalSchema: "quartile",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
