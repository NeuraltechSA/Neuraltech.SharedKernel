using Neuraltech.SharedKernel.Domain.Base;
using System;

namespace Neuraltech.SharedKernel.Infraestructure.DTO
{
    public readonly record struct OptionalParam<T>(T Value, bool HasValue)
    {
        public static OptionalParam<T> Some(T value) => new(value, true);
        public static OptionalParam<T> None() => new(default!, false);

        public static bool TryParse(string? value, out OptionalParam<T> result)
        {
            if (value == null)
            {
                result = None();
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                result = Some((T)(object)value);
                return true;
            }

            try
            {
                var convertedValue = (T)Convert.ChangeType(value, typeof(T));
                result = Some(convertedValue);
                return true;
            }
            catch
            {
                result = None();
                return false;
            }
        }

        public static implicit operator Optional<T>(OptionalParam<T> param) => 
            param.HasValue ? Optional<T>.Some(param.Value) : Optional<T>.None();
    }
}
