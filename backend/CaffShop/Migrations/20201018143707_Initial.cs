using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CaffShop.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    password_hash = table.Column<byte[]>(nullable: true),
                    password_salt = table.Column<byte[]>(nullable: true),
                    is_admin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "caff_items",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    OwnerId = table.Column<long>(nullable: false),
                    UploadedAt = table.Column<DateTime>(nullable: false),
                    PreviewPath = table.Column<string>(nullable: true),
                    CaffPath = table.Column<string>(nullable: true),
                    OriginalName = table.Column<string>(nullable: true),
                    InnerName = table.Column<string>(nullable: true),
                    CaffFileSize = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_caff_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_caff_items_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    content = table.Column<string>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    author_id = table.Column<long>(nullable: false),
                    caff_item_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchases",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    purchased_at = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<long>(nullable: false),
                    caff_item_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchases", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchases_caff_items_caff_item_id",
                        column: x => x.caff_item_id,
                        principalTable: "caff_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchases_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_caff_items_OwnerId",
                table: "caff_items",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_author_id",
                table: "comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_caff_item_id",
                table: "purchases",
                column: "caff_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_user_id",
                table: "purchases",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "purchases");

            migrationBuilder.DropTable(
                name: "caff_items");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
