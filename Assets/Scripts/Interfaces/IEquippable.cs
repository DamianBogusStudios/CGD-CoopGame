
using UnityEngine;

    public interface IEquippable : IInteractable
    {
        /// <summary>
        /// Slot this item equips to
        /// </summary>
        public EquipSlot ItemEquipSlot { get; set; }
        /// <summary>
        /// Flag for whether item is currently equipped
        /// </summary>
        public bool Equipped { get; set; }
        /// <summary>
        /// Flag for whether item is available to equip
        /// </summary>
        public bool Equippable { get; set; }

        public abstract void Equip(int viewId);
        
        public abstract void Unequip(int viewId);

    }

    public enum EquipSlot 
    {
        RightHand = 0,
        LeftHand = 1,
    }


