using Photon.Pun;
using CGD.Audio;
using UnityEngine;


namespace CGD
{
    public class Torch : Item
    {
        [Header("Settings")]
        [SerializeField] private float intensity;
        [SerializeField] private Renderer emissiveMat;

        private bool flashLightOn = false;
        private Color alphaStart;

        private static Light pointLight;
        public static Light PointLight { get { return pointLight; } }


        private AudioClip on;
        private AudioClip off;

        #region MonoBehaviour
        private void Awake()
        {
            pointLight = GetComponentInChildren<Light>();
            alphaStart = emissiveMat.material.color;
        }

        private void Start()
        {
            on = AudioController.Instance.GetClip(Audio.Gameplay.LightToolOn);
            off = AudioController.Instance.GetClip(Audio.Gameplay.LightToolOff);
        }
        #endregion

        #region Interface Function Overrides
        public override void Interact(int viewId) 
        {
            if((Equipped && owner == viewId) || !Equipped)
                photonView.RPC(nameof(ToggleTorch), RpcTarget.All, flashLightOn); 
        }
        public override void Equip(int viewId) => photonView.RPC(nameof(TorchEquipped), RpcTarget.All, viewId);
        public override void Unequip(int viewId) => photonView.RPC(nameof(TorchDropped), RpcTarget.All, viewId);
        #endregion

        #region RPCs
        [PunRPC]
        private void TorchEquipped(int viewId)
        {
            var view = PhotonView.Find(viewId);

            if (view != null && view.TryGetComponent<PlayerAnimController>(out var animController))
            {
                var slot = animController.EquipItem(itemEquipSlot);
                OnExitFocus();
                rb.isKinematic = true;

                transform.SetParent(slot);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                owner = viewId;

                foreach(var collider in colliders) 
                {
                    collider.enabled = false;
                }
            }
        }

        [PunRPC]
        private void TorchDropped(int viewId)
        {
            var view = PhotonView.Find(viewId);

            if (view != null && view.TryGetComponent<PlayerAnimController>(out var animController))
            {
                animController.UnEquipItem();
                transform.SetParent(null);
                owner = -1;
                rb.isKinematic = false;

                foreach (var collider in colliders)
                {
                    collider.enabled = true;
                }
            }
        }

        [PunRPC]
        private void ToggleTorch(bool lastFlashLightStatus)
        {
            flashLightOn = !lastFlashLightStatus;

            if (flashLightOn)
            {
                pointLight.intensity = intensity;
                emissiveMat.material.SetColor("_EmissionColor", alphaStart * pointLight.intensity);
                source.PlayOneShot(on, 2);
            }
            else
            {
                pointLight.intensity = 0.0f;
                emissiveMat.material.SetColor("_EmissionColor", alphaStart * Color.black);
                source.PlayOneShot(off, 2);
            }
        }
        #endregion

    }
}