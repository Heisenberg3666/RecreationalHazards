using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using RecreationalHazards.API;
using System;

namespace RecreationalHazards
{
    public class RecreationalHazards : Plugin<Config>
    {

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

            CustomItem.RegisterItems();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomItem.UnregisterItems();

            Api = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
