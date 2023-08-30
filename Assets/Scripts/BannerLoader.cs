using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BannerLoader : MonoBehaviour
{

    [SerializeField] Image bannerImage;

    [SerializeField] string imageURL;

    private Sprite bannerSprite;
    private Texture bannerTexture;

    [SerializeField] GameObject animationUI;

    public void Awake()
    {
        SetImage(imageURL);
    }

    public void SetImage(string url)
    {
        StartCoroutine(DownloadImage(url));
    }

    public void UpdateImage()
    {
        StartCoroutine(DownloadImage(imageURL));
    }

    private IEnumerator DownloadImage(string url)
    {
        animationUI.SetActive(true);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        bannerTexture = DownloadHandlerTexture.GetContent(www);
        CreateSprite();
        animationUI.SetActive(false);
    }

    public void CreateSprite()
    {
        bannerSprite = Sprite.Create((Texture2D)bannerTexture, new Rect(0.0f, 0.0f, bannerTexture.width, bannerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        bannerImage.sprite = bannerSprite; // apply the new sprite to the image
    }
}
