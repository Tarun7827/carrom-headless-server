using System;
namespace CarromHeadless.Networking
{
    public interface IWebSocketTransport
    {
        event Action<string> MessageReceived;
        event Action<string> ClientDisconnected;
        void Start();
        void Stop();
        void Send(string clientId, string payload);
    }
}
