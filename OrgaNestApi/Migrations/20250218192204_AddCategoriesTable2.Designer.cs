﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrgaNestApi.Infrastructure.Database;

#nullable disable

namespace OrgaNestApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250218192204_AddCategoriesTable2")]
    partial class AddCategoriesTable2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Sqlite:ForeignKeys", true);

            modelBuilder.Entity("OrgaNestApi.Common.Domain.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("FamilyId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("FamilyId");

                    b.HasIndex("UserId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.ExpenseShare", b =>
                {
                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("TEXT");

                    b.HasKey("ExpenseId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ExpenseShares");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.Family", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Families");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.ShoppingItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsPurchased")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ShoppingListId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingListId");

                    b.ToTable("ShoppingItems");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.ShoppingList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingLists");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.UserFamily", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FamilyId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "FamilyId");

                    b.HasIndex("FamilyId");

                    b.ToTable("UserFamilies");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.Expense", b =>
                {
                    b.HasOne("OrgaNestApi.Common.Domain.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("OrgaNestApi.Common.Domain.Family", "Family")
                        .WithMany("Expenses")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OrgaNestApi.Common.Domain.User", "User")
                        .WithMany("Expenses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Family");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.ExpenseShare", b =>
                {
                    b.HasOne("OrgaNestApi.Common.Domain.Expense", "Expense")
                        .WithMany("ExpenseShares")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OrgaNestApi.Common.Domain.User", "User")
                        .WithMany("ExpenseShares")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Expense");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.ShoppingItem", b =>
                {
                    b.HasOne("OrgaNestApi.Common.Domain.ShoppingList", "ShoppingList")
                        .WithMany("Items")
                        .HasForeignKey("ShoppingListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShoppingList");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.ShoppingList", b =>
                {
                    b.HasOne("OrgaNestApi.Common.Domain.User", "User")
                        .WithMany("ShoppingLists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.UserFamily", b =>
                {
                    b.HasOne("OrgaNestApi.Common.Domain.Family", "Family")
                        .WithMany("UserFamilies")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OrgaNestApi.Common.Domain.User", "User")
                        .WithMany("UserFamilies")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Family");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.Expense", b =>
                {
                    b.Navigation("ExpenseShares");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.Family", b =>
                {
                    b.Navigation("Expenses");

                    b.Navigation("UserFamilies");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.ShoppingList", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("OrgaNestApi.Common.Domain.User", b =>
                {
                    b.Navigation("ExpenseShares");

                    b.Navigation("Expenses");

                    b.Navigation("ShoppingLists");

                    b.Navigation("UserFamilies");
                });
#pragma warning restore 612, 618
        }
    }
}
