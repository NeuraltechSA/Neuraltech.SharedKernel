namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public record DoubleValueObject(double Value) : ValueObject<double>(Value);
