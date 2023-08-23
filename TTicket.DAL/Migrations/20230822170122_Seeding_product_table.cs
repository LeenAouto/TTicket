using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TTicket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Seeding_product_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "User",
            //    keyColumn: "Id",
            //    keyValue: new Guid("59595c35-2424-45db-ab48-9ee6934de7fe"));

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0be7fd1c-2ce9-49b5-8215-106f5603da6e"), "RiCH" },
                    { new Guid("3cedb0e2-8468-4df9-ad3e-b46d55828a8d"), "Ibraq" },
                    { new Guid("a72de71b-2614-4950-9550-8f03ab9433e4"), "Ole5" },
                    { new Guid("a9f33e03-e63e-49d9-b0dc-fab15e1b29f9"), "Availo" },
                    { new Guid("c1ad4403-588b-4186-9970-45affacae3be"), "Msegat" },
                    { new Guid("d38d6e40-97a3-4658-be34-fec26c2be5ae"), "Reedoo" },
                    { new Guid("efb6dd29-ed26-48c8-a348-e2b59bf43263"), "Dots" },
                    { new Guid("fc8456a9-7300-48c4-979a-4db868d88046"), "Sigma5" }
                });

            //migrationBuilder.InsertData(
            //    table: "User",
            //    columns: new[] { "Id", "Address", "DateOfBirth", "Email", "FirstName", "LastName", "MobilePhone", "Password", "StatusUser", "TypeUser", "Username" },
            //    values: new object[] { new Guid("96448664-d6c8-42e4-9f93-65dccdb6def7"), "Saudi Arabia, Qassim, Buraydah", new DateTime(2000, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "leen.aouto@gmail.com", "Leen", "Aouto", "0545529216", "euFyR/GNXbvnR5m67HGT5A==;6V1GBtK3fSqamrMoLbEt/pGeekF5czrDQKIMcDwJ9Cg=", (byte)1, (byte)1, "manager" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("0be7fd1c-2ce9-49b5-8215-106f5603da6e"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("3cedb0e2-8468-4df9-ad3e-b46d55828a8d"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("a72de71b-2614-4950-9550-8f03ab9433e4"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("a9f33e03-e63e-49d9-b0dc-fab15e1b29f9"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("c1ad4403-588b-4186-9970-45affacae3be"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("d38d6e40-97a3-4658-be34-fec26c2be5ae"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("efb6dd29-ed26-48c8-a348-e2b59bf43263"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: new Guid("fc8456a9-7300-48c4-979a-4db868d88046"));

            //migrationBuilder.DeleteData(
            //    table: "User",
            //    keyColumn: "Id",
            //    keyValue: new Guid("96448664-d6c8-42e4-9f93-65dccdb6def7"));

            //migrationBuilder.InsertData(
            //    table: "User",
            //    columns: new[] { "Id", "Address", "DateOfBirth", "Email", "FirstName", "LastName", "MobilePhone", "Password", "StatusUser", "TypeUser", "Username" },
            //    values: new object[] { new Guid("59595c35-2424-45db-ab48-9ee6934de7fe"), "Saudi Arabia, Qassim, Buraydah", new DateTime(2000, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "leen.aouto@gmail.com", "Leen", "Aouto", "0545529216", "VHXdBussrAks4If4DarVZg==;fHXwTTrzNGgJttELwo9s305e3zhbx+T+ns+vLn/0nFw=", (byte)1, (byte)1, "manager" });
        }
    }
}
