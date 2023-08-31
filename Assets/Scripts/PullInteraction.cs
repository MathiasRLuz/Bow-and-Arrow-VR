using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PullInteraction : XRBaseInteractable {
    public static event Action<float> OnPullActionReleased;
    public static event Action<int> OnTotalArrowsUpdated;
    public static event Action OnArrowsFinished;
    [SerializeField] private int totalArrows = 10;
    [SerializeField] private Transform start, end, notch;    
    public float pullAmount { get; private set; } = 0.0f;
    private LineRenderer _lineRenderer;
    private IXRSelectInteractor _pullingInteractor = null;
    private AudioSource _audioSource;
    private Vector3 _oldNotchPos;
    protected override void Awake() {
        base.Awake();
        _lineRenderer = GetComponent<LineRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _oldNotchPos = notch.localPosition;
    }

    public void SetPullInteractor(SelectEnterEventArgs args) {
        _pullingInteractor = args.interactorObject;
    }

    public void Release() {
        PlayReleaseSound();
        OnPullActionReleased?.Invoke(pullAmount);
        _pullingInteractor = null;
        pullAmount = 0;        
        notch.localPosition = new Vector3(_oldNotchPos.x, _oldNotchPos.y, 0);        
        UpdateString();        
        CheckIfLastArrow();
    }

    private void CheckIfLastArrow() {
        totalArrows--;
        OnTotalArrowsUpdated?.Invoke(totalArrows);
        if (totalArrows < 1) {
            OnArrowsFinished?.Invoke();
        }
    }

    private void PlayReleaseSound() {
        _audioSource.Play();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase) {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic) {
            if (isSelected) {
                Vector3 pullPosition = _pullingInteractor.transform.position;
                pullAmount = CalculatePull(pullPosition);
                UpdateString();
                HapticFeedback();
            }
        }
    }

    private void HapticFeedback() {
        if (_pullingInteractor != null) {
            ActionBasedController currentController = _pullingInteractor.transform.GetComponent<ActionBasedController>();
            currentController.SendHapticImpulse(pullAmount, .1f);
        }
    }

    private float CalculatePull(Vector3 pullPosition) {
        Vector3 pullDirection = pullPosition - start.position;
        Vector3 targetDirection = end.position - start.position;
        float maxLength = targetDirection.magnitude;
        targetDirection.Normalize();
        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        return Mathf.Clamp(pullValue, 0, 1);
    }

    private void UpdateString() {
        Vector3 linePosition = Vector3.forward * Mathf.Lerp(start.localPosition.z, end.localPosition.z, pullAmount);
        notch.localPosition = new Vector3(notch.localPosition.x, notch.localPosition.y, linePosition.z + 0.2f);
        _lineRenderer.SetPosition(1, linePosition);
    }
}
