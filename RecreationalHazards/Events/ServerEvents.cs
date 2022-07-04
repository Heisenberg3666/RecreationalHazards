using Exiled.Events.EventArgs;
using Exiled.Events.Handlers;
using Exiled.Loader;
using RecreationalHazards.API.Entities;
using RecreationalHazards.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RecreationalHazards.Events
{
    internal class ServerEvents
    {
        public void RegisterEvents()
        {
            Player.Verified += OnVerified;
        }

        public void UnregisterEvents()
        {
            Player.Verified -= OnVerified;
        }

        private void OnVerified(VerifiedEventArgs e)
        {
            Assembly exiledAssembly = Loader.GetPlugin(nameof(RecreationalHazards)).Assembly;
            Type[] itemTypes = exiledAssembly.GetTypes()
                .Where(x => x.Namespace == nameof(RecreationalHazards) + ".API.Items")
                .ToArray();

            List<DrugStatistics> playerStats = new List<DrugStatistics>();

            foreach (Type type in itemTypes)
            {
                playerStats.Add(new DrugStatistics()
                {
                    DrugType = type.Name,
                    DrugsCurrentlyOn = 0,
                    DrugsTaken = 0,
                    ConsumptionStage = ConsumptionStage.FirstTime,
                    LastTaken = DateTime.UtcNow,
                });
            }

            RecreationalHazards.Instance.Api.PlayerStatistics.Add(e.Player.Id, playerStats);
        }
    }
}
