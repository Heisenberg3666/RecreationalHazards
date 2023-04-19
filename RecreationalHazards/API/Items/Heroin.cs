﻿using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using MEC;
using RecreationalHazards.API.Entities;
using RecreationalHazards.API.Enums;
using System.Collections.Generic;
using Exiled.Events.EventArgs.Player;

namespace RecreationalHazards.API.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class Heroin : DrugItem
    {
        public override uint Id { get; set; } = 2;
        public override string Name { get; set; } = "Heroin Injection";
        public override string Description { get; set; } = "Find somewhere secluded and shoot up, just make sure you don't run out.";
        public override float Weight { get; set; } = 2.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideLocker
                }
            }
        };

        public override int MaximumUse { get; set; } = 4;
        public override int StandardAmount { get; set; } = 2;
        public override int AddictionAmount { get; set; } = 2;
        public override float AddictionTime { get; set; } = 240f;
        public override string AddictionPrompt { get; set; } = "You have become <color=red>addicted</color>. Shoot up to satiate your addiction (and to stop the ants crawling on you).";
        public override string AddictionDropMessage { get; set; } = "Are you sure you want to give up heroin?";

        public override Dictionary<ConsumptionStage, List<EffectProperties>> Effects { get; set; } = new Dictionary<ConsumptionStage, List<EffectProperties>>()
        {
            [ConsumptionStage.FirstTime] = new List<EffectProperties>()
            {
                new EffectProperties()
                {
                    EffectType = EffectType.Concussed,
                    ActivationTime = 0f,
                    ActiveTime = 120f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.Deafened,
                    ActivationTime = 0f,
                    ActiveTime = 120f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.Invigorated,
                    ActivationTime = 0f,
                    ActiveTime = 120f
                }
            },
            [ConsumptionStage.Standard] = new List<EffectProperties>()
            {
                new EffectProperties()
                {
                    EffectType = EffectType.Concussed,
                    ActivationTime = 0f,
                    ActiveTime = 120f
                }
            },
            [ConsumptionStage.Overdose] = new List<EffectProperties>()
            {
                new EffectProperties()
                {
                    EffectType = EffectType.Asphyxiated,
                    ActivationTime = 0f,
                    ActiveTime = 600f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.Blinded,
                    ActivationTime = 0f,
                    ActiveTime = 600f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.Deafened,
                    ActivationTime = 0f,
                    ActiveTime = 600f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.AmnesiaVision,
                    ActivationTime = 0f,
                    ActiveTime = 600f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.Disabled,
                    ActivationTime = 0f,
                    ActiveTime = 600f
                }
            }
        };

        public override List<EffectProperties> AddictionEffects { get; set; } = new List<EffectProperties>()
        {
            new EffectProperties()
            {
                EffectType = EffectType.Bleeding,
                ActivationTime = 0f,
                ActiveTime = 600f
            },
            new EffectProperties()
            {
                EffectType = EffectType.Burned,
                ActivationTime = 0f,
                ActiveTime = 600f
            }
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.UsedItem += OnUsedItem;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.UsedItem -= OnUsedItem;

            base.UnsubscribeEvents();
        }

        private void OnChangingRole(ChangingRoleEventArgs e)
        {
            RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[nameof(Heroin)][e.Player.Id] = 0;
            RecreationalHazards.Instance.Api.TotalDrugsUsed[nameof(Heroin)][e.Player.Id] = 0;
        }

        private void OnUsedItem(UsedItemEventArgs e)
        {
            if (!Check(e.Player.CurrentItem))
                return;

            int usedDrugs = RecreationalHazards.Instance.Api.TotalDrugsUsed[nameof(Heroin)][e.Player.Id];

            if (usedDrugs >= AddictionAmount)
                foreach (EffectProperties effectProperty in AddictionEffects)
                    e.Player.DisableEffect(effectProperty.EffectType);

            ConsumptionStage consumptionStage = GetConsumptionStage(nameof(Heroin), e.Player);

            IncreaseDrugCount(nameof(Heroin), e.Player);
            StartEffects(consumptionStage, e.Player);

            Timing.CallDelayed(GetDrugTime(consumptionStage),
                () => RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[nameof(Heroin)][e.Player.Id]--);

            if (usedDrugs++ >= AddictionAmount)
            {
                Timing.CallDelayed(AddictionTime, () =>
                {
                    foreach (EffectProperties effectProperty in AddictionEffects)
                    {
                        Timing.CallDelayed(effectProperty.ActivationTime,
                            () => e.Player.EnableEffect(effectProperty.EffectType, effectProperty.ActiveTime));
                    }

                    e.Player.ShowHint(AddictionPrompt, 5f);
                });
            }
        }
    }
}
