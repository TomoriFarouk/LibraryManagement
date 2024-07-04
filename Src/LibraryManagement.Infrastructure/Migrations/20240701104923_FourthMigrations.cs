using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FourthMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patron_books_BookId",
                table: "patron");

            migrationBuilder.DropIndex(
                name: "IX_patron_BookId",
                table: "patron");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "patron");

            migrationBuilder.AddColumn<int>(
                name: "BooksId",
                table: "patron",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_patron_BooksId",
                table: "patron",
                column: "BooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_patron_books_BooksId",
                table: "patron",
                column: "BooksId",
                principalTable: "books",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patron_books_BooksId",
                table: "patron");

            migrationBuilder.DropIndex(
                name: "IX_patron_BooksId",
                table: "patron");

            migrationBuilder.DropColumn(
                name: "BooksId",
                table: "patron");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "patron",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_patron_BookId",
                table: "patron",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_patron_books_BookId",
                table: "patron",
                column: "BookId",
                principalTable: "books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
