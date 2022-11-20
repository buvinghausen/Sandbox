using FluentValidation;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWasm.Client.Components;

public sealed class FluentValidator : ComponentBase
{
    private static readonly char[] Separators = { '.', '[' };
    private IValidator _validator;

    [Inject] private IServiceProvider ServiceProvider { get; set; }
    [CascadingParameter] private EditContext EditContext { get; set; }

    protected override void OnInitialized()
    {
        if (EditContext == null)
        {
            throw new InvalidOperationException($"{nameof(FluentValidator)} requires a cascading " +
                                                $"parameter of type {nameof(EditContext)}.");
        }

        var validatorType = typeof(IValidator<>).MakeGenericType(EditContext.Model.GetType());
        _validator = ServiceProvider.GetService(validatorType) as IValidator;

        if (_validator == null)
        {
            throw new InvalidOperationException($"{nameof(FluentValidator)} model validator not found");
        }

        var messages = new ValidationMessageStore(EditContext);
        EditContext.OnFieldChanged += (sender, _)
            => ValidateModel((EditContext)sender, messages);

        EditContext.OnValidationRequested += (sender, _)
            => ValidateModel((EditContext)sender, messages, true);
    }

    private void ValidateModel(EditContext editContext, ValidationMessageStore messages, bool onSubmit = false)
    {
        var context = new ValidationContext<object>(editContext.Model);
        var validationResult = _validator.Validate(context);
        messages.Clear();
        foreach (var error in validationResult.Errors)
        {
            var fieldIdentifier = ToFieldIdentifier(editContext, error.PropertyName);
            if (!editContext.IsModified(fieldIdentifier) && !onSubmit) continue;
            messages.Add(fieldIdentifier, error.ErrorMessage);
        }
        editContext.NotifyValidationStateChanged();
    }

    private static FieldIdentifier ToFieldIdentifier(EditContext editContext, string propertyPath)
    {
        var obj = editContext.Model;
        while (true)
        {
            var nextTokenEnd = propertyPath.IndexOfAny(Separators);
            if (nextTokenEnd < 0)
            {
                return new FieldIdentifier(obj, propertyPath);
            }

            var nextToken = propertyPath[..nextTokenEnd];
            propertyPath = propertyPath[(nextTokenEnd + 1)..];

            object newObj;
            if (nextToken.EndsWith("]"))
            {
                nextToken = nextToken[..^1];
                var prop = obj.GetType().GetProperty("Item")!;
                var indexerType = prop.GetIndexParameters()[0].ParameterType;
                var indexerValue = Convert.ChangeType(nextToken, indexerType);
                newObj = prop.GetValue(obj, new[] { indexerValue });
            }
            else
            {
                var prop = obj.GetType().GetProperty(nextToken);
                if (prop == null)
                {
                    throw new InvalidOperationException($"Could not find property named {nextToken} on object of type {obj.GetType().FullName}.");
                }
                newObj = prop.GetValue(obj);
            }

            if (newObj == null)
            {
                return new FieldIdentifier(obj, nextToken);
            }

            obj = newObj;
        }
    }
}
