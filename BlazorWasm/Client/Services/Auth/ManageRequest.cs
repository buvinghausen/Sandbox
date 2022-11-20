using System.Runtime.Serialization;

using FluentValidation;

namespace BlazorWasm.Client.Services.Auth;

// Keep this as a separate class to run the different validator as fields are not required
[DataContract]
public sealed class ManageRequest : RegisterRequest
{
}

internal sealed class ManageRequestValidator : AbstractValidator<ManageRequest>
{
    public ManageRequestValidator()
    {
        RuleFor(m => m.Email)
            .EmailAddress();
    }
}