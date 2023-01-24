using System.ServiceModel;

using Grpc.Core;

namespace Grpc.Contracts.Counter;

[ServiceContract(Name = "grpc.counter.v1.CounterService")]
public interface ICounterService
{
    [OperationContract]
    Task<CounterMessage> AccumulateCountAsync(IAsyncStreamReader<CounterMessage> requestStream);
}
