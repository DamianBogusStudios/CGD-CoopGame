using CGD.Case;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Linq;


namespace CGD.Gameplay
{
    public class GameNotifications : MonoBehaviour, IOnEventCallback
    {
        /// <summary>
        /// duration of notification on screen
        /// </summary>
        [SerializeField] private float notifTime;

        /// <summary>
        /// display text
        /// </summary>
        [SerializeField] private CanvasGroup notifCanvasGroup;
        
        /// <summary>
        /// display text
        /// </summary>
        [SerializeField] private TextMeshProUGUI tmp;

        #region Setup
        public void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        #endregion


        #region IOnEventCallback 
        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            Sprite icon = null;
            string txt = "";

            if (eventCode == GameSettings.PlayerSubmittedClue)
            {
                var data = (object[])photonEvent.CustomData;

                if (PhotonNetwork.CurrentRoom.Players.TryGetValue((int)data[1], out Player player)) 
                {
                    txt = player.NickName.Substring(0, 12) + " submitted clue";
                    ShowNotification(icon, txt);
                }
            }
            if (eventCode == GameSettings.PlayerSharedClue)
            {
                var data = (object[])photonEvent.CustomData;

                if (PhotonNetwork.CurrentRoom.Players.TryGetValue((int)data[1], out Player player))
                {
                    txt = player.NickName.Substring(0, 12) + " shared clue";
                    ShowNotification(icon, txt);
                }
            }

        }
        #endregion


        #region Main
        private void ShowNotification(Sprite icon, string text) 
        {
            notifCanvasGroup.alpha = 0;
            tmp.text = text;

            var seq = DOTween.Sequence();
            seq.Append(notifCanvasGroup.DOFade(1, 0.5f));
            seq.AppendInterval(notifTime);
            seq.Append(notifCanvasGroup.DOFade(0, 0.5f));
        }
        #endregion


    }
}
