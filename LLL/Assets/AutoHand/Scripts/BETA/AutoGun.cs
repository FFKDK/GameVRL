using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Autohand
{
    public enum SlideLoadType
    {
        HandLoaded,
        ShotLoaded
    }

    [Serializable]
    public class UnityGunHitEvent : UnityEvent<AutoGun, RaycastHit> { }
    [Serializable]
    public class UnityGunEvent : UnityEvent<AutoGun> { }
    [Serializable]
    public class UnityGunSlideEvent : UnityEvent<AutoGun, SlideLoadType> { }

    [RequireComponent(typeof(Grabbable))]
    public class AutoGun : MonoBehaviour
    {
        [Header("Auto Gun")]
        public Transform shootForward;
        public GrabbableHeldJoint slideJoint;

        [Header("Gun Settings")]
        public float hitForce = 2500f;
        public float recoilForce = 500f;
        public float maxHitDistance = 1000f;
        public bool useBulletPenetration = false;

        [Header("Fire Rate Settings")]
        public float fireRate = 0.1f;

        [Header("Events")]
        public UnityGunEvent OnShoot;
        public UnityGunEvent OnEmptyShoot;
        public UnityGunHitEvent OnHitEvent;
        public UnityGunSlideEvent OnSlideEvent;

        [Header("Shotgun Mode Settings")]
        public bool shotgunModeEnabled = false;
        public int shotgunBarrels = 3;
        public float shotgunReloadTime = 1.5f;

        public int maxShotsBeforeOverheat = 15;
        public float overheatCooldown = 7.0f;
        public Text overheatText;

        public float coolingTime = 2.0f;

        private Grabbable grabbable;
        private bool slideLoaded = true;
        private int shotsFired;
        private bool isOverheated;
        private float lastOverheatTime;
        private float lastShotTime;
        private bool isFiring;
        private Coroutine fireCoroutine;
        private int shotgunShotsFired = 0;
        private bool isShotgunReloading = false;

        private void Start()
        {
            grabbable = GetComponent<Grabbable>();
            overheatText.enabled = false;
        }

        private void FixedUpdate()
        {
            if (!isOverheated && shotsFired > 0 && Time.time - lastShotTime >= coolingTime)
            {
                shotsFired = 0;
            }

            if (isOverheated && Time.time - lastOverheatTime >= overheatCooldown)
            {
                isOverheated = false;
                shotsFired = 0;
                overheatText.enabled = false;
            }
        }

        public void PressTrigger()
        {
            if (!isFiring)
            {
                fireCoroutine = StartCoroutine(FireCoroutine());
            }
        }

        public void ReleaseTrigger()
        {
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
            isFiring = false;
        }

        private IEnumerator FireCoroutine()
        {
            isFiring = true;
            while (isFiring)
            {
                Shoot();
                yield return new WaitForSeconds(fireRate);
            }
        }

        public void Shoot()
        {
            if (shotgunModeEnabled)
            {
                if (!isShotgunReloading && shotgunShotsFired < 1)
                {
                    for (int i = 0; i < shotgunBarrels; i++)
                    {
                        SingleShot();
                    }
                    shotgunShotsFired++;
                    if (shotgunShotsFired >= 1)
                    {
                        StartCoroutine(ShotgunReloadCoroutine());
                    }
                }
            }
            else
            {
                SingleShot();
            }
        }

        private void SingleShot()
        {
            if (!isOverheated && slideLoaded)
            {
                lastShotTime = Time.time;

                grabbable.body.AddForceAtPosition(-shootForward.forward * recoilForce / 10f, shootForward.position);
                grabbable.body.AddForceAtPosition(shootForward.up * recoilForce, shootForward.position);
                OnShoot?.Invoke(this);

                RaycastHit hit;
                if (useBulletPenetration)
                {
                    var raycasthits = Physics.RaycastAll(shootForward.position, shootForward.forward, maxHitDistance);
                    foreach (var hitInfo in raycasthits)
                    {
                        if (hitInfo.rigidbody != grabbable.body)
                        {
                            hit = hitInfo;
                            OnHit(hit);
                        }
                    }
                }
                else if (Physics.Raycast(shootForward.position, shootForward.forward, out hit, maxHitDistance))
                {
                    if (hit.rigidbody != grabbable.body)
                    {
                        OnHit(hit);
                    }
                }

                shotsFired++;
                if (shotsFired >= maxShotsBeforeOverheat)
                {
                    isOverheated = true;
                    lastOverheatTime = Time.time;
                    overheatText.enabled = true;
                    overheatText.text = "1100C";
                }
            }
            else if (!slideLoaded)
            {
                OnEmptyShoot?.Invoke(this);
            }
        }

        private IEnumerator ShotgunReloadCoroutine()
        {
            isShotgunReloading = true;
            yield return new WaitForSeconds(shotgunReloadTime);
            shotgunShotsFired = 0;
            isShotgunReloading = false;
        }

        private void OnHit(RaycastHit hit)
        {
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(hit.normal * -hitForce, hit.point);
            }

            OnHitEvent?.Invoke(this, hit);
        }

        public void ToggleShotgunMode()
        {
            shotgunModeEnabled = !shotgunModeEnabled;
        }

        private void OnDisable()
        {
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
            isFiring = false;
        }
    }
}
