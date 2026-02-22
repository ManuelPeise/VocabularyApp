using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitializeDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ResourceKey = table.Column<string>(type: "TEXT", nullable: false),
                    TranslationType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogMessageTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    ExeptionMessage = table.Column<string>(type: "TEXT", nullable: true),
                    Stacktrace = table.Column<string>(type: "TEXT", nullable: true),
                    Module = table.Column<string>(type: "TEXT", nullable: false),
                    LogLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogMessageTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartOfSpeachTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ResourceKey = table.Column<string>(type: "TEXT", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOfSpeachTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCredentialsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentialsTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettingsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Culture = table.Column<int>(type: "INTEGER", nullable: false),
                    UseLocalDataStore = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAutoDataSyncEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettingsTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyCategoryTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SourceLanguage = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyCategoryTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupGuid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Word = table.Column<string>(type: "TEXT", nullable: false),
                    Article = table.Column<string>(type: "TEXT", nullable: true),
                    ExampleSentence = table.Column<string>(type: "TEXT", nullable: true),
                    Ipa = table.Column<string>(type: "TEXT", nullable: true),
                    IsReviewRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartOfSpeechId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyTable_LanguageTable_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LanguageTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyTable_PartOfSpeachTable_PartOfSpeechId",
                        column: x => x.PartOfSpeechId,
                        principalTable: "PartOfSpeachTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileImage = table.Column<byte[]>(type: "BLOB", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserRole = table.Column<int>(type: "INTEGER", nullable: false),
                    UserCredentialsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserSettingsId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTable_UserCredentialsTable_UserCredentialsId",
                        column: x => x.UserCredentialsId,
                        principalTable: "UserCredentialsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTable_UserSettingsTable_UserSettingsId",
                        column: x => x.UserSettingsId,
                        principalTable: "UserSettingsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularySessionTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularySessionTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularySessionTable_VocabularyCategoryTable_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "VocabularyCategoryTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyToCategoriesTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VocabularyId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyToCategoriesTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyToCategoriesTable_VocabularyCategoryTable_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "VocabularyCategoryTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyToCategoriesTable_VocabularyTable_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "VocabularyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyProgressTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimesSeen = table.Column<int>(type: "INTEGER", nullable: false),
                    Success = table.Column<int>(type: "INTEGER", nullable: false),
                    Failed = table.Column<int>(type: "INTEGER", nullable: false),
                    VocabularyId = table.Column<int>(type: "INTEGER", nullable: false),
                    VocabularySessionEntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyProgressTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyProgressTable_VocabularySessionTable_VocabularySessionEntityId",
                        column: x => x.VocabularySessionEntityId,
                        principalTable: "VocabularySessionTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VocabularyProgressTable_VocabularyTable_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "VocabularyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularySessionResultTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Success = table.Column<int>(type: "INTEGER", nullable: false),
                    Failed = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDirty = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdExternal = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularySessionResultTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularySessionResultTable_VocabularySessionTable_SessionId",
                        column: x => x.SessionId,
                        principalTable: "VocabularySessionTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularySessionVocabulary",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    VocabularyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularySessionVocabulary", x => new { x.SessionId, x.VocabularyId });
                    table.ForeignKey(
                        name: "FK_VocabularySessionVocabulary_VocabularySessionTable_SessionId",
                        column: x => x.SessionId,
                        principalTable: "VocabularySessionTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularySessionVocabulary_VocabularyTable_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "VocabularyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserCredentialsId",
                table: "UserTable",
                column: "UserCredentialsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserSettingsId",
                table: "UserTable",
                column: "UserSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyProgressTable_VocabularyId",
                table: "VocabularyProgressTable",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyProgressTable_VocabularySessionEntityId",
                table: "VocabularyProgressTable",
                column: "VocabularySessionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularySessionResultTable_SessionId",
                table: "VocabularySessionResultTable",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularySessionTable_CategoryId",
                table: "VocabularySessionTable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularySessionVocabulary_VocabularyId",
                table: "VocabularySessionVocabulary",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTable_LanguageId",
                table: "VocabularyTable",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTable_PartOfSpeechId",
                table: "VocabularyTable",
                column: "PartOfSpeechId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyToCategoriesTable_CategoryId",
                table: "VocabularyToCategoriesTable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyToCategoriesTable_VocabularyId_CategoryId",
                table: "VocabularyToCategoriesTable",
                columns: new[] { "VocabularyId", "CategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogMessageTable");

            migrationBuilder.DropTable(
                name: "UserTable");

            migrationBuilder.DropTable(
                name: "VocabularyProgressTable");

            migrationBuilder.DropTable(
                name: "VocabularySessionResultTable");

            migrationBuilder.DropTable(
                name: "VocabularySessionVocabulary");

            migrationBuilder.DropTable(
                name: "VocabularyToCategoriesTable");

            migrationBuilder.DropTable(
                name: "UserCredentialsTable");

            migrationBuilder.DropTable(
                name: "UserSettingsTable");

            migrationBuilder.DropTable(
                name: "VocabularySessionTable");

            migrationBuilder.DropTable(
                name: "VocabularyTable");

            migrationBuilder.DropTable(
                name: "VocabularyCategoryTable");

            migrationBuilder.DropTable(
                name: "LanguageTable");

            migrationBuilder.DropTable(
                name: "PartOfSpeachTable");
        }
    }
}
