﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using com.b_velop.Mqtt.Context;

namespace com.b_velop.Mqtt.Context.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200529185415_AddMeasureTypes")]
    partial class AddMeasureTypes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.MeasureTime", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Timestamp");

                    b.ToTable("MeasureTimes");
                });

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.MeasureType", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Name");

                    b.ToTable("MeasureTypes");
                });

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.MeasureValue", b =>
                {
                    b.Property<string>("RoomName")
                        .HasColumnType("text");

                    b.Property<string>("MeasureTimeTimestamp")
                        .HasColumnType("text");

                    b.Property<string>("SensorTypeName")
                        .HasColumnType("text");

                    b.Property<string>("MeasureTypeName")
                        .HasColumnType("text");

                    b.Property<DateTime?>("MeasureTimeTimestamp1")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.HasKey("RoomName", "MeasureTimeTimestamp", "SensorTypeName", "MeasureTypeName");

                    b.HasIndex("MeasureTimeTimestamp1");

                    b.HasIndex("MeasureTypeName");

                    b.HasIndex("SensorTypeName");

                    b.ToTable("MeasureValues");
                });

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.MqttMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ContentType")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("Topic")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("MqttMessages");
                });

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.MqttUser", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.HasKey("Username");

                    b.ToTable("MqttUsers");
                });

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.Room", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Name");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.SensorType", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Name");

                    b.ToTable("SensorTypes");
                });

            modelBuilder.Entity("com.b_velop.Mqtt.Domain.Models.MeasureValue", b =>
                {
                    b.HasOne("com.b_velop.Mqtt.Domain.Models.MeasureTime", "MeasureTime")
                        .WithMany("MeasureValues")
                        .HasForeignKey("MeasureTimeTimestamp1");

                    b.HasOne("com.b_velop.Mqtt.Domain.Models.MeasureType", "MeasureType")
                        .WithMany()
                        .HasForeignKey("MeasureTypeName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("com.b_velop.Mqtt.Domain.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("com.b_velop.Mqtt.Domain.Models.SensorType", "SensorType")
                        .WithMany()
                        .HasForeignKey("SensorTypeName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
