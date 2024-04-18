﻿// <auto-generated />
using LeagueTool.Services.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LeagueTool.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20240417122801_Add_Config_Data")]
    partial class Add_Config_Data
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-preview.3.24172.4");

            modelBuilder.Entity("LeagueTool.Models.ChampionSave", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(0);

                    b.Property<int>("ChampionKey")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChampionName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Lanes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Played")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Tries")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ChampionSaves");
                });

            modelBuilder.Entity("LeagueTool.Models.ConfigData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(0);

                    b.Property<string>("ConfigName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConfigValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ConfigName")
                        .IsUnique();

                    b.ToTable("ConfigDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
