using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Software_Engineering.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Gender_ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Gender_ID);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    User_Type_ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type_Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.User_Type_ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User_Type_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    First_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Last_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Password_Hash = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Created_At = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Is_Activated = table.Column<bool>(type: "INTEGER", nullable: false),
                    Activation_Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_User_Type_ID",
                        column: x => x.User_Type_ID,
                        principalTable: "UserTypes",
                        principalColumn: "User_Type_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Admin_Level = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Last_Login = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_Admins_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carers",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Availability_Schedule = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carers", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_Carers_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clinicians",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    License_Number = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Specialisation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicians", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_Clinicians_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarerAccesses",
                columns: table => new
                {
                    Access_ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Carer_User_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Patient_User_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Permission_Level = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Granted_By_User_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Revoked_By_User_ID = table.Column<int>(type: "INTEGER", nullable: true),
                    Granted_At = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Revoked_At = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CarerUser_ID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarerAccesses", x => x.Access_ID);
                    table.ForeignKey(
                        name: "FK_CarerAccesses_Carers_CarerUser_ID",
                        column: x => x.CarerUser_ID,
                        principalTable: "Carers",
                        principalColumn: "User_ID");
                    table.ForeignKey(
                        name: "FK_CarerAccesses_Users_Carer_User_ID",
                        column: x => x.Carer_User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarerAccesses_Users_Granted_By_User_ID",
                        column: x => x.Granted_By_User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarerAccesses_Users_Patient_User_ID",
                        column: x => x.Patient_User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarerAccesses_Users_Revoked_By_User_ID",
                        column: x => x.Revoked_By_User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    Clinician_User_ID = table.Column<int>(type: "INTEGER", nullable: true),
                    Gender_ID = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Date_of_Birth = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_Patients_Clinicians_Clinician_User_ID",
                        column: x => x.Clinician_User_ID,
                        principalTable: "Clinicians",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Patients_Genders_Gender_ID",
                        column: x => x.Gender_ID,
                        principalTable: "Genders",
                        principalColumn: "Gender_ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Patients_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Gender_ID", "Name" },
                values: new object[,]
                {
                    { 1, "Male" },
                    { 2, "Female" },
                    { 3, "Other" },
                    { 4, "Prefer not to say" }
                });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "User_Type_ID", "Description", "Type_Name" },
                values: new object[,]
                {
                    { 1, "System Administrator with full access", "Admin" },
                    { 2, "Healthcare professional managing patients", "Clinician" },
                    { 3, "Patient user monitoring pressure data", "Patient" },
                    { 4, "Authorized carer with access to patient data", "Carer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarerAccesses_Carer_User_ID",
                table: "CarerAccesses",
                column: "Carer_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CarerAccesses_CarerUser_ID",
                table: "CarerAccesses",
                column: "CarerUser_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CarerAccesses_Granted_By_User_ID",
                table: "CarerAccesses",
                column: "Granted_By_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CarerAccesses_Patient_User_ID",
                table: "CarerAccesses",
                column: "Patient_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CarerAccesses_Revoked_By_User_ID",
                table: "CarerAccesses",
                column: "Revoked_By_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Clinician_User_ID",
                table: "Patients",
                column: "Clinician_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Gender_ID",
                table: "Patients",
                column: "Gender_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_User_Type_ID",
                table: "Users",
                column: "User_Type_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "CarerAccesses");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Carers");

            migrationBuilder.DropTable(
                name: "Clinicians");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}
