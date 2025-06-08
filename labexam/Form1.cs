using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using labexam.ch;
using labexam.ch.Classes;
using labexam.ctrl;
using labexam.ut;



namespace labexam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // UI utility class depends on this, don't change
            Name = nameof(Form1);

            // We don't want to handle ui adaptation, so disable resizing altogether
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;

            // Fixed window size
            Size = new(1280, 720);

            // Window backcolor
            BackColor = default;

            Click += (s, e) => UI.Unfocus();

            Load += (s, e) => { Preparation(); UI.Unfocus(); };
        }




        LeChoisi player1 = null;
        LeChoisi player2 = null;

        void Preparation()
        {
            const string tag = "sc_";
            const string btn_tag = tag + "b";



            // List of the abstract Le Choisi types for the combobox selections
            var ChosenCandidates = new Dictionary<string, Func<string, int, LeChoisi>>()
            {
                { nameof(Trickster), (name, playerindex) => new ch.Classes.Trickster(name, playerindex) },
                { nameof(Strategist), (name, playerindex) => new ch.Classes.Strategist(name, playerindex) },
                { nameof(Tanker), (name, playerindex) => new ch.Classes.Tanker(name, playerindex) },
                { nameof(Berserker), (name, playerindex) => new ch.Classes.Berserker(name, playerindex) },
            };



            UI.Create.PlayerSelection(1, tag, out var player1NameInput, out var player1Selection);
            UI.Create.PlayerSelection(2, tag, out var player2NameInput, out var player2Selection);

            var items = ChosenCandidates.Keys.ToArray();
            player1Selection.Items.AddRange(items);
            player2Selection.Items.AddRange(items);

            var StartBattleBtn = new Button
            {
                Location = UI.CenterPoint,
                Text = "Start Battle",
                Size = new(100, 33),
                Tag = btn_tag
            };

            Controls.Add(StartBattleBtn);
            UI.Center(StartBattleBtn);



            Label err = null;

            StartBattleBtn.Click += (s, e) =>
            {
                if (    // Just null checks here
                        !string.IsNullOrEmpty(player1NameInput.Text)
                        && !string.IsNullOrEmpty(player2NameInput.Text)
                        && player1Selection.SelectedItem is string selected1
                        && player2Selection.SelectedItem is string selected2
                    )
                {
                    // We assign the fields
                    if (ChosenCandidates.TryGetValue(selected1, out var create1))
                        player1 = create1(player1NameInput.Text, 1);

                    if (ChosenCandidates.TryGetValue(selected2, out var create2))
                        player2 = create2(player2NameInput.Text, 2);

                    // To the next
                    Transition(tag, btn_tag);

                    // Disable the button
                    StartBattleBtn.Enabled = false;
                }
                else
                    if (err == null)
                {
                    err = UI.CreateLabel("Input fields cannot be left empty", 2, new(UI.CenterPoint.X, UI.CenterPoint.Y + 40), null, Color.Red);
                    UI.TweenColor(err, Color.Red, err.BackColor, 1, EaseFunction.QuadIn, () => { Controls.Remove(err); err = null; });
                }
            };
        }


        /// <summary>
        /// Animated exit and entry
        /// </summary>
        void Transition(string tag, string btnTag)
        {
            int p1Point = UI.csw() / 5;
            int p2Point = UI.csw() * 4 / 5;
            int baseY = UI.csh() / 4;

            // Animate (exit) and dispose
            static void ExitWithTween(Control c) =>
                UI.Tween(c, c.Location, new(c.Location.X, -c.Height), 0.1f, EaseFunction.ExpoIn, c.Dispose);

            // Exit
            foreach (var control in UI.FindControlsByTag(tag))
                ExitWithTween(control);

            if (UI.FindControlByTag<Button>(btnTag) is { } btn)
                UI.Tween(btn, btn.Location, new(btn.Location.X, UI.csh()), 0.1f, EaseFunction.ExpoIn, btn.Dispose);



            // Create and tween (entry)
            static void TweenPlayerBattle(LeChoisi player)
            {
                int ind = player.PlayerIndex;

                _ = new UISet(player);
                var uiset = UI.PlayerUI[player.PlayerIndex];


                UI.Tween
                    (
                    uiset.Title,
                    new(ind == 1 ? 0 - uiset.Title.Width : UI.csw() + uiset.Title.Width, uiset.Title.Location.Y),
                    uiset.Title.Location,
                    1f,
                    EaseFunction.ExpoOut
                    );

                UI.Tween
                    (
                    uiset.HpBar,
                    new(ind == 1 ? 0 - uiset.HpBar.Width : UI.csw() + uiset.HpBar.Width, uiset.HpBar.Location.Y),
                    uiset.HpBar.Location,
                    1f,
                    EaseFunction.ExpoOut
                    );

                UI.Tween
                    (
                    uiset.HpText,
                    new(ind == 1 ? 0 - uiset.HpText.Width : UI.csw() + uiset.HpText.Width, uiset.HpText.Location.Y),
                    uiset.HpText.Location,
                    1f,
                    EaseFunction.ExpoOut
                    );
            }

            // Entry
            TweenPlayerBattle(player1);
            TweenPlayerBattle(player2);

            UI.WaitFor(300, TransitionPT2);
        }

        async void TransitionPT2()
        {
            // Create log box
            UI.Battle.CreateLogBox();
            var bl = UI.Battle.LogBox;

            // Delay it a bit
            await Task.Delay(100);
            UI.Tween(bl, new(bl.Location.X, UI.csh()), bl.Location, .5f, EaseFunction.ExpoOut);
            await Task.Delay(10);
            bl.Visible = true;

            // Create and tween shapes (entry)
            static void TweenPlayerRepresentation(LeChoisi player)
            {
                UI.Create.PlayerRepresentation(player);

                var r = player.Representation;
                r.Visible = false;
                UI.WaitFor(10, () => r.Visible = true);

                int ind = player.PlayerIndex;

                UI.Tween(r,
                    new(ind == 1 ? 0 - r.Width : UI.csw() + r.Width, r.Location.Y),
                    r.Location,
                    1f,
                    EaseFunction.ExpoOut
                    );
            }

            TweenPlayerRepresentation(player1);
            TweenPlayerRepresentation(player2);

            await Task.Delay(2000);
            StartBattle();
        }



        /// <summary>
        /// Start!!!!!!!!!!!
        /// </summary>
        async void StartBattle()
        {
            // true = p1, false = p2
            var firstMover = Rand.Switch();

            LeChoisi attacker, defender;

            attacker = firstMover ? player1 : player2;
            defender = firstMover ? player2 : player1;

            UI.Battle.Log($"{attacker.Title} goes first");
            await Task.Delay(1000);


            UI.Battle.Log("Battle Start!");
            await Task.Delay(1000);

            int turn = 1;
            while (!player1.Dead && !player2.Dead)
            {
                UI.Battle.Log("");
                UI.Battle.Log($"Turn {turn}");

                await Turn.Perform(attacker, defender);
                if (defender.Dead) break;

                turn++;
                await Task.Delay(1000);

                (attacker, defender) = (defender, attacker);
            }

            await Task.Delay(1000);
            UI.Battle.Log(player1.Dead ? $"{player2.Name} wins!" : $"{player1.Name} wins!");
        }
    }
}
