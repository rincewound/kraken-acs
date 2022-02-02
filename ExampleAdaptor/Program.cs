// See https://aka.ms/new-console-template for more information
using EventStore.Client;


var client = new kraken_client.Client(EventStoreClientSettings.Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000"));

var events = client.streamEvents(0);
while (true)
{
    
    //events.ForEachAsync(e =>
    //{
    //    var krakenEvent = kraken.events.Event.fromUTF8(e.Event.Data.ToArray());
    //    Console.WriteLine($"Received an event of type {krakenEvent.evt}");
    //});

    await foreach (var e in events)
    {
        var krakenEvent = kraken.events.Event.fromUTF8(e.Event.Data.ToArray());
        Console.WriteLine($"Received an event of type {krakenEvent.evt}");
    }
}