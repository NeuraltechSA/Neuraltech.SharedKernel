using System.Collections.Generic;

namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public record ListValueObject<T>(IReadOnlyList<T> Value) : ValueObject<IReadOnlyList<T>>(Value);