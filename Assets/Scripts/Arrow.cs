using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform tip;
    [SerializeField] private LayerMask layer;
    private Rigidbody _rigidbody;
    private bool _inAir = false;
    private Vector3 _lastPosition = Vector3.zero;
    private AudioSource _audioSource;
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        Stop();        
    }

    private void Stop() {
        _inAir = false;
        SetPhysics(false);
    }

    private void OnDisable() {
        PullInteraction.OnPullActionReleased -= PullInteraction_OnPullActionReleased;
    }

    private void OnEnable() {
        PullInteraction.OnPullActionReleased += PullInteraction_OnPullActionReleased;
    }

    private void PullInteraction_OnPullActionReleased(float value) {
        PullInteraction.OnPullActionReleased -= PullInteraction_OnPullActionReleased;
        transform.parent = null;
        _inAir = true;
        SetPhysics(true);
        Vector3 force = transform.forward * value * speed;
        _rigidbody.AddForce(force, ForceMode.Impulse);
        StartCoroutine(RotateWithVelocity());
        _lastPosition = tip.position;
    }

    private IEnumerator RotateWithVelocity() {
        yield return new WaitForFixedUpdate();
        while (_inAir) {
            Quaternion newRotation = Quaternion.LookRotation(_rigidbody.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }

    private void FixedUpdate() {
        if (_inAir) {
            CheckCollision();
            _lastPosition = tip.position;
        }
    }

    private void CheckCollision() {
        if (Physics.Linecast(_lastPosition, tip.position,out RaycastHit hitInfo)) {
            if (hitInfo.transform.gameObject.layer != 8) {
                if (hitInfo.transform.TryGetComponent(out Rigidbody body)) {
                    _rigidbody.interpolation = RigidbodyInterpolation.None;
                    transform.parent = hitInfo.transform;
                    body.AddForce(_rigidbody.velocity, ForceMode.Impulse);
                    PlaySoundOnHit();
                }
                Stop();
            }
        }
    }

    private void PlaySoundOnHit() {
        _audioSource.Play();
    }

    private void SetPhysics(bool usePhysics) {
        _rigidbody.useGravity = usePhysics;
        _rigidbody.isKinematic = !usePhysics;
    }
}
