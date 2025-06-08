using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using labexam.ch;
using labexam.ctrl;


namespace labexam.ut
{
    /// <summary>
    /// Utility class for <see cref="Form1"/>
    /// </summary>
    internal static class UI
    {
        /// <summary>
        /// Ui layour anchors
        /// </summary>
        public static class Layout
        {
            public static int P1Point => csw() / 5;
            public static int P2Point => csw() * 4 / 5;

            public static int BaseY => csh() / 4;
        }

        /// <summary>
        /// To avoid cluttering <see cref="Form1"/>
        /// </summary>
        public static class Create
        {
            /// <summary>
            /// Preparation: Name Input and Character Selection
            /// </summary>
            public static void PlayerSelection(int playerIndex, string tag, out TextBox nameInput, out ComboBox selectionBox)
            {
                // Player index should not go below or beyond
                playerIndex = int.Clamp(playerIndex, 1, 2);

                // Position the Labels accordingly
                int x = playerIndex == 1 ? Layout.P1Point : Layout.P2Point;
                int y = Layout.BaseY;



                var nameLabel = CreateLabel($"Player {playerIndex} Name:", 2, new(x, y - 20), tag);

                nameInput = new TextBox
                {
                    Location = new(x, y),
                    TextAlign = HorizontalAlignment.Center,
                    BorderStyle = BorderStyle.None,
                    MaxLength = 24,
                    Tag = tag
                };

                form.Controls.Add(nameInput);
                Center(nameInput);



                var charLabel = CreateLabel($"Player {playerIndex} Character:", 2, new(x, y + 40), tag);

                selectionBox = new ComboBox
                {
                    Location = new(x, y + 60),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    FlatStyle = FlatStyle.Popup,
                    Tag = tag
                };

                form.Controls.Add(selectionBox);
                Center(selectionBox);
            }

            /// <summary>
            /// Create a Random shape with a random color
            /// </summary>
            public static void PlayerRepresentation(LeChoisi player)
            {
                int shape = Rand.Range(1, 3);

                int x = player.PlayerIndex == 1 ? Layout.P1Point : Layout.P2Point;
                int y = CenterPoint.Y;

                Shape sh = shape switch
                {
                    1 => new Circle(),
                    2 => new Quad(),
                    3 => new Triangle(),
                    _ => new Circle()
                };

                sh.Size = new(150, 150);

                // Random colors
                sh.Color = Color.FromArgb(Rand.Range(1, 255), Rand.Range(1, 255), Rand.Range(1, 255));

                sh.Location = new(x, y);
                sh.Tag = $"PLAYER{player.PlayerIndex}_SHAPE";

                form.Controls.Add(sh);
                Center(sh);

                player.Representation = sh;
            }
        }


        /// <summary>
        /// Related to the battle
        /// </summary>
        public static class Battle
        {
            static ListBox logs;

            /// <summary>
            /// LogBox getter
            /// </summary>
            public static ListBox LogBox { get { return logs; } }

            /// <summary>
            /// Create the log box
            /// </summary>
            public static void CreateLogBox()
            {
                if (logs != null) return;

                var BattleLogs = new ListBox
                {
                    Tag = "battle_log",

                    Width = csw() / 3,
                    Height = csw() / 4,
                    Location = CenterPoint,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    Font = new Font("Consolas", 10),
                    HorizontalScrollbar = true,
                    BorderStyle = BorderStyle.None,

                    Visible = false
                };

                form.Controls.Add(BattleLogs);
                Center(BattleLogs);

                logs = BattleLogs;
            }

            /// <summary>
            /// Log something to the Log Box
            /// </summary>
            public static void Log(string msg)
            {
                logs.Items.Add(msg);
                logs.TopIndex = logs.Items.Count - 1;
            }


            /// <summary>
            /// Display an animated HurtText
            /// </summary>
            public static void HurtText(LeChoisi player, Damage dmg)
            {
                var origin = player.Representation;

                var label = CreateLabel($"-{dmg.amount}", 2, origin.Location, default, dmg.amount > 0 ? Color.Red : Color.Gray, default, FontStyle.Bold);
                label.BringToFront();

                int xx = player.PlayerIndex == 1 ? origin.Location.X + 20 : origin.Location.X + origin.Width - 20;

                label.Location = new Point(xx, origin.Location.Y + 20);
                Center(label);

                // Random initial velocity
                float vx = (player.PlayerIndex == 1 ? Rand.RangeF(-5f, -1f) : Rand.RangeF(5f, 1f)) * Rand.RangeF(80f, 120f);
                float vy = -Rand.RangeF(120f, 160f);

                float gravity = 800;

                float duration = 1f;
                int totalFrames = (int)(duration * 100);
                int frame = 0;

                PointF position = label.Location;

                Color startCol = label.ForeColor;
                Color endCol = label.BackColor;

                Update100(timer =>
                {
                    // Δtime = 10ms (Update100()'s update interval)
                    float dt = 0.01f;
                    frame++;
                    float t = frame * dt;

                    // Motion
                    position.X += vx * dt;
                    position.Y += vy * dt;
                    vy += gravity * dt;

                    label.Location = new((int)position.X, (int)position.Y);

                    // Color fade
                    float c = float.Min(frame / (float)totalFrames, 1f);
                    float eased = EaseFunction.ExpoIn(c);

                    int r = (int)float.Lerp(startCol.R, endCol.R, eased);
                    int g = (int)float.Lerp(startCol.G, endCol.G, eased);
                    int b = (int)float.Lerp(startCol.B, endCol.B, eased);
                    int a = (int)float.Lerp(startCol.A, endCol.A, eased);

                    label.ForeColor = Color.FromArgb(a, r, g, b);

                    if (frame >= totalFrames)
                    {
                        label.Dispose();
                        StopUpdate(timer);
                    }
                });
            }
        }



        /// <summary>
        /// Player UI Storage
        /// </summary>
        /// <remarks>
        /// The <see cref="UISet"/> constructor automatically registers constructed UIs here
        /// </remarks>
        public static Dictionary<int, UISet> PlayerUI = new();



        /// <summary>
        /// Default text color for <see cref="Label"/>s created through <c><see cref="CreateLabel(string, float, Point, string, Color, Color, FontStyle)"/></c>
        /// </summary>
        static readonly Color def_textColor = Color.Black;



        /// <summary>
        /// Reference to <see cref="Form1"/>
        /// </summary>
        public static readonly Form form = Application.OpenForms[nameof(Form1)];

        /// <summary>
        /// Keeps track of <see cref="Label"/>s created through <c><see cref="CreateLabel(string, float, Point, string, Color, Color, FontStyle)"/></c>
        /// </summary>
        static readonly List<Label> labels = [];

        /// <summary>
        /// Find a <see cref="Label"/> by <paramref name="tag"/>
        /// </summary>
        /// <returns>
        /// The Label
        /// </returns>
        public static Label FindLabel(string tag) => labels.FirstOrDefault(l => l.Tag?.ToString() == tag);


        /// <summary>
        /// Shorthand writing for ClientSize.Width
        /// </summary>
        public static int csw()
            => form.ClientSize.Width;
        /// <summary>
        /// Shorthand writing for ClientSize.Height
        /// </summary>
        public static int csh()
            => form.ClientSize.Height;

        /// <summary>
        /// Client center point
        /// </summary>
        public static Point CenterPoint
            => new(csw() / 2, csh() / 2);



        public static void Center(Control l)
        {
            int x = l.Location.X - l.Width / 2;
            int y = l.Location.Y - l.Height / 2;

            l.Location = new(x, y);
        }



        /// <summary>
        /// Create a <see cref="Label"/>
        /// </summary>
        /// <remarks>
        /// <paramref name="tag"/>: Identifier <br></br>
        /// <paramref name="textCol"/>'s default is <c><see cref="def_textColor"/></c>
        /// </remarks>
        public static Label CreateLabel(string text, float fontSize, Point location, string tag = null, Color textCol = default, Color backCol = default, FontStyle fontStyle = default)
        {
            Label l = new()
            {
                Text = text,
                Font = new Font("Arial", fontSize * 5, fontStyle),
                Location = location,
                Tag = tag,

                AutoSize = true,
                ForeColor = textCol == default ? def_textColor : textCol,
                // default is form.Backcolor
                BackColor = backCol,
            };

            form.Controls.Add(l);
            labels.Add(l);

            Center(l);

            return l;
        }

        /// <summary>
        /// Update a <see cref="Label"/>'s text
        /// </summary>
        public static void UpdateLabel(Label label, string new_text)
        {
            int x = label.Location.X + label.Width / 2;
            int y = label.Location.Y + label.Height / 2;

            var loc = new Point(x, y);


            label.Text = new_text;
            label.Location = loc;

            Center(label);
        }

        /// <summary>
        /// Destroy a <see cref="Label"/>
        /// </summary>
        public static void DestroyLabel(Label label)
        {
            form.Controls.Remove(label);
            label.Dispose();
        }





        /// <summary>
        /// Updates <paramref name="callback"/> every 10ms
        /// </summary>
        /// <remarks>
        /// Use lambda functions:
        /// <code>
        /// <see cref="UI"/>.Update120(timer => 
        /// {
        ///     ...
        /// });
        /// </code>
        /// To stop the Timer, call <c><see cref="UI.StopUpdate(Timer)"/>;</c> inside the lambda itself
        /// </remarks>
        public static void Update100(Action<Timer> callback)
        {
            var timer = new Timer { Interval = 1000 / 100 };

            timer.Tick += (s, e) => callback(timer);
            timer.Start();
        }

        /// <summary>
        /// Stop <paramref name="timer"/>
        /// </summary>
        public static void StopUpdate(Timer timer)
        {
            timer?.Stop();
            timer?.Dispose();
        }

        /// <summary>
        /// Wait for x <paramref name="milliseconds"/> before invoking <paramref name="callback"/>
        /// </summary>
        public static void WaitFor(int milliseconds, Action callback)
        {
            var timer = new Timer { Interval = milliseconds };

            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                callback?.Invoke();
            };
            timer.Start();
        }






        /// <summary>
        /// Tween a <see cref="Control"/> <paramref name="from"/> origin point <paramref name="to"/> destination
        /// </summary>
        /// <remarks>
        /// <paramref name="easing"/>: See <see cref="EaseFunction"/> members
        /// </remarks>
        public static void Tween(Control ctrl, Point from, Point to, float duration, Func<float, float> easing, Action then = null)
        {
            int totalFrames = (int)(duration * 100f);
            int frame = 0;

            Update100(tm =>
            {
                float t = float.Min(frame / (float)totalFrames, 1f);
                float eased = easing?.Invoke(t) ?? t;

                int x = (int)float.Lerp(from.X, to.X, eased);
                int y = (int)float.Lerp(from.Y, to.Y, eased);

                ctrl.Location = new(x, y);

                frame++;
                if (frame > totalFrames)
                {
                    StopUpdate(tm);
                    then?.Invoke();
                }
            });
        }

        /// <summary>
        /// Apply a shaking animation to a <see cref="Control"/>
        /// </summary>
        public static void Shake(Control ctrl, float duration = .3f, int intensity = 5, bool damp = true, Func<float, float> dampEasing = null)
        {
            Point ogPos = ctrl.Location;
            int totalFrames = (int)(duration * 100f);
            int frame = 0;

            float angle = Rand.Single() * MathF.Tau;

            Update100(timer =>
            {
                if (frame % 5 == 0)
                    angle = Rand.Single() * MathF.Tau;

                float t = frame / (float)totalFrames;
                float damping = damp ? dampEasing(1 - t) : 1;

                float offsetX = MathF.Sin(frame * 0.5f) * intensity * MathF.Cos(angle) * damping;
                float offsetY = MathF.Sin(frame * 0.5f) * intensity * MathF.Sin(angle) * damping;

                ctrl.Location = new(ogPos.X + (int)offsetX, ogPos.Y + (int)offsetY);

                frame++;
                if (frame > totalFrames)
                {
                    ctrl.Location = ogPos;
                    StopUpdate(timer);
                }
            });
        }

        /// <summary>
        /// Tween Colors
        /// </summary>
        public static void TweenColor(Control ctrl, Color startCol, Color tartgetCol, float duration, Func<float, float> easing = null, Action then = null)
        {
            int totalFrames = (int)(duration * 100f);
            int frame = 0;

            Color start = startCol;
            Color end = tartgetCol;

            Update100(timer =>
            {
                float t = float.Min(frame / (float)totalFrames, 1f);
                float eased = easing?.Invoke(t) ?? t;

                int r = (int)float.Lerp(start.R, end.R, eased);
                int g = (int)float.Lerp(start.G, end.G, eased);
                int b = (int)float.Lerp(start.B, end.B, eased);
                int a = (int)float.Lerp(start.A, end.A, eased);

                if (ctrl is Shape shape)
                    shape.Color = Color.FromArgb(a, r, g, b);
                else
                    ctrl.ForeColor = Color.FromArgb(a, r, g, b);


                frame++;
                if (frame > totalFrames)
                {
                    StopUpdate(timer);
                    then?.Invoke();
                }
            });
        }





        /// <summary>
        /// Find a <see cref="Control"/> by tag
        /// </summary>
        public static T FindControlByTag<T>(string tag) where T : Control
            => form.Controls.Cast<Control>().FirstOrDefault(c => c.Tag as string == tag && c is T) as T;

        /// <summary>
        /// Find <see cref="Control"/>s by tag
        /// </summary>
        public static List<Control> FindControlsByTag(string tag)
            => form.Controls.Cast<Control>().Where(c => c.Tag as string == tag).ToList();





        static Button dummy = null;
        /// <summary>
        /// Shift focus to an invisible dummy control to effectively unfocus other controls
        /// </summary>
        public static void Unfocus()
        {
            if (dummy == null)
            {
                dummy = new Button
                {
                    TabStop = false,
                    // Position it way offscreen
                    Location = new(-10000, -10000),
                };
                form.Controls.Add(dummy);
            }

            dummy.Focus();
        }
    }
}
