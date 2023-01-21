using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjects.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControl : RigidbodyEntity
    {
        public static PlayerControl Main;

        [Space(10)] [Header("Control")] 
        [SerializeField] private float speed = 180;
        [SerializeField] private Transform gunFire;
        [SerializeField] private DropTable startItems;

        // public event UnityAction<PlayerHungerChangedEventArgs> HungerChanged;
        // public event UnityAction<PlayerStaminaChangedEventArgs> StaminaChanged;
        // public event UnityAction<PlayerThirstChangedEventArgs> ThirstChanged;

        private Camera _mainCamera;
        
        private int _ammoInMag;

        [Space(10)] [Header("Inventory")]
        public Inventory myInventory;

        [SerializeField] private bool canPlace;

        private new void Awake()
        {
            base.Awake();
            Main = this;
            myInventory = GetComponent<Inventory>();
        }


        private void Start()
        {
            _mainCamera = Camera.main;
            GameManager.Player = this;

            foreach (var item in startItems.Realise())
            {
                myInventory.TryPut(item);
            }
        }

        void FixedUpdate()
        {
            Movement();
            Aim();
        }

        private void Update()
        {
            HotBarControl();
            Placing();
        }
        
        
        private void Placing()
        {
            if (!canPlace && Input.GetAxis("Fire1") < 0.5f) canPlace = true;
            if (!canPlace || Input.GetAxis("Fire1") < 0.5f) return;

            if (myInventory.selectedHotBarSlot.Items is null ||
                myInventory.selectedHotBarSlot.Items.item.GetType() != typeof(PlaceableInfo)) return;

            var placeableInfo = (PlaceableInfo)myInventory.selectedHotBarSlot.Items.item;

            var instantiate = Instantiate(placeableInfo.buildingPrefab);
            
            var transformPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transformPosition.z = 1;
            instantiate.transform.position = transformPosition;
            
            canPlace = false;
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
            control.Normalize();
            MyRigidbody.AddForce(control * speed);
        }
        
        protected override bool Die()
        {
            Debug.LogWarning("Ты умер, но Иван дал тебе новый шанс");
            hp = maxHp;
            return false;
        }
    }
}
