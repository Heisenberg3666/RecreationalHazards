using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using RecreationalHazards.API;
using RecreationalHazards.Events;
using System;

namespace RecreationalHazards
{
    public class RecreationalHazards : Plugin<Config>
    {
        private ServerEvents _serverEvents;

        public static RecreationalHazards Instance;
        public RecreationalHazardsApi Api;

        public override string Name => "RecreationalHazards";
        public override string Author => "Heisenberg3666";
        public override Version Version => new Version(1, 0, 0, 0);
        public override Version RequiredExiledVersion => new Version(5, 2, 2);

        public override void OnEnabled()
        {
            Instance = this;
            Api = new RecreationalHazardsApi();
            _serverEvents = new ServerEvents();

            CustomItem.RegisterItems();
            RegisterEvents();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            CustomItem.UnregisterItems();

            _serverEvents = null;
            Api = null;
            Instance = null;

            base.OnDisabled();
        }

        public void RegisterEvents()
        {
            _serverEvents.RegisterEvents();
        }

        public void UnregisterEvents()
        {
            _serverEvents.RegisterEvents();
        }
    }
}
