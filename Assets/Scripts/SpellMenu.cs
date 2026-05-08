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
    GameObject castStartLabel;
    SpellSlot[,] castStartMap = new SpellSlot[castStartHeight,castStartWidth];
    static int castContinuousWidth = 3;
    static int castContinuousHeight = 1;
    float castContinuousStartingPointX = 100;
    float castContinuousStartingPointY = 0;
    GameObject castContinuousLabel;
    SpellSlot[,] castContinuousMap = new SpellSlot[castContinuousHeight,castContinuousWidth];
    /*static int castEndWidth = 0;
    static int castEndHeight = 0;
    float castEndStartingPointX = 100;
    float castEndStartingPointY = -200;
    SpellSlot[,] castEndMap = new SpellSlot[castEndHeight,castEndWidth];*/
    [SerializeField] Transform canvasTransform;
    static int inventoryWidth = 1;
    static int inventoryHeight = 3;
    float inventoryStartingPointX = -600;
    float inventoryStartingPointY = 0;
    GameObject spellInventoryLabel;
    SpellSlot[,] spellInventoryMap = new SpellSlot[inventoryHeight,inventoryWidth];
    SpellSlot.SpellType heldSpell = SpellSlot.SpellType.Empty;
    [SerializeField] GameObject ballPrefab;
    ManaManager manaManager;


    void Start()
    {
        manaManager = GameObject.Find("ManaManager").transform.GetComponent<ManaManager>();
        simpleFPSController = GetComponent<SimpleFPSController>();
        PopulateSpellMenuMaps();
        PopulateSpellInventoryMap();
        PopulateUILabels();
        spellInventoryMap[0,0].AssignSpell(SpellSlot.SpellType.Ball);
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            CastContinuous();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CastStart();
        }

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

    void CastStart()
    {
        // Iterate through the list of spell slots in the CastStartMap array.
        // If a non-empty spell slot is found (AKA if spellSlot.spellType isn't empty)...
        // ...do whatever unique action that spelltype is for.
        // In the case of "Ball" (for testing purposes), summon a ball.
        for (int i = 0; i < castStartHeight; i++)
        {
            for (int j = 0; j < castStartWidth; j++)
            {
                if (castStartMap[i,j].spellType == SpellSlot.SpellType.Empty)
                { continue; }

                if (castStartMap[i,j].spellType == SpellSlot.SpellType.Ball)
                {
                    if (manaManager.manaAmount > 0)
                    {
                        Instantiate(ballPrefab, transform.position + transform.forward, transform.rotation);
                        manaManager.LoseMana(5);
                    }
                }
            }
        }
    }

    void CastContinuous()
    {
        for (int i = 0; i < castContinuousHeight; i++)
        {
            for (int j = 0; j < castContinuousWidth; j++)
            {
                if (castContinuousMap[i,j].spellType == SpellSlot.SpellType.Empty)
                { continue; }

                if (castContinuousMap[i,j].spellType == SpellSlot.SpellType.Ball)
                {
                    if (manaManager.manaAmount > 0)
                    {
                        Instantiate(ballPrefab, transform.position + transform.forward, transform.rotation);
                        manaManager.LoseMana(5);
                    }
                }
            }
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

        /*for (int i = 0; i < castEndHeight; i++)
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
        }*/
    }

    void OpenInventory(bool openState)
    {
        spellInventoryLabel.SetActive(openState);
        for (int i = 0; i < inventoryHeight; i++) {
            for (int j = 0; j < inventoryWidth; j++) {
                spellInventoryMap[i,j].uiObject.SetActive(openState);
            }
        }
    }

    void OpenSpellMenu(bool openState)
    {
        castStartLabel.SetActive(openState);
        for (int i = 0; i < castStartHeight; i++) {
            for (int j = 0; j < castStartWidth; j++) {
                castStartMap[i,j].uiObject.SetActive(openState);
            }
        }
        castContinuousLabel.SetActive(openState);
        for (int i = 0; i < castContinuousHeight; i++) {
            for (int j = 0; j < castContinuousWidth; j++) {
                castContinuousMap[i,j].uiObject.SetActive(openState);
            }
        }
        /*for (int i = 0; i < castEndHeight; i++) {
            for (int j = 0; j < castEndWidth; j++) {
                castEndMap[i,j].uiObject.SetActive(openState);
            }
        }*/
    }
    
    void PopulateUILabels()
    {
        // Cast Start Label
        castStartLabel = new GameObject();
        castStartLabel.name = "castStartLabel";
        castStartLabel.transform.SetParent(canvasTransform, false);
        TextMeshProUGUI tmpA = castStartLabel.AddComponent<TextMeshProUGUI>();
        tmpA.text = "Cast\nStart";
        tmpA.alignment = TextAlignmentOptions.Right;
        tmpA.fontSize = 30;
        tmpA.color = Color.red;
        RectTransform rectA = castStartLabel.GetComponent<RectTransform>();
        rectA.anchoredPosition = new Vector2(
            castStartStartingPointX - buttonWidth*2,
            castStartStartingPointY
        );
        castStartLabel.SetActive(false);

        // Cast Continuous Label
        castContinuousLabel = new GameObject();
        castContinuousLabel.name = "castContinuousLabel";
        castContinuousLabel.transform.SetParent(canvasTransform, false);
        TextMeshProUGUI tmpB = castContinuousLabel.AddComponent<TextMeshProUGUI>();
        tmpB.text = "Cast\nContinuous";
        tmpB.alignment = TextAlignmentOptions.Right;
        tmpB.fontSize = 30;
        tmpB.color = Color.red;
        RectTransform rectB = castContinuousLabel.GetComponent<RectTransform>();
        rectB.anchoredPosition = new Vector2(
            castContinuousStartingPointX - buttonWidth*2,
            castContinuousStartingPointY
        );
        castContinuousLabel.SetActive(false);

        // Inventory Label
        spellInventoryLabel = new GameObject();
        spellInventoryLabel.name = "spellInventoryLabel";
        spellInventoryLabel.transform.SetParent(canvasTransform, false);
        TextMeshProUGUI tmpD = spellInventoryLabel.AddComponent<TextMeshProUGUI>();
        tmpD.text = "Inventory";
        tmpD.alignment = TextAlignmentOptions.Center;
        tmpD.fontSize = 30;
        tmpD.color = Color.red;
        RectTransform rectD = spellInventoryLabel.GetComponent<RectTransform>();
        rectD.anchoredPosition = new Vector2(
            inventoryStartingPointX,
            inventoryStartingPointY - buttonWidth
        );
        spellInventoryLabel.SetActive(false);
    }
}

public class SpellSlot
{
    public GameObject uiObject;
    public enum SpellType { Empty, Ball, Cube }
    // ^^^ TODO: Change this from being an enum to being a whole class.
    // That way we can write the FUNCTION of each spelltype IN THE CLASS ITSELF.
    // Actually wait no.
    // Instead, we can make the class, but make it a STATIC class that just returns the function of each one of these enum types.
    // It would basically just be a static class with ONE function in it that is called "GetSpellTypeFunction" or something.
    // That function would have like a billion if statements checking which enum type was passed in as an argument.
    // Then it would return a thing to do, like spawning in a ball, or deleting a block.
    // But what datatype should that return be??
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
