using Exiled.Events.Handlers;

namespace RecreationalHazards.Events
{
    internal class ServerEvents
    {
        public void RegisterEvents()
        {
            Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        public void UnregisterEvents()
        {
            Server.WaitingForPlayers -= OnWaitingForPlayers;
        }

        private void OnWaitingForPlayers()
        {

        }
    }
}
