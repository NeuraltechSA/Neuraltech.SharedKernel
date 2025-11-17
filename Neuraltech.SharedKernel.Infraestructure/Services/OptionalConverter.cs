using Neuraltech.SharedKernel.Application.UseCases.Update;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neuraltech.SharedKernel.Infraestructure.Services
{
    public class OptionalConverter<T> : JsonConverter<Optional<T>>
    {
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<T>(ref reader, options);
            return Optional<T>.Some(value!);
        }

        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
