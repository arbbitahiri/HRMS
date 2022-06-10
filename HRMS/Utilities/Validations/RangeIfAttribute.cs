using HRMS.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace HRMS.Utilities.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public class RangeIfAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string required;
    private readonly int minimum;
    private readonly int maximum;

    public RangeIfAttribute(string required, int minimum, int maximum)
    {
        this.required = required;
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        var resourceManager = new ResourceManager(typeof(Resource));
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-rangeif", resourceManager.GetString(ErrorMessageResourceName));
        MergeAttribute(context.Attributes, "data-val-rangeif-min", minimum.ToString());
        MergeAttribute(context.Attributes, "data-val-rangeif-max", maximum.ToString());
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
        bool propertyRequired = (bool)instance.GetType().GetProperty(required).GetValue(instance, null);

        if (propertyRequired)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(Resource.RequiredField);
            }
            else
            {
                //if (minimum >= (int)instance || (int)instance >= maximum)
                if (minimum >= Convert.ToSingle(value) || Convert.ToSingle(value) >= maximum)
                {
                    var resourceManager = new ResourceManager(typeof(Resource));
                    return new ValidationResult(resourceManager.GetString(ErrorMessageResourceName));
                }
            }
        }
        return ValidationResult.Success;
    }
}
