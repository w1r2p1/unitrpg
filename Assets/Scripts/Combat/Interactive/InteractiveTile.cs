﻿using System.Collections;
using Combat.Interactive.Rules;
using UnityEngine;

namespace Combat.Interactive {
    [ExecuteInEditMode]
    [RequireComponent(typeof(IScriptedEvent))]
    public class InteractiveTile : MonoBehaviour {
        public Vector2 GridPosition;
        public string Id;

        private ITileInteractivityRule _rule;
        private IScriptedEvent _event;
        private MapGrid _grid;

        void Awake() {
            _rule = GetComponent<ITileInteractivityRule>();
            if (_rule == null) {
                _rule = new DummyRule();
            }

            _event = GetComponent<IScriptedEvent>();
            _grid = CombatObjects.GetMap();
        }

        bool CanBeUsed() {
            return _rule.CanBeUsed();
        }

        void Update() {
#if UNITY_EDITOR
            if (_grid != null) {
                SnapToGrid();
            }
#endif
        }

        void SnapToGrid() {
            var mapOffset = _grid.transform.position;
            var tileSize = _grid.tileSizeInPixels;

            transform.position = mapOffset + (new Vector3(GridPosition.x, GridPosition.y)*tileSize);
        }

        IEnumerator Use(Grid.Unit unit) {
            var battle = CombatObjects.GetBattleState().Model;

            var tile = battle.GetInteractiveTileByLocation(GridPosition);
            if (tile.CanTrigger()) {
                var unitModel = unit.model;
                battle.TriggerInteractiveTile(tile, unitModel);
            }

            yield return _event.Play();
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(32f, 32f, 32f));
        }
    }
}