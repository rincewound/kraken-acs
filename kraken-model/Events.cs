using kraken.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kraken.events
{
    public enum EventType
    {
        CreateGrid,
        AddUserToGrid,
        AddAccessRight,
        RemoveAccessRight,
        AddTag,
        RemoveTag
    }

    public class Event
    {
        public EventType evt;
        public Dictionary<string, string> data = new Dictionary<string, string>();

        public void applyToModel(OrganisationalModel model)
        {
            switch (evt)
            {
                case EventType.CreateGrid:
                    this.createGrid(model);
                    break;
                case EventType.AddUserToGrid:
                    addUserToGrid(model);
                    break;
                case EventType.AddAccessRight:
                    addAccessRight(model);
                    break;
                case EventType.RemoveAccessRight:
                    removeAccessRight(model);
                    break;
                case EventType.AddTag:
                    throw new NotImplementedException();
                    break;
                case EventType.RemoveTag:
                    throw new NotImplementedException();
                    break;
                default: throw new NotImplementedException();
            };
        }

        private void removeAccessRight(OrganisationalModel model)
        {
            var grid = model.getGrid(data["grid"]);
            var user = new User { name = data["name"] };
            var rgt = new AccessRight { name = data["accessright"] };
            grid.removeAccessRight(user, rgt);
        }

        private void addAccessRight(OrganisationalModel model)
        {
            var grid = model.getGrid(data["grid"]);
            var user = new User { name = data["name"] };
            var rgt = new AccessRight { name = data["accessright"] };
            grid.addAccessRightForUser(user, rgt);
        }

        private void addUserToGrid(OrganisationalModel model)
        {
            var grid = model.getGrid(data["grid"]);
            var user = new User { name = data["name"] };
            grid.addUser(user);
        }

        private void createGrid(OrganisationalModel model)
        {
            model.addGrid(data["grid"]);
        }

        public byte[] toUTF8()
        {
            return Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
        }

        public static Event fromUTF8(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Event>(str);
        }

    }
}
