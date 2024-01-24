using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_of_Change.Migrations
{
    /// <inheritdoc />
    public partial class changeAdditionalNotificationField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Implementation_Notification_Employees",
                table: "ChangeRequest",
                newName: "Additional_Notification");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Additional_Notification",
                table: "ChangeRequest",
                newName: "Implementation_Notification_Employees");
        }
    }
}
