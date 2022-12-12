using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WingtipToys.Migrations
{
    /// <inheritdoc />
    public partial class SeedingUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 1; i <= 150; i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns : new [] {
                        "Id",
                        "UserName",
                        "Email",
                        "EmailConfirmed",
                        "SecurityStamp",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAdress"
                    },
                    values: new object[]{
                        Guid.NewGuid().ToString(),
                        "user-" + i.ToString("D3"),
                        $"email"+i.ToString("D3") + "@example.com",
                        true,
                        Guid.NewGuid().ToString(),
                        false,
                        false,
                        false,
                        0,
                        "...@#$..."
                    }
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
