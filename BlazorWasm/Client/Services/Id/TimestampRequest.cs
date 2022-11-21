using System.Runtime.Serialization;

namespace BlazorWasm.Client.Services.Id;

[DataContract]
public sealed class TimestampRequest
{
    public TimestampRequest()
    {    
    }

    public TimestampRequest(Guid id)
    {
        Id = id;
    }

    [DataMember(Order = 1)]
    public Guid Id { get; set; }
}
