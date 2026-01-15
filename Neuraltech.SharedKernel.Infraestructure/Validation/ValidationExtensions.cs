namespace Neuraltech.SharedKernel.Infraestructure.Validation
{
    public static class ValidationExtensions
    {
        public static bool BeAValidGuid(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
                
            return Guid.TryParse(value, out _);
        }
    }
}
