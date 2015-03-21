﻿using System;
using System.Collections;
using UnityEngine;

public class ShowingEXP : StateMachineBehaviour {

	public GameObject ExpPanelPrefab;

	private EXPBubble ExpBubble;
	private GameObject ExpPanel;
	private Animator Animator;
	private Grid.UnitManager UnitManager;
	private GridCameraController CameraController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		UnitManager = CombatObjects.GetUnitManager();
		CameraController = CombatObjects.GetCameraController();
		Animator = animator;

        BattleState state = CombatObjects.GetBattleState();
        state.MarkUnitActed(state.SelectedUnit);

		LockControls();

		ExpPanel = Instantiate(ExpPanelPrefab) as GameObject;
		ExpBubble = ExpPanel.transform.FindChild("Panel/EXP Bubble").GetComponent<EXPBubble>();
		ExpBubble.StartCoroutine(AnimateThenExit());
    }

	private IEnumerator AnimateThenExit() {
		yield return ExpBubble.StartCoroutine(ExpBubble.AnimateToExp(50, 0.7f));
		yield return new WaitForSeconds(2);
		Animator.SetTrigger("no_exp");
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Destroy(ExpPanel);
		UnlockControls();
	}

	private void LockControls() {
		UnitManager.Lock();
		CameraController.Lock();
	}
	
	private void UnlockControls() {
		UnitManager.Unlock();
		CameraController.Unlock();
	}
}
