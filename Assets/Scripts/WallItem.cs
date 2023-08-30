using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class WallItem : MonoBehaviour
{
    public Image authorAvatar;

    [SerializeField] TMP_Text authorNameText;
    [SerializeField] TMP_Text wallTextText;
    [SerializeField] TMP_Text wallDateText;
    [SerializeField] Image wallImage;

    public int postId;

    public string authorName;
    public string authorAvatarValue;
    public string wallImageValue;
    public string wallText;
    public string wallDate;

    public string[] wallDateArray;

    private string day;
    private string hours;
    private string minutes;

    private Texture wallImageTexture;
    private Sprite wallImageSprite;

    public int views;
    public int likes;

    private FeedManager feedManager;

    void Awake()
    {
        SetUp();
    }

    public void SetUp()
    {
        authorNameText.text = authorName;
        wallTextText.text = wallText;
        wallDateText.text = wallDate;

        if (wallImageValue == "none")
        {
            wallImage.gameObject.SetActive(false);
        }
        else
        {
            UpdateImage();
        }

        CheckDateWord();

        feedManager = FindObjectOfType<FeedManager>();
    }

    public void UpdateImage()
    {
        StartCoroutine(DownloadImage(wallImageValue));
    }

    private IEnumerator DownloadImage(string url)
    {
        UnityWebRequest www = null;

        if (url != "none")
        {
            wallImage.gameObject.SetActive(true);

            www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                wallImageTexture = DownloadHandlerTexture.GetContent(www);
                Rect rect = new Rect(0, 0, wallImageTexture.width, wallImageTexture.height);
                wallImageSprite = Sprite.Create((Texture2D)wallImageTexture, rect, new Vector2(0.5f, 0.5f));

                wallImage.sprite = wallImageSprite;
            }
        }
        else if (url == "none")
        {
            wallImage.gameObject.SetActive(false);
        }

        yield return www.isDone;
    }

    private void CheckDateWord()
    {
        string firstWord = "";
        string secondWord = "";

        wallDateArray = wallDate.Split(' ');

        if (wallDateArray.Length > 1)
        {
            string[] wallDateDate = wallDateArray[0].Split('-');
            string[] wallDateTime = wallDateArray[1].Split(':');

            int wallDateDay = int.Parse(wallDateDate[2]);
            int wallDateMinutes = int.Parse(wallDateTime[1]);
            int wallDateHours = int.Parse(wallDateTime[0]);

            DateTime today = DateTime.Now;

            hours = DateTime.Now.ToString("HH");
            minutes = DateTime.Now.ToString("mm");

            if (wallDateDay == DateTime.Now.Day)
            {
                firstWord = "Сегодня";
            }
            else if (wallDateDay + 1 == DateTime.Now.Day)
            {
                firstWord = "Вчера";
            }
            else if (wallDateDay + 2 == DateTime.Now.Day)
            {
                firstWord = "Позавчера";
            }
            else
            {
                firstWord = wallDateArray[0];
            }

            if (wallDateMinutes == int.Parse(minutes))
            {
                secondWord = "только что";
            }
            else
            {
                secondWord = $"в {wallDateHours}:{wallDateMinutes}";
            }

            wallDateText.text = $"{firstWord} {secondWord}";
        }
    }

    public void SetImage()
    {
        feedManager.SetImage(wallImage);
    }
}
