using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceScheduler.Slots
{
    public class Rink
    {
        public string BuildingName { get; private set; }
        public string RinkName { get; private set; }

        public Rink(string buildingName, string rinkName)
        {
            BuildingName = buildingName;
            RinkName = rinkName;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", BuildingName, RinkName);
        }
    }
}
