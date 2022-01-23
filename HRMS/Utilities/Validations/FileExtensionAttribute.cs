using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace HRMS.Utilities.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class FileExtensionAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string allowedFormats;

    public FileExtensionAttribute(string allowedFormats)
    {
        this.allowedFormats = allowedFormats;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "accept", allowedFormats);

        //var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());

        MergeAttribute(context.Attributes, "data-val-fileextesion", $"Nuk lejohet: {allowedFormats}");
        MergeAttribute(context.Attributes, "data-val-fileextesion-formats", allowedFormats);
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
        var file = value as IFormFile;
        string[] formats = allowedFormats.Split(",");
        if (file != null)
        {
            string extension = Path.GetExtension(file.FileName);
            if (!formats.Contains(extension))
            {
                return new ValidationResult($"Nuk eshte ne formatin e duhur: {allowedFormats}");
            }
        }
        return ValidationResult.Success;
    }
}
