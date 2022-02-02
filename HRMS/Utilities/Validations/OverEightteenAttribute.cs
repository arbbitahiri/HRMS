using HRMS.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace HRMS.Utilities.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class OverEightteenAttribute : ValidationAttribute, IClientModelValidator
{
    public OverEightteenAttribute()
    {

    }

    public void AddValidation(ClientModelValidationContext context)
    {
        var resourceManager = new ResourceManager(typeof(Resource));
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-overeightteen", resourceManager.GetString(ErrorMessageResourceName));
    }

    private static bool MergeAttribute(IDictionary<string, string> attributes, string v, string requiredField)
    {
        if (attributes.ContainsKey(v))
        {
            return false;
        }

        attributes.Add(v, requiredField);
        return true;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        DateTime? selectedDate = DateTime.ParseExact((string)value, "dd/MM/yyyy", null);
        if (selectedDate == null)
        {
            return ValidationResult.Success;
        }

        if ((DateTime.Now - selectedDate).Value.TotalDays >= 18 * 364)
        {
            return ValidationResult.Success;
        }

        var resourceManager = new ResourceManager(typeof(Resource));
        return new ValidationResult(resourceManager.GetString(ErrorMessageResourceName));
    }
}
