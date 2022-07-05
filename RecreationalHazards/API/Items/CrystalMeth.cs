using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs;
using MEC;
using RecreationalHazards.API.Entities;
using RecreationalHazards.API.Enums;
using System.Collections.Generic;

namespace RecreationalHazards.API.Items
{
    [CustomItem(ItemType.Painkillers)]
    public class CrystalMeth : DrugItem
    {
        public override uint Id { get; set; } = 4;
        public override string Name { get; set; } = "Meth Crystal";
        public override string Description { get; set; } = "Eat some of this <color=blue>sky blue</color> and get high, just make sure not to go too high.";
        public override float Weight { get; set; } = 5f;
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

        public override int MaximumUse { get; set; } = 4;
        public override int StandardAmount { get; set; } = 2;
        public override int AddictionAmount { get; set; } = 3;
        public override float AddictionTime { get; set; } = 240f;
        public override string AddictionPrompt { get; set; } = "You have become <color=red>addicted</color>. Eat some more to satiate your addiction (and to become higher).";
        public override string AddictionDropMessage { get; set; } = "Are you sure you want to give up <color=blue>sky blue</color>?";

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
                    EffectType = EffectType.Exhausted,
                    ActivationTime = 0f,
                    ActiveTime = 120f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.MovementBoost,
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
                },
                new EffectProperties()
                {
                    EffectType = EffectType.Exhausted,
                    ActivationTime = 0f,
                    ActiveTime = 120f
                },
                new EffectProperties()
                {
                    EffectType = EffectType.MovementBoost,
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
                    EffectType = EffectType.Amnesia,
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
            RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[nameof(CrystalMeth)].Add(e.Player.Id, 0);
            RecreationalHazards.Instance.Api.TotalDrugsUsed[nameof(CrystalMeth)].Add(e.Player.Id, 0);
        }

        private void OnChangingRole(ChangingRoleEventArgs e)
        {
            RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[nameof(CrystalMeth)][e.Player.Id] = 0;
            RecreationalHazards.Instance.Api.TotalDrugsUsed[nameof(CrystalMeth)][e.Player.Id] = 0;
        }

        private void OnUsedItem(UsedItemEventArgs e)
        {
            if (!Check(e.Player.CurrentItem))
                return;

            int usedDrugs = RecreationalHazards.Instance.Api.TotalDrugsUsed[nameof(CrystalMeth)][e.Player.Id];

            if (usedDrugs >= AddictionAmount)
                foreach (EffectProperties effectProperty in AddictionEffects)
                    e.Player.DisableEffect(effectProperty.EffectType);

            ConsumptionStage consumptionStage = GetConsumptionStage(nameof(CrystalMeth), e.Player);

            IncreaseDrugCount(nameof(CrystalMeth), e.Player);
            StartEffects(consumptionStage, e.Player);

            Timing.CallDelayed(GetDrugTime(consumptionStage),
                () => RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[nameof(CrystalMeth)][e.Player.Id]--);

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