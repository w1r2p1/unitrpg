﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using DG.Tweening;
using Models.Dialogue;
using UnityEngine;

public class OverlayDialogueController : MonoBehaviour, IDialogueController {
    private DialoguePortraitView _portraitView;
    private string _currentSpeakerName;
    private tk2dSlicedSprite _panelSprite;
    private tk2dTextMesh _bodyText;
    private tk2dTextMesh _speakerText;

    private const float IntroTime = 0.5f;

    private void Awake() {
        _portraitView = transform.FindChild("Portrait View").GetComponent<DialoguePortraitView>();
        _panelSprite = transform.FindChild("Text Panel").GetComponent<tk2dSlicedSprite>();
        _bodyText = transform.FindChild("Text Panel/Text").GetComponent<tk2dTextMesh>();
        _speakerText = transform.FindChild("Text Panel/Speaker").GetComponent<tk2dTextMesh>();
    }

    public void ChangeSpeaker(string speaker) {
        _currentSpeakerName = speaker;
    }

    public void ChangeEmotion(string speaker, EmotionalResponse response) {
        // The overlay only updates the actor currently speaking, and they always face left.
        if (speaker == _currentSpeakerName) {
            _portraitView.SetActor(speaker, response.emotion, Facing.Left);
        }
    }

    public IEnumerator Initialize(Cutscene model) {
        var currentPosition = new Vector3(transform.position.x, transform.position.y*.5f, transform.position.z);
        var newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.position = currentPosition;

        var currentColor = new Color(_panelSprite.color.r, _panelSprite.color.g, _panelSprite.color.b, 0);
        var endColor = new Color(_panelSprite.color.r, _panelSprite.color.g, _panelSprite.color.b, _panelSprite.color.a);
        _panelSprite.color = currentColor;

        // The portrait view must be populated before the dialogue actually starts
        // so we can tween it in. Grab the first speaker's portrait and add it hidden.
        var firstDeck = model.Decks[0];
        var firstSpeaker = firstDeck.Speaker;
        var firstCard = firstDeck.Cards[0];
        var firstEmotion = firstCard.EmotionalResponses[firstSpeaker];
        ChangeSpeaker(firstSpeaker);
        ChangeEmotion(firstSpeaker, firstEmotion);
        _portraitView.FadeOut(0);

        const Ease easing = Ease.OutCubic;
        yield return DOTween.Sequence()
            .Append(_panelSprite.DOColor(endColor, IntroTime).SetEase(easing))
            .Join(transform.DOMove(newPosition, IntroTime).SetEase(easing))
            .Insert(IntroTime/2f, _portraitView.GetFadeInTween(IntroTime).SetEase(easing))
            .WaitForCompletion();
    }

    public IEnumerator End() {
        var newPosition = new Vector3(transform.position.x, transform.position.y*.5f, transform.position.z);

        const Ease easing = Ease.InCubic;
        yield return DOTween.Sequence()
            .Append(transform.DOMove(newPosition, IntroTime).SetEase(easing))
            .Join(_panelSprite.DOFade(0, IntroTime).SetEase(easing))
            .Join(_portraitView.GetFadeOutTween(IntroTime).SetEase(easing))
            .Join(_bodyText.DOFade(0, IntroTime).SetEase(easing))
            .Join(_speakerText.DOFade(0, IntroTime).SetEase(easing))
            .WaitForCompletion();

        Destroy(gameObject);
    }

}