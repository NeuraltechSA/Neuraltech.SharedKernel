using System;

namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public record DateTimeValueObject(DateTime Value) : ValueObject<DateTime>(Value);