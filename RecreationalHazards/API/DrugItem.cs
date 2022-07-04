using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using MEC;
using RecreationalHazards.API.Entities;
using RecreationalHazards.API.Enums;
using System.Collections.Generic;

namespace RecreationalHazards.API
{
    public abstract class DrugItem : CustomItem
    {
        /// <summary>
        /// This is the maximum amount of a drug that you can have at one time.
        /// </summary>
        public virtual int MaximumUse { get; set; }

        /// <summary>
        /// This is the amount of drugs that it takes to become used to the effects.
        /// </summary>
        public virtual int StandardAmount { get; set; }
        
        /// <summary>
        /// These are all the effects that can be given to the player depending on their consumption stage.
        /// </summary>
        public virtual Dictionary<ConsumptionStage, List<EffectProperties>> Effects { get; set; }

        /// <summary>
        /// This gives the player the effects corresponding to their consumption stage.
        /// </summary>
        /// <param name="consumptionStage"></param>
        /// <param name="player"></param>
        public void StartEffects(ConsumptionStage consumptionStage, Player player)
        {
            foreach (EffectProperties effectProperty in Effects[consumptionStage])
            {
                Timing.CallDelayed(effectProperty.ActivationTime,
                   () => player.EnableEffect(effectProperty.EffectType, effectProperty.ActiveTime));
            }
        }

        public void StopEffects(ConsumptionStage consumptionStage, Player player)
        {
            foreach (EffectProperties effectProperty in Effects[consumptionStage])
            {
                player.DisableEffect(effectProperty.EffectType);
            }
        }

        public void IncreaseDrugCount(string itemName, Player player)
        {
            RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[itemName][player.Id]++;
            RecreationalHazards.Instance.Api.TotalDrugsUsed[itemName][player.Id]++;
        }

        public ConsumptionStage GetConsumptionStage(string itemName, Player player)
        {
            int usingDrugs = RecreationalHazards.Instance.Api.DrugsCurrentlyUsing[itemName][player.Id];
            int totalDrugs = RecreationalHazards.Instance.Api.TotalDrugsUsed[itemName][player.Id];

            if (usingDrugs > MaximumUse)
                return ConsumptionStage.Overdose;
            if (totalDrugs >= StandardAmount)
                return ConsumptionStage.Standard;

            return ConsumptionStage.FirstTime;
        }
    }
}
