﻿using Contexts.Battle.Models;
using Contexts.Battle.Signals;
using Contexts.BattlePrep.Signals;
using strange.extensions.command.impl;
using UnityEditor;

namespace Contexts.Battle.Commands {
    public class BackCommand : Command {
        [Inject]
        public BattleViewState Model { get; set; }

        [Inject]
        public UnitDeselectedSignal UnitDeselectedSignal { get; set; }

        [Inject]
        public UnitSelectedSignal UnitSelectedSignal { get; set; }

        [Inject]
        public ExitContextMenuSignal ExitContextMenuSignal { get; set; }

        [Inject]
        public ShowBattlePrepSignal ShowBattlePrepSignal { get; set; }

        public override void Execute() {
            var state = Model.State;

            switch (state) {
                case BattleUIState.SelectingAction:
                    Model.SelectedCombatant = null;
                    UnitDeselectedSignal.Dispatch();
                    Model.State = BattleUIState.SelectingUnit;
                    break;
                case BattleUIState.SelectingFightAction:
                    Model.State = BattleUIState.SelectingAction;
                    break;
                case BattleUIState.ForecastingCombat:
                    Model.SelectedAttackTarget = null;
                    Model.State = BattleUIState.SelectingAttackTarget;
                    break;
                case BattleUIState.Surveying:
                    Model.State = BattleUIState.Preparations;
                    ShowBattlePrepSignal.Dispatch();
                    break;
                case BattleUIState.SelectingAttackTarget:
                case BattleUIState.SelectingMoveLocation:
                    var combatant = Model.SelectedCombatant;
                    var dimensions = Model.Dimensions;
                    var worldPosition = dimensions.GetWorldPositionForGridPosition(combatant.Position);
                    UnitSelectedSignal.Dispatch(worldPosition);
                    Model.State = BattleUIState.SelectingAction;
                    break;
            }
        }
    }
}
