namespace BlazorWasm.Client.Shared;

public static class Policies
{
    // This policy will enforce that the user has not logged in
    public const string Anonymous = "anon";

    // This policy will enforce that the user has logged in but email verification is not required
    public const string Authorized = "auth";

    // This policy will enforce that the user has verified their email
    public const string Verified = "verified";

    // This policy will enforce that the user is an employee
    public const string Employee = "employee";

    // This policy will enforce that the user is an admin
    public const string Admin = "admin";

    // This policy will enforce that the user is both an employee & admin
    public const string EmployeeAdmin = "employee_admin";
}
