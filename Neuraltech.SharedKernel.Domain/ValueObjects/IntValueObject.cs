namespace Neuraltech.SharedKernel.Domain.ValueObjects;

public record IntValueObject(int Value) : ValueObject<int>(Value);