﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleState : MonoBehaviour {
	public Grid.Unit SelectedUnit;
	public Vector2 SelectedGridPosition;

    // Movement
	public Vector2 MovementDestination;

    // Attack
    public Grid.Unit AttackTarget;


    // Record of a unit's actions during a single turn
    private struct UnitActionState {
        public int DistanceMoved;
        public bool Acted;
    }

    private Dictionary<Grid.Unit, UnitActionState> States = new Dictionary<Grid.Unit, UnitActionState>();

    public void ResetMovementState() {
        SelectedUnit = null;
        AttackTarget = null;
        SelectedGridPosition = Vector2.zero;
        MovementDestination = Vector2.zero;
    }

    public void ResetTurnState() {
        States = new Dictionary<Grid.Unit, UnitActionState>();
    }

    public bool UnitActed(Grid.Unit unit) {

        if (!States.ContainsKey(unit)) {
            States[unit] = new UnitActionState();
        }

        return States[unit].Acted;
    }

    public int GetRemainingDistance(Grid.Unit unit) {

        if (!States.ContainsKey(unit)) {
            States[unit] = new UnitActionState();
        }
        Models.Character character = unit.GetCharacter();
        return character.Movement - States[unit].DistanceMoved;
    }

    public void MarkUnitMoved(Grid.Unit unit, int distance) {
        
        if (!States.ContainsKey(unit)) {
            States[unit] = new UnitActionState();
        }
        UnitActionState state = States[unit];
        state.DistanceMoved += distance;

        States[unit] = state;
    }

    public void MarkUnitActed(Grid.Unit unit) {
        if (!States.ContainsKey(unit)) {
            States[unit] = new UnitActionState();
        }
        UnitActionState state = States[unit];
        state.Acted = true;

        States[unit] = state;
    }
}