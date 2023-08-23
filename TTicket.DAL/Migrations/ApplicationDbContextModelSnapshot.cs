﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TTicket.DAL;

#nullable disable

namespace TTicket.DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TTicket.Models.Attachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("AttachedToId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("Attacher")
                        .HasColumnType("tinyint");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("TTicket.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TicketId");

                    b.HasIndex("UserId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("TTicket.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Product");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0be7fd1c-2ce9-49b5-8215-106f5603da6e"),
                            Name = "RiCH"
                        },
                        new
                        {
                            Id = new Guid("a72de71b-2614-4950-9550-8f03ab9433e4"),
                            Name = "Ole5"
                        },
                        new
                        {
                            Id = new Guid("a9f33e03-e63e-49d9-b0dc-fab15e1b29f9"),
                            Name = "Availo"
                        },
                        new
                        {
                            Id = new Guid("efb6dd29-ed26-48c8-a348-e2b59bf43263"),
                            Name = "Dots"
                        },
                        new
                        {
                            Id = new Guid("d38d6e40-97a3-4658-be34-fec26c2be5ae"),
                            Name = "Reedoo"
                        },
                        new
                        {
                            Id = new Guid("fc8456a9-7300-48c4-979a-4db868d88046"),
                            Name = "Sigma5"
                        },
                        new
                        {
                            Id = new Guid("3cedb0e2-8468-4df9-ad3e-b46d55828a8d"),
                            Name = "Ibraq"
                        },
                        new
                        {
                            Id = new Guid("c1ad4403-588b-4186-9970-45affacae3be"),
                            Name = "Msegat"
                        });
                });

            modelBuilder.Entity("TTicket.Models.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint");

                    b.Property<Guid?>("SupportId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("SupportId");

                    b.HasIndex("UserId");

                    b.ToTable("Ticket");
                });

            modelBuilder.Entity("TTicket.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MobilePhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("StatusUser")
                        .HasColumnType("tinyint");

                    b.Property<byte>("TypeUser")
                        .HasColumnType("tinyint");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = new Guid("96448664-d6c8-42e4-9f93-65dccdb6def7"),
                            Address = "Saudi Arabia, Qassim, Buraydah",
                            DateOfBirth = new DateTime(2000, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "leen.aouto@gmail.com",
                            FirstName = "Leen",
                            LastName = "Aouto",
                            MobilePhone = "0545529216",
                            Password = "euFyR/GNXbvnR5m67HGT5A==;6V1GBtK3fSqamrMoLbEt/pGeekF5czrDQKIMcDwJ9Cg=",
                            StatusUser = (byte)1,
                            TypeUser = (byte)1,
                            Username = "manager"
                        });
                });

            modelBuilder.Entity("TTicket.Models.Comment", b =>
                {
                    b.HasOne("TTicket.Models.Ticket", "Ticket")
                        .WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TTicket.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ticket");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TTicket.Models.Ticket", b =>
                {
                    b.HasOne("TTicket.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TTicket.Models.User", "Support")
                        .WithMany()
                        .HasForeignKey("SupportId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("TTicket.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Support");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
