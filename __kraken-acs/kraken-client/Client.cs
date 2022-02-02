using EventStore.Client;

namespace kraken_client
{
    public class Client
    {
        const string streamName = "kraken-events";

        EventStoreClient client;
        public Client(EventStoreClientSettings settings)
        {
            client = new EventStoreClient(settings);
        }

        public void pushEvent(kraken.events.Event e)
        {
            var theEvent = new EventData(
                Uuid.NewUuid(),
                "Event",
                e.toUTF8()
                );

           var task = client.AppendToStreamAsync(streamName, StreamState.Any, new[] { theEvent });
           task.Wait();
        }

        public EventStoreClient.ReadStreamResult streamEvents(ulong startPos )
        {
            StreamPosition s = new StreamPosition(startPos);
            return client.ReadStreamAsync(Direction.Forwards, streamName, s, 10);
        }
    }
}