using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopEngineScript : MonoBehaviour
{
    public GameObject HatDisplay;
    public GameObject SkinDisplay;
    public GameObject WallpaperDisplay;
    public GameObject HackeroniText;

    public GameObject ShopButtonPrefab;
    public GameObject ContentObject;

    public List<GameObject> Tabs = new List<GameObject>();
    
    public AudioClip SelectionSound;
    public AudioClip BuySound;
    public AudioClip FailToBuySound;
    private AudioSource audioSource;

    private int Hackeronis;

    public static ColorStatus[] ColorList = new ColorStatus[] 
        {
            new ColorStatus() { Name = "Default", Color = new Color32(255, 219, 102, 255)},
            new ColorStatus() { Name = "Burnt", Color = new Color32(85, 82, 48, 255)},
            new ColorStatus() { Name = "Wet", Color = new Color32(28, 39, 115, 255)},
            new ColorStatus() { Name = "Rotten", Color = new Color32(47, 60, 41, 255)},
            new ColorStatus() { Name = "Grayscale", Color = new Color32(113, 113, 113, 255)},
            new ColorStatus() { Name = "Sweet", Color = new Color32(255, 181, 255, 255)},
            new ColorStatus() { Name = "Sour", Color = new Color32(177, 255, 0, 255)},
            new ColorStatus() { Name = "Frozen", Color = new Color32(201, 254, 255, 255)},
            new ColorStatus() { Name = "Poisonous", Color = new Color32(125, 42, 114, 255)},
            new ColorStatus() { Name = "Chili", Color = new Color32(200, 45, 40, 255)},
            new ColorStatus() { Name = "Silver", Color = new Color32(199, 221, 220, 255)},
            new ColorStatus() { Name = "Radioactive", Color = new Color32(62, 204, 68, 255)},
            new ColorStatus() { Name = "Golden", Color = new Color32(255, 211, 0, 255)},
            new ColorStatus() { Name = "Obsidian", Color = new Color32(50, 5, 63, 255)},
            new ColorStatus() { Name = "Diamond", Color = new Color32(90, 231, 255, 255)},
            new ColorStatus() { Name = "Black_Hole", Color = new Color32(0, 0, 0, 255)},
        };

    List<SpriteStatus>[] SpriteStatusLists = new List<SpriteStatus>[3];
    string[] ListName = new string[] { "Hats", "Skins", "Wallpapers" };
    SpriteStatus[] CurrentEquipment = new SpriteStatus[3];
    ColorStatus CurrentColor = new ColorStatus();
    GameObject[] EquipmentDisplays = new GameObject[3];
    private Sprite forgottenSprite;

    private int PreviousHackeronis;
    private float HackeronisAnimationTimer;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = ButtonSoundManager.singletonSource; 
        SpriteStatusLists = new List<SpriteStatus>[3] { new List<SpriteStatus>(), new List<SpriteStatus>(), new List<SpriteStatus>() };
        CurrentEquipment = new SpriteStatus[3] { new SpriteStatus(), new SpriteStatus(), new SpriteStatus() };
        EquipmentDisplays = new GameObject[3] { HatDisplay, SkinDisplay, WallpaperDisplay };

        Hackeronis = PlayerPrefs.GetInt("Hackeronis", 0);
        LoadPlayerPrefs(0);
        LoadPlayerPrefs(1);
        LoadPlayerPrefs(2);
        LoadColorPlayerPrefs();

        CheckForDiscrepancies();
        CheckForColorDiscrepancies();

        DisplayCurrentEquipment();

        DisplayShopItems(0);
        HackeroniText.GetComponent<TextMeshProUGUI>().text = Hackeronis + " Hackeronis";
        
        forgottenSprite = Resources.Load<Sprite>("Shop/ForgottenSprite");
    }

    void CheckForDiscrepancies()
    {
        for (int i = 0; i < SpriteStatusLists.Length; i++)
        {
            int EquippedCount = 0;
            foreach (SpriteStatus SpriteStatus in SpriteStatusLists[i])
            {
                if (SpriteStatus.Status == "Equipped")
                {
                    EquippedCount += 1;
                }
            }

            if (EquippedCount != 1)
            {
                foreach (SpriteStatus SpriteStatus in SpriteStatusLists[i])
                {
                    if (SpriteStatus.Name == "None" || SpriteStatus.Name == "Default")
                    {
                        SpriteStatus.Status = "Equipped";
                    }
                    else
                    {
                        if(SpriteStatus.Status == "Equipped")
                        {
                            SpriteStatus.Status = "Available";
                        }
                    }
                }
                SavePlayerPrefs(i);
                LoadPlayerPrefs(i);
            }
        }
    }

    void CheckForColorDiscrepancies()
    {
        int EquippedCount = 0;
        foreach (ColorStatus ColorStatus in ColorList)
        {
            if (ColorStatus.Status == "Equipped")
            {
                EquippedCount += 1;
            }
        }

        if (EquippedCount != 1)
        {
            foreach (ColorStatus ColorStatus in ColorList)
            {
                if (ColorStatus.Name == "Default")
                {
                    //print("Default is equipped!");
                    ColorStatus.Status = "Equipped";
                }
                else
                {
                    if (ColorStatus.Status == "Equipped")
                    {
                        ColorStatus.Status = "Available";
                    }
                }
            }
            //print("Current Color2: " + CurrentColor.Name);
            SaveColorPlayerPrefs();
            //print("Current Color3: " + CurrentColor.Name);
            LoadColorPlayerPrefs();
            //print("Current Color4: " + CurrentColor.Name);
        }
    }

    void DisplayCurrentEquipment()
    {
        HatDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Hats/" + CurrentEquipment[0].Name + "-" + CurrentEquipment[0].Cost);
        if(CurrentEquipment[0].Name == "None") {HatDisplay.SetActive(false);}
        else{HatDisplay.SetActive(true);}

        SkinDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Skins/" + CurrentEquipment[1].Name + "-" + CurrentEquipment[1].Cost);
        SkinDisplay.GetComponent<Image>().color = CurrentColor.Color;
        WallpaperDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Wallpapers/" + CurrentEquipment[2].Name + "-" + CurrentEquipment[2].Cost);
    }

    void LoadPlayerPrefs(int Type)
    {
        List<Sprite> SpriteList = new List<Sprite>(Resources.LoadAll<Sprite>(ListName[Type]));

        string PlayerPrefString = PlayerPrefs.GetString(ListName[Type], "");
        string[] PlayerPrefArray = PlayerPrefString.Split(',');

        string NewPlayerPrefString = "";

        SpriteStatusLists[Type] = new List<SpriteStatus>();

        foreach (Sprite Sprite in SpriteList)
        {
            string Avalability = "Unavailable";
            if (Sprite.name.Split('-')[0] == "None" || Sprite.name.Split('-')[0] == "Default")
            {
                Avalability = "Available";
            }

            if(PlayerPrefString.Contains(Sprite.name))
            {
                foreach(string SpriteStatus in PlayerPrefArray)
                {
                    if(SpriteStatus.Split('=')[0] == Sprite.name)
                    {
                        Avalability = SpriteStatus.Split('=')[1];
                    }
                }
            }

            SpriteStatus newSpriteStatus = new SpriteStatus() { Sprite = Sprite, Name = Sprite.name.Split('-')[0], Cost = int.Parse(Sprite.name.Split('-')[1]), Status = Avalability };
            SpriteStatusLists[Type].Add(newSpriteStatus);

            if(Avalability == "Equipped")
            {
                CurrentEquipment[Type] = newSpriteStatus;
            }

            NewPlayerPrefString += Sprite.name + "=" + Avalability + ",";
        }

        SpriteStatusLists[Type].Sort(SortByCost);

        PlayerPrefs.SetString(ListName[Type], NewPlayerPrefString);

        //print("Load " + Type + ": " + "Current: " + CurrentEquipment[Type].Name + "\n" + NewPlayerPrefString);
    }

    void LoadColorPlayerPrefs()
    {
        string PlayerPrefString = PlayerPrefs.GetString("Colors", "");
        string[] PlayerPrefArray = PlayerPrefString.Split(',');
        string NewPlayerPrefString = "";

        for(int i = 0; i < ColorList.Length; i++)
        {
            ColorList[i].Cost = 120 * i;

            string Availability = "Unavailable";
            if(ColorList[i].Name == "Default")
            {
                Availability = "Available";
            }

            foreach(string ColorStatus in PlayerPrefArray)
            {
                if(ColorStatus.Split('=')[0] == ColorList[i].Name + "-" + ColorList[i].Cost)
                {
                    if (ColorStatus.Split('=')[1] == "Equipped" || ColorStatus.Split('=')[1] == "Available" || ColorStatus.Split('=')[1] == "Unavailable")
                    {
                        Availability = ColorStatus.Split('=')[1];
                    }
                }
            }

            ColorList[i].Status = Availability;
            NewPlayerPrefString += ColorList[i].Name + "=" + Availability + ",";

            if (Availability == "Equipped")
            {
                CurrentColor = ColorList[i];
            }
        }

        PlayerPrefs.SetString("Colors", NewPlayerPrefString);

        //print("Load Colors" + ": " + "Current: " + CurrentColor.Name + "\n" + NewPlayerPrefString);
    }

    void ClearShopButtons()
    {
        foreach(Transform child in ContentObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayShopItems(int ItemType) {

        ClearShopButtons();

        int i = 0;
        
        bool isEverythingBought = true;
        foreach (SpriteStatus ShopItem in SpriteStatusLists[ItemType])
        {
            if(ShopItem.Name.Contains("Forgotten") && isEverythingBought != true)
            {
                continue; 
            }
            GameObject ShopButton = Instantiate(ShopButtonPrefab);
            ShopButton.transform.SetParent(ContentObject.transform);
            ShopButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            float xPos = (Mathf.Floor(i / 2f) * 250) + 50;
            float yPos = ((((i + 1) % 2) * 2) - 1) * 120;
            ShopButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, yPos);

            ShopButton.transform.GetChild(0).GetComponent<Image>().sprite = ShopItem.Sprite;
            ShopButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ShopItem.Name.Replace("_", " ");
            ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Costs: " + ShopItem.Cost.ToString();

            ShopButton.GetComponent<ShopButtonScript>().ButtonCode = "Equip" + ListName[ItemType] + "=" + ShopItem.Name;

            if(ShopItem.Status == "Equipped")
            {
                ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Equipped";
                ShopButton.GetComponent<Image>().color = new Color32(48, 131, 220, 255);
                ShopButton.GetComponent<Button>().interactable = false;
            }
            else if(ShopItem.Status == "Available")
            {
                ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Bought";
                ShopButton.GetComponent<Image>().color = new Color32(40, 140, 136, 255);
                ShopButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Costs: " + ShopItem.Cost.ToString();
                ShopButton.GetComponent<Image>().color = new Color32(61, 43, 61, 255);
                ShopButton.GetComponent<Button>().interactable = true;
                
                if(ShopItem.Name.Contains("Forgotten"))
                {
                    if (forgottenSprite == null) forgottenSprite = Resources.Load<Sprite>("Shop/ForgottenSprite");
                    ShopButton.transform.GetChild(0).GetComponent<Image>().sprite = forgottenSprite;
                }
                isEverythingBought = false;
            }

            i++;
        }

        int ContentObjectWidth = 300 + (Mathf.FloorToInt((i-1)/2) * 250);
        ContentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentObjectWidth, 484);
    }

    void DisplayColorItems()
    {
        ClearShopButtons();

        int i = 0;
        foreach (ColorStatus ColorItem in ColorList)
        {
            GameObject ShopButton = Instantiate(ShopButtonPrefab);
            ShopButton.transform.SetParent(ContentObject.transform);
            ShopButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            float xPos = (Mathf.Floor(i / 2f) * 250) + 50;
            float yPos = ((((i + 1) % 2) * 2) - 1) * 120;
            ShopButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, yPos);

            ShopButton.transform.GetChild(0).GetComponent<Image>().sprite = CurrentEquipment[1].Sprite;
            ShopButton.transform.GetChild(0).GetComponent<Image>().color = ColorItem.Color;
            ShopButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ColorItem.Name.Replace("_", " ");
            ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Costs: " + ColorItem.Cost.ToString();

            ShopButton.GetComponent<ShopButtonScript>().ButtonCode = "EquipColors=" + ColorItem.Name;

            if (ColorItem.Status == "Equipped")
            {
                ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Equipped";
                ShopButton.GetComponent<Image>().color = new Color32(48, 131, 220, 255);
                ShopButton.GetComponent<Button>().interactable = false;
            }
            else if (ColorItem.Status == "Available")
            {
                ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Bought";
                ShopButton.GetComponent<Image>().color = new Color32(40, 140, 136, 255);
                ShopButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                ShopButton.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Costs: " + ColorItem.Cost.ToString();
                ShopButton.GetComponent<Image>().color = new Color32(61, 43, 61, 255);
                ShopButton.GetComponent<Button>().interactable = true;
            }

            i++;
        }

        int ContentObjectWidth = 300 + (Mathf.FloorToInt((i - 1) / 2) * 250);
        ContentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentObjectWidth, 484);
    }

    public void ShopButtonListener(string ButtonCode)
    {
        if(ButtonCode.Contains("Tab"))
        {
            audioSource.PlayOneShot(SelectionSound);
            SwitchTab(ButtonCode);
        }
        else if (ButtonCode.Contains("EquipColors"))
        {
            EquipColor(ButtonCode);
        }
        else if (ButtonCode.Contains("Equip"))
        {
            Equip(ButtonCode);
        }
    }

    void SwitchTab(string ButtonCode)
    {
        foreach(GameObject Tab in Tabs)
        {
            Tab.GetComponent<Button>().interactable = true;
        }

        if (ButtonCode == "HatsTab")
        {
            Tabs[0].GetComponent<Button>().interactable = false;
            DisplayShopItems(0);
        }
        else if(ButtonCode == "SkinsTab")
        {
            Tabs[1].GetComponent<Button>().interactable = false;
            DisplayShopItems(1);
        }
        else if (ButtonCode == "ColorsTab")
        {
            Tabs[2].GetComponent<Button>().interactable = false;
            DisplayColorItems();
        }
        else if (ButtonCode == "WallpapersTab")
        {
            Tabs[3].GetComponent<Button>().interactable = false;
            DisplayShopItems(2);
        }
    }

    void Equip(string ButtonCode)
    {
        int Type = 0;

        if (ButtonCode.Split('=')[0] == "EquipHats")
        {
            Type = 0;
        }
        else if (ButtonCode.Split('=')[0] == "EquipSkins")
        {
            Type = 1;
        }
        else if (ButtonCode.Split('=')[0] == "EquipWallpapers")
        {
            Type = 2;
        }

        foreach (SpriteStatus Item in SpriteStatusLists[Type])
        {
            if(Item.Name == ButtonCode.Split('=')[1])
            {
                if (Item.Status == "Available")
                {
                    CurrentEquipment[Type].Status = "Available";
                    CurrentEquipment[Type] = Item;
                    Item.Status = "Equipped";
                    PlayerPrefs.SetString("Current" + ListName[Type], ListName[Type] + "/" + Item.Name + "-" + Item.Cost);
                    
                    audioSource.PlayOneShot(SelectionSound);
                }
                else if (Item.Status == "Unavailable" && Hackeronis >= Item.Cost)
                {
                    CurrentEquipment[Type].Status = "Available";
                    CurrentEquipment[Type] = Item;
                    Item.Status = "Equipped";
                    PlayerPrefs.SetString("Current" + ListName[Type], ListName[Type] + "/" + Item.Name + "-" + Item.Cost);

                    SpendMoney(Item.Cost);
                    
                    audioSource.PlayOneShot(BuySound);
                } else {
                    audioSource.PlayOneShot(FailToBuySound);
                }

                SavePlayerPrefs(Type);
                DisplayCurrentEquipment();
            }
        }
        DisplayShopItems(Type);
    }

    void EquipColor(string ButtonCode)
    {
        foreach (ColorStatus ColorStatus in ColorList)
        {
            if (ColorStatus.Name == ButtonCode.Split('=')[1])
            {
                if (ColorStatus.Status == "Available")
                {
                    CurrentColor.Status = "Available";
                    CurrentColor = ColorStatus;
                    ColorStatus.Status = "Equipped";
                    PlayerPrefs.SetString("CurrentColors", ColorStatus.Name);

                    audioSource.PlayOneShot(SelectionSound);
                }
                else if (ColorStatus.Status == "Unavailable" && Hackeronis >= ColorStatus.Cost)
                {
                    CurrentColor.Status = "Available";
                    CurrentColor = ColorStatus;
                    ColorStatus.Status = "Equipped";
                    PlayerPrefs.SetString("CurrentColors", ColorStatus.Name);

                    SpendMoney(ColorStatus.Cost);

                    audioSource.PlayOneShot(BuySound);
                } else {
                    audioSource.PlayOneShot(FailToBuySound);
                }

                SaveColorPlayerPrefs();
                DisplayCurrentEquipment();
            }
        }
        DisplayColorItems();
    }

    void SpendMoney(int Amount)
    {
        PreviousHackeronis = Hackeronis;
        Hackeronis -= Amount;
        HackeronisAnimationTimer = 1f;

        PlayerPrefs.SetInt("Hackeronis", Hackeronis);
    }

    void SavePlayerPrefs(int Type)
    {
        List<SpriteStatus> SpriteStatusList = SpriteStatusLists[Type];

        string NewPlayerPrefString = "";

        foreach(SpriteStatus SpriteStatus in SpriteStatusList)
        {
            NewPlayerPrefString += SpriteStatus.Name + "-";
            NewPlayerPrefString += SpriteStatus.Cost + "=";
            NewPlayerPrefString += SpriteStatus.Status + ",";
        }

        PlayerPrefs.SetString(ListName[Type], NewPlayerPrefString);

        //print("Save: " + NewPlayerPrefString);
    }

    void SaveColorPlayerPrefs()
    {
        string NewPlayerPrefString = "";

        foreach (ColorStatus ColorStatus in ColorList)
        {
            NewPlayerPrefString += ColorStatus.Name + "-";
            NewPlayerPrefString += ColorStatus.Cost + "=";
            NewPlayerPrefString += ColorStatus.Status + ",";
        }

        PlayerPrefs.SetString("Colors", NewPlayerPrefString);

        //print("Save: " + NewPlayerPrefString);
    }

    static int SortByCost(SpriteStatus One, SpriteStatus Two)
    {
        return One.Cost.CompareTo(Two.Cost);
    }

    // Update is called once per frame
    void Update()
    {
        if (HackeronisAnimationTimer != 0)
        {
            HackeronisAnimation();
        }

        if(Input.GetKeyDown("r"))
        {
            ResetPlayerPrefs();
        }
    }

    void ResetPlayerPrefs()
    {
        for(int i = 0; i < SpriteStatusLists.Length; i++)
        {
            foreach(SpriteStatus SpriteStatus in SpriteStatusLists[i])
            {
                SpriteStatus.Status = "Unavailable";
            }

            SavePlayerPrefs(i);
        }

        foreach (ColorStatus ColorStatus in ColorList)
        {
            ColorStatus.Status = "Unavailable";
        }

        SaveColorPlayerPrefs();

        Hackeronis = 9000;
        PlayerPrefs.SetInt("Hackeronis", Hackeronis);
        DisplayCurrentEquipment();
    }

    void HackeronisAnimation()
    {
        HackeronisAnimationTimer -= Time.deltaTime * 2f;

        if(HackeronisAnimationTimer > 0)
        {
            HackeroniText.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(Hackeronis + ((float)(PreviousHackeronis - Hackeronis) * HackeronisAnimationTimer)) + " Hackeronis";
        }
        else
        {
            HackeroniText.GetComponent<TextMeshProUGUI>().text = Hackeronis + " Hackeronis";
            HackeronisAnimationTimer = 0;
        }
    }
}

public class ColorStatus
{
    public string Name;
    public Color32 Color;
    public int Cost;
    public string Status;
}

public class SpriteStatus
{
    public string Name;
    public Sprite Sprite;
    public int Cost;
    public string Status;
}