//using Grpc.Contracts.Counter;
//using Grpc.Core;

//namespace Grpc.Server.Services;

//internal sealed class CounterService : ICounterService
//{
//    private readonly HttpContext _context;
//    private readonly ILogger<CounterService> _logger;

//    public CounterService(IHttpContextAccessor accessor, ILogger<CounterService> logger)
//    {
//        _context = accessor.HttpContext!;
//        _logger = logger;
//    }

//    public async Task<CounterMessage> AccumulateCountAsync(IAsyncStreamReader<CounterMessage> requestStream)
//    {
//        _logger.ConnectionId(_context.Connection.Id);

//        await foreach (var message in requestStream.ReadAllAsync())
//        {
//            _logger.IncrementCount(message.Count);

//            _counter.Increment(message.Count);
//        }

//        return new CounterMessage { Count = _counter.Count };
//    }
//}
