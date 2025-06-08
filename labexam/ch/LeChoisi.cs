using System.Drawing;
using labexam.ctrl;
using labexam.ut;

namespace labexam.ch
{
    /// <summary>
    /// Base class
    /// </summary>
    internal abstract class LeChoisi(string name, int atk, int maxHP, int playerIndex)
    {
        public string Name { get; } = name;

        /// <summary>
        /// Current hit points
        /// </summary>
        public int HP { get; protected set; } = maxHP;

        public int MaxHP { get; protected set; } = maxHP;

        public int ATK { get; protected set; } = atk;

        /// <summary>
        /// Player 1, 2, which is which?
        /// </summary>
        /// 
        /// init setter, assigned once during contruction and immutable afterwards
        public int PlayerIndex { get; init; } = int.Clamp(playerIndex, 1, 2);

        /// <summary>
        /// A shape representing the player's character
        /// </summary>
        public Shape Representation { get; set; }

        public bool Dead { get { return HP <= 0; } }



        /// <summary>
        /// Take damage
        /// </summary>
        /// Modified as virtual. Might a subclass want a unique reaction logic, this can be optionally overriden
        public virtual void TakeDamage(Damage damage)
        {
            damage.from = HP;
            HP = int.Max(0, HP - damage.amount);
            damage.to = HP;

            UI.PlayerUI[playerIndex].UpdateHPBar();

            UI.Battle.Log($"{Title} took {damage.amount} damage! (HP: {damage.from} -> {damage.to})");


            if (damage.amount > 0)
            {
                if (HP <= 0)
                    UI.TweenColor(Representation, Color.Red, UI.form.BackColor, 1f, EaseFunction.QuadOut);
                else
                    UI.TweenColor(Representation, Color.Red, Representation.Color, .5f, EaseFunction.QuadOut);

                UI.Shake(Representation, .5f, 5, true, EaseFunction.QuadOut);
            }
            UI.Battle.HurtText(this, damage);

            if (!string.IsNullOrEmpty(damage.note))
                UI.Battle.Log(damage.note);
        }

        public abstract Damage Attack();

        /// <summary>
        /// Character Title
        /// </summary>
        public virtual string Title { get; protected set; } = $"{name}";
    }
}
