using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kraken.adaptor
{
    public interface IAdaptorImpl
    {
        void on_event(kraken.model.OrganisationalModel model, kraken.events.Event e);
    }
}
