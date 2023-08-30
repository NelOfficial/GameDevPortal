using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class DataBaseManager : MonoBehaviour
{
    [Header("Wall UI Settings")]
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject wallAnimationUI;
    [SerializeField] Animator wallUpdateContainer;
    [SerializeField] Transform wallParent;

    [Header("Register UI Settings")]
    [SerializeField] GameObject registerAnimationUI;
    [SerializeField] TMP_InputField register_AccountNameInputField;
    [SerializeField] TMP_InputField register_AccountDescriptionInputField;
    [SerializeField] TMP_InputField register_AccountPasswordInputField;
    [SerializeField] TMP_Text register_MessageText;

    [Header("Login UI Settings")]
    [SerializeField] GameObject loginAnimationUI;
    [SerializeField] TMP_InputField login_AccountNameInputField;
    [SerializeField] TMP_InputField login_AccountPasswordInputField;
    [SerializeField] TMP_Text login_MessageText;
    [SerializeField] Image profileAvatar;
    [SerializeField] Image profileBanner;

    [Header("Profile wall creation UI Settings")]
    [SerializeField] GameObject createWallAnimationUI;
    [SerializeField] TMP_InputField createWallTextInputField;

    public int lastPostsIdsCount;
    public string[] postsIds;
    public string[] postValues;
    private string[] postAuthorValues;

    public string[] accountData;

    private FeedManager feedManager;
    private ProfileManager profileManager;

    public bool updatingFeed;


    private void Start()
    {
        feedManager = FindObjectOfType<FeedManager>();
        profileManager = FindObjectOfType<ProfileManager>();
        UpdateWalls();
    }


    private void DestroyWalls()
    {
        foreach (Transform child in wallParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateWalls()
    {
        StartCoroutine(OffsetWallUpdate());
        feedManager.UpdateFeedUI();
    }

    public void CreateWall()
    {
        StartCoroutine(CreateWall_UWR(createWallTextInputField.text, profileManager.userId));
    }

    private IEnumerator CreateWall_UWR(string text, int userId)
    {
        createWallAnimationUI.SetActive(true);
        WWWForm form = new WWWForm();
        form.AddField("text", text);
        form.AddField("userId", userId);

        WWW createPostRequest = new WWW("https://gamedevportal.7m.pl/admin/createPost.php", form);

        Debug.Log(createPostRequest.text);

        yield return createPostRequest;
        createWallAnimationUI.SetActive(false);
    }

    private IEnumerator OffsetWallUpdate()
    {
        updatingFeed = true;
        wallUpdateContainer.Play("feedUpdate");
        yield return new WaitForSeconds(0.5f);
        wallUpdateContainer.GetComponent<Animator>().Play("feedUpdateOut");
        StartCoroutine(GetPostsIds());

        if (postsIds != null && lastPostsIdsCount == postsIds.Length && lastPostsIdsCount > 0)
        {
            WallItem[] walls = FindObjectsOfType<WallItem>();

            updatingFeed = true;
            for (int i = 0; i < walls.Length; i++)
            {
                // Getting Post with id

                WWWForm form = new WWWForm();
                form.AddField("id", walls[i].postId);

                WWW getPostRequest = new WWW("https://gamedevportal.7m.pl/admin/getPostById.php", form);

                yield return getPostRequest;

                postValues = getPostRequest.text.Split('\t');


                if (!string.IsNullOrEmpty(postValues[0]))
                {
                    walls[i].wallText = postValues[1];
                    walls[i].wallImageValue = postValues[2];
                    walls[i].wallDate = postValues[5];
                    walls[i].postId = int.Parse(postValues[6]);
                }
                else
                {
                    Destroy(walls[i].gameObject);
                    updatingFeed = false;
                    UpdateWalls();
                }
 

                // Get Author data with id

                WWWForm authorForm = new WWWForm();
                authorForm.AddField("id", int.Parse(postValues[0]));
                WWW wwwAuthorRequest = new WWW("https://gamedevportal.7m.pl/admin/getAuthorById.php", authorForm);

                yield return wwwAuthorRequest;
                postAuthorValues = wwwAuthorRequest.text.Split('\t');

                walls[i].authorName = postAuthorValues[1];
                walls[i].authorAvatarValue = postAuthorValues[4];

                // Get AuthorAvatar data with url

                UnityWebRequest wwwAvatarRequest = UnityWebRequestTexture.GetTexture(postAuthorValues[4]);
                yield return wwwAvatarRequest.SendWebRequest();

                Texture avatarTexture = DownloadHandlerTexture.GetContent(wwwAvatarRequest);

                Sprite avatarSprite = Sprite.Create((Texture2D)avatarTexture, new Rect(0.0f, 0.0f, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                walls[i].authorAvatar.sprite = avatarSprite; // apply the new sprite to the image

                walls[i].SetUp();

            }
            updatingFeed = false;
        }
        else if (postsIds != null && lastPostsIdsCount++ != postsIds.Length)
        {
            updatingFeed = true;

            WWW www = new WWW("https://gamedevportal.7m.pl/admin/getPosts.php");
            yield return www;

            postsIds = www.text.Split('\t');

            int diff = postsIds.Length - lastPostsIdsCount;

            for (int i = lastPostsIdsCount; i < postsIds.Length; i++)
            {

                // Getting Post with id

                WWWForm form = new WWWForm();
                form.AddField("id", postsIds[i]);

                WWW getPostRequest = new WWW("https://gamedevportal.7m.pl/admin/getPostById.php", form);

                yield return getPostRequest;

                postValues = getPostRequest.text.Split('\t');

                GameObject wallSpawned = Instantiate(wallPrefab);
                wallSpawned.transform.SetParent(wallParent);
                wallSpawned.transform.localScale = new Vector3(1, 1, 1);

                WallItem wallSpawnedItem = wallSpawned.GetComponent<WallItem>();

                wallSpawnedItem.wallText = postValues[1];
                wallSpawnedItem.wallImageValue = postValues[2];
                wallSpawnedItem.wallDate = postValues[5];
                wallSpawnedItem.postId = int.Parse(postValues[6]);

                // Get Author data with id

                WWWForm authorForm = new WWWForm();
                authorForm.AddField("id", int.Parse(postValues[0]));
                WWW wwwAuthorRequest = new WWW("https://gamedevportal.7m.pl/admin/getAuthorById.php", authorForm);

                yield return wwwAuthorRequest;
                postAuthorValues = wwwAuthorRequest.text.Split('\t');

                wallSpawnedItem.authorName = postAuthorValues[1];
                wallSpawnedItem.authorAvatarValue = postAuthorValues[4];

                // Get AuthorAvatar data with url

                UnityWebRequest wwwAvatarRequest = UnityWebRequestTexture.GetTexture(postAuthorValues[4]);
                yield return wwwAvatarRequest.SendWebRequest();

                Texture avatarTexture = DownloadHandlerTexture.GetContent(wwwAvatarRequest);

                Sprite avatarSprite = Sprite.Create((Texture2D)avatarTexture, new Rect(0.0f, 0.0f, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                wallSpawnedItem.authorAvatar.sprite = avatarSprite; // apply the new sprite to the image

                wallSpawnedItem.SetUp();
                i++;

                wallAnimationUI.SetActive(false);
            }

            updatingFeed = false;
        }

        if (wallParent.childCount < 1)
        {
            DestroyWalls();
            StartCoroutine(GetPosts(true));
        }
            
        yield break;
    }

    private IEnumerator GetPostsIds()
    {
        WWW www = new WWW("https://gamedevportal.7m.pl/admin/getPosts.php");
        yield return www;

        postsIds = www.text.Split('\t');

        lastPostsIdsCount = postsIds.Length;
    }

    private IEnumerator GetPosts(bool updateWalls)
    {
        feedManager.walls = null;
        wallAnimationUI.SetActive(true);

        WWW www = new WWW("https://gamedevportal.7m.pl/admin/getPosts.php");
        yield return www;

        postsIds = www.text.Split('\t');

        for (int i = 0; i < postsIds.Length;)
        {
            // Getting Post with id

            WWWForm form = new WWWForm();
            form.AddField("id", postsIds[i]);

            WWW getPostRequest = new WWW("https://gamedevportal.7m.pl/admin/getPostById.php", form);

            yield return getPostRequest;

            postValues = getPostRequest.text.Split('\t');

            GameObject wallSpawned = Instantiate(wallPrefab);
            wallSpawned.transform.SetParent(wallParent);
            wallSpawned.transform.localScale = new Vector3(1, 1, 1);

            WallItem wallSpawnedItem = wallSpawned.GetComponent<WallItem>();

            wallSpawnedItem.wallText = postValues[1];
            wallSpawnedItem.wallImageValue = postValues[2];
            wallSpawnedItem.wallDate = postValues[5];
            wallSpawnedItem.postId = int.Parse(postValues[6]);

            // Get Author data with id

            WWWForm authorForm = new WWWForm();
            authorForm.AddField("id", int.Parse(postValues[0]));
            WWW wwwAuthorRequest = new WWW("https://gamedevportal.7m.pl/admin/getAuthorById.php", authorForm);

            yield return wwwAuthorRequest;
            postAuthorValues = wwwAuthorRequest.text.Split('\t');

            wallSpawnedItem.authorName = postAuthorValues[1];
            wallSpawnedItem.authorAvatarValue = postAuthorValues[4];

            // Get AuthorAvatar data with url

            UnityWebRequest wwwAvatarRequest = UnityWebRequestTexture.GetTexture(postAuthorValues[4]);
            yield return wwwAvatarRequest.SendWebRequest();

            Texture avatarTexture = DownloadHandlerTexture.GetContent(wwwAvatarRequest);

            Sprite avatarSprite = Sprite.Create((Texture2D)avatarTexture, new Rect(0.0f, 0.0f, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            wallSpawnedItem.authorAvatar.sprite = avatarSprite; // apply the new sprite to the image

            wallSpawnedItem.SetUp();
            i++;
        }
        
        wallAnimationUI.SetActive(false);

        feedManager.walls = FindObjectsOfType<WallItem>();

        updatingFeed = false;
    }

    private IEnumerator GetPostById(int postId)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", postId);

        WWW www = new WWW("https://gamedevportal.7m.pl/admin/getPostById.php", form);

        yield return www;

        postValues = www.text.Split('\t');
    }

    private IEnumerator GetAuthorById(int authorId)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", authorId);

        WWW www = new WWW("https://gamedevportal.7m.pl/admin/getAuthorById.php", form);

        yield return www;

        postAuthorValues = www.text.Split('\t');
    }


    public void RegisterAccountr()
    {
        if (string.IsNullOrEmpty(register_AccountNameInputField.text))
        {
            register_MessageText.text = "Название студии не может быть пустым.";
        }
        if (string.IsNullOrEmpty(register_AccountPasswordInputField.text))
        {
            register_MessageText.text = "Пароль не может быть пустым.";
        }
        if(!string.IsNullOrEmpty(register_AccountNameInputField.text) && !string.IsNullOrEmpty(register_AccountPasswordInputField.text))
        {
            StartCoroutine(RegisterAccount_UWR());
        }
    }

    public void LoginAccountr()
    {
        if (string.IsNullOrEmpty(login_AccountNameInputField.text))
        {
            register_MessageText.text = "Логин не может быть пустым.";
        }
        if (string.IsNullOrEmpty(login_AccountPasswordInputField.text))
        {
            register_MessageText.text = "Пароль не может быть пустым.";
        }
        if (!string.IsNullOrEmpty(login_AccountNameInputField.text) && !string.IsNullOrEmpty(login_AccountPasswordInputField.text))
        {
            StartCoroutine(LoginAccount_UWR());
        }
    }

    public void LoginAccountLegacy(string login, string password)
    {
        StartCoroutine(LoginAccountLegacy_UWR(login, password));
    }

    private IEnumerator RegisterAccount_UWR()
    {
        registerAnimationUI.SetActive(true);

        WWWForm registerForm = new WWWForm();
        registerForm.AddField("name", register_AccountNameInputField.text);
        registerForm.AddField("description", register_AccountDescriptionInputField.text);
        registerForm.AddField("password", register_AccountPasswordInputField.text);

        WWW accountRegisterRequest = new WWW("https://gamedevportal.7m.pl/admin/accountSignUp.php", registerForm);
        yield return accountRegisterRequest;

        register_MessageText.text = accountRegisterRequest.text;
        registerAnimationUI.SetActive(false);
    }

    private IEnumerator LoginAccount_UWR()
    {
        loginAnimationUI.SetActive(true);

        WWWForm loginForm = new WWWForm();
        loginForm.AddField("name", login_AccountNameInputField.text);
        loginForm.AddField("password", login_AccountPasswordInputField.text);

        WWW accountLoginRequest = new WWW("https://gamedevportal.7m.pl/admin/accountSignIn.php", loginForm);
        yield return accountLoginRequest;

        if (accountLoginRequest.text == "Неправильный логин или пароль.")
        {
            login_MessageText.text = "Неправильный логин или пароль.";
        }
        if (accountLoginRequest.text == "Такой студии не существует.")
        {
            login_MessageText.text = "Такой студии не существует.";
        }

        if (accountLoginRequest.text != "Неправильный логин или пароль." && accountLoginRequest.text != "Такой студии не существует.")
        {
            accountData = accountLoginRequest.text.Split('\t');
            profileManager.userId = int.Parse(accountData[0]);
            profileManager._profileName = accountData[1];
            profileManager._profilePassword = accountData[2];
            profileManager._profileDescription = accountData[3];

            UnityWebRequest wwwAvatarLoginRequest = UnityWebRequestTexture.GetTexture(accountData[4]);
            yield return wwwAvatarLoginRequest.SendWebRequest();

            Texture avatarTexture = DownloadHandlerTexture.GetContent(wwwAvatarLoginRequest);

            Sprite avatarSprite = Sprite.Create((Texture2D)avatarTexture, new Rect(0.0f, 0.0f, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            profileAvatar.sprite = avatarSprite; // apply the new sprite to the image

            UnityWebRequest wwwBannerLoginRequest = UnityWebRequestTexture.GetTexture(accountData[5]);
            yield return wwwBannerLoginRequest.SendWebRequest();

            Texture bannerTexture = DownloadHandlerTexture.GetContent(wwwBannerLoginRequest);

            Sprite bannerSprite = Sprite.Create((Texture2D)bannerTexture, new Rect(0.0f, 0.0f, bannerTexture.width, bannerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            profileBanner.sprite = avatarSprite; // apply the new sprite to the image

            profileManager.SaveUserLocalData();
            profileManager.CheckUser(false);
        }



        loginAnimationUI.SetActive(false);
    }

    private IEnumerator LoginAccountLegacy_UWR(string login, string password)
    {
        loginAnimationUI.SetActive(true);

        WWWForm loginForm = new WWWForm();
        loginForm.AddField("name", login);
        loginForm.AddField("password", password);

        WWW accountLoginRequest = new WWW("https://gamedevportal.7m.pl/admin/accountSignIn.php", loginForm);
        yield return accountLoginRequest;

        if (accountLoginRequest.text == "Неправильный логин или пароль.")
        {
            profileManager.DeleteProfileLocalData();
            profileManager.CheckUser(false);
        }
        if (accountLoginRequest.text == "Такой студии не существует.")
        {
            profileManager.DeleteProfileLocalData();
            profileManager.CheckUser(false);
        }

        if (accountLoginRequest.text != "Неправильный логин или пароль." && accountLoginRequest.text != "Такой студии не существует.")
        {
            accountData = accountLoginRequest.text.Split('\t');
            profileManager.userId = int.Parse(accountData[0]);
            profileManager._profileName = accountData[1];
            profileManager._profilePassword = accountData[2];
            profileManager._profileDescription = accountData[3];
            Debug.Log(accountLoginRequest.text);
            profileManager._profileNameText.text = profileManager._profileName;
            profileManager.logged = true;

            UnityWebRequest wwwAvatarLoginRequest = UnityWebRequestTexture.GetTexture(accountData[4]);
            yield return wwwAvatarLoginRequest.SendWebRequest();

            Texture avatarTexture = DownloadHandlerTexture.GetContent(wwwAvatarLoginRequest);

            Sprite avatarSprite = Sprite.Create((Texture2D)avatarTexture, new Rect(0.0f, 0.0f, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            profileAvatar.sprite = avatarSprite; // apply the new sprite to the image

            UnityWebRequest wwwBannerLoginRequest = UnityWebRequestTexture.GetTexture(accountData[5]);
            yield return wwwBannerLoginRequest.SendWebRequest();

            Texture bannerTexture = DownloadHandlerTexture.GetContent(wwwBannerLoginRequest);

            Sprite bannerSprite = Sprite.Create((Texture2D)bannerTexture, new Rect(0.0f, 0.0f, bannerTexture.width, bannerTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            profileBanner.sprite = avatarSprite; // apply the new sprite to the image

            profileManager.SaveUserLocalData();
            profileManager.CheckUser(false);
        }
        loginAnimationUI.SetActive(false);
    }
}
