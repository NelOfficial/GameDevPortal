using UnityEngine;
using System.Collections.Generic;

public class UI_ButtonsManager : MonoBehaviour
{

    public UI_Button[] allButtons;
    public List<UI_Button> contentBarButtons = new List<UI_Button>();
    public List<UI_Button> bottomBarButtons = new List<UI_Button>();

    public GameObject[] _tabs;

    [SerializeField] bool _automaticlySearchButtons;

    private void Start()
    {
        Application.targetFrameRate = 120;

        allButtons = FindObjectsOfType<UI_Button>();

        if (_automaticlySearchButtons)
        {
            for (int i = 0; i < allButtons.Length; i++)
            {
                if (allButtons[i].buttonType == UI_Button.ButtonType.ContentBar)
                {
                    contentBarButtons.Add(allButtons[i]);
                }
            }

            for (int i = 0; i < allButtons.Length; i++)
            {
                if (allButtons[i].buttonType == UI_Button.ButtonType.BottomBar)
                {
                    bottomBarButtons.Add(allButtons[i]);
                }
            }
        }
    }

    public void DeselectContentBarButtons()
    {
        foreach (UI_Button button in contentBarButtons)
        {
            button.Deselect();
        }
    }

    public void DeselectBottomBarButtons()
    {
        foreach (UI_Button button in bottomBarButtons)
        {
            button.Deselect();
        }
    }
}
