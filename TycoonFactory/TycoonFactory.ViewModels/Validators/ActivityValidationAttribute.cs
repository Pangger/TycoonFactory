using System.ComponentModel.DataAnnotations;
using TycoonFactory.DAL.Entities.Enums;
using TycoonFactory.ViewModels.UiModels;

namespace TycoonFactory.ViewModels.Validators;

public class ActivityValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var validationResult = ValidationResult.Success;
        if (value is ActivityUiModel activity)
        {
            if (activity.StartDateTime > activity.EndDateTime)
            {
                validationResult =
                    new ValidationResult("End datetime should be greater than the start date",
                        new[] {validationContext.DisplayName});
            }
            else if (activity.EndDateTime < DateTime.Now)
            {
                validationResult =
                    new ValidationResult("End datetime should be greater than the current time",
                        new[] {validationContext.DisplayName});
            }
            else if (!activity.Androids.Any())
            {
                validationResult =
                    new ValidationResult("Choose at least one android", new[] {validationContext.DisplayName});
            }
            else if (activity.Type == ActivityType.BuildComponent &&
                     activity.Androids.Count > 1)
            {
                validationResult = new ValidationResult("To build a component you need only 1 android",
                    new[] {validationContext.DisplayName});
            }
        }
        else
            validationResult =
                new ValidationResult("ActivityUiModel cast error", new[] {validationContext.DisplayName});

        return validationResult;
    }
}