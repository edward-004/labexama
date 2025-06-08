using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using labexam.ch;

namespace labexam.ut
{
    internal static class Turn
    {
        public static async Task Perform(LeChoisi attacker, LeChoisi defender)
        {
            UI.Battle.Log($"{attacker.Title} takes the turn");
            await Task.Delay(500);

            UI.Battle.Log($"{attacker.Title} attacks {defender.Title}");
            await Task.Delay(1000);

            var damage = attacker.Attack();
            defender.TakeDamage(damage);

            await Task.Delay(100);
        }
    }
}
