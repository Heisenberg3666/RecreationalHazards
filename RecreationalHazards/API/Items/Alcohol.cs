using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs;
using MEC;
using RecreationalHazards.API.Entities;
using RecreationalHazards.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecreationalHazards.API.Items
{
    [CustomItem(ItemType.SCP207)]
    public class Alcohol : DrugItem
    {
        public override uint Id { get; set; } = 1;
        public override string Name { get; set; } = "Alcohol";
        public override string Description { get; set; } = "";
        public override float Weight { get; set; } = 7.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = SpawnLocation.InsideLocker
                }
            }
        };

        public override int MaximumUse { get; set; } = 5;
        public override int StandardAmount { get; set; } = 2;

        public override Dictionary<ConsumptionStage, List<EffectProperties>> Effects { get; set; } = new Dictionary<ConsumptionStage, List<EffectProperties>>()
        {
            [ConsumptionStage.FirstTime] = new List<EffectProperties>()
            {
                new EffectProperties()
                {
                    EffectType = EffectType.Deafened,
                    ActivationTime = 0f,
                    ActiveTime = 10f
                }
            },
            [ConsumptionStage.Standard] = new List<EffectProperties>()
            {
                new EffectProperties()
                {
                    EffectType = EffectType.Concussed,
                    ActivationTime = 0f,
                    ActiveTime = 10f
                }
            },
            [ConsumptionStage.Overdose] = new List<EffectProperties>()
            {
                new EffectProperties()
                {
                    EffectType = EffectType.Poisoned,
                    ActivationTime = 0f,
                    ActiveTime = 10f
                }
            }
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.UsedItem += OnUsedItem;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.UsedItem -= OnUsedItem;

            base.UnsubscribeEvents();
        }

        private void OnVerified(VerifiedEventArgs e)
        {
            RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[nameof(Alcohol)].Add(e.Player.Id, 0);
            RecreationalHazards.Instance.Api.TotalDrugsUsed[nameof(Alcohol)].Add(e.Player.Id, 0);
        }

        private void OnChangingRole(ChangingRoleEventArgs e)
        {
            RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[nameof(Alcohol)][e.Player.Id] = 0;
            RecreationalHazards.Instance.Api.TotalDrugsUsed[nameof(Alcohol)][e.Player.Id] = 0;
        }

        private void OnUsedItem(UsedItemEventArgs e)
        {
            if (!Check(e.Player.CurrentItem))
                return;

            IncreaseDrugCount(nameof(Alcohol), e.Player);

            StartEffects(GetConsumptionStage(nameof(Alcohol), e.Player), e.Player);
        }
    }
}
