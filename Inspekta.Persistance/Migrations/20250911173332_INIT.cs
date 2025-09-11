using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inspekta.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class INIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "PassHash", "Salt", "Role", "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy" },
                values: new object[]
                {
                    Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    "Inspekta",
                    "1ad8a4c6fa0a84514f1f1fc92b6e16130233a571764db6c5bae8413d1e33957f",
                    "RtK3gvva4x7jPnjb",
                    0,
                    new DateTime(2025,9,11,0,0,0,DateTimeKind.Utc),
                    Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    new DateTime(2025,9,11,0,0,0,DateTimeKind.Utc),
                    Guid.Parse("00000000-0000-0000-0000-000000000001")
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
