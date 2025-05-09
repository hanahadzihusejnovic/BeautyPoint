using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeautyPoint.Migrations
{
    /// <inheritdoc />
    public partial class AddVolumeToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Volume",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Products");
        }
    }
}
