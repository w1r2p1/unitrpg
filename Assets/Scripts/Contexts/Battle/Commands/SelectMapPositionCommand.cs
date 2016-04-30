﻿using Contexts.Battle.Models;
using Contexts.Battle.Signals;
using Contexts.Battle.Views;
using Models.Fighting.Characters;
using strange.extensions.command.impl;
using UnityEngine;

namespace Contexts.Battle.Commands {
    public class SelectMapPositionCommand : Command {
        [Inject]
        public BattleViewState BattleViewModel { get; set; }

        [Inject]
        public Vector2 Position { get; set; }

        [Inject]
        public UnitSelectedSignal UnitSelectedSignal { get; set; }

        public override void Execute() {
            var state = BattleViewModel.State;
            var combatant = BattleViewModel.Map.GetAtPosition(Position);
            if (state == BattleUIState.SelectingUnit) {
                if (combatant != null && combatant.Army == ArmyType.Friendly) {
                    // Mark the unit at Position as selected, change the battle state.
                    BattleViewModel.SelectedCombatant = combatant;

                    var mapView = GameObject.FindObjectOfType<MapView>();
                    var worldPosition = mapView.GetWorldPositionForGridPosition(combatant.Position);
                    UnitSelectedSignal.Dispatch(worldPosition);
                }
            } else if (state == BattleUIState.SelectingMoveLocation) {
                // Make the unit move
            } else if (state == BattleUIState.SelectingAttackTarget) {
                // Forecast the fight against this unit
            }
        }
    }
}
