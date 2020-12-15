using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nex.Data.Mssql.Migrations
{
    public partial class NmxIam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateRole("nex_application");

            migrationBuilder.EnsureSchema(
                name: "nex");

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_account_keys",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_account_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_accounts",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    sync_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    sync_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenant_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    sync_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_groups_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "nex",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_domains",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tenant_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_domains", x => x.id);
                    table.ForeignKey(
                        name: "fk_tenant_domains_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "nex",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_service_accounts",
                schema: "nex",
                columns: table => new
                {
                    service_account_id = table.Column<int>(type: "int", nullable: false),
                    tenant_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_account_tenants", x => new { x.service_account_id, x.tenant_id });
                    table.ForeignKey(
                        name: "fk_service_account_tenants_service_accounts_service_account_id",
                        column: x => x.service_account_id,
                        principalSchema: "nex",
                        principalTable: "service_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_service_account_tenants_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "nex",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pseudonym = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    email_hash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    sync_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tenant_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "nex",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_permissions",
                schema: "nex",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    permission_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_permissions", x => new { x.group_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_group_permissions_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "nex",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalSchema: "nex",
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_service_accounts",
                schema: "nex",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    service_account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_service_accounts", x => new { x.group_id, x.service_account_id });
                    table.ForeignKey(
                        name: "fk_group_service_accounts_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "nex",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_service_accounts_service_accounts_service_account_id",
                        column: x => x.service_account_id,
                        principalSchema: "nex",
                        principalTable: "service_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_members",
                schema: "nex",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    member_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_user1s", x => new { x.group_id, x.member_id });
                    table.ForeignKey(
                        name: "fk_group_user1s_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "nex",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_user1s_users_member_id",
                        column: x => x.member_id,
                        principalSchema: "nex",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_owners",
                schema: "nex",
                columns: table => new
                {
                    owned_group_id = table.Column<int>(type: "int", nullable: false),
                    owner_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_users", x => new { x.owned_group_id, x.owner_id });
                    table.ForeignKey(
                        name: "fk_group_users_groups_owned_group_id",
                        column: x => x.owned_group_id,
                        principalSchema: "nex",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_users_users_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "nex",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personal_access_tokens",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    service_account_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_personal_access_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_personal_access_tokens_service_accounts_service_account_id",
                        column: x => x.service_account_id,
                        principalSchema: "nex",
                        principalTable: "service_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_personal_access_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "nex",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_guest_users",
                schema: "nex",
                columns: table => new
                {
                    guest_tenant_id = table.Column<int>(type: "int", nullable: false),
                    guest_user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_users", x => new { x.guest_tenant_id, x.guest_user_id });
                    table.ForeignKey(
                        name: "fk_tenant_users_tenants_guest_tenant_id",
                        column: x => x.guest_tenant_id,
                        principalSchema: "nex",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tenant_users_users_guest_user_id",
                        column: x => x.guest_user_id,
                        principalSchema: "nex",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_permissions",
                schema: "nex",
                columns: table => new
                {
                    permission_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permission_users", x => new { x.permission_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_permission_users_permissions_permission_id",
                        column: x => x.permission_id,
                        principalSchema: "nex",
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_permission_users_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "nex",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "nex",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    known_role_id = table.Column<short>(type: "smallint", nullable: false),
                    tenant_id = table.Column<int>(type: "int", nullable: true),
                    sync_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    personal_access_token_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_roles_personal_access_tokens_personal_access_token_id",
                        column: x => x.personal_access_token_id,
                        principalSchema: "nex",
                        principalTable: "personal_access_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_roles_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "nex",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_roles",
                schema: "nex",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_roles", x => new { x.group_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_group_roles_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "nex",
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_roles_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "nex",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                schema: "nex",
                columns: table => new
                {
                    permission_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permission_roles", x => new { x.permission_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_permission_roles_permissions_permission_id",
                        column: x => x.permission_id,
                        principalSchema: "nex",
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_permission_roles_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "nex",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_account_roles",
                schema: "nex",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false),
                    service_account_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_service_accounts", x => new { x.role_id, x.service_account_id });
                    table.ForeignKey(
                        name: "fk_role_service_accounts_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "nex",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_service_accounts_service_accounts_service_account_id",
                        column: x => x.service_account_id,
                        principalSchema: "nex",
                        principalTable: "service_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "nex",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_users", x => new { x.role_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_role_users_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "nex",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_users_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "nex",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_group_user1s_member_id",
                schema: "nex",
                table: "group_members",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_users_owner_id",
                schema: "nex",
                table: "group_owners",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_permissions_permission_id",
                schema: "nex",
                table: "group_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_roles_role_id",
                schema: "nex",
                table: "group_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_service_accounts_service_account_id",
                schema: "nex",
                table: "group_service_accounts",
                column: "service_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_tenant_id",
                schema: "nex",
                table: "groups",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_personal_access_tokens_service_account_id",
                schema: "nex",
                table: "personal_access_tokens",
                column: "service_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_personal_access_tokens_user_id",
                schema: "nex",
                table: "personal_access_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_permission_roles_role_id",
                schema: "nex",
                table: "role_permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_personal_access_token_id",
                schema: "nex",
                table: "roles",
                column: "personal_access_token_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_tenant_id",
                schema: "nex",
                table: "roles",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_account_keys_name",
                schema: "nex",
                table: "service_account_keys",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_role_service_accounts_service_account_id",
                schema: "nex",
                table: "service_account_roles",
                column: "service_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_accounts_name",
                schema: "nex",
                table: "service_accounts",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_domains_tenant_id",
                schema: "nex",
                table: "tenant_domains",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_users_guest_user_id",
                schema: "nex",
                table: "tenant_guest_users",
                column: "guest_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_account_tenants_tenant_id",
                schema: "nex",
                table: "tenant_service_accounts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_name",
                schema: "nex",
                table: "tenants",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_permission_users_user_id",
                schema: "nex",
                table: "user_permissions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_users_user_id",
                schema: "nex",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "nex",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_users_email_hash",
                schema: "nex",
                table: "users",
                column: "email_hash");

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id",
                schema: "nex",
                table: "users",
                column: "tenant_id");

            migrationBuilder
                .GrantAll("nex.group_members", "nex_application")
                .GrantAll("nex.group_owners", "nex_application")
                .GrantAll("nex.group_permissions", "nex_application")
                .GrantAll("nex.group_roles", "nex_application")
                .GrantAll("nex.group_service_accounts", "nex_application")
                .GrantAll("nex.role_permissions", "nex_application")
                .GrantAll("nex.service_account_keys", "nex_application")
                .GrantAll("nex.service_account_roles", "nex_application")
                .GrantAll("nex.tenant_domains", "nex_application")
                .GrantAll("nex.tenant_guest_users", "nex_application")
                .GrantAll("nex.tenant_service_accounts", "nex_application")
                .GrantAll("nex.user_permissions", "nex_application")
                .GrantAll("nex.user_roles", "nex_application")
                .GrantAll("nex.groups", "nex_application")
                .GrantAll("nex.permissions", "nex_application")
                .GrantAll("nex.roles", "nex_application")
                .GrantAll("nex.personal_access_tokens", "nex_application")
                .GrantAll("nex.service_accounts", "nex_application")
                .GrantAll("nex.users", "nex_application")
                .GrantAll("nex.tenants", "nex_application");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_members",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "group_owners",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "group_permissions",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "group_roles",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "group_service_accounts",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "role_permissions",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "service_account_keys",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "service_account_roles",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "tenant_domains",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "tenant_guest_users",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "tenant_service_accounts",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "user_permissions",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "groups",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "personal_access_tokens",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "service_accounts",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "users",
                schema: "nex");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "nex");

            migrationBuilder.DropRole("nex_application");
        }
    }
}
