using EventStore.Client;
using kraken.model;
using System.Threading;

namespace kraken.adaptor
{
    public class Adaptor
    {
        IAdaptorImpl adaptor;
        AutoResetEvent evt = new AutoResetEvent(false);
        ulong lastEventNumber;
        OrganisationalModel model;

        public Adaptor(IAdaptorImpl impl, OrganisationalModel model)
        {
            this.adaptor = impl;
            this.model = model;
        }

        public async void run()
        {
            var client = new kraken_client.Client(EventStoreClientSettings.Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000"));

            while (!evt.WaitOne(1000))
            {
                var events = client.streamEvents(lastEventNumber);
                await foreach (var e in events)
                {
                    var krakenEvent = kraken.events.Event.fromUTF8(e.Event.Data.ToArray());
                    Console.WriteLine($"Received an event of type {krakenEvent.evt}");

                    this.adaptor.on_event(model, krakenEvent);
                    this.lastEventNumber = e.Event.EventNumber;
                    krakenEvent.applyToModel(model);
                }
            }
        }
    }
}