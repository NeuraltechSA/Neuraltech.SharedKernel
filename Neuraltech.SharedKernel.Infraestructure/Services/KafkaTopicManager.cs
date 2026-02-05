

using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Neuraltech.SharedKernel.Infraestructure.Services
{
    public sealed class KafkaTopicManager
    {
        private static async ValueTask UpdateTopic(
            IAdminClient client, ConfigResource configResource, Dictionary<string, string> configs
        )
        {
            var toUpdate = new Dictionary<ConfigResource, List<ConfigEntry>>
                    {
                        {
                            configResource,
                            configs.Select(kv => new ConfigEntry { Name = kv.Key, Value = kv.Value }).ToList()
                        }
                    };
            await client.IncrementalAlterConfigsAsync(toUpdate);
        }
        public static async ValueTask CreateOrUpdateTopic(
            IAdminClient client, string topicName, Dictionary<string, string> configs
        )
        {
            var configResource = new ConfigResource
            {
                Name = topicName,
                Type = ResourceType.Topic
            };

            try
            {

                await client.CreateTopicsAsync([ new TopicSpecification
                    {
                        Name = topicName,
                        Configs = configs,
                    }]);
            }
            catch (CreateTopicsException e)
            {

                if (e.Results.Any(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
                {
                    await UpdateTopic(client, configResource, configs);
                }
                else
                {
                    throw;
                }
            }

        }
    }
}
