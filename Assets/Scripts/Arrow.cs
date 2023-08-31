using System.Collections;
using UnityEngine;
using System;
public class Arrow : MonoBehaviour
{
    public static event Action<int> OnPointsAdded;
    [SerializeField] private float speed = 10f;
    [SerializeField] private int maxPoint = 25;
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
                CheckTarget(hitInfo);
                Stop();
            }
        }
    }

    private void CheckTarget(RaycastHit hitInfo) {
        GameObject hitGO = hitInfo.collider.gameObject;
        if (hitGO.layer == 6) {
            int points = CalculatePoints(CalculateDistanceFromTargetCenter(hitGO.transform.position));
            Debug.Log(points);
            AddPoints(points);
        }
    }

    private float CalculateDistanceFromTargetCenter(Vector3 targetCenter) {
        float distance = Vector3.Distance(tip.position, targetCenter);
        Debug.Log($"targetCenter {targetCenter} - tipPosition {tip.position} - distance {distance}");
        return distance;
    }

    private int CalculatePoints(float distance) {       
        if (distance > 0.15f) { // 10
            if (distance > 0.21f) { // 9
                if (distance > 0.3f) { // 8
                    if (distance > 0.4f) { // 7 
                        if (distance > 0.53f) { // 6
                            if (distance > 0.63f) { // 5
                                if (distance > 0.75f) { // 4
                                    if (distance > 0.89f) { // 3
                                        if (distance > 1.05f) { // 2
                                            if (distance > 1.2f) { // 1
                                                return 1;
                                            } else {
                                                return 2;
                                            }
                                        } else {
                                            return 3;
                                        }
                                    } else {
                                        return 4;
                                    }
                                } else {
                                    return 5;
                                }
                            } else {
                                return 6;
                            }
                        } else {
                            return 7;
                        }
                    } else {
                        return 8;
                    }
                } else {
                    return 9;
                }
            } else {
                return 10;
            }
        } else {
            return maxPoint;
        }            
    }

    private void AddPoints(int points) {
        if (points > 0) {
            OnPointsAdded?.Invoke(points);
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
