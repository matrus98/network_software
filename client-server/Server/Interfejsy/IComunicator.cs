using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfejsy
{
    interface IComunicator
    {
        void Start(CommunicatorD onCommand, CommunicatorD onDisconnect);
        void Stop();
    }
}
