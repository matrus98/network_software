using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SerialListener : IListener
    {
        public void Start(CommunicatorD onConnect)
        {
            //stworzenie jednego odpowiadacza
            throw new NotImplementedException();
        }

        public void Stop()
        {
            //właściwie nic nie musi robić, bo jest jeden wątek
            throw new NotImplementedException();
        }
    }
}
