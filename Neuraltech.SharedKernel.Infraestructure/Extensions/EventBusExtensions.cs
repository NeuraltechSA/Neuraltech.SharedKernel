using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neuraltech.SharedKernel.Domain.Contracts;
using Neuraltech.SharedKernel.Infraestructure.Services.MassTransit;


namespace Neuraltech.SharedKernel.Infraestructure.Extensions
{
    public static class EventBusExtensions
    {
        /*
        public static IHostApplicationBuilder UseMassTransit<T>(
            this IHostApplicationBuilder builder, 
            Type consumerType, 
            Action<IKafkaFactoryConfigurator, IRiderRegistrationContext>? kafkaConfigurator = null,
            Action<IRiderRegistrationConfigurator>? riderConfigurator = null

        )
            where T : DbContext, IUnitOfWork
        {
            
            // Bus In-Memory para eventos de dominio
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumers(consumerType.Assembly);

                x.AddLogging(cfg => cfg.AddConsole());
                //x.AddEntityFrameworkOutbox<T>(o =>
                //{
                //    o.UsePostgres();
                //    o.UseBusOutbox();
                //});


                x.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });

            });

            // Bus Kafka para eventos de integración
            builder.Services.AddMassTransit(x =>
            {

                x.AddConsumers(consumerType.Assembly);

                x.AddLogging(cfg => cfg.AddConsole());

                x.AddEntityFrameworkOutbox<T>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });
                
                x.UsingInMemory();

                x.AddRider(rider =>
                {

                    rider.AddConsumers(consumerType.Assembly);

                    if (riderConfigurator != null) riderConfigurator(rider);

                    rider.UsingKafka((context, cfg) =>
                    {
                        cfg.Host(builder.Configuration["Kafka:BootstrapServers"], host =>
                        {
                            host.UseSasl(sasl =>
                            {
                                sasl.Username = builder.Configuration["Kafka:Username"];
                                sasl.Password = builder.Configuration["Kafka:Password"];
                                sasl.Mechanism  = Enum.Parse<SaslMechanism>(builder.Configuration["Kafka:SaslMechanism"]!);
                                sasl.SecurityProtocol = Enum.Parse<SecurityProtocol>(builder.Configuration["Kafka:SecurityProtocol"]!);
                            });
                        });

                        if (kafkaConfigurator != null) kafkaConfigurator(cfg, context);
                    });
                });
            });

            builder.Services.AddTransient<IEventBus, EventBus>();
            builder.Services.AddTransient<ISnapshotPublisher, SnapshotPublisher>();

            return builder;
        }*/


        public static IHostApplicationBuilder UseMassTransit<T>(
            this IHostApplicationBuilder builder,
            Action<IBusRegistrationConfigurator>? inMemoryBusConfigurator = null,
            Action<IRiderRegistrationConfigurator>? riderConfigurator = null,
            Action<IKafkaFactoryConfigurator, IRiderRegistrationContext>? kafkaConfigurator = null
            //Action<IRiderRegistrationConfigurator>? riderConfigurator = null

        )
            where T : DbContext, IUnitOfWork
        {
            builder.Services.AddMassTransit(x =>
            {
                inMemoryBusConfigurator?.Invoke(x);

                x.AddLogging(cfg => cfg.AddConsole());

                x.AddEntityFrameworkOutbox<T>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                });

                x.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });

                x.AddRider(rider =>
                {
                    riderConfigurator?.Invoke(rider);


                    rider.UsingKafka((context, cfg) =>
                    {

                        kafkaConfigurator?.Invoke(cfg, context);


                        cfg.Host(builder.Configuration["Kafka:BootstrapServers"], host =>
                        {
                            host.UseSasl(sasl =>
                            {
                                sasl.Username = builder.Configuration["Kafka:Username"];
                                sasl.Password = builder.Configuration["Kafka:Password"];
                                sasl.Mechanism = Enum.Parse<SaslMechanism>(builder.Configuration["Kafka:SaslMechanism"]!);
                                sasl.SecurityProtocol = Enum.Parse<SecurityProtocol>(builder.Configuration["Kafka:SecurityProtocol"]!);
                            });
                        });

                    });
                });
            });

            builder.Services.AddTransient<IEventBus, EventBus>();
            builder.Services.AddTransient<ISnapshotPublisher, SnapshotPublisher>();

            return builder;
        }

    }
}
