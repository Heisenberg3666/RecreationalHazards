using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using MEC;
using RecreationalHazards.API.Entities;
using RecreationalHazards.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecreationalHazards.API
{
    public abstract class DrugItem : CustomItem
    {
        /// <summary>
        /// This is the maximum amount of a drug that you can have at one time.
        /// </summary>
        public virtual int MaximumUse { get; set; }
        
        /// <summary>
        /// The message shown to the addicted player after dropping the drug.
        /// </summary>
        public virtual string AddictionMessage { get; set; }

        /// <summary>
        /// This is the amount needed to become addicted to the drug.
        /// </summary>
        public virtual int AmountForAddiction { get; set; }

        /// <summary>
        /// This is the amount of time it will take for the player to get withdrawal symptoms.
        /// </summary>
        public virtual float TimeForWithdrawal { get; set; }

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

        public ConsumptionStage CalculateConsumptionStage(string itemName, Player player)
        {
            DrugStatistics drugStats = RecreationalHazards.Instance.Api.PlayerStatistics[player.Id]
                .FirstOrDefault(x => x.DrugType == itemName);

            if (drugStats.DrugsCurrentlyOn > MaximumUse)
                return ConsumptionStage.Overdose;

            if (drugStats.DrugsTaken >= AmountForAddiction)
                return ConsumptionStage.Withdrawal;

            if (drugStats.DrugsTaken < 1)
                return ConsumptionStage.Standard;

            return ConsumptionStage.FirstTime;
        }

        public IEnumerator<float> AddictionHandler(string itemName, Player player)
        {
            while (true)
            {
                DrugStatistics drugStats = RecreationalHazards.Instance.Api.PlayerStatistics[player.Id]
                    .FirstOrDefault(x => x.DrugType == itemName);

                float secondsSinceDrug = (float)(DateTime.UtcNow - drugStats.LastTaken).TotalSeconds;

                if (secondsSinceDrug > TimeForWithdrawal)
                {
                    StartEffects(ConsumptionStage.Withdrawal, player);
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
