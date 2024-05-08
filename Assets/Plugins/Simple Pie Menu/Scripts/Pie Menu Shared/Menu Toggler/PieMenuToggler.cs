using System.Collections;
using UnityEngine;

namespace SimplePieMenu
{
    public class PieMenuToggler : MonoBehaviour
    {
        private PieMenuSharedReferences references;
        private PieMenuInfoPanelSettingsHandler infoPanelHandler;
        private PieMenuAnimationsSettingsHandler animationsHandler;

        private PieMenu pieMenu;
        private PieMenuInfo pieMenuInfo;
        private PieMenuElements pieMenuElements;
        private GameObject pieMenuGO;
        private MenuItemSelectionHandler selectionHandler;
        private Animator animator;

        private void Update()
        {
            CloseOnReturnPressed();
        }

        public void SetActive(PieMenu pieMenu, bool isActive)
        {
            enabled = true;

            GetPieMenuComponents(pieMenu);

            if (references == null)
                GetSharedComponents();

            DisableInfoPanel(pieMenu);

            if (isActive)
                ShowPieMenu();
            else
                HidePieMenu();
        }

        private void ShowPieMenu()
        {
            pieMenuGO.SetActive(true);
            animationsHandler.PlayAnimation(animator, PieMenuAnimationsSettingsHandler.TriggerActiveTrue);
            StartCoroutine(WaitForAudioAndAnimationToFinishPlaiyng(pieMenu, true));
        }

        private void HidePieMenu()
        {
            selectionHandler.ToggleSelection(false);
            references.AudioSettingsHandler.PlayAudio(pieMenuElements.MouseClickAudioSource);
            animationsHandler.PlayAnimation(animator, PieMenuAnimationsSettingsHandler.TriggerActiveFalse);
            StartCoroutine(WaitForAudioAndAnimationToFinishPlaiyng(pieMenu, false));
        }

        private IEnumerator WaitForAudioAndAnimationToFinishPlaiyng(PieMenu pieMenu, bool isActive)
        {
            float timeToWait = CalculateTimeToWait(pieMenu);

            yield return new WaitForSeconds(timeToWait);

            if (isActive)
            {
                EnableInfoPanel(pieMenu);
                selectionHandler.ToggleSelection(true);
                selectionHandler.EnableClickDetecting();
            }
            else
            {
                pieMenuGO.SetActive(false);
                enabled = false;
            }

            pieMenu.PieMenuInfo.SetActiveState(isActive);
        }

        private float CalculateTimeToWait(PieMenu pieMenu)
        {
            float timeToWait;

            PieMenuInfo pieMenuInfo = pieMenu.PieMenuInfo;
            float audioClipLength = pieMenuInfo.MouseClick.length;
            float animationClipLength = pieMenuInfo.Animation.length;

            timeToWait = Mathf.Max(audioClipLength, animationClipLength);
            return timeToWait;
        }

        private void CloseOnReturnPressed()
        {
            if (pieMenu != null)
            {
                bool returnPressed = selectionHandler.InputDeviceGetter.InputDevice.IsCloseButtonPressed();
                bool closeable = pieMenu.PieMenuInfo.IsCloseable;

                if (returnPressed && closeable)
                {
                    SetActive(pieMenu, false);
                }
            }
        }

        private void GetSharedComponents()
        {
            references = PieMenuShared.References;
            infoPanelHandler = references.InfoPanelSettingsHandler;
            animationsHandler = references.AnimationsSettingsHandler;
        }

        private void GetPieMenuComponents(PieMenu pieMenu)
        {
            this.pieMenu = pieMenu;
            pieMenuInfo = pieMenu.PieMenuInfo;
            pieMenuElements = pieMenu.PieMenuElements;
            pieMenuGO = pieMenuElements.PieMenu.gameObject;
            selectionHandler = pieMenu.SelectionHandler;
            animator = pieMenuElements.Animator;
        }

        private void DisableInfoPanel(PieMenu pieMenu)
        {
            if (pieMenuInfo.InfoPanelEnabled)
            {
                infoPanelHandler.SetActive(pieMenu, false);
            }
        }

        private void EnableInfoPanel(PieMenu pieMenu)
        {
            if (pieMenuInfo.InfoPanelEnabled)
            {
                infoPanelHandler.SetActive(pieMenu, true);
                infoPanelHandler.ModifyHeader(pieMenu, string.Empty);
                infoPanelHandler.ModifyDetails(pieMenu, string.Empty);
            }
        }
    }
}
