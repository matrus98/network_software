using Server.Communicators;
using Server.Interfejsy;
using Shared;
using System.IO;

namespace Server.Listeners
{
    class FileClientListener : IListener
    {
        CommunicatorD onConnect;

        public int Port { get; private set; }

        public FileClientListener()
        {
            if (!Directory.Exists(SharedConstants.DIRECTORY_FOR_COMMUNICATION))
                Directory.CreateDirectory(SharedConstants.DIRECTORY_FOR_COMMUNICATION);
        }

        public void Start(CommunicatorD onConnect)
        {
            this.onConnect += onConnect;

            FileCommunicator file = new FileCommunicator();

            this.onConnect(file);
        }

        public void Stop()
        {
            
        }
    }
}
