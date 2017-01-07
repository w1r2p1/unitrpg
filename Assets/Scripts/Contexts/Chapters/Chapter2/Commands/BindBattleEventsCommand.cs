﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Contexts.Chapters.Chapter2.Models;
using Assets.Contexts.Chapters.Chapter2.Signals;
using Assets.Contexts.OverlayDialogue.Signals.Public;
using Contexts.Battle.Models;
using Contexts.Battle.Signals.Public;
using Contexts.Global.Signals;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace Assets.Contexts.Chapters.Chapter2.Commands {
    public class BindBattleEventsCommand : Command {
        [Inject]
        public StartDialogueSignal StartDialogueSignal { get; set; }

        [Inject]
        public BattleEventRegistry EventRegistry { get; set; }

        [Inject]
        public DialogueCompleteSignal DialogueCompleteSignal { get; set; }

        [Inject]
        public MarkHouseVisitedSignal MarkHouseVisitedSignal { get; set; }

        [Inject]
        public HouseLightTransitionCompleteSignal HouseLightTransitionCompleteSignal { get; set; }

        [Inject]
        public HouseLightEnableSignal HouseLightEnableSignal { get; set; }

        [Inject]
        public BattleViewState BattleModel { get; set; }

        [Inject]
        public EastmerePlazaState EastmerePlaza { get; set; }

        [Inject]
        public PushSceneSignal PushSceneSignal { get; set; }

        [Inject]
        public ScenePopCompleteSignal ScenePopCompleteSignal { get; set; }

        [Inject]
        public SpawnCombatantSignal SpawnCombatantSignal { get; set; }

        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject View { get; set; }

        public override void Execute() {
            var innHouseDialogues = new Dictionary<string, string> {
                { "Janek", "janek_inn_visit" },
                { "Liat", "liat_inn_visit" }
            };
            EventRegistry.RegisterHandler("inspect_inn", DoVisit(innHouseDialogues, House.Inn));


            var genericHouseDialogues = new Dictionary<string, string> {
                { "Janek", "janek_house_visit" },
                { "Liat", "liat_house_visit" }
            };
            EventRegistry.RegisterHandler("inspect_house", DoVisit(genericHouseDialogues, House.Generic));

            EventRegistry.RegisterHandler("inspect_clinic", DoClinicVisit());
        }

        private IEnumerator DoClinicVisit() {
            // Wait for the player to go through the dialogue
            var dialogueComplete = false;
            Action action = null;
            action = () => {
                dialogueComplete = true;
                StrangeUtils.RemoveOnceListener(DialogueCompleteSignal, action);
            };
            ScenePopCompleteSignal.AddOnce(action);
            PushSceneSignal.Dispatch("chapter_2_clinic_visit");

            // Preload Maelle in the background.
            var path = "Prefabs/Combatants/Maelle";
            var maellePrefab = Resources.Load(path) as GameObject;
            while (!dialogueComplete) {
                yield return new WaitForEndOfFrame();
            }

            var maelle = GameObject.Instantiate(maellePrefab);

            var units = View.transform.FindChild("Battle View/Map/Units");
            maelle.transform.SetParent(units);

            var position = new Vector2(35, 19);
            var secondPositionOption = new Vector2(37, 19);

            var map = BattleModel.Map;
            if (map.IsBlocked(position)) {
                position = secondPositionOption;
            }

            var dimensions = BattleModel.Dimensions;
            var worldPosition = dimensions.GetWorldPositionForGridPosition(position);
            maelle.transform.position = worldPosition;

            SpawnCombatantSignal.Dispatch(maelle);
        }

        private IEnumerator DoVisit(Dictionary<string, string> nameToDialogue, House houseType) {
            // Choose the dialogue based on the current combatant
            var combatant = BattleModel.SelectedCombatant;
            if (!nameToDialogue.ContainsKey(combatant.Name)) {
               throw new ArgumentException("Could not find " + houseType + " visit dialogue for " + combatant.Name); 
            }
            var dialogueName = nameToDialogue[combatant.Name];
            var path = Path.Combine("Chapter 2", dialogueName);

            // Wait for the player to go through the dialogue
            var dialogueComplete = false;
            Action action = null;
            action = () => {
                dialogueComplete = true;
                StrangeUtils.RemoveOnceListener(DialogueCompleteSignal, action);
            };
            DialogueCompleteSignal.AddOnce(action);
            StartDialogueSignal.Dispatch(path);
            while (!dialogueComplete) {
                yield return new WaitForEndOfFrame();
            }

            // Wait for the house light to go off and whatnot.
            var houseDisabled = false;
            Action houseDisabledCallback = null;
            houseDisabledCallback = () => {
                houseDisabled = true;
                StrangeUtils.RemoveOnceListener(HouseLightTransitionCompleteSignal, houseDisabledCallback);
            };
            HouseLightTransitionCompleteSignal.AddListener(houseDisabledCallback);
            MarkHouseVisitedSignal.Dispatch(houseType);
            while (!houseDisabled) {
                yield return new WaitForEndOfFrame();
            }

            Debug.LogFormat("House Visit {0} complete. All done?: {1}", houseType, EastmerePlaza.HouseVisitsComplete);
            if (EastmerePlaza.HouseVisitsComplete) {
                HouseLightEnableSignal.Dispatch(House.Clinic);
            }
        }
    }
}