using UnityEngine;

namespace GameObjects.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControl : RigidbodyEntity
    {
        [SerializeField] private float speed = 5;
        private Camera _mainCamera;
        private float _recallTimer;
        private int _ammoInMag;
        private bool _canShoot;

        public GunInfo currentGun;
        
        private new void Start()
        {
            base.Start();
            _mainCamera = Camera.main;
            _ammoInMag = currentGun.ammoInMag;
            GameManager.Player = this;
        }

        void FixedUpdate()
        {
            Movement();
            Aim();
            ShootControl();
            // Debug.DrawLine(transform.position, transform.position + Forward);
        }

        private void Aim()
        {
            LookAt(_mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
        
        private void Movement()
        {
            var control = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            MyRigidbody.AddForce(control * speed);
        }

        private void ShootControl()
        {
            _recallTimer -= Time.deltaTime;
            if (_recallTimer > 0) return;
            
            if (Input.GetAxis("Fire1") < 0.5f && !currentGun.isFullAmmo) _canShoot = true;
            if (Input.GetAxis("Fire1") < 0.5f) return;
            
            if (!_canShoot) return;

            Shoot();
        }

        private void Shoot()
        {
            GameManager.Shoot(transform.position, Forward, currentGun, this);
            MyRigidbody.AddForce(-Forward * currentGun.recoil);

            _ammoInMag--;
            if (_ammoInMag > 0)
                _recallTimer = 1 / currentGun.rate;
            else
            {
                _recallTimer = currentGun.reloadTime;
                _ammoInMag = currentGun.ammoInMag;
            }

            if (!currentGun.isFullAmmo) _canShoot = false;
        }
    }
}