using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public float ShopScore { get; set; }
        public int ProgressBattlePass { get; set; }
        public List<ShopItem> Items { get; set; }
        public List<ShopItem> BattlePassItems { get; set; }
    }
}
