using Server.Interfejsy;

namespace Server
{
    delegate string CommandD(string command);
    delegate void CommunicatorD(IComunicator commander);
}
