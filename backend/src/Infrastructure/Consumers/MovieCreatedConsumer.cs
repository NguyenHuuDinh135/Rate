using backend.Application.Common.Contracts;
using Elastic.Clients.Elasticsearch;
using MassTransit;

namespace backend.Infrastructure.Consumers;

public class MovieCreatedConsumer : IConsumer<MovieCreatedEvent>
{
    private readonly ElasticsearchClient _elasticClient;

    public MovieCreatedConsumer(ElasticsearchClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task Consume(ConsumeContext<MovieCreatedEvent> context)
    {
        // Index dữ liệu phim sang Elasticsearch
        await _elasticClient.IndexAsync(
            context.Message,
            i => i.Index("movies"));
    }
}