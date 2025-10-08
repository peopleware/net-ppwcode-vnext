// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PPWCode.AspNetCore.API.I;

public class ComparePropertyAttribute : ValidationAttribute
{
    private readonly ComparisonTypeEnum _comparisonType;
    private readonly string _otherPropertyName;

    public ComparePropertyAttribute(string otherPropertyName, ComparisonTypeEnum comparisonType)
    {
        _otherPropertyName = otherPropertyName;
        _comparisonType = comparisonType;
        ErrorMessage = "{0} must be {1} {2}.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        PropertyInfo? otherProperty = validationContext.ObjectType.GetProperty(
            _otherPropertyName,
            BindingFlags.Public | BindingFlags.Instance);

        if (otherProperty == null)
        {
            return new ValidationResult($"Unknown property: {_otherPropertyName}");
        }

        object? otherValue = otherProperty.GetValue(validationContext.ObjectInstance);

        // If either is null → no error (optional)
        if ((value == null) || (otherValue == null))
        {
            return ValidationResult.Success;
        }

        if (value is not IComparable leftComparable)
        {
            return new ValidationResult($"{validationContext.DisplayName} does not implement IComparable");
        }

        if (otherValue is not IComparable)
        {
            return new ValidationResult($"{_otherPropertyName} does not implement IComparable");
        }

        int result;
        try
        {
            result = leftComparable.CompareTo(otherValue);
        }
        catch (ArgumentException)
        {
            return new ValidationResult($"Cannot compare {validationContext.DisplayName} to {_otherPropertyName}");
        }

        bool isValid =
            _comparisonType switch
            {
                ComparisonTypeEnum.Equal => result == 0,
                ComparisonTypeEnum.NotEqual => result != 0,
                ComparisonTypeEnum.GreaterThan => result > 0,
                ComparisonTypeEnum.GreaterThanOrEqual => result >= 0,
                ComparisonTypeEnum.LessThan => result < 0,
                ComparisonTypeEnum.LessThanOrEqual => result <= 0,
                _ => throw new NotSupportedException()
            };

        if (!isValid)
        {
            return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName, _comparisonType.ToString().ToLower(), _otherPropertyName));
        }

        return ValidationResult.Success;
    }
}
