using RecreationalHazards.API.Enums;
using System;

namespace RecreationalHazards.API.Entities
{
    public class DrugStatistics
    {
        public string DrugType { get; set; }
        public int DrugsCurrentlyOn { get; set; }
        public int DrugsTaken { get; set; }
        public DateTime LastTaken { get; set; }
        public ConsumptionStage ConsumptionStage { get; set; }
    }
}
