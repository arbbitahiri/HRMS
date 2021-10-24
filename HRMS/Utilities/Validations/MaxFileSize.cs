using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Utilities.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MaxFileSize : ValidationAttribute, IClientModelValidator
    {
        private readonly int maxKb;

        public MaxFileSize(int maxKb)
        {
            this.maxKb = maxKb;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-maxfilesize", $"Nuk lejohet kjo madhesi: {KbToMb(maxKb)}");
            MergeAttribute(context.Attributes, "data-val-maxfilesize-size", maxKb.ToString());
        }

        private static string KbToMb(int kb) => $"{kb / 1024:###,###.##} Mb";

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
            if (file != null)
            {
                if (file.Length / 1024 > maxKb)
                {
                    return new ValidationResult($"Nuk lejohet kjo madhesi: {maxKb}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
