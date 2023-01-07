using UnityEngine;

namespace GameObjects.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControl : RigidbodyEntity
    {
        [SerializeField] private float speed = 180;
        [SerializeField] private Transform gunFire;
        private Camera _mainCamera;
        private float _recallTimer;
        private int _ammoInMag;
        private bool _canShoot = true;

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
        }

        private void Aim()
        {
            LookAt(_mainCamera.ScreenToWorldPoint(Input.mousePosition), gunFire.position);
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

        protected override void Die()
        {
            Debug.LogWarning("Ты умер, но Иван дал тебе новый шанс");
            hp = maxHp;
        }

        private void Shoot()
        {
            var position = gunFire.position;
            // GameManager.Shoot(position, _mainCamera.ScreenToWorldPoint(Input.mousePosition)-position, currentGun, this);
            GameManager.Shoot(position, Forward, currentGun, this);
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