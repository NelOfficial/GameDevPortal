using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject loggedProfile;
    [SerializeField] GameObject registerProfile;
    [SerializeField] GameObject warningRegisterProfile;
    [SerializeField] GameObject registerFormProfile;

    [SerializeField] TMP_InputField studioNameInputField;
    [SerializeField] TMP_InputField studioDescInputField;

    public TMP_Text _profileNameText;
    [SerializeField] TMP_Text _profileDescriptionText;

    [HideInInspector] public string _profileName;
    [HideInInspector] public string _profilePassword;
    [HideInInspector] public string _profileDescription;
    [HideInInspector] public int userId;

    public bool logged;

    private DataBaseManager dataBase;

    private void Start()
    {
        dataBase = FindObjectOfType<DataBaseManager>();
        logged = false;
    }

    public void DeleteProfileLocalData()
    {
        PlayerPrefs.DeleteKey("currentProfileId");
        PlayerPrefs.DeleteKey("currentProfileName");
        PlayerPrefs.DeleteKey("currentProfilePassword");
        PlayerPrefs.DeleteKey("currentProfileDescription");

        registerProfile.SetActive(true);
        registerFormProfile.SetActive(false);
        warningRegisterProfile.SetActive(true);
        loggedProfile.SetActive(false);

        CheckUser(false);
    }

    public void SaveUserLocalData()
    {
        PlayerPrefs.SetString("currentProfileId", userId.ToString());
        PlayerPrefs.SetString("currentProfileName", _profileName);
        PlayerPrefs.SetString("currentProfilePassword", _profilePassword);
        PlayerPrefs.SetString("currentProfileDescription", _profileDescription);

        CheckUser(false);
    }

    public void CheckUser(bool logIn)
    {
        if (!logged)
        {
            if (PlayerPrefs.HasKey("currentProfileName") && PlayerPrefs.HasKey("currentProfilePassword")
            || !string.IsNullOrEmpty(_profileName) && !string.IsNullOrEmpty(_profilePassword))
            {
                _profileNameText.text = _profileName;
                registerProfile.SetActive(false);
                registerFormProfile.SetActive(false);
                warningRegisterProfile.SetActive(false);
                loggedProfile.SetActive(true);
                if (logIn)
                {
                    dataBase.LoginAccountLegacy(PlayerPrefs.GetString("currentProfileName"), PlayerPrefs.GetString("currentProfilePassword"));
                }
            }
            else
            {
                registerProfile.SetActive(true);
                registerFormProfile.SetActive(false);
                warningRegisterProfile.SetActive(true);
                loggedProfile.SetActive(false);
            }
        }
    }
}
