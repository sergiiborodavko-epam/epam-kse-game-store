﻿// <auto-generated />
using System;
using EpamKse.GameStore.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EpamKse.GameStore.DataAccess.Migrations
{
    [DbContext(typeof(GameStoreDbContext))]
    [Migration("20250624113653_AddBlackListUpd")]
    partial class AddBlackListUpd
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<string>("GenreIds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("PublisherId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("PublisherId");

                    b.ToTable("Games");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Description for Game 1",
                            GenreIds = "1,2,4",
                            Price = 49.99m,
                            ReleaseDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Stock = 12,
                            Title = "Game 1"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Description for Game 2",
                            GenreIds = "11,13",
                            Price = 59.99m,
                            ReleaseDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Stock = 12,
                            Title = "Game 2"
                        });
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.GameBan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId", "Country")
                        .IsUnique();

                    b.ToTable("GameCountryBans", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Country = "UA",
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            GameId = 1
                        });
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.GameFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<int>("PlatformId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.HasKey("Id");

                    b.HasIndex("GameId", "PlatformId")
                        .IsUnique();

                    b.ToTable("GameFiles", (string)null);
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("ParentGenreId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentGenreId");

                    b.ToTable("Genres");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Strategy"
                        },
                        new
                        {
                            Id = 2,
                            Name = "RTS",
                            ParentGenreId = 1
                        },
                        new
                        {
                            Id = 3,
                            Name = "TBS",
                            ParentGenreId = 1
                        },
                        new
                        {
                            Id = 4,
                            Name = "RPG"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Sports"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Races",
                            ParentGenreId = 5
                        },
                        new
                        {
                            Id = 7,
                            Name = "Rally",
                            ParentGenreId = 5
                        },
                        new
                        {
                            Id = 8,
                            Name = "Arcade",
                            ParentGenreId = 5
                        },
                        new
                        {
                            Id = 9,
                            Name = "Formula",
                            ParentGenreId = 5
                        },
                        new
                        {
                            Id = 10,
                            Name = "Off-road",
                            ParentGenreId = 5
                        },
                        new
                        {
                            Id = 11,
                            Name = "Action"
                        },
                        new
                        {
                            Id = 12,
                            Name = "FPS",
                            ParentGenreId = 11
                        },
                        new
                        {
                            Id = 13,
                            Name = "TPS",
                            ParentGenreId = 11
                        },
                        new
                        {
                            Id = 14,
                            Name = "Adventure",
                            ParentGenreId = 11
                        },
                        new
                        {
                            Id = 15,
                            Name = "Puzzle & Skill"
                        });
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.HistoricalPrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("HistoricalPrices", (string)null);
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.License", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Licenses", (string)null);
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalSum")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders", (string)null);
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("Platforms", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "android"
                        },
                        new
                        {
                            Id = 2,
                            Name = "ios"
                        },
                        new
                        {
                            Id = 3,
                            Name = "windows"
                        },
                        new
                        {
                            Id = 4,
                            Name = "vr"
                        });
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomePageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Publishers", (string)null);
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Country")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Country = 0,
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            Email = "admin@example.com",
                            FullName = "Admin User",
                            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=1$hIWcROP/j0uU/PceT+/jHw$Kn1RHnAoDdMitEPzaT43//MwsEDJMwAjEPr8liXCHrM",
                            Role = "Admin",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("GameOrder", b =>
                {
                    b.Property<int>("GamesId")
                        .HasColumnType("int");

                    b.Property<int>("OrdersId")
                        .HasColumnType("int");

                    b.HasKey("GamesId", "OrdersId");

                    b.HasIndex("OrdersId");

                    b.ToTable("OrderGames", (string)null);
                });

            modelBuilder.Entity("GamePlatform", b =>
                {
                    b.Property<int>("GamesId")
                        .HasColumnType("int");

                    b.Property<int>("PlatformsId")
                        .HasColumnType("int");

                    b.HasKey("GamesId", "PlatformsId");

                    b.HasIndex("PlatformsId");

                    b.ToTable("GamePlatforms", (string)null);
                });

            modelBuilder.Entity("PlatformPublisher", b =>
                {
                    b.Property<int>("PublisherPlatformsId")
                        .HasColumnType("int");

                    b.Property<int>("PublishersId")
                        .HasColumnType("int");

                    b.HasKey("PublisherPlatformsId", "PublishersId");

                    b.HasIndex("PublishersId");

                    b.ToTable("PublisherPlatforms", (string)null);
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Game", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Publisher", "Publisher")
                        .WithMany("Games")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.GameBan", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Game", "Game")
                        .WithMany("GameBans")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.GameFile", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Game", null)
                        .WithMany("GameFiles")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Genre", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Genre", "ParentGenre")
                        .WithMany("SubGenres")
                        .HasForeignKey("ParentGenreId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ParentGenre");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.HistoricalPrice", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Game", "Game")
                        .WithMany("HistoricalPrices")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.License", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Order", "Order")
                        .WithOne("License")
                        .HasForeignKey("EpamKse.GameStore.Domain.Entities.License", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Order", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("GameOrder", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EpamKse.GameStore.Domain.Entities.Order", null)
                        .WithMany()
                        .HasForeignKey("OrdersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GamePlatform", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EpamKse.GameStore.Domain.Entities.Platform", null)
                        .WithMany()
                        .HasForeignKey("PlatformsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PlatformPublisher", b =>
                {
                    b.HasOne("EpamKse.GameStore.Domain.Entities.Platform", null)
                        .WithMany()
                        .HasForeignKey("PublisherPlatformsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EpamKse.GameStore.Domain.Entities.Publisher", null)
                        .WithMany()
                        .HasForeignKey("PublishersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Game", b =>
                {
                    b.Navigation("GameBans");

                    b.Navigation("GameFiles");

                    b.Navigation("HistoricalPrices");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Genre", b =>
                {
                    b.Navigation("SubGenres");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Order", b =>
                {
                    b.Navigation("License")
                        .IsRequired();
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.Publisher", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("EpamKse.GameStore.Domain.Entities.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
