using System.Net;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

[Collection("E2E")]
public class Flow07_HRMDailyOperationsTests : NiagaOneTestBase
{
    [Fact]
    public async Task HR_Should_Manage_Departments_Employees_And_Attendance()
    {
        // ── Login as HR user ─────────────────────────────────────────────
        var token = await LoginAsync("hr@niagaone.com", "Password123!");

        // ── Create 3 departments ─────────────────────────────────────────
        var departments = new[]
        {
            new { name = "Engineering", code = "ENG" },
            new { name = "Sales", code = "SALES" },
            new { name = "Operations", code = "OPS" }
        };

        var departmentIds = new List<string>();
        foreach (var dept in departments)
        {
            var deptResponse = await AuthPost(token, "/api/departments", dept);
            deptResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var deptResult = await ReadAs<JsonElement>(deptResponse);
            departmentIds.Add(deptResult.GetProperty("id").GetString()!);
        }

        var engineeringId = departmentIds[0];

        // ── Resolve a userId for the employee ────────────────────────────
        string userId;
        try
        {
            userId = TestDataStore.GetGuid("cashierUserId").ToString();
        }
        catch (KeyNotFoundException)
        {
            // Fall back: login as admin and find a user from the list
            var adminToken = await LoginAsync("admin@niagaone.com", "Password123!");
            var usersResponse = await AuthGet(adminToken, "/api/users?pageSize=10");
            usersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var users = await ReadAs<JsonElement>(usersResponse);
            userId = users.GetProperty("items")[0].GetProperty("id").GetString()!;
        }

        // ── Create employee ──────────────────────────────────────────────
        var employeeResponse = await AuthPost(token, "/api/employees", new
        {
            userId,
            departmentId = engineeringId,
            position = "Software Engineer",
            hireDate = "2025-01-15",
            baseSalary = 15000000
        });
        employeeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var employee = await ReadAs<JsonElement>(employeeResponse);
        var employeeId = employee.GetProperty("id").GetString()!;
        employeeId.Should().NotBeNullOrEmpty();

        // ── Clock in ─────────────────────────────────────────────────────
        var clockInResponse = await AuthPost(token, "/api/attendance/clock-in", new
        {
            employeeId
        });
        clockInResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var attendance = await ReadAs<JsonElement>(clockInResponse);
        var attendanceId = attendance.GetProperty("id").GetString()!;

        // ── Brief wait so clock-out shows nonzero duration ───────────────
        await Task.Delay(1000);

        // ── Clock out ────────────────────────────────────────────────────
        var clockOutResponse = await AuthPost(token, $"/api/attendance/{attendanceId}/clock-out");
        clockOutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var clockOutResult = await ReadAs<JsonElement>(clockOutResponse);
        clockOutResult.GetProperty("clockOut").GetString().Should().NotBeNullOrEmpty();

        // ── Create leave request ─────────────────────────────────────────
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var leaveResponse = await AuthPost(token, "/api/leave-requests", new
        {
            employeeId,
            leaveType = "Annual",
            startDate = tomorrow,
            endDate = tomorrow.AddDays(1),
            reason = "Family event"
        });
        leaveResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var leave = await ReadAs<JsonElement>(leaveResponse);
        var leaveRequestId = leave.GetProperty("id").GetString()!;

        // ── Approve leave request ────────────────────────────────────────
        var approveResponse = await AuthPost(token, $"/api/leave-requests/{leaveRequestId}/approve");
        approveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // ── Verify approved status ───────────────────────────────────────
        var leaveGetResponse = await AuthGet(token, $"/api/leave-requests/{leaveRequestId}");
        leaveGetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var leaveDetail = await ReadAs<JsonElement>(leaveGetResponse);
        leaveDetail.GetProperty("status").GetString().Should().Be("Approved");

        // ── Store IDs for subsequent flows ───────────────────────────────
        TestDataStore.Set("employeeId", employeeId);
        TestDataStore.Set("engineeringDeptId", departmentIds[0]);
        TestDataStore.Set("salesDeptId", departmentIds[1]);
        TestDataStore.Set("opsDeptId", departmentIds[2]);
    }
}
