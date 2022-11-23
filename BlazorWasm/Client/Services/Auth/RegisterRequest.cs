using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using FluentValidation;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
public class RegisterRequest
{
    [EmailAddress]
    [Required]
    [DataMember(Order = 1)]
    public string Email { get; set; }

    [Required]
    [DataMember(Order = 2)]
    public string FirstName { get; set; }

    [Required]
    [DataMember(Order = 3)]
    public string LastName { get; set; }

    [DataType(DataType.Password)]
    [Required]
    [DataMember(Order = 4)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Required]
    [DataMember(Order = 5)]
    public string ConfirmPassword { get; set; }

    [Phone]
    [Required]
    [DataMember(Order = 6)]
    public string PhoneNumber { get; set; }

    [Required]
    [DataMember(Order = 7)]
    public string TimeZone { get; set; }
}

internal sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.FirstName)
            .NotEmpty();

        RuleFor(r => r.LastName)
            .NotEmpty();

        // TODO: add regex match
        RuleFor(r => r.PhoneNumber)
            .NotEmpty();

        // TODO: Add complexity requirement match
        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(4);

        RuleFor(r => r.ConfirmPassword)
            .NotEmpty()
            .Equal(r => r.Password);

        // TODO: Add timezone service to gRPC and create dropdown on register form
        RuleFor(r => r.TimeZone)
            .NotEmpty();
    }
}
