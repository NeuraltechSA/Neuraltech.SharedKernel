using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Neuraltech.SharedKernel.Application.UseCases.Update
{

    /// <summary>
    /// Represents an optional value. Useful for unset properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Value"></param>
    /// <param name="HasValue"></param>
    public readonly record struct Optional<T>(T Value, bool HasValue)
    {

        /// <summary>
        /// Creates an optional value with a value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Optional<T> Some(T value) => new(value, true);

        /// <summary>
        /// Creates an optional value with no value (unset property).
        /// </summary>
        /// <returns></returns>
        public static Optional<T> None() => new(default!, false);

        /// <summary>
        /// Converts a value to an optional value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Optional<T>(T value) => Some(value);


        /// <summary>
        /// Retrieves the current value if it is set; otherwise, returns the specified default value.
        /// </summary>
        /// <param name="defaultValue">The value to return if the current value is not set.</param>
        /// <returns>The current value if it is set; otherwise, <paramref name="defaultValue"/>.</returns>
        public T GetValueOrDefault(T defaultValue) => HasValue ? Value : defaultValue;
    }
}
