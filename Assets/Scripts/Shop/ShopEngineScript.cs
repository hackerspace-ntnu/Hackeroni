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

    private SpriteStatus CurrentHat;
    private SpriteStatus CurrentSkin;
    private SpriteStatus CurrentWallpaper;
    private int Hackeronis;

    private List<SpriteStatus> HatList = new List<SpriteStatus>();
    private List<SpriteStatus> SkinList = new List<SpriteStatus>();
    private List<SpriteStatus> WallpaperList = new List<SpriteStatus>();
    private List<ColorStatus> ColorList = new List<ColorStatus>();

    // Start is called before the first frame update
    void Start()
    {
        Hackeronis = PlayerPrefs.GetInt("Hackeronis", 0);
        Hackeronis = 1000;
        LoadPlayerPrefs("Hats");
        LoadPlayerPrefs("Skins");
        LoadPlayerPrefs("Wallpapers");
        LoadColorPlayerPrefs();

        print(CurrentHat.Name + ", " + CurrentSkin.Name);
        HatDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Hats/" + CurrentHat.Name + "-" + CurrentHat.Cost);
        SkinDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Skins/" + CurrentSkin.Name + "-" + CurrentSkin.Cost);
        //WallpaperDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Wallpapers/" + CurrentWallpaper.Name);

        DisplayHats();
        HackeroniText.GetComponent<TextMeshProUGUI>().text = Hackeronis + " Hackeronis";
    }

    void LoadPlayerPrefs(string Type)
    {
        List<Sprite> SpriteList = new List<Sprite>(Resources.LoadAll<Sprite>(Type));

        string PlayerPrefString = PlayerPrefs.GetString(Type, "");
        string[] PlayerPrefArray = PlayerPrefString.Split(',');

        string NewPlayerPrefString = "";

        if(Type == "Hats")
        {
            HatList.Clear();
        }
        else if (Type == "Skins")
        {
            SkinList.Clear();
        }
        else if (Type == "Wallpapers")
        {
            WallpaperList.Clear();
        }

        bool NothingEquipped = true;
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
                        if(Avalability == "Equipped")
                        {
                            NothingEquipped = false;
                        }
                    }
                }
            }

            SpriteStatus newSpriteStatus = new SpriteStatus() { Sprite = Sprite, Name = Sprite.name.Split('-')[0], Cost = int.Parse(Sprite.name.Split('-')[1]), Status = Avalability };

            if (Type == "Hats")
            {
                HatList.Add(newSpriteStatus);

                if(newSpriteStatus.Status == "Equipped")
                {
                    CurrentHat = newSpriteStatus;

                    if (CurrentHat.Name == "None")
                    {
                        HatDisplay.SetActive(false);
                    }
                    else
                    {
                        HatDisplay.SetActive(true);
                    }
                }
            }
            else if (Type == "Skins")
            {
                SkinList.Add(newSpriteStatus);

                if (newSpriteStatus.Status == "Equipped")
                {
                    CurrentSkin = newSpriteStatus;
                }
            }
            else if (Type == "Wallpapers")
            {
                WallpaperList.Add(newSpriteStatus);

                if (newSpriteStatus.Status == "Equipped")
                {
                    CurrentWallpaper = newSpriteStatus;
                }
            }

            NewPlayerPrefString += Sprite.name + "=" + Avalability + ",";
        }

        if (Type == "Hats")
        {
            if(NothingEquipped == true)
            {
                foreach(SpriteStatus Hat in HatList)
                {
                    if(Hat.Name == "None")
                    {
                        CurrentHat = Hat;
                        Hat.Status = "Equipped";
                    }
                }
            }
            HatList.Sort(SortByCost);
        }
        else if (Type == "Skins")
        {
            if (NothingEquipped == true)
            {
                foreach (SpriteStatus Skin in SkinList)
                {
                    if (Skin.Name == "Default")
                    {
                        CurrentSkin = Skin;
                        Skin.Status = "Equipped";
                    }
                }
            }
            SkinList.Sort(SortByCost);
        }
        else if (Type == "Wallpapers")
        {
            WallpaperList.Sort(SortByCost);
        }

        PlayerPrefs.SetString(Type, NewPlayerPrefString);

        print("Load: " + NewPlayerPrefString);
    }

    void LoadColorPlayerPrefs()
    {
        ColorList = new List<ColorStatus>()
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
            new ColorStatus() { Name = "Black Hole", Color = new Color32(0, 0, 0, 255)},
        };

        string PlayerPrefString = PlayerPrefs.GetString("Color", "");
        string[] PlayerPrefArray = PlayerPrefString.Split(',');
        string newPlayerPrefString = "";

        for(int i = 0; i < ColorList.Count; i++)
        {
            ColorList[i].Cost = 120 * i;

            string Availability = "Unavailable";
            if(i == 0)
            {
                Availability = "Available";
            }

            foreach(string ColorStatus in PlayerPrefArray)
            {
                if(ColorStatus.Split('=')[0] == ColorList[i].Name)
                {
                    Availability = ColorList[i].Status;
                }
            }

            ColorList[i].Status = Availability;

            newPlayerPrefString += ColorList[i].Name + "=" + Availability;
        }
    }

    void ClearShopButtons()
    {
        foreach(Transform child in ContentObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayHats() {

        ClearShopButtons();

        int i = 0;
        foreach (SpriteStatus Hat in HatList)
        {
            GameObject ShopButton = Instantiate(ShopButtonPrefab);
            ShopButton.transform.SetParent(ContentObject.transform);
            ShopButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            float xPos = (Mathf.Floor(i / 2f) * 250) + 50;
            float yPos = ((((i + 1) % 2) * 2) - 1) * 120;
            ShopButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, yPos);

            ShopButton.transform.GetChild(0).GetComponent<Image>().sprite = Hat.Sprite;
            ShopButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Hat.Name.Replace("_", " ");

            ShopButton.GetComponent<ShopButtonScript>().ButtonCode = "EquipHat=" + Hat.Name;

            i++;
        }

        int ContentObjectWidth = 300 + (Mathf.FloorToInt((i-1)/2) * 250);
        ContentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentObjectWidth, 484);
    }

    void DisplaySkins()
    {
        ClearShopButtons();

        int i = 0;
        foreach (SpriteStatus Skin in SkinList)
        {
            GameObject ShopButton = Instantiate(ShopButtonPrefab);
            ShopButton.transform.SetParent(ContentObject.transform);
            ShopButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            float xPos = (Mathf.Floor(i / 2f) * 250) + 50;
            float yPos = ((((i + 1) % 2) * 2) - 1) * 120;
            ShopButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, yPos);

            ShopButton.transform.GetChild(0).GetComponent<Image>().sprite = Skin.Sprite;
            ShopButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Skin.Name.Replace("_", " ");

            ShopButton.GetComponent<ShopButtonScript>().ButtonCode = "EquipSkin=" + Skin.Name;

            i++;
        }

        int ContentObjectWidth = 300 + (Mathf.FloorToInt((i-1) / 2) * 250);
        ContentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentObjectWidth, 484);
    }

    void DisplayColors()
    {
        ClearShopButtons();

        int i = 0;
        foreach (ColorStatus SkinColor in ColorList)
        {
            GameObject ShopButton = Instantiate(ShopButtonPrefab);
            ShopButton.transform.SetParent(ContentObject.transform);
            ShopButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            float xPos = (Mathf.Floor(i / 2f) * 250) + 50;
            float yPos = ((((i + 1) % 2) * 2) - 1) * 120;
            ShopButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, yPos);

            ShopButton.transform.GetChild(0).GetComponent<Image>().sprite = CurrentSkin.Sprite;
            ShopButton.transform.GetChild(0).GetComponent<Image>().color = SkinColor.Color;
            ShopButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = SkinColor.Name;

            i++;
        }

        int ContentObjectWidth = 300 + (Mathf.FloorToInt((i-1) / 2) * 250);
        ContentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentObjectWidth, 484);
    }

    void DisplayWallpapers()
    {
        ClearShopButtons();

        int i = 0;
        foreach (SpriteStatus Wallpaper in WallpaperList)
        {
            GameObject ShopButton = Instantiate(ShopButtonPrefab);
            ShopButton.transform.SetParent(ContentObject.transform);
            ShopButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            float xPos = (Mathf.Floor(i / 2f) * 250) + 50;
            float yPos = ((((i + 1) % 2) * 2) - 1) * 120;
            ShopButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, yPos);

            ShopButton.transform.GetChild(0).GetComponent<Image>().sprite = Wallpaper.Sprite;
            ShopButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Wallpaper.Name.Replace("_", " ");

            ShopButton.GetComponent<ShopButtonScript>().ButtonCode = "EquipWallpaper=" + Wallpaper.Name;

            i++;
        }

        int ContentObjectWidth = 300 + (Mathf.FloorToInt((i-1) / 2) * 250);
        ContentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentObjectWidth, 484);
    }

    public void ShopButtonListener(string ButtonCode)
    {
        if(ButtonCode.Contains("Tab"))
        {
            SwitchTab(ButtonCode);
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
            DisplayHats();
        }
        else if(ButtonCode == "SkinsTab")
        {
            Tabs[1].GetComponent<Button>().interactable = false;
            DisplaySkins();
        }
        else if (ButtonCode == "ColorsTab")
        {
            Tabs[2].GetComponent<Button>().interactable = false;
            DisplayColors();
        }
        else if (ButtonCode == "WallpapersTab")
        {
            Tabs[3].GetComponent<Button>().interactable = false;
            DisplayWallpapers();
        }
    }

    void Equip(string ButtonCode)
    {
        if(ButtonCode.Split('=')[0] == "EquipHat")
        {
            foreach(SpriteStatus Hat in HatList)
            {
                if(Hat.Name == ButtonCode.Split('=')[1])
                {
                    if (Hat.Status == "Available")
                    {
                        CurrentHat.Status = "Available";
                        CurrentHat = Hat;
                        Hat.Status = "Equipped";
                        HatDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Hats/" + Hat.Name + "-" + CurrentHat.Cost);

                        if (Hat.Name == "None")
                        {
                            HatDisplay.SetActive(false);
                        }
                        else
                        {
                            HatDisplay.SetActive(true);
                        }
                    }
                    else if(Hat.Status == "Unavailable")
                    {
                        if (Hackeronis >= Hat.Cost)
                        {
                            CurrentHat.Status = "Available";
                            CurrentHat = Hat;
                            Hat.Status = "Equipped";

                            Hackeronis -= Hat.Cost;
                            HatDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Hats/" + Hat.Name + "-" + CurrentHat.Cost);

                            if(Hat.Name == "None")
                            {
                                HatDisplay.SetActive(false);
                            }
                            else
                            {
                                HatDisplay.SetActive(true);
                            }
                        }
                    }

                    SavePlayerPrefs("Hats");
                }
            }
        }
        else if (ButtonCode.Split('=')[0] == "EquipSkin")
        {
            foreach (SpriteStatus Skin in SkinList)
            {
                if (Skin.Name == ButtonCode.Split('=')[1])
                {
                    if (Skin.Status == "Available")
                    {
                        CurrentSkin.Status = "Available";
                        CurrentSkin = Skin;
                        Skin.Status = "Equipped";
                        SkinDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Skin/" + Skin.Name + "-" + CurrentSkin.Cost);
                    }
                    else if (Skin.Status == "Unavailable")
                    {
                        if (Hackeronis >= Skin.Cost)
                        {
                            CurrentHat.Status = "Available";
                            CurrentHat = Skin;
                            Skin.Status = "Equipped";

                            Hackeronis -= Skin.Cost;
                            SkinDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Skin/" + Skin.Name + "-" + CurrentSkin.Cost);
                        }
                    }
                    SavePlayerPrefs("Skins");
                }
            }
        }
        else if (ButtonCode.Split('=')[0] == "EquipWallpaper")
        {
            foreach (SpriteStatus Wallpaper in WallpaperList)
            {
                if (Wallpaper.Name == ButtonCode.Split('=')[1])
                {
                    if (Wallpaper.Status == "Available")
                    {
                        CurrentWallpaper.Status = "Available";
                        CurrentWallpaper = Wallpaper;
                        Wallpaper.Status = "Equipped";
                        WallpaperDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Wallpaper/" + Wallpaper.Name + "-" + CurrentWallpaper.Cost);
                    }
                    else if (Wallpaper.Status == "Unavailable")
                    {
                        if (Hackeronis >= Wallpaper.Cost)
                        {
                            CurrentWallpaper.Status = "Available";
                            CurrentWallpaper = Wallpaper;
                            Wallpaper.Status = "Equipped";

                            Hackeronis -= Wallpaper.Cost;
                            WallpaperDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Wallpaper/" + Wallpaper.Name + "-" + CurrentWallpaper.Cost);
                        }
                    }
                    SavePlayerPrefs("Wallpapers");
                }
            }
        }

        HackeroniText.GetComponent<TextMeshProUGUI>().text = Hackeronis + " Hackeronis";
    }

    void SavePlayerPrefs(string Type)
    {
        List<SpriteStatus> SpriteStatusList = new List<SpriteStatus>();

        if (Type == "Hats")
        {
            SpriteStatusList = HatList;
        }
        else if (Type == "Skins")
        {
            SpriteStatusList = SkinList;
        }
        else if (Type == "Wallpapers")
        {
            SpriteStatusList = WallpaperList;
        }

        string NewPlayerPrefString = "";

        foreach(SpriteStatus SpriteStatus in SpriteStatusList)
        {
            NewPlayerPrefString += SpriteStatus.Name + "-";
            NewPlayerPrefString += SpriteStatus.Cost + "=";
            NewPlayerPrefString += SpriteStatus.Status + ",";
        }

        PlayerPrefs.SetString(Type, NewPlayerPrefString);

        print("Save: " + NewPlayerPrefString);
    }

    static int SortByCost(SpriteStatus One, SpriteStatus Two)
    {
        return One.Cost.CompareTo(Two.Cost);
    }

    // Update is called once per frame
    void Update()
    {
        
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