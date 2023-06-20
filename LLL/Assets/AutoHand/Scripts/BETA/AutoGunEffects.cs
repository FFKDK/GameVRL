using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand
{
    [RequireComponent(typeof(AutoGun))]
    public class AutoGunEffects : MonoBehaviour
    {

        [Header("Visual Effects")]
        public bool ejectUnfiredBullet = true;
        public GameObject bullet;
        public GameObject bulletShell;
        public ParticleSystem shootParticle;
        public float ejectedBulletLifetime = 3f;
        public Transform shellEjectionSpawnPoint;
        public Transform shellEjectionDirection;
        public float shellEjectionForce = 50;

        [Header("Sound Effects")]
        public AudioSource shootSound;
        public AudioSource emptyShootSound;

        [Header("Laser Beam")]
        public LineRenderer laserBeam;
        public float laserBeamDuration = 0.2f;

        private Dictionary<GameObject, float> bulletLifetimeTracker = new Dictionary<GameObject, float>();
        private Dictionary<GameObject, float> shellLifetimeTracker = new Dictionary<GameObject, float>();
        private List<GameObject> bulletPool = new List<GameObject>();
        private List<GameObject> bulletShellPool = new List<GameObject>();
        private List<ParticleSystem> activeParticlePool = new List<ParticleSystem>();
        private List<ParticleSystem> inactiveParticlePool = new List<ParticleSystem>();

        private AutoGun gun;
        private float lastShootTime;

        private void OnEnable()
        {
            gun = GetComponent<AutoGun>();
            gun.OnShoot.AddListener(OnShoot);
            gun.OnEmptyShoot.AddListener(OnEmptyShoot);
            gun.OnSlideEvent.AddListener(OnSlideLoaded);
        }

        private void OnDisable()
        {
            gun.OnShoot.RemoveListener(OnShoot);
            gun.OnEmptyShoot.RemoveListener(OnEmptyShoot);
            gun.OnSlideEvent.RemoveListener(OnSlideLoaded);
        }

        private void Update()
        {
            if (laserBeam != null)
            {
                if (Time.time - lastShootTime < laserBeamDuration)
                {
                    laserBeam.SetPosition(0, gun.shootForward.position);
                    RaycastHit hit;
                    if (Physics.Raycast(gun.shootForward.position, gun.shootForward.forward, out hit))
                    {
                        laserBeam.SetPosition(1, hit.point);
                    }
                    else
                    {
                        laserBeam.SetPosition(1, gun.shootForward.position + gun.shootForward.forward * 100);
                    }
                    laserBeam.enabled = true;
                }
                else
                {
                    laserBeam.enabled = false;
                }
            }
        }

        private void FixedUpdate()
        {
            CheckBulletLifetime();
        }

        void OnShoot(AutoGun gun)
        {
            shootSound?.PlayOneShot(shootSound.clip);
            lastShootTime = Time.time;
        }

        void OnEmptyShoot(AutoGun gun)
        {
            emptyShootSound?.PlayOneShot(emptyShootSound.clip);
        }

        void OnSlideLoaded(AutoGun gun, SlideLoadType loadType)
        {
            if (loadType == SlideLoadType.ShotLoaded)
            {
                EjectShell();
            }
            else
            {
                EjectBullet();
            }
        }

        public void EjectBullet()
        {
            if (bullet != null)
            {
                GameObject newBullet;
                if (bulletPool.Count > 0)
                {
                    newBullet = bulletPool[0];
                    bulletPool.RemoveAt(0);
                    newBullet.transform.position = shellEjectionSpawnPoint.position;
                    newBullet.transform.rotation = shellEjectionSpawnPoint.rotation;
                    newBullet.SetActive(true);
                }
                else
                {
                    newBullet = Instantiate(bullet, shellEjectionSpawnPoint.position, shellEjectionSpawnPoint.rotation);
                }

                var body = newBullet.GetComponent<Rigidbody>();
                if (body != null)
                {
                    var gunGrabbable = gun.GetComponent<Grabbable>();
                    if (gunGrabbable && AutoHandPlayer.Instance.IsHolding(gunGrabbable))
                    {
                        body.velocity = AutoHandPlayer.Instance.body.velocity;
                    }
                    body.AddForce(shellEjectionDirection.forward * shellEjectionForce, ForceMode.Force);
                }
                bulletLifetimeTracker.Add(newBullet, ejectedBulletLifetime);
            }
        }

        public void EjectShell()
        {
            if (bulletShell != null)
            {
                GameObject newShell;
                if (bulletShellPool.Count > 0)
                {
                    newShell = bulletShellPool[0];
                    bulletShellPool.RemoveAt(0);
                    newShell.transform.position = shellEjectionSpawnPoint.position;
                    newShell.transform.rotation = shellEjectionSpawnPoint.rotation;
                    newShell.SetActive(true);
                }
                else
                {
                    newShell = Instantiate(bulletShell, shellEjectionSpawnPoint.position, shellEjectionSpawnPoint.rotation);
                }

                var body = newShell.GetComponent<Rigidbody>();
                if (body != null)
                {
                    var gunGrabbable = gun.GetComponent<Grabbable>();
                    if (gunGrabbable && AutoHandPlayer.Instance.IsHolding(gunGrabbable))
                    {
                        body.velocity = AutoHandPlayer.Instance.body.velocity;
                    }
                    body.AddForce(shellEjectionDirection.forward * shellEjectionForce, ForceMode.Force);
                }
                shellLifetimeTracker.Add(newShell, ejectedBulletLifetime);
            }
        }

        void CheckBulletLifetime()
        {
            if (bulletLifetimeTracker.Count > 0)
            {
                var bulletKeys = new List<GameObject>(bulletLifetimeTracker.Keys);
                foreach (var bullet in bulletKeys)
                {
                    bulletLifetimeTracker[bullet] -= Time.deltaTime;
                    if (bulletLifetimeTracker[bullet] <= 0)
                    {
                        bullet.SetActive(false);
                        bulletPool.Add(bullet);
                        bulletLifetimeTracker.Remove(bullet);
                    }
                }
            }

            if (shellLifetimeTracker.Count > 0)
            {
                var shellKeys = new List<GameObject>(shellLifetimeTracker.Keys);
                foreach (var shell in shellKeys)
                {
                    shellLifetimeTracker[shell] -= Time.deltaTime;
                    if (shellLifetimeTracker[shell] <= 0)
                    {
                        shell.SetActive(false);
                        bulletShellPool.Add(shell);
                        shellLifetimeTracker.Remove(shell);
                    }
                }
            }
        }
    }
}
