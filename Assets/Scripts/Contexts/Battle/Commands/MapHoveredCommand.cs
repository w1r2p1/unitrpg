﻿using Contexts.Battle.Models;
using Contexts.Battle.Signals;
using strange.extensions.command.impl;
using UnityEngine;

namespace Contexts.Battle.Commands {
    public class MapHoveredCommand : Command {
        [Inject]
        public BattleViewState Model { get; set; }

        [Inject]
        public GridPosition Position { get; set; }

        [Inject]
        public HoveredTileChangeSignal HoveredTileChangeSignal { get; set; }

        [Inject]
        public HoverTileDisableSignal HoverTileDisableSignal { get; set; }

        public override void Execute() {
            var map = Model.Map;
            if (map == null) {
                return;
            }

            if (map.IsBlockedByEnvironment(Position.GridCoordinates)) {
                HoverTileDisableSignal.Dispatch();
            } else {
                Model.HoveredTile = Position.GridCoordinates;
                HoveredTileChangeSignal.Dispatch(Position.WorldCoordinates);
            }
        }
    }
}
