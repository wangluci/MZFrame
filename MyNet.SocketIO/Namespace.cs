using System;
using System.Collections.Concurrent;

namespace MyNet.SocketIO
{
    public delegate void ConnectionListener(NSClient client);
    public class Namespace
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private ConcurrentDictionary<string, NSClient> _clients = new ConcurrentDictionary<string, NSClient>();
        private ConnectionListener _connectListeners;
        public Namespace(string name)
        {
            _name = name;
        }
        public void AddConnectListener(ConnectionListener listener)
        {
            _connectListeners += listener;
        }
        public void EmitConnect(NSClient client)
        {
            _connectListeners?.Invoke(client);
        }
        public bool Contain(string id)
        {
            return _clients.ContainsKey(id);
        }
        public void AddClient(NSClient nscl)
        {
            _clients.TryAdd(nscl.Client.SessionID, nscl);
        }
        public bool TryGetClient(string id, out NSClient client)
        {
            return _clients.TryGetValue(id, out client);
        }
        public NSClient RemoveClient(string id)
        {
            NSClient client;
            if (_clients.TryRemove(id, out client))
            {
                return client;
            }
            else
            {
                return null;
            }
        }

    }
}
