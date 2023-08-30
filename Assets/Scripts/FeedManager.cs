using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedManager : MonoBehaviour
{
    [SerializeField] RectTransform feedContent;

    [HideInInspector] public WallItem[] walls;

    [SerializeField] DataBaseManager dataBaseManager;

    [HideInInspector] public Sprite openedImage;
    [HideInInspector] public Image imageAnimation;
    [HideInInspector] public GameObject imageCloseOverlay;

    public void UpdateFeedUI()
    {
        //if (walls != null)
        //{
        //    feedContent.sizeDelta = new Vector2(1080, 0);

        //    foreach (WallItem wall in walls)
        //    {
        //        feedContent.sizeDelta = new Vector2(1080,
        //            wall.GetComponent<RectTransform>().sizeDelta.y + 20 + feedContent.sizeDelta.y);
        //    }
        //}

        if (!dataBaseManager.updatingFeed)
        {
            if (feedContent.anchoredPosition.y <= -300)
            {
                dataBaseManager.UpdateWalls();
                feedContent.sizeDelta = new Vector2(1080, -250);
            }
        }
    }
    public void SetImage(Image image)
    {
        openedImage = image.sprite;
        imageAnimation.gameObject.SetActive(true);
        imageAnimation.sprite = openedImage;

        imageAnimation.GetComponent<Animator>().Play("openImage");
        imageCloseOverlay.SetActive(true);
    }

    public void CloseImage()
    {
        StartCoroutine(CloseImage_Co());
    }

    private IEnumerator CloseImage_Co()
    {
        imageAnimation.GetComponent<Animator>().Play("closeImage");
        yield return new WaitForSeconds(0.25f);
        imageCloseOverlay.SetActive(false);
        imageAnimation.gameObject.SetActive(false);
        yield break;
    }
}
