using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour
{
    private TMP_Text buttonText;
    public Image buttonImage;
    private UI_ButtonsManager buttonsManager;

    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;

    [SerializeField] Sprite activeSprite;
    [SerializeField] Sprite inactiveSprite;

    [SerializeField] bool _enabled = false;

    public enum ButtonType
    {
        ContentBar,
        BottomBar,
        Custom
    }

    public ButtonType buttonType;

    private void Start()
    {
        buttonText = this.transform.GetChild(0).GetComponent<TMP_Text>();

        buttonsManager = FindObjectOfType<UI_ButtonsManager>();

        OnAwakeUpdateUI();
    }

    public void UpdateButtonUI()
    {
        if (buttonType == ButtonType.ContentBar)
        {
            buttonsManager.DeselectContentBarButtons();
        }
        else if (buttonType == ButtonType.BottomBar)
        {
            buttonsManager.DeselectBottomBarButtons();
        }


        if (_enabled == false)
        {
            if (buttonText != null)
            {
                buttonText.color = activeColor;
            }
            buttonImage.sprite = activeSprite;
            _enabled = true;
        }
        else if (_enabled == true)
        {
            Deselect();
        }
    }

    public void Deselect()
    {
        _enabled = false;
        if (buttonText != null)
        {
            buttonText.color = inactiveColor;
        }
        buttonImage.sprite = inactiveSprite;
    }

    public void OpenTab(GameObject tab)
    {
        for (int i = 0; i < buttonsManager._tabs.Length; i++)
        {
            buttonsManager._tabs[i].SetActive(false);
        }

        tab.SetActive(true);
    }

    private void OnAwakeUpdateUI()
    {
        if (buttonType == ButtonType.ContentBar)
        {
            buttonsManager.DeselectContentBarButtons();
        }
        else if (buttonType == ButtonType.BottomBar)
        {
            buttonsManager.DeselectBottomBarButtons();
        }

        if (_enabled)
        {
            if (buttonText != null)
            {
                buttonText.color = activeColor;
            }
            buttonImage.sprite = activeSprite;
        }
        else
        {
            Deselect();
        }
    }
}
