using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform notch;
    private XRGrabInteractable _bow;
    private bool _arrowNotched = false;
    private GameObject _currentArrow = null;

    private void Start() {
        _bow = GetComponent<XRGrabInteractable>();        
    }

    private void OnDisable() {
        PullInteraction.OnPullActionReleased -= PullInteraction_OnPullActionReleased;
    }

    private void OnEnable() {
        PullInteraction.OnPullActionReleased += PullInteraction_OnPullActionReleased;
    }

    private void PullInteraction_OnPullActionReleased(float value) {
        _arrowNotched = false;
        _currentArrow = null;
    }

    private IEnumerator DelaySpawn() {
        yield return new WaitForSeconds(1f);
        _currentArrow = Instantiate(arrow, notch);
    }

    private void Update() {
        if (_bow.isSelected && _arrowNotched == false) {
            _arrowNotched = true;
            StartCoroutine("DelaySpawn");
        }
        if (!_bow.isSelected && _currentArrow != null) {
            Destroy(_currentArrow);
            PullInteraction_OnPullActionReleased(1f);
        }
    }
}
