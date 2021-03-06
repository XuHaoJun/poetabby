﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RCB.JavaScript.Models;
using RCB.JavaScript.Models.Utils;

namespace RCB.JavaScript.Migrations
{
    [DbContext(typeof(PoeDbContext))]
    [Migration("20200820155331_AddPoeLeagueModel")]
    partial class AddPoeLeagueModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0-preview.7.20365.15");

            modelBuilder.Entity("RCB.JavaScript.Models.PoeCharacterModel", b =>
                {
                    b.Property<string>("CharacterId")
                        .HasColumnType("text");

                    b.Property<PoeAccount>("Account")
                        .HasColumnType("jsonb");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CharacterName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Class")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<CountAnalysis>("CountAnalysis")
                        .HasColumnType("jsonb");

                    b.Property<bool>("Dead")
                        .HasColumnType("boolean");

                    b.Property<PoeDepth>("Depth")
                        .HasColumnType("jsonb");

                    b.Property<long>("EnergyShield")
                        .HasColumnType("bigint");

                    b.Property<long>("Experience")
                        .HasColumnType("bigint");

                    b.Property<List<PoeItem>>("Items")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("LeagueName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<long>("LifeUnreserved")
                        .HasColumnType("bigint");

                    b.Property<bool>("Online")
                        .HasColumnType("boolean");

                    b.Property<PathOfBuilding>("Pob")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("PobCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Rank")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(15001);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("CharacterId");

                    b.HasIndex("CharacterId")
                        .IsUnique();

                    b.HasIndex("CountAnalysis")
                        .HasMethod("gin");

                    b.HasIndex("Items")
                        .HasMethod("gin");

                    b.HasIndex("Pob")
                        .HasMethod("gin");

                    b.HasIndex("CharacterName", "AccountName")
                        .IsUnique();

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("RCB.JavaScript.Models.PoeLeagueModel", b =>
                {
                    b.Property<string>("LeagueId")
                        .HasColumnType("text");

                    b.Property<bool>("DelveEvent")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("EndAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Realm")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RegisterAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<List<PoeLeagueRules>>("Rules")
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LeagueId");

                    b.HasIndex("LeagueId")
                        .IsUnique();

                    b.ToTable("Leagues");
                });
#pragma warning restore 612, 618
        }
    }
}
