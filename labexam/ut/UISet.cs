using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using labexam.ch;
using labexam.ctrl;
using static labexam.ut.UI;

namespace labexam.ut
{
    internal class UISet
    {
        public LeChoisi Player { get; }
        public Label Title { get; }
        public ProgressBar HpBar { get; }
        public Label HpText { get; }

        /// <summary>
        /// Construct a new UISet (there's only ever going to be 2)
        /// </summary>
        /// <remarks>
        /// Includes: Title Label and HPBar
        /// </remarks>
        public UISet(LeChoisi player, bool autoRegister = true)
        {
            if (UI.PlayerUI.ContainsKey(player.PlayerIndex))
                return;

            int x = player.PlayerIndex == 1 ? Layout.P1Point : Layout.P2Point;
            int y = Layout.BaseY;

            string tag_prefix = $"PLAYER{player.PlayerIndex}_";

            var title = CreateLabel(player.Title, 2, new(x, y), tag_prefix + "TITLE");

            var hpBar = new ProgressBar
            {
                Location = new(x, y + 30),

                Minimum = 0,
                Maximum = player.MaxHP,
                Value = player.HP,

                Tag = tag_prefix + "HPBAR"
            };

            form.Controls.Add(hpBar);
            Center(hpBar);

            var hpText = CreateLabel($"{player.HP}/{player.MaxHP}", 2, new(x, y + 60), tag_prefix + "HPTEXT");


            Player = player;
            Title = title;
            HpBar = hpBar;
            HpText = hpText;

            if (autoRegister)
                UI.PlayerUI[player.PlayerIndex] = this;
        }

        /// <summary>
        /// As well as the text
        /// </summary>
        public void UpdateHPBar()
        {
            HpBar.Value = Player.HP;
            UpdateLabel(HpText, $"{Player.HP}/{Player.MaxHP}");
        }
    }

}
