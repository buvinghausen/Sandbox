using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
public sealed class VerifyEmailRequest
{
    [Required]
    [DataMember(Order = 1)]
    public string Code { get; set; }
}
