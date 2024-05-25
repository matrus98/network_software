using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfejsy
{
    interface IListener
    {
        void Start(CommunicatorD onConnect);
        void Stop();
    }
}
