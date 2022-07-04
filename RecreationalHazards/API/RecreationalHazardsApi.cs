using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RecreationalHazards.API
{
    public class RecreationalHazardsApi
    {
        public Dictionary<string, Dictionary<int, int>> TotalDrugsUsed;
        public Dictionary<string, Dictionary<int, int>> DrugsCurrentlyUsing;

        public RecreationalHazardsApi()
        {
            TotalDrugsUsed = new Dictionary<string, Dictionary<int, int>>();
            DrugsCurrentlyUsing = new Dictionary<string, Dictionary<int, int>>();

            Assembly exiledAssembly = Loader.GetPlugin(nameof(RecreationalHazards)).Assembly;
            Type[] itemTypes = exiledAssembly.GetTypes()
                .Where(x => x.Namespace == nameof(RecreationalHazards) + ".API.Items")
                .ToArray();

            foreach (Type itemType in itemTypes)
            {
                TotalDrugsUsed.Add(itemType.Name, new Dictionary<int, int>());
                DrugsCurrentlyUsing.Add(itemType.Name, new Dictionary<int, int>());
            }
        }
    }
}
