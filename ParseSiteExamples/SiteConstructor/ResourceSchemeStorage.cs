using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseKit.Data
{
    class ResourceStat
    {
        public string className;
        public int resourceWeight;
        public List<ActionStat> actionsStat;

        public ResourceStat(string className, int resourceWeight, List<ActionStat> actionsStat)
        {
            this.actionsStat = actionsStat;
            this.className = className;
            this.resourceWeight = resourceWeight;
        }
    }

    class ActionStat
    {
        public string name;
        public int weight;
        public int calledCount;

        public ActionStat(string name, int weight, int calledCount)
        {
            this.name = name;
            this.weight = weight;
            this.calledCount = calledCount;
        }
    }
}
