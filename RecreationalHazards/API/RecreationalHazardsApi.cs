using MEC;
using RecreationalHazards.API.Entities;
using System.Collections.Generic;

namespace RecreationalHazards.API
{
    public class RecreationalHazardsApi
    {
        public Dictionary<int, List<DrugStatistics>> PlayerStatistics;
        public Dictionary<string, List<CoroutineHandle>> Coroutines;

        public RecreationalHazardsApi()
        {
            PlayerStatistics = new Dictionary<int, List<DrugStatistics>>();
            Coroutines = new Dictionary<string, List<CoroutineHandle>>();
        }
    }
}
