using System;
using Unity.VisualScripting;
using UnityEngine;

namespace GameObjects.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControl : RigidbodyEntity
    {
        public static PlayerControl Main;

        [Space(10)] [Header("Control")] 
        [SerializeField] private float speed = 180;
        [SerializeField] private Transform gunFire;

        private Camera _mainCamera;
        
        private int _ammoInMag;

        [Space(10)] [Header("Inventory")]
        public Inventory myInventory;

        private void Awake()
        {
            Main = this;
            myInventory = GetComponent<Inventory>();
        }


        private new void Start()
        {
            base.Start();
            _mainCamera = Camera.main;
            GameManager.Player = this;

            myInventory.TryPut(new Items { item = GameManager.MainItemDB.GetItemInfo("wpn_assault_rifle"), count = 1 });
            myInventory.TryPut(new Items { item = GameManager.MainItemDB.GetItemInfo("ammo_machine_gun"), count = 100 });
        }

        void FixedUpdate()
        {
            Movement();
            Aim();
            
        }

        private void Update()
        {
            HotBarControl();
        }

        private void HotBarControl()    
        {
            for (var i = 0; i < 8; i++)
            {
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
                    myInventory.SetSelectedSlot(i);
            }
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
        
        protected override void Die()
        {
            Debug.LogWarning("Ты умер, но Иван дал тебе новый шанс");
            hp = maxHp;
        }
    }
}