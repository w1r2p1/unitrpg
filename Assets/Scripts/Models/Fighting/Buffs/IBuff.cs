﻿using System.Collections.Generic;
using Models.Combat;
using Models.Fighting.Skills;

namespace Models.Fighting.Buffs {
    public interface IBuff {
        Unit Host { get; set; }

        /// <summary>
        /// Determine if the Host is still eligible for this buff. For example, the Leadership buff only applies if the
        /// Host is within X squares of their Leader. 
        /// </summary>
        /// <param name="battle">The world state.</param>
        /// <returns>True if the Host is eligible, False otherwise</returns>
        bool CanApply(IOldBattle battle);

        bool AppliesToSkill(SkillType skillType);

        /// <summary>
        /// Modify an effect on a unit. For example, a buff that decreases incoming damage by 50% would return a new Effect with
        /// its damage halved.
        /// </summary>
        /// <param name="effect">The effect to modify.</param>
        /// <returns>The modified effect.</returns>
        IEffect Modify(IEffect effect);

        IDictionary<StatType, StatMod> StatMods { get; }
        
        Stat Modify(Stat stat);

        string Name { get; }

        Attribute Apply(Attribute attribute);

        void OnRemove();
    }
}