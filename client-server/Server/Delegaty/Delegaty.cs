using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    delegate string CommandD(string command);

    delegate void CommunicatorD(ICommunicator commander);
}
