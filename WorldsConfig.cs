using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCUnlocked.Config;

namespace VRCMinorUI
{
    internal class WorldsConfig
    {
        [ConfigItem("Recent Worlds On Top", false)]
        public bool? RecentWorldsAtTop { get; set; }
        [ConfigItem("Favourite Worlds On Top", false)]
        public bool? FavouriteWorldsAtTop { get; set; }
    }
}
