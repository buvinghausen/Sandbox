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
    [DataMember(Order = 1)]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    [DataMember(Order = 2)]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    /// <summary>
    /// Set a persistent cookie that will last after the browser is closed
    /// </summary>
    [DataMember(Order = 3)]
    [Display(Name = "Remember me?")]
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