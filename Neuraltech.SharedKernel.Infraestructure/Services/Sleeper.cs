using Neuraltech.SharedKernel.Domain.Contracts;

namespace Neuraltech.SharedKernel.Infraestructure.Services
{
    public class Sleeper : ISleeper
    {
        public async Task Sleep(TimeSpan delay, CancellationToken cancellationToken = default)
        {
            await Task.Delay(delay, cancellationToken);
        }
    }
}
