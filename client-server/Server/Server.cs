using Server.Interfejsy;
using Server.Modules;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    class Server
    {
        public string IP_Address { get; private set; }

        //Kolekcja nasłuchiwaczy
        List<IListener> serverListeners;
        //Kolekcja odpowiadaczy
        List<ICommunicator> serverCommunicators;
        //Kolekcdja usług
        Dictionary<string, IServiceModule> serverServices;

        public Server(string ipAddress)
        {
            IP_Address = ipAddress;
            serverListeners = new List<IListener>();
            serverCommunicators = new List<ICommunicator>();
            serverServices = new Dictionary<string, IServiceModule>();
        }

        public void CreateConfigurationService()
        {
            ConfigurationService service = new ConfigurationService(this);

            this.AddService("conf", service);
        }

        public void Start()
        {
            foreach (IListener listener in serverListeners)
                listener.Start(AddCommunicator);

            Console.WriteLine("Server has started");
        }

        public void AddListener(IListener listener) => serverListeners.Add(listener);
        public void RemoveListener(IListener listener) => serverListeners.Remove(listener);
        public void RemoveListener(Type type)
        {
            IListener listener = serverListeners.Find(l => l.GetType() == type);

            if(listener != null)
            {
                RemoveListener(listener);
            }
        }

        private void AddCommunicator(ICommunicator commander)
        {
            serverCommunicators.Add(commander);

            commander.Start(get_and_perform_Service, RemoveCommunicator);
        }

        public string get_and_perform_Service(string in_command)
        {
            int index_of_end_of_service = in_command.IndexOf(" ");
            string failure_message = "Requested service not found";

            if (index_of_end_of_service < 0)
                return failure_message;

            IServiceModule service;
            serverServices.TryGetValue(in_command.Substring(0, index_of_end_of_service), out service);

            if (service == null)
                return failure_message;

            return service.AnswerCommand(in_command.Substring(index_of_end_of_service + 1)) + "\n";
        }

        private void RemoveCommunicator(ICommunicator commander) => serverCommunicators.Remove(commander);

        public void AddService(string serviceName, IServiceModule service) => serverServices.Add(serviceName, service);

        public void RemoveService(string serviceName) => serverServices.Remove(serviceName);

        public void Stop()
        {
            foreach (ICommunicator communicator in serverCommunicators.ToArray())
                communicator.Stop();

            foreach (IListener listener in serverListeners.ToArray())
                listener.Stop();

            serverListeners.Clear();
            serverCommunicators.Clear();
            serverServices.Clear();

            System.Environment.Exit(0);
        }
    }
}
