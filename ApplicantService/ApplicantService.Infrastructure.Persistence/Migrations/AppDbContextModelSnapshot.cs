﻿// <auto-generated />
using System;
using ApplicantService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApplicantService.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ApplicantService.Core.Domain.Applicant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateOnly?>("Birthday")
                        .HasColumnType("date");

                    b.Property<string>("Citizenship")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Applicants");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ApplicantId")
                        .HasColumnType("uuid");

                    b.Property<int>("DocumentType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ApplicantId");

                    b.ToTable("Documents");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.DocumentFileInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DocumentId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.ToTable("FilesInfo");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.EducationDocumentTypeCache", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Deprecated")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EducationDocumentTypesCache");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.EducationDocument", b =>
                {
                    b.HasBaseType("ApplicantService.Core.Domain.Document");

                    b.Property<Guid>("ApplicantIdCache")
                        .HasColumnType("uuid");

                    b.Property<Guid>("EducationDocumentTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("EducationDocumentTypeId");

                    b.ToTable("EducationDocuments");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.Passport", b =>
                {
                    b.HasBaseType("ApplicantService.Core.Domain.Document");

                    b.Property<Guid>("ApplicantIdCache")
                        .HasColumnType("uuid");

                    b.Property<string>("BirthPlace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("IssueYear")
                        .HasColumnType("date");

                    b.Property<string>("IssuedByWhom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SeriesNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Passports");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.Document", b =>
                {
                    b.HasOne("ApplicantService.Core.Domain.Applicant", "Applicant")
                        .WithMany("Documents")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Applicant");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.DocumentFileInfo", b =>
                {
                    b.HasOne("ApplicantService.Core.Domain.Document", "Document")
                        .WithMany("FilesInfo")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.EducationDocument", b =>
                {
                    b.HasOne("ApplicantService.Core.Domain.EducationDocumentTypeCache", "EducationDocumentType")
                        .WithMany()
                        .HasForeignKey("EducationDocumentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ApplicantService.Core.Domain.Document", null)
                        .WithOne()
                        .HasForeignKey("ApplicantService.Core.Domain.EducationDocument", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EducationDocumentType");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.Passport", b =>
                {
                    b.HasOne("ApplicantService.Core.Domain.Document", null)
                        .WithOne()
                        .HasForeignKey("ApplicantService.Core.Domain.Passport", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.Applicant", b =>
                {
                    b.Navigation("Documents");
                });

            modelBuilder.Entity("ApplicantService.Core.Domain.Document", b =>
                {
                    b.Navigation("FilesInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
