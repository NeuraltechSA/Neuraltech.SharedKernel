using Neuraltech.SharedKernel.Domain.Exceptions;

namespace Neuraltech.SharedKernel.Domain.Services
{
    public static class Ensure
    {
        public static void NotNullOrWhiteSpace(string? value, Func<string?, Exception>? exceptionFactory = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                var exception = exceptionFactory?.Invoke(value) ?? ValueIsNullOrEmptyException.Create(value);
                throw exception;
            }
        }

        public static void StartsWith(string value, string prefix, Func<string, string, Exception>? exceptionFactory = null)
        {
            if (!value.StartsWith(prefix))
            {
                var exception = exceptionFactory?.Invoke(value, prefix) ?? InvalidStringFormatException.CreateStartsWith(value, prefix);
                throw exception;
            }
        }

        public static void EndsWith(string value, string suffix, Func<string, string, Exception>? exceptionFactory = null)
        {
            if (!value.EndsWith(suffix))
            {
                var exception = exceptionFactory?.Invoke(value, suffix) ?? InvalidStringFormatException.CreateEndsWith(value, suffix);
                throw exception;
            }
        }

        public static void StringContains(string value, string substring, Func<string, string, Exception>? exceptionFactory = null)
        {
            if (!value.Contains(substring))
            {
                var exception = exceptionFactory?.Invoke(value, substring) ?? InvalidStringFormatException.CreateContains(value, substring);
                throw exception;
            }
        }

        public static void StringNotContains(string value, string substring, Func<string, string, Exception>? exceptionFactory = null)
        {
            if (value.Contains(substring))
            {
                var exception = exceptionFactory?.Invoke(value, substring) ?? InvalidStringFormatException.CreateNotContains(value, substring);
                throw exception;
            }
        }

        public static void InRange<T>(
            T value, T min, T max, Func<T, T, T, Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if(value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                var exception = exceptionFactory?.Invoke(value, min, max) ?? ValueOutOfRangeException.Create(value, min, max);
                throw exception;
            }
        }

        public static void GreaterOrEqualThan<T>(
            T value, T minimum, Func<T, T, Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(minimum) < 0)
            {
                var exception = exceptionFactory?.Invoke(value, minimum) ?? ValueOutOfRangeException.CreateGreaterOrEqualThan(value, minimum);
                throw exception;
            }
        }

        public static void LessOrEqualThan<T>(
            T value, T maximum, Func<T, T, Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(maximum) > 0)
            {
                var exception = exceptionFactory?.Invoke(value, maximum) ?? ValueOutOfRangeException.CreateLessOrEqualThan(value, maximum);
                throw exception;
            }
        }

        public static void GreaterThan<T>(
            T value, T minimum, Func<T, T, Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(minimum) <= 0)
            {
                var exception = exceptionFactory?.Invoke(value, minimum) ?? ValueOutOfRangeException.CreateGreaterThan(value, minimum);
                throw exception;
            }
        }

        public static void LessThan<T>(
            T value, T maximum, Func<T, T, Exception>? exceptionFactory = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(maximum) >= 0)
            {
                var exception = exceptionFactory?.Invoke(value, maximum) ?? ValueOutOfRangeException.CreateLessThan(value, maximum);
                throw exception;
            }
        }

        public static void HasLength<T>(IEnumerable<T> value, int length, Func<IEnumerable<T>, int, Exception>? exceptionFactory = null)
        {
            if(value.Count() != length)
            {
                var exception = exceptionFactory?.Invoke(value, length) ?? InvalidLengthException.Create(value, length);
                throw exception;
            }
        }

        public static void HasLengthBetween<T>(IEnumerable<T> value, int minLength, int maxLength, Func<IEnumerable<T>, int, int, Exception>? exceptionFactory = null)
        {
            if (value.Count() < minLength || value.Count() > maxLength)
            {
                var exception = exceptionFactory?.Invoke(value, minLength, maxLength) ?? InvalidLengthException.Create(value, minLength, maxLength);
                throw exception;
            }
        }

        public static void HasMinLength<T>(IEnumerable<T> value, int minLength, Func<IEnumerable<T>, int, Exception>? exceptionFactory = null)
        {
            if(value.Count() < minLength)
            {
                var exception = exceptionFactory?.Invoke(value, minLength) ?? InvalidLengthException.CreateMinLength(value, minLength);
                throw exception;
            }
        }

        public static void HasMaxLength<T>(IEnumerable<T> value, int maxLength, Func<IEnumerable<T>, int, Exception>? exceptionFactory = null)
        {
            if(value.Count() > maxLength)
            {
                var exception = exceptionFactory?.Invoke(value, maxLength) ?? InvalidLengthException.CreateMaxLength(value, maxLength);
                throw exception;
            }
        }
        public static void HasAny<T>(IEnumerable<T> value, Func<IEnumerable<T>, int, Exception>? exceptionFactory = null)
        {
            HasMinLength(value, 1, exceptionFactory);
        }

        public static void IsEmpty<T>(IEnumerable<T> value, Func<IEnumerable<T>, int, Exception>? exceptionFactory = null)
        {
            HasMaxLength(value, 0, exceptionFactory);
        }

        public static void IsTrue(bool value, Func<bool, Exception>? exceptionFactory = null)
        {
            if (!value)
            {
                var exception = exceptionFactory?.Invoke(value) ?? InvalidBooleanException.TrueExpected();
                throw exception;
            }
        }

        public static void IsFalse(bool value, Func<bool, Exception>? exceptionFactory = null)
        {
            if (value)
            {
                var exception = exceptionFactory?.Invoke(value) ?? InvalidBooleanException.FalseExpected();
                throw exception;
            }
        }

        public static void NotNull<T>(T? value, Func<T?, Exception>? exceptionFactory = null)
        {
            if (value is null)
            {
                var exception = exceptionFactory?.Invoke(value) ?? ValueIsNullException.Create();
                throw exception;
            }
        }

        public static void IsNull<T>(T? value, Func<T?, Exception>? exceptionFactory = null)
        {
            if (value is not null)
            {
                var exception = exceptionFactory?.Invoke(value) ?? ExpectedNullException.Create();
                throw exception;
            }
        }

        public static void Contains<T>(T item, IEnumerable<T> allowedValues, Func<IEnumerable<T>, T, Exception>? exceptionFactory = null)
        {
            if (!allowedValues.Contains(item))
            {
                var exception = exceptionFactory?.Invoke(allowedValues, item) ?? UnexpectedValueException.CreateFromAllowed(item, allowedValues);
                throw exception;
            }
        }

        public static void NotContains<T>(T item, IEnumerable<T> forbiddenValues, Func<IEnumerable<T>, T, Exception>? exceptionFactory = null)
        {
            if (forbiddenValues.Contains(item))
            {
                var exception = exceptionFactory?.Invoke(forbiddenValues, item) ?? UnexpectedValueException.CreateFromForbidden(item, forbiddenValues);
                throw exception;
            }
        }
    }
}
