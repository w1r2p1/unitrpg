﻿using System.Collections.Generic;
using System.Linq;
using Models.Fighting.Effects;

namespace Models.Fighting.Skills {
    public class Kinesis : AbstractSkillStrategy {
        public Kinesis() : base(SkillType.Kinesis, true, false) {
        }

        protected override ICombatBuffProvider GetBuffProvider(ICombatant attacker) {
            return new NullBuffProvider();
        }

        protected override SkillHit ComputeResult(ICombatant attacker, ICombatant defender, IRandomizer randomizer) {
            var myKinesis = attacker.GetAttribute(Attribute.AttributeType.Special).Value;
            var hit = new List<IEffect> {new Damage(myKinesis)};
            return new SkillHit(hit);
        }
    }
}