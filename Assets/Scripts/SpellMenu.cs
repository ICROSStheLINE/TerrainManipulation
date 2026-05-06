using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellMenu : MonoBehaviour
{

    SimpleFPSController simpleFPSController;
    [SerializeField] GameObject buttonGameObjectPrefab;
    float buttonWidth = 75;
    float buttonHeight = 75;
    static int castStartWidth = 3;
    static int castStartHeight = 1;
    float castStartStartingPointX = 100;
    float castStartStartingPointY = 200;
    SpellSlot[,] castStartMap = new SpellSlot[castStartHeight,castStartWidth];
    static int castContinuousWidth = 3;
    static int castContinuousHeight = 1;
    float castContinuousStartingPointX = 100;
    float castContinuousStartingPointY = 0;
    SpellSlot[,] castContinuousMap = new SpellSlot[castContinuousHeight,castContinuousWidth];
    static int castEndWidth = 3;
    static int castEndHeight = 1;
    float castEndStartingPointX = 100;
    float castEndStartingPointY = -200;
    SpellSlot[,] castEndMap = new SpellSlot[castEndHeight,castEndWidth];
    [SerializeField] Transform canvasTransform;
    static int inventoryWidth = 1;
    static int inventoryHeight = 3;
    float inventoryStartingPointX = -600;
    float inventoryStartingPointY = 0;
    SpellSlot[,] spellInventoryMap = new SpellSlot[inventoryHeight,inventoryWidth];
    SpellSlot.SpellType heldSpell = SpellSlot.SpellType.Empty;


    void Start()
    {
        simpleFPSController = GetComponent<SimpleFPSController>();
        PopulateSpellMenuMaps();
        PopulateSpellInventoryMap();
        spellInventoryMap[0,0].AssignSpell(SpellSlot.SpellType.Ball);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            simpleFPSController.SetCameraUseState(false);
            OpenSpellMenu(true);
            OpenInventory(true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            simpleFPSController.SetCameraUseState(true);
            OpenSpellMenu(false);
            OpenInventory(false);
        }
    }

    void InteractWithSlot(SpellSlot spellSlot)
    {
        if (spellSlot.spellType == SpellSlot.SpellType.Empty &&
            heldSpell != SpellSlot.SpellType.Empty)
        {
            spellSlot.AssignSpell(heldSpell);
            heldSpell = SpellSlot.SpellType.Empty;
            return;
        }
        
        if (spellSlot.spellType != SpellSlot.SpellType.Empty &&
            heldSpell == SpellSlot.SpellType.Empty)
        {
            heldSpell = spellSlot.spellType;
            spellSlot.PickUpSpell();
            return;
        }
    }

    void PopulateSpellInventoryMap()
    {
        for (int i = 0; i < inventoryHeight; i++)
        {
            for (int j = 0; j < inventoryWidth; j++)
            {
                spellInventoryMap[i,j] = new SpellSlot();
                spellInventoryMap[i,j].uiObject = Instantiate(buttonGameObjectPrefab);
                spellInventoryMap[i,j].uiObject.transform.SetParent(canvasTransform, false);
                RectTransform rect = spellInventoryMap[i,j].uiObject.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    inventoryStartingPointX + (buttonWidth * j),
                    inventoryStartingPointY + (buttonHeight * i)
                );
                spellInventoryMap[i,j].uiObject.SetActive(false);
                Button button = spellInventoryMap[i,j].uiObject.GetComponent<Button>();
                SpellSlot spellSlot = spellInventoryMap[i,j];
                button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
                spellInventoryMap[i,j].uiObject.name = "spellInventoryButton[" + i + "," + j + "]";
            }
        }
    }

    void PopulateSpellMenuMaps()
    {
        for (int i = 0; i < castStartHeight; i++)
        {
            for (int j = 0; j < castStartWidth; j++)
            {
                castStartMap[i,j] = new SpellSlot();
                castStartMap[i,j].uiObject = Instantiate(buttonGameObjectPrefab);
                castStartMap[i,j].uiObject.transform.SetParent(canvasTransform, false);
                RectTransform rect = castStartMap[i,j].uiObject.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    castStartStartingPointX + (buttonWidth * j),
                    castStartStartingPointY + (buttonHeight * i)
                );
                castStartMap[i,j].uiObject.SetActive(false);
                Button button = castStartMap[i,j].uiObject.GetComponent<Button>();
                SpellSlot spellSlot = castStartMap[i,j];
                button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
                castStartMap[i,j].uiObject.name = "spellStartButton[" + i + "," + j + "]";
            }
        }

        for (int i = 0; i < castContinuousHeight; i++)
        {
            for (int j = 0; j < castContinuousWidth; j++)
            {
                castContinuousMap[i,j] = new SpellSlot();
                castContinuousMap[i,j].uiObject = Instantiate(buttonGameObjectPrefab);
                castContinuousMap[i,j].uiObject.transform.SetParent(canvasTransform, false);
                RectTransform rect = castContinuousMap[i,j].uiObject.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    castContinuousStartingPointX + (buttonWidth * j),
                    castContinuousStartingPointY + (buttonHeight * i)
                );
                castContinuousMap[i,j].uiObject.SetActive(false);
                Button button = castContinuousMap[i,j].uiObject.GetComponent<Button>();
                SpellSlot spellSlot = castContinuousMap[i,j];
                button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
                castContinuousMap[i,j].uiObject.name = "spellContinuousButton[" + i + "," + j + "]";
            }
        }

        for (int i = 0; i < castEndHeight; i++)
        {
            for (int j = 0; j < castEndWidth; j++)
            {
                castEndMap[i,j] = new SpellSlot();
                castEndMap[i,j].uiObject = Instantiate(buttonGameObjectPrefab);
                castEndMap[i,j].uiObject.transform.SetParent(canvasTransform, false);
                RectTransform rect = castEndMap[i,j].uiObject.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    castEndStartingPointX + (buttonWidth * j),
                    castEndStartingPointY + (buttonHeight * i)
                );
                castEndMap[i,j].uiObject.SetActive(false);
                Button button = castEndMap[i,j].uiObject.GetComponent<Button>();
                SpellSlot spellSlot = castEndMap[i,j];
                button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
                castEndMap[i,j].uiObject.name = "spellEndButton[" + i + "," + j + "]";
            }
        }
    }

    void OpenInventory(bool openState)
    {
        for (int i = 0; i < inventoryHeight; i++) {
            for (int j = 0; j < inventoryWidth; j++) {
                spellInventoryMap[i,j].uiObject.SetActive(openState);
            }
        }
    }

    void OpenSpellMenu(bool openState)
    {
        for (int i = 0; i < castStartHeight; i++) {
            for (int j = 0; j < castStartWidth; j++) {
                castStartMap[i,j].uiObject.SetActive(openState);
            }
        }
        for (int i = 0; i < castContinuousHeight; i++) {
            for (int j = 0; j < castContinuousWidth; j++) {
                castContinuousMap[i,j].uiObject.SetActive(openState);
            }
        }
        for (int i = 0; i < castEndHeight; i++) {
            for (int j = 0; j < castEndWidth; j++) {
                castEndMap[i,j].uiObject.SetActive(openState);
            }
        }
    }
}

public class SpellSlot
{
    public GameObject uiObject;
    public enum SpellType { Empty, Ball, Cube }
    public SpellType spellType;
    GameObject spellIcon;
    public void PickUpSpell()
    {
        if (spellType == SpellType.Empty)
        {
            return; // If it's empty then there's nothing to pick up!
        }

        GameObject.Destroy(spellIcon);

        spellType = SpellType.Empty;
    }
    public void AssignSpell(SpellType spellType)
    {
        if (this.spellType != SpellType.Empty)
        {
            return; // If it has something in the slot then you can't assign it something else! (TODO: Allow spell swapping)
        }

        CreateSpellIcon(spellType);

        this.spellType = spellType;
    }

    void CreateSpellIcon(SpellType spellType)
    {
        spellIcon = new GameObject();
        spellIcon.name = spellType.ToString();
        spellIcon.transform.SetParent(uiObject.transform, false);
        TextMeshProUGUI tmp = spellIcon.AddComponent<TextMeshProUGUI>();
        tmp.text = spellType.ToString();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 24;
        tmp.color = Color.black;
    }
}
