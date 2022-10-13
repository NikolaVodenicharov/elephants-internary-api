﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(InternaryContext))]
    [Migration("20221006130448_AddShadowProperties")]
    partial class AddShadowProperties
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CampaignMentors", b =>
                {
                    b.Property<Guid>("CampaignId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MentorId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CampaignId", "MentorId");

                    b.HasIndex("MentorId");

                    b.ToTable("CampaignMentors");
                });

            modelBuilder.Entity("Core.Features.Campaigns.Entities.Campaign", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.Intern", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<string>("PersonalEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Interns");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.InternCampaign", b =>
                {
                    b.Property<Guid>("InternId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CampaignId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SpecialityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("InternId", "CampaignId");

                    b.HasIndex("CampaignId");

                    b.HasIndex("SpecialityId");

                    b.ToTable("InternCampaigns");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.State", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CampaignId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("InternId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Justification")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.HasIndex("InternId", "CampaignId");

                    b.ToTable("State");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.Status", b =>
                {
                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StatusId");

                    b.ToTable("Status");

                    b.HasData(
                        new
                        {
                            StatusId = 0,
                            Name = "Candidate"
                        },
                        new
                        {
                            StatusId = 1,
                            Name = "Intern"
                        },
                        new
                        {
                            StatusId = 2,
                            Name = "Rejected"
                        },
                        new
                        {
                            StatusId = 3,
                            Name = "RejectedToStart"
                        },
                        new
                        {
                            StatusId = 4,
                            Name = "Hired"
                        });
                });

            modelBuilder.Entity("Core.Features.LearningTopics.Entities.LearningTopic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("LearningTopics");
                });

            modelBuilder.Entity("Core.Features.Mentors.Entities.Mentor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Mentors");
                });

            modelBuilder.Entity("Core.Features.Specialties.Entities.Speciality", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("nvarchar(125)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Specialties");
                });

            modelBuilder.Entity("LearningTopicSpecialities", b =>
                {
                    b.Property<Guid>("LearningTopicId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SpecialityId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LearningTopicId", "SpecialityId");

                    b.HasIndex("SpecialityId");

                    b.ToTable("LearningTopicSpecialities");
                });

            modelBuilder.Entity("MentorSpecialties", b =>
                {
                    b.Property<Guid>("MentorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SpecialityId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("MentorId", "SpecialityId");

                    b.HasIndex("SpecialityId");

                    b.ToTable("MentorSpecialties");
                });

            modelBuilder.Entity("CampaignMentors", b =>
                {
                    b.HasOne("Core.Features.Campaigns.Entities.Campaign", null)
                        .WithMany()
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("CampaignId");

                    b.HasOne("Core.Features.Mentors.Entities.Mentor", null)
                        .WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("MentorId");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.InternCampaign", b =>
                {
                    b.HasOne("Core.Features.Campaigns.Entities.Campaign", "Campaign")
                        .WithMany("InternCampaigns")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Features.Interns.Entities.Intern", "Intern")
                        .WithMany("InternCampaigns")
                        .HasForeignKey("InternId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Features.Specialties.Entities.Speciality", "Speciality")
                        .WithMany("InternCampaigns")
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");

                    b.Navigation("Intern");

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.State", b =>
                {
                    b.HasOne("Core.Features.Interns.Entities.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Features.Interns.Entities.InternCampaign", "InternCampaign")
                        .WithMany("States")
                        .HasForeignKey("InternId", "CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InternCampaign");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("LearningTopicSpecialities", b =>
                {
                    b.HasOne("Core.Features.LearningTopics.Entities.LearningTopic", null)
                        .WithMany()
                        .HasForeignKey("LearningTopicId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("FK_LearningTopicId");

                    b.HasOne("Core.Features.Specialties.Entities.Speciality", null)
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("FK_LearningTopic_SpecialityId");
                });

            modelBuilder.Entity("MentorSpecialties", b =>
                {
                    b.HasOne("Core.Features.Mentors.Entities.Mentor", null)
                        .WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("FK_MentorId");

                    b.HasOne("Core.Features.Specialties.Entities.Speciality", null)
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("FK_SpecialityId");
                });

            modelBuilder.Entity("Core.Features.Campaigns.Entities.Campaign", b =>
                {
                    b.Navigation("InternCampaigns");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.Intern", b =>
                {
                    b.Navigation("InternCampaigns");
                });

            modelBuilder.Entity("Core.Features.Interns.Entities.InternCampaign", b =>
                {
                    b.Navigation("States");
                });

            modelBuilder.Entity("Core.Features.Specialties.Entities.Speciality", b =>
                {
                    b.Navigation("InternCampaigns");
                });
#pragma warning restore 612, 618
        }
    }
}
