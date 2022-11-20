using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
public sealed class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    [DataMember(Order = 1)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [DataMember(Order = 2)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [DataMember(Order = 3)]
    public string ConfirmPassword { get; set; }

    [Required]
    [DataMember(Order = 4)]
    public string Code { get; set; }
}
