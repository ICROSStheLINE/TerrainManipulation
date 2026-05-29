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
    public static int castStartWidth = 15;
    public static int castStartHeight = 1;
    float castStartStartingPointX = -300;
    float castStartStartingPointY = 200;
    GameObject castStartLabel;
    public SpellSlot[,] castStartMap = new SpellSlot[castStartHeight,castStartWidth];
    public static int castContinuousWidth = 5;
    public static int castContinuousHeight = 1;
    float castContinuousStartingPointX = 100;
    float castContinuousStartingPointY = 0;
    GameObject castContinuousLabel;
    public SpellSlot[,] castContinuousMap = new SpellSlot[castContinuousHeight,castContinuousWidth];
    [SerializeField] Transform canvasTransform;
    static int inventoryWidth = 3;
    static int inventoryHeight = 6;
    float inventoryStartingPointX = -600;
    float inventoryStartingPointY = 0;
    GameObject spellInventoryLabel;
    public SpellSlot[,] spellInventoryMap = new SpellSlot[inventoryHeight,inventoryWidth];
    SpellSlot heldSpellSlot = new SpellSlot();


    void Start()
    {
        simpleFPSController = GetComponent<SimpleFPSController>();
        PopulateSpellMenuMaps();
        PopulateSpellInventoryMap();
        PopulateUILabels();
        spellInventoryMap[0,0].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[1,0].AssignSpell(SpellSlot.SpellType.EggA);
        spellInventoryMap[2,0].AssignSpell(SpellSlot.SpellType.OpenParenthesisA);
        spellInventoryMap[3,0].AssignSpell(SpellSlot.SpellType.CloseParenthesisA);
        spellInventoryMap[4,0].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[5,0].AssignSpell(SpellSlot.SpellType.Spark);
        spellInventoryMap[0,1].AssignSpell(SpellSlot.SpellType.OpenParenthesisA);
        spellInventoryMap[1,1].AssignSpell(SpellSlot.SpellType.CloseParenthesisA);
        spellInventoryMap[2,1].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[3,1].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[4,1].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[5,1].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[0,2].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[1,2].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[2,2].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[3,2].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[4,2].AssignSpell(SpellSlot.SpellType.Ball);
        spellInventoryMap[5,2].AssignSpell(SpellSlot.SpellType.Ball);
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
            heldSpellSlot.spellType != SpellSlot.SpellType.Empty)
        {
            spellSlot.AssignSpell(heldSpellSlot.spellType);
            spellSlot.CopySpellInputs(heldSpellSlot);
            heldSpellSlot.ClearSpell();
            return;
        }
        
        if (spellSlot.spellType != SpellSlot.SpellType.Empty &&
            heldSpellSlot.spellType == SpellSlot.SpellType.Empty)
        {
            heldSpellSlot.spellType = spellSlot.spellType;
            heldSpellSlot.CopySpellInputs(spellSlot);
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
                spellInventoryMap[i,j] = new SpellSlot(false);
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
                castStartMap[i,j] = new SpellSlot(true);
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
                castContinuousMap[i,j] = new SpellSlot(false);
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
    public enum SpellType { Empty, Ball, Cube, EggA, OpenParenthesisA, CloseParenthesisA, Spark }
    public SpellType spellType;
    public float manaResistance = 1;
    GameObject spellIcon;
    List<GameObject> spellInputObjects = new List<GameObject>();
    TMP_InputField manaResistanceInput;
    bool showsSpellInputs;

    public SpellSlot(bool showsSpellInputs = false)
    {
        this.showsSpellInputs = showsSpellInputs;
    }

    public void ClearSpell()
    {
        spellType = SpellType.Empty;
        manaResistance = 1;
    }

    public void CopySpellInputs(SpellSlot spellSlot)
    {
        manaResistance = spellSlot.manaResistance;

        if (manaResistanceInput != null)
        {
            manaResistanceInput.text = manaResistance.ToString();
        }
    }

    public void PickUpSpell()
    {
        if (spellType == SpellType.Empty)
        {
            return; // If it's empty then there's nothing to pick up!
        }

        GameObject.Destroy(spellIcon);
        DestroySpellInputs();

        ClearSpell();
    }
    public void AssignSpell(SpellType spellType)
    {
        if (this.spellType != SpellType.Empty)
        {
            return; // If it has something in the slot then you can't assign it something else! (TODO: Allow spell swapping)
        }

        CreateSpellIcon(spellType);

        this.spellType = spellType;

        if (HasManaResistanceInput(spellType) && showsSpellInputs)
        {
            CreateManaResistanceInput();
        }
    }

    void CreateSpellIcon(SpellType spellType)
    {
        spellIcon = new GameObject();
        spellIcon.name = spellType.ToString();
        spellIcon.transform.SetParent(uiObject.transform, false);
        TextMeshProUGUI tmp = spellIcon.AddComponent<TextMeshProUGUI>();
        tmp.text = spellType.ToString();
        if (spellType == SpellType.OpenParenthesisA)
            { tmp.text = "(A"; }
        if (spellType == SpellType.CloseParenthesisA)
            { tmp.text = ")A"; }
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 24;
        tmp.color = Color.black;
    }

    bool HasManaResistanceInput(SpellType spellType)
    {
        if (spellType == SpellType.EggA)
        { return true; }

        return false;
    }

    void CreateManaResistanceInput()
    {
        manaResistanceInput = CreateFloatInput(
            "ManaResistanceInput",
            "MR",
            manaResistance,
            new Vector2(0, -55),
            delegate (float newValue) { manaResistance = newValue; }
        );
    }

    TMP_InputField CreateFloatInput(string inputName, string labelText, float startingValue, Vector2 anchoredPosition, System.Action<float> onValueChanged)
    {
        GameObject inputObject = new GameObject();
        inputObject.name = inputName;
        inputObject.transform.SetParent(uiObject.transform, false);
        spellInputObjects.Add(inputObject);

        RectTransform rect = inputObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(70, 25);
        rect.anchoredPosition = anchoredPosition;

        Image image = inputObject.AddComponent<Image>();
        image.color = Color.white;

        TMP_InputField input = inputObject.AddComponent<TMP_InputField>();
        input.contentType = TMP_InputField.ContentType.DecimalNumber;
        input.targetGraphic = image;

        GameObject textObject = new GameObject();
        textObject.name = "Text";
        textObject.transform.SetParent(inputObject.transform, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.color = Color.black;
        text.fontSize = 16;
        text.alignment = TextAlignmentOptions.Center;
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        input.textComponent = text;

        GameObject placeholderObject = new GameObject();
        placeholderObject.name = "Placeholder";
        placeholderObject.transform.SetParent(inputObject.transform, false);
        TextMeshProUGUI placeholder = placeholderObject.AddComponent<TextMeshProUGUI>();
        placeholder.text = labelText;
        placeholder.color = Color.gray;
        placeholder.fontSize = 16;
        placeholder.alignment = TextAlignmentOptions.Center;
        RectTransform placeholderRect = placeholderObject.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        input.placeholder = placeholder;
        input.text = startingValue.ToString();

        input.onValueChanged.AddListener(delegate (string textValue)
        {
            float floatValue;
            if (float.TryParse(textValue, out floatValue))
            {
                onValueChanged(floatValue);
            }
        });

        return input;
    }

    void DestroySpellInputs()
    {
        foreach (GameObject inputObject in spellInputObjects)
        {
            GameObject.Destroy(inputObject);
        }

        spellInputObjects.Clear();
        manaResistanceInput = null;
    }
}
