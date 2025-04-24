using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace redis.WebAPi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BenchmarkFinalData",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CacheName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalDuration = table.Column<double>(type: "float", nullable: false),
                    TimeUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GetsRPS = table.Column<double>(type: "float", nullable: false),
                    GetsAverageLatency = table.Column<double>(type: "float", nullable: false),
                    GetsP50 = table.Column<double>(type: "float", nullable: false),
                    GetsP99 = table.Column<double>(type: "float", nullable: false),
                    GetsP99_90 = table.Column<double>(type: "float", nullable: false),
                    GetsP99_99 = table.Column<double>(type: "float", nullable: false),
                    CompressedHistogram = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarkFinalData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BenchmarkQueue",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pw = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Clients = table.Column<int>(type: "int", nullable: false),
                    Threads = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Requests = table.Column<int>(type: "int", nullable: false),
                    Pipeline = table.Column<int>(type: "int", nullable: false),
                    Times = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarkQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BenchmarkRequest",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pw = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Clients = table.Column<int>(type: "int", nullable: false),
                    Threads = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Requests = table.Column<int>(type: "int", nullable: false),
                    Pipeline = table.Column<int>(type: "int", nullable: false),
                    Times = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarkRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BenchmarkResultData",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CacheName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalDuration = table.Column<double>(type: "float", nullable: false),
                    TimeUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GetsRPS = table.Column<double>(type: "float", nullable: false),
                    GetsAverageLatency = table.Column<double>(type: "float", nullable: false),
                    GetsP50 = table.Column<double>(type: "float", nullable: false),
                    GetsP99 = table.Column<double>(type: "float", nullable: false),
                    GetsP99_90 = table.Column<double>(type: "float", nullable: false),
                    GetsP99_99 = table.Column<double>(type: "float", nullable: false),
                    CompressedHistogram = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarkResultData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SnapshotEntity",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapshotEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestCaseEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCaseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuiteId = table.Column<int>(type: "int", nullable: false),
                    SuiteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Steps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SnapshotEntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCaseEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestCaseEntity_SnapshotEntity_SnapshotEntityId",
                        column: x => x.SnapshotEntityId,
                        principalSchema: "dbo",
                        principalTable: "SnapshotEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseEntity_SnapshotEntityId",
                table: "TestCaseEntity",
                column: "SnapshotEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenchmarkFinalData",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BenchmarkQueue",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BenchmarkRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BenchmarkResultData",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TestCaseEntity");

            migrationBuilder.DropTable(
                name: "SnapshotEntity",
                schema: "dbo");
        }
    }
}
