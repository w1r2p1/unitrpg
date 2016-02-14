﻿using Models.Fighting;
using Models.Fighting.Characters;
using Models.Fighting.Skills;
using NUnit.Framework;
using UnityEngine;

namespace Tests {
    [TestFixture]
    public class FightingTest {
        private class ConstantRandomizer : IRandomizer {
            private int _constant;

            public ConstantRandomizer(int constant) {
                _constant = constant;
            }

            public int GetNextRandom() {
                return _constant;
            }
        }

        [Test]
        public void TestBasicFight() {
            var attackerCharacter = new CharacterBuilder()
                .Attributes(new AttributesBuilder()
                    .Health(15)
                    .Skill(12)
                    .Defense(2)
                    .Special(0)
                    .Speed(13)
                    .Strength(7)
                .Build())
                .Weapons("Shortsword")
                .Build();

            var defenderCharacter = new CharacterBuilder()
                .Attributes(new AttributesBuilder()
                    .Health(15)
                    .Skill(12)
                    .Defense(2)
                    .Special(0)
                    .Speed(13)
                    .Strength(3)
                .Build())
                .Weapons("Shortsword")
                .Build();

            var attacker = new BaseCombatant(attackerCharacter);
            var defender = new BaseCombatant(defenderCharacter);

            attacker.Position = new Vector2(0, 1);
            defender.Position = new Vector2(0, 0);

            var simulator = new FightSimulator(new ConstantRandomizer(100));

            var result = simulator.Simulate(attacker, defender, new MeleeAttack());
            var init = result.Initial;
            Assert.NotNull(init);

            var dmg = init.GetDefenderDamage();
            Assert.AreEqual(5, dmg);

            var counter = result.Counter;
            Assert.NotNull(counter);

            var counterDmg = counter.GetDefenderDamage();
            Assert.AreEqual(1, counterDmg);
        }
    }
}