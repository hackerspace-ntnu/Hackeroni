using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopEngineScript : MonoBehaviour
{

    public GameObject HatDisplay;
    public GameObject SkinDisplay;

    public GameObject ShopButtonPrefab;
    public GameObject ContentObject;

    public List<GameObject> Tabs = new List<GameObject>();

    private string CurrentTab;

    private SpriteStatus CurrentHat;
    private SpriteStatus CurrentSkin;
    private SpriteStatus CurrentWallpaper;

    private List<SpriteStatus> HatList = new List<SpriteStatus>();
    private List<SpriteStatus> SkinList = new List<SpriteStatus>();
    private List<SpriteStatus> WallpaperList = new List<SpriteStatus>();
    private List<ColorStatus> ColorList = new List<ColorStatus>();

    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerPrefs("Hats");
        LoadPlayerPrefs("Skins");
        LoadPlayerPrefs("Wallpapers");
        LoadColorPlayerPrefs();

        DisplayHats();
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

        foreach (Sprite Sprite in SpriteList)
        {
            string Avalability = "Unavailable";

            if(Sprite.name.Split('-')[0] == "None" || Sprite.name.Split('-')[0] == "Default")
            {
                Avalability = "Equipped";
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

            if (Sprite.name.Split('-')[0] == "None" || Sprite.name.Split('-')[0] == "Default")
            {
                Avalability = "Equipped";
            }


            SpriteStatus newSpriteStatus = new SpriteStatus() { Sprite = Sprite, Name = Sprite.name.Split('-')[0], Cost = int.Parse(Sprite.name.Split('-')[1]), Status = Avalability };

            if (Type == "Hats")
            {
                HatList.Add(newSpriteStatus);

                if(newSpriteStatus.Status == "Equipped")
                {
                    CurrentHat = newSpriteStatus;
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
            HatList.Sort(SortByCost);
        }
        else if (Type == "Skins")
        {
            SkinList.Sort(SortByCost);
        }
        else if (Type == "Wallpapers")
        {
            WallpaperList.Sort(SortByCost);
        }

        PlayerPrefs.SetString(Type, NewPlayerPrefString);

        print(NewPlayerPrefString);
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