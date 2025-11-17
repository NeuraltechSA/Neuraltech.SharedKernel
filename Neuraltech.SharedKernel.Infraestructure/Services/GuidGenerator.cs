using Neuraltech.SharedKernel.Domain.Contracts;

namespace Neuraltech.SharedKernel.Infraestructure.Services
{
    public class GuidGenerator : IGuidGenerator
    {
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
    }
}
