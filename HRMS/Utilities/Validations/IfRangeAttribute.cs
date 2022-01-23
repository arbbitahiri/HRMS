using HRMS.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Utilities.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public class IfRangeAttribute: ValidationAttribute, IClientModelValidator
{
    private readonly string required;
    private readonly int minimum;
    private readonly int maximum;

    public IfRangeAttribute(string required, int minimum, int maximum)
    {
        this.required = required;
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-requiredif", Resource.RequiredField);
        //MergeAttribute(context.Attributes, "data-val-requiredif-value", minimum);
        //MergeAttribute(context.Attributes, "data-val-requiredif-valueon", maximum);
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
        object instance = validationContext.ObjectInstance;
        var isRequired = instance.GetType().GetProperty(required).GetValue(instance, null);

        if ((bool)isRequired)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(Resource.RequiredField);
            }
            else
            {
                if (minimum >= (int)instance || (int)instance >= maximum)
                {
                    return new ValidationResult(Resource.SalaryRange);
                }
            }
        }
        return ValidationResult.Success;
    }
}
