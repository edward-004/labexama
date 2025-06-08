using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using labexam.ut;

namespace labexam.ch.Classes
{
    /// <summary>
    /// High variance attacker
    /// </summary>
    internal class Trickster(string name, int playerIndex) : LeChoisi(name, atk: 100, maxHP: 1032, playerIndex)
    {
        public override string Title => $"{Name} the Trickster";

        public override Damage Attack()
        {
            int roll = Rand.Range(0, 99);

            int dmg;

            if (roll < 20)
                dmg = 0;
            else if (roll < 60)
                dmg = Rand.Range((int)(ATK * 0.5f), (int)(ATK * 1.1f)); // Normal
            else
                dmg = Rand.Range((int)(ATK * 1.5f), (int)(ATK * 2.5f)); // Big

            return new Damage() { amount = dmg, note = dmg == 0 ? "Miss!" : (roll >= 80 ? "Sneak Attack!" : null) };
        }
    }

    /// <summary>
    /// Low Attack, 50% crit chance
    /// </summary>
    /// <remarks>
    /// Crit = 250% multiplier
    /// </remarks>
    internal class Strategist(string name, int playerInder) : LeChoisi(name, atk: 66, maxHP: 912, playerInder)
    {
        public override string Title => $"Strategist {Name}";

        public override Damage Attack()
        {
            // 90% - 110%
            int baseDmg = Rand.Range((int)(ATK * 0.9f), (int)(ATK * 1.1f));

            int roll = Rand.Range(1, 100);

            // 50%
            bool crit = roll <= 50;
            int dmg = crit ? (int)(baseDmg * 2.5f) : baseDmg;

            return new Damage { amount = dmg, note = crit ? "CRIT!" : null };
        }
    }

    /// <summary>
    /// Hits weak, High resistance
    /// </summary>
    /// <remarks>
    /// Lower HP = higher damage reduction, up to 75%
    /// </remarks>
    internal class Tanker(string name, int playerIndex) : LeChoisi(name, atk: 50, maxHP: 1503, playerIndex)
    {
        public override string Title => $"Tanker {Name}";

        public override Damage Attack()
        {
            // 80% - 110%
            int dmg = Rand.Range((int)(ATK * 0.8f), (int)(ATK * 1.1f));

            return new Damage() { amount = dmg };
        }

        public override void TakeDamage(Damage damage)
        {
            damage.from = HP;

            float hpRatio = 1f - (HP / (float)MaxHP); // 0 is full, goes higher up to 1 when HP is closer to 0
            float reduction = 1f - hpRatio * .75f; // up to 75%

            int reduced = (int)(damage.amount * reduction);
            HP = int.Max(0, HP - reduced);
            damage.amount = reduced;

            damage.to = HP;

            UI.PlayerUI[PlayerIndex].UpdateHPBar();

            UI.Battle.Log($"{Title} took {damage.amount} reduced damage! (HP: {damage.from} -> {damage.to})");

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
    }

    /// <summary>
    /// Hits harder the lower their HP
    /// </summary>
    /// <remarks>
    /// Lower HP = higher damage, up to 175%
    /// </remarks>
    internal class Berserker(string name, int playerIndex) : LeChoisi(name, atk: 90, maxHP: 1001, playerIndex)
    {
        public override string Title => $"{Name} the Berserker";

        public override Damage Attack()
        {
            float hpRatio = 1f - (HP / (float)MaxHP); // 0 is full, goes higher up to 1 when HP is closer to 0
            float boost = 1f + hpRatio * .75f; // up to 175%

            int dmg = (int)(ATK * boost);
            return new Damage { amount = dmg, note = hpRatio > 0.5f ? "Rage Strike!" : null };
        }
    }
}
