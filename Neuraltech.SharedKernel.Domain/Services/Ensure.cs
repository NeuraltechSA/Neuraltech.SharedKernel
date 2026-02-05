using Neuraltech.SharedKernel.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Neuraltech.SharedKernel.Domain.Services
{
    public static class Ensure
    {
        public static void NotNullOrWhiteSpace(string? value, Func<Exception>? exceptionFactory = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                var exception = exceptionFactory?.Invoke() ?? ValueIsNullOrEmptyException.Create(value);
                throw exception;
            }
        }


        public static void EndsWith(string value, string suffix, Func<Exception>? exceptionFactory = null)
        {
            if (!value.EndsWith(suffix))
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidStringFormatException.CreateEndsWith(value, suffix);
                throw exception;
            }
        }

        public static void StringContains(string value, string substring, Func<Exception>? exceptionFactory = null)
        {
            if (!value.Contains(substring))
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidStringFormatException.CreateContains(value, substring);
                throw exception;
            }
        }

        public static void StringNotContains(string value, string substring, Func<Exception>? exceptionFactory = null)
        {
            if (value.Contains(substring))
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidStringFormatException.CreateNotContains(value, substring);
                throw exception;
            }
        }

        public static void InRange<T>(
            T value, T min, T max, Func<Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if(value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                var exception = exceptionFactory?.Invoke() ?? ValueOutOfRangeException.Create(value, min, max);
                throw exception;
            }
        }

        public static void GreaterOrEqualThan<T>(
            T value, T minimum, Func<Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(minimum) < 0)
            {
                var exception = exceptionFactory?.Invoke() ?? ValueOutOfRangeException.CreateGreaterOrEqualThan(value, minimum);
                throw exception;
            }
        }

        public static void LessOrEqualThan<T>(
            T value, T maximum, Func<Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(maximum) > 0)
            {
                var exception = exceptionFactory?.Invoke() ?? ValueOutOfRangeException.CreateLessOrEqualThan(value, maximum);
                throw exception;
            }
        }

        public static void GreaterThan<T>(
            T value, T minimum, Func<Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(minimum) <= 0)
            {
                var exception = exceptionFactory?.Invoke() ?? ValueOutOfRangeException.CreateGreaterThan(value, minimum);
                throw exception;
            }
        }

        public static void LessThan<T>(
            T value, T maximum, Func<Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(maximum) >= 0)
            {
                var exception = exceptionFactory?.Invoke() ?? ValueOutOfRangeException.CreateLessThan(value, maximum);
                throw exception;
            }
        }

        public static void HasLength<T>(IEnumerable<T> value, int length, Func<Exception>? exceptionFactory = null)
        {
            if(value.Count() != length)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidLengthException.Create(value, length);
                throw exception;
            }
        }

        public static void HasLengthBetween<T>(IEnumerable<T> value, int minLength, int maxLength, Func<Exception>? exceptionFactory = null)
        {
            if (value.Count() < minLength || value.Count() > maxLength)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidLengthException.Create(value, minLength, maxLength);
                throw exception;
            }
        }

        public static void HasMinLength<T>(IEnumerable<T> value, int minLength, Func<Exception>? exceptionFactory = null)
        {
            if(value.Count() < minLength)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidLengthException.CreateMinLength(value, minLength);
                throw exception;
            }
        }

        public static void HasMaxLength<T>(IEnumerable<T> value, int maxLength, Func<Exception>? exceptionFactory = null)
        {
            if(value.Count() > maxLength)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidLengthException.CreateMaxLength(value, maxLength);
                throw exception;
            }
        }
        public static void HasAny<T>(IEnumerable<T> value, Func<Exception>? exceptionFactory = null)
        {
            HasMinLength(value, 1, exceptionFactory);
        }

        public static void IsEmpty<T>(IEnumerable<T> value, Func<Exception>? exceptionFactory = null)
        {
            HasMaxLength(value, 0, exceptionFactory);
        }

        public static void IsTrue(bool value, Func<Exception>? exceptionFactory = null)
        {
            if (!value)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidBooleanException.TrueExpected();
                throw exception;
            }
        }

        public static void IsFalse(bool value, Func<Exception>? exceptionFactory = null)
        {
            if (value)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidBooleanException.FalseExpected();
                throw exception;
            }
        }

        public static void NotNull<T>(T? value, Func<Exception>? exceptionFactory = null)
        {
            if (value is null)
            {
                var exception = exceptionFactory?.Invoke() ?? ValueIsNullException.Create();
                throw exception;
            }
        }

        public static void IsNull<T>(T? value, Func<Exception>? exceptionFactory = null)
        {
            if (value is not null)
            {
                var exception = exceptionFactory?.Invoke() ?? ExpectedNullException.Create();
                throw exception;
            }
        }

        public static void Contains<T>(T item, IEnumerable<T> allowedValues, Func<Exception>? exceptionFactory = null)
        {
            if (!allowedValues.Contains(item))
            {
                var exception = exceptionFactory?.Invoke() ?? UnexpectedValueException.CreateFromAllowed(item, allowedValues);
                throw exception;
            }
        }

        public static void NotContains<T>(T item, IEnumerable<T> forbiddenValues, Func<Exception>? exceptionFactory = null)
        {
            if (forbiddenValues.Contains(item))
            {
                var exception = exceptionFactory?.Invoke() ?? UnexpectedValueException.CreateFromForbidden(item, forbiddenValues);
                throw exception;
            }
        }

        public static void StartsWith(string? value, string prefix, Func<Exception>? exceptionFactory = null)
        {
            if (value is null || !value.StartsWith(prefix))
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidPrefixException.Create(value ?? string.Empty, prefix);
                throw exception;
            }
        }

        public static Match Matches(string? value, string pattern, Func<Exception>? exceptionFactory = null)
        {
            if (value is null)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidStringFormatException.CreatePattern(string.Empty, pattern);
                throw exception;
            }

            var match = Regex.Match(value, pattern);

            if (!match.Success)
            {
                var exception = exceptionFactory?.Invoke() ?? InvalidStringFormatException.CreatePattern(value, pattern);
                throw exception;
            }

            return match;
        }
    }
}
