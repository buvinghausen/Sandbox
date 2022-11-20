using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using FluentValidation;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
public sealed class LoginRequest
{
    public LoginRequest()
    {
    }

    public LoginRequest(string email, string password, bool rememberMe = false)
    {
        Email = email;
        Password = password;
        RememberMe = rememberMe;
    }

    /// <summary>
    /// Email address which is the user's username
    /// </summary>
    [Required]
    [EmailAddress]
    [DataMember(Order = 1)]
    public string Email { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [DataMember(Order = 2)]
    public string Password { get; set; }

    /// <summary>
    /// Set a persistent cookie that will last after the browser is closed
    /// </summary>
    [Display(Name = "Remember me?")]
    [DataMember(Order = 3)]
    public bool RememberMe { get; set; }
}

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty()
            .EmailAddress();

        // TODO: Add complexity requirements
        RuleFor(l => l.Password)
            .NotEmpty()
            .MinimumLength(4);
    }
}