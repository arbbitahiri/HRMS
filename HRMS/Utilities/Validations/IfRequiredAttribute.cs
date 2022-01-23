using HRMS.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Utilities.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class IfRequiredAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string dependsOn;
    private readonly string valueOn;

    public IfRequiredAttribute(string dependsOn, string valueOn)
    {
        this.dependsOn = dependsOn;
        this.valueOn = valueOn;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-requiredif", Resource.RequiredField);
        MergeAttribute(context.Attributes, "data-val-requiredif-value", dependsOn);
        MergeAttribute(context.Attributes, "data-val-requiredif-valueon", valueOn);
    }

    private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (attributes.ContainsKey(key))
        {
            return false;
        }
        attributes.Add(key, value);
        return true;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        object instance = validationContext.ObjectInstance;
        var type = instance.GetType();

        bool.TryParse(type.GetProperty(dependsOn).GetValue(instance)?.ToString(), out bool propertyValue);
        var propertyDependingValue = type.GetProperty(dependsOn).GetValue(instance, null);

        if ((propertyDependingValue ?? "").ToString() == (valueOn ?? ""))
        {
            if (propertyValue && string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(Resource.RequiredField);
            }
        }
        return ValidationResult.Success;
    }
}
