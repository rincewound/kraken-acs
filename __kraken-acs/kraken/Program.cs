// See https://aka.ms/new-console-template for more information
//using EventStore.Client;
//using kraken;
//using kraken.model;
//using kraken.serializer;

//var model = new OrganisationalModel();
//model.addGrid("foo");
//model.addGrid("foo.bar");

//var sg = model.getGrid("foo.bar");

//Serializer.Serialize(model, "test.txt");

//var model2 = Serializer.Derserialize("test.txt");

//var sg2 = model2.getGrid("foo.bar");

//var settings = EventStoreClientSettings.Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
//var c = new kraken_client.Client(settings);

//var events = c.streamEvents(0);

//await events.ForEachAsync(e =>
//{
//    var evt = kraken.events.Event.fromUTF8(e.Event.Data.ToArray());
//    evt.applyToModel(model2);
//});

using CommandLine;
using EventStore.Client;

class Program
{
    [Verb("CreateGrid", HelpText = "Create a new access grid, this can be a subgrid of anothjer grid")]
    class createGridOptions
    {
        [Value(0)]
        public string GridName { get; set; }
    }

    [Verb("AddUserToGrid", HelpText = "Adds a given user to a grid without any special rights")]
    class addUserToGridOptions
    {
        [Value(0)]
        public string GridName { get; set; }

        [Value(1)]
        public string UserName { get; set; }
    }

    [Verb("AddAccessRight", HelpText = "Adds an access right for a user to a grid")]
    class addAccessRightsOptions
    {
        [Value(0)]
        public string GridName { get; set; }

        [Value(1)]
        public string UserName { get; set; }

        [Value(2)]
        public string AccessRight { get; set; }
    }

    static void Main(string[] args)
    {
        kraken.events.Event evt = new kraken.events.Event();
        Parser.Default.ParseArguments<createGridOptions,
                                      addUserToGridOptions,
                                      addAccessRightsOptions>(args)
                          .WithParsed<createGridOptions>(cgrid =>
                          {
                              evt.evt = kraken.events.EventType.CreateGrid;
                              evt.data["grid"] = cgrid.GridName;
                              Console.WriteLine($"Add access grid {cgrid.GridName}");
                          })
                          .WithParsed<addUserToGridOptions>(ad =>
                         {
                             evt.evt = kraken.events.EventType.AddUserToGrid;
                             evt.data["grid"] = ad.GridName;
                             evt.data["name"] = ad.UserName;
                             Console.WriteLine($"Add user {ad.UserName} to access grid {ad.GridName}");
                         })
                          .WithParsed<addAccessRightsOptions>(ad =>
                          {
                              evt.evt = kraken.events.EventType.AddAccessRight;
                              evt.data["grid"] = ad.GridName;
                              evt.data["name"] = ad.UserName;
                              evt.data["accessright"] = ad.AccessRight;
                              Console.WriteLine($"Add accessright {ad.AccessRight} for user {ad.UserName} to access grid {ad.GridName}");
                          });



        var client = new kraken_client.Client(EventStoreClientSettings.Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000"));
        client.pushEvent(evt);
    }
}