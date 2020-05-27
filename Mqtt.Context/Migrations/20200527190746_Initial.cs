﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace com.b_velop.Mqtt.Context.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasureTimes",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureTimes", x => x.Timestamp);
                });

            migrationBuilder.CreateTable(
                name: "MeasureType",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureType", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "MqttMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Topic = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MqttMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MqttUsers",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MqttUsers", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SensorTypes",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "MeasureValues",
                columns: table => new
                {
                    MeasureTimeTimestamp = table.Column<string>(nullable: false),
                    MeasureTypeName = table.Column<string>(nullable: false),
                    RoomName = table.Column<string>(nullable: false),
                    SensorTypeName = table.Column<string>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    MeasureTimeTimestamp1 = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureValues", x => new { x.RoomName, x.MeasureTimeTimestamp, x.SensorTypeName, x.MeasureTypeName });
                    table.ForeignKey(
                        name: "FK_MeasureValues_MeasureTimes_MeasureTimeTimestamp1",
                        column: x => x.MeasureTimeTimestamp1,
                        principalTable: "MeasureTimes",
                        principalColumn: "Timestamp",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasureValues_MeasureType_MeasureTypeName",
                        column: x => x.MeasureTypeName,
                        principalTable: "MeasureType",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasureValues_Rooms_RoomName",
                        column: x => x.RoomName,
                        principalTable: "Rooms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasureValues_SensorTypes_SensorTypeName",
                        column: x => x.SensorTypeName,
                        principalTable: "SensorTypes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasureValues_MeasureTimeTimestamp1",
                table: "MeasureValues",
                column: "MeasureTimeTimestamp1");

            migrationBuilder.CreateIndex(
                name: "IX_MeasureValues_MeasureTypeName",
                table: "MeasureValues",
                column: "MeasureTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_MeasureValues_SensorTypeName",
                table: "MeasureValues",
                column: "SensorTypeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasureValues");

            migrationBuilder.DropTable(
                name: "MqttMessages");

            migrationBuilder.DropTable(
                name: "MqttUsers");

            migrationBuilder.DropTable(
                name: "MeasureTimes");

            migrationBuilder.DropTable(
                name: "MeasureType");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "SensorTypes");
        }
    }
}
