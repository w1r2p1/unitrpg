﻿using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Contexts.Battle.Utilities;
using Models.Fighting;
using Models.Fighting.Characters;
using Models.Fighting.Effects;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Contexts.Battle.Views {
    [RequireComponent(typeof(CombatantController))]
    [RequireComponent(typeof(CombatantAnimator))]
    public class CombatantView : View {

        public string CharacterName;
        public string CombatantId;
        public ArmyType Army = ArmyType.Friendly;
        public CombatantState State = CombatantState.Idle;
        public MathUtils.CardinalDirection Facing = MathUtils.CardinalDirection.S;
        public float SecondsPerSquare = 0.3f;

        public Signal DeathSignal {
            get { return _animator.DeathSignal; }
        }

        public Signal AttackCompleteSignal {
            get { return _animator.AttackCompleteSignal; }
        }

        public Signal DodgeCompleteSignal {
            get { return _animator.DodgeCompleteSignal; }
        }

        public Signal<WeaponHitConnection> AttackConnectedSignal = new Signal<WeaponHitConnection>();

        private CombatantController _controller;
        private CombatantAnimator _animator;

        void Awake() {
            base.Awake();

            _controller = GetComponent<CombatantController>();
            _animator = GetComponent<CombatantAnimator>();
        }

        public void PrepareForCombat(MathUtils.CardinalDirection direction) {
            Facing = direction;
            State = CombatantState.CombatReady;
        }

        public void ReturnToRest() {
            State = CombatantState.Idle;
        }

        public IEnumerator Attack(ICombatant receiver, WeaponHitSeverity severity) {
            var attackComplete = false;
            var onComplete = new Action(() => {
                attackComplete = true;
            });

            _animator.AttackConnectedSignal.AddOnce(() => {
                var connection = new WeaponHitConnection(severity, receiver);
                AttackConnectedSignal.Dispatch(connection);
            });

            _animator.AttackCompleteSignal.AddOnce(onComplete);
            State = CombatantState.Attacking;

            while (!attackComplete) {
                yield return new WaitForEndOfFrame();
            }
            State = CombatantState.CombatReady;
        }

        public IEnumerator Dodge() {
            var dodgeComplete = false;
            _animator.DodgeCompleteSignal.AddOnce(() => {
                dodgeComplete = true;
            });
            State = CombatantState.Dodging;

            while (!dodgeComplete) {
                yield return new WaitForEndOfFrame();
            }

            State = CombatantState.CombatReady;
        }

        public IEnumerator FollowPath(IList<Vector3> path, MapDimensions dimensions) {
            return _controller.FollowPath(path, dimensions);
        }
    }
}