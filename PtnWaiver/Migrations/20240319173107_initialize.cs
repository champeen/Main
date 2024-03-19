using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PtnWaiver.Migrations
{
    /// <inheritdoc />
    public partial class initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Approver = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllowedAttachmentExtensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExtensionName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedAttachmentExtensions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Priority = table.Column<string>(type: "text", nullable: true),
                    SentToDisplayName = table.Column<string>(type: "text", nullable: true),
                    SentToUsername = table.Column<string>(type: "text", nullable: true),
                    SentToEmail = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    PtnId = table.Column<int>(type: "integer", nullable: true),
                    WaiverId = table.Column<int>(type: "integer", nullable: true),
                    TaskId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Group = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverUsername = table.Column<string>(type: "text", nullable: false),
                    PrimaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverUsername = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupApprovers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PorProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PorProject", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductProcess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProcess", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PTN",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocId = table.Column<string>(type: "text", nullable: true),
                    PtnPin = table.Column<string>(type: "text", nullable: false),
                    SubjectType = table.Column<List<string>>(type: "text[]", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    GroupApprover = table.Column<string>(type: "text", nullable: false),
                    PtrNumber = table.Column<string>(type: "text", nullable: true),
                    PdfLocation = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverUsername = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverUsername = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    RejectedBeforeSubmission = table.Column<bool>(type: "boolean", nullable: true),
                    RejectedByApprover = table.Column<bool>(type: "boolean", nullable: true),
                    RejectedReason = table.Column<string>(type: "text", nullable: true),
                    SubmittedForApprovalUser = table.Column<string>(type: "text", nullable: true),
                    SubmittedForApprovalUserFullName = table.Column<string>(type: "text", nullable: true),
                    SubmittedForApprovalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovedByUser = table.Column<string>(type: "text", nullable: true),
                    ApprovedByUserFullName = table.Column<string>(type: "text", nullable: true),
                    ApprovedByDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedBylUser = table.Column<string>(type: "text", nullable: true),
                    CompletedBylUserFullName = table.Column<string>(type: "text", nullable: true),
                    CompletedByDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTN", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PtnPin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtnPin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PtnStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Default = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtnStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaiverStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Default = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiverStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Waiver",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WaiverNumber = table.Column<string>(type: "text", nullable: true),
                    RevisionNumber = table.Column<int>(type: "integer", nullable: false),
                    PorProject = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProductProcess = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DateClosed = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CorrectiveActionDueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PrimaryApproverUsername = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    PrimaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverUsername = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverFullName = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverEmail = table.Column<string>(type: "text", nullable: true),
                    SecondaryApproverTitle = table.Column<string>(type: "text", nullable: true),
                    RejectedBeforeSubmission = table.Column<bool>(type: "boolean", nullable: true),
                    RejectedByApprover = table.Column<bool>(type: "boolean", nullable: true),
                    RejectedReason = table.Column<string>(type: "text", nullable: true),
                    SubmittedForApprovalUser = table.Column<string>(type: "text", nullable: true),
                    SubmittedForApprovalUserFullName = table.Column<string>(type: "text", nullable: true),
                    SubmittedForApprovalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovedByUser = table.Column<string>(type: "text", nullable: true),
                    ApprovedByUserFullName = table.Column<string>(type: "text", nullable: true),
                    ApprovedByDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedBylUser = table.Column<string>(type: "text", nullable: true),
                    CompletedBylUserFullName = table.Column<string>(type: "text", nullable: true),
                    CompletedByDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PTNId = table.Column<int>(type: "integer", nullable: false),
                    PtnDocId = table.Column<string>(type: "text", nullable: true),
                    CreatedUser = table.Column<string>(type: "text", nullable: false),
                    CreatedUserFullName = table.Column<string>(type: "text", nullable: true),
                    CreatedUserEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedUser = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserFullName = table.Column<string>(type: "text", nullable: true),
                    ModifiedUserEmail = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedUser = table.Column<string>(type: "text", nullable: true),
                    DeletedUserFullName = table.Column<string>(type: "text", nullable: true),
                    DeletedUserEmail = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waiver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waiver_PTN_PTNId",
                        column: x => x.PTNId,
                        principalTable: "PTN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Waiver_PTNId",
                table: "Waiver",
                column: "PTNId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "AllowedAttachmentExtensions");

            migrationBuilder.DropTable(
                name: "EmailHistory");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "GroupApprovers");

            migrationBuilder.DropTable(
                name: "PorProject");

            migrationBuilder.DropTable(
                name: "ProductProcess");

            migrationBuilder.DropTable(
                name: "PtnPin");

            migrationBuilder.DropTable(
                name: "PtnStatus");

            migrationBuilder.DropTable(
                name: "SubjectType");

            migrationBuilder.DropTable(
                name: "Waiver");

            migrationBuilder.DropTable(
                name: "WaiverStatus");

            migrationBuilder.DropTable(
                name: "PTN");
        }
    }
}
