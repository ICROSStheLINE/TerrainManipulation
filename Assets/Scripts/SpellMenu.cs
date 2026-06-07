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
    float castStartStartingPointX = -270;
    // float castStartStartingPointY = 225;
    float castStartStartingPointY = 375;
    GameObject castStartLabel;
    public SpellSlot[,] castStartMap = new SpellSlot[castStartHeight,castStartWidth];
    // Cast Close
    public static int castCloseWidth = 1;
    float castCloseStartingPointX = -270;
    float castCloseStartingPointY = 200;
    public List<SpellSlot> castCloseMap = new List<SpellSlot>();
    // Cast Middle
    public static int castMiddleWidth = 1;
    float castMiddleStartingPointX = 300;
    float castMiddleStartingPointY = 200;
    public List<SpellSlot> castMiddleMap = new List<SpellSlot>();
    public static int castFarWidth = 1;
    float castFarStartingPointX = -270;
    float castFarStartingPointY = 25;
    public List<SpellSlot> castFarMap = new List<SpellSlot>();
    public static int castLeftWidth = 1;
    float castLeftStartingPointX = 300;
    float castLeftStartingPointY = 25;
    public List<SpellSlot> castLeftMap = new List<SpellSlot>();
    public static int castRightWidth = 1;
    float castRightStartingPointX = -270;
    float castRightStartingPointY = -150;
    public List<SpellSlot> castRightMap = new List<SpellSlot>();
    public static int castContinuousWidth = 5;
    public static int castContinuousHeight = 1;
    float castContinuousStartingPointX = 100;
    float castContinuousStartingPointY = -300;
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
        spellInventoryMap[4,0].AssignSpell(SpellSlot.SpellType.OpenParenthesisA);
        spellInventoryMap[5,0].AssignSpell(SpellSlot.SpellType.CloseParenthesisA);
        spellInventoryMap[0,1].AssignSpell(SpellSlot.SpellType.EggB);
        spellInventoryMap[1,1].AssignSpell(SpellSlot.SpellType.OpenParenthesisB);
        spellInventoryMap[2,1].AssignSpell(SpellSlot.SpellType.CloseParenthesisB);
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
        for (int i = 0; i < castCloseWidth; i++)
        {
            castCloseMap.Add(new SpellSlot(true));
            castCloseMap[i].uiObject = Instantiate(buttonGameObjectPrefab);
            castCloseMap[i].uiObject.transform.SetParent(canvasTransform, false);
            RectTransform rect = castCloseMap[i].uiObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                castCloseStartingPointX + buttonWidth,
                castCloseStartingPointY + buttonHeight
            );
            castCloseMap[i].uiObject.SetActive(false);
            Button button = castCloseMap[i].uiObject.GetComponent<Button>();
            SpellSlot spellSlot = castCloseMap[i];
            button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
            castCloseMap[i].uiObject.name = "castCloseButton[" + i + "]";
        }

        for (int i = 0; i < castMiddleWidth; i++)
        {
            castMiddleMap.Add(new SpellSlot(true));
            castMiddleMap[i].uiObject = Instantiate(buttonGameObjectPrefab);
            castMiddleMap[i].uiObject.transform.SetParent(canvasTransform, false);
            RectTransform rect = castMiddleMap[i].uiObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                castMiddleStartingPointX + buttonWidth,
                castMiddleStartingPointY + buttonHeight
            );
            castMiddleMap[i].uiObject.SetActive(false);
            Button button = castMiddleMap[i].uiObject.GetComponent<Button>();
            SpellSlot spellSlot = castMiddleMap[i];
            button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
            castMiddleMap[i].uiObject.name = "castMiddleButton[" + i + "]";
        }

        for (int i = 0; i < castFarWidth; i++)
        {
            castFarMap.Add(new SpellSlot(true));
            castFarMap[i].uiObject = Instantiate(buttonGameObjectPrefab);
            castFarMap[i].uiObject.transform.SetParent(canvasTransform, false);
            RectTransform rect = castFarMap[i].uiObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                castFarStartingPointX + buttonWidth,
                castFarStartingPointY + buttonHeight
            );
            castFarMap[i].uiObject.SetActive(false);
            Button button = castFarMap[i].uiObject.GetComponent<Button>();
            SpellSlot spellSlot = castFarMap[i];
            button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
            castFarMap[i].uiObject.name = "castFarButton[" + i + "]";
        }

        for (int i = 0; i < castLeftWidth; i++)
        {
            castLeftMap.Add(new SpellSlot(true));
            castLeftMap[i].uiObject = Instantiate(buttonGameObjectPrefab);
            castLeftMap[i].uiObject.transform.SetParent(canvasTransform, false);
            RectTransform rect = castLeftMap[i].uiObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                castLeftStartingPointX + buttonWidth,
                castLeftStartingPointY + buttonHeight
            );
            castLeftMap[i].uiObject.SetActive(false);
            Button button = castLeftMap[i].uiObject.GetComponent<Button>();
            SpellSlot spellSlot = castLeftMap[i];
            button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
            castLeftMap[i].uiObject.name = "castLeftButton[" + i + "]";
        }

        for (int i = 0; i < castRightWidth; i++)
        {
            castRightMap.Add(new SpellSlot(true));
            castRightMap[i].uiObject = Instantiate(buttonGameObjectPrefab);
            castRightMap[i].uiObject.transform.SetParent(canvasTransform, false);
            RectTransform rect = castRightMap[i].uiObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(
                castRightStartingPointX + buttonWidth,
                castRightStartingPointY + buttonHeight
            );
            castRightMap[i].uiObject.SetActive(false);
            Button button = castRightMap[i].uiObject.GetComponent<Button>();
            SpellSlot spellSlot = castRightMap[i];
            button.onClick.AddListener(delegate {InteractWithSlot(spellSlot);} );
            castRightMap[i].uiObject.name = "castRightButton[" + i + "]";
        }

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
        for (int i = 0; i < castCloseWidth; i++)
        {
            castCloseMap[i].uiObject.SetActive(openState);
        }

        for (int i = 0; i < castMiddleWidth; i++)
        {
            castMiddleMap[i].uiObject.SetActive(openState);
        }

        for (int i = 0; i < castFarWidth; i++)
        {
            castFarMap[i].uiObject.SetActive(openState);
        }

        for (int i = 0; i < castLeftWidth; i++)
        {
            castLeftMap[i].uiObject.SetActive(openState);
        }

        for (int i = 0; i < castRightWidth; i++)
        {
            castRightMap[i].uiObject.SetActive(openState);
        }

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
    public enum SpellType { Empty, Ball, Cube, EggA, EggB, OpenParenthesisA, CloseParenthesisA, OpenParenthesisB, CloseParenthesisB, Spark }
    public enum ManaFlowType { NoManaFlow, ContinuousManaFlow, ManaFlowOnE }
    public SpellType spellType;
    public float manaResistancePercent = 50;
    public float manaFlowAmount = 1;
    public ManaFlowType manaFlowType = ManaFlowType.NoManaFlow;
    GameObject spellIcon;
    List<GameObject> spellInputObjects = new List<GameObject>();
    TMP_InputField manaResistanceInput;
    Image manaResistanceInputImage;
    TMP_InputField manaFlowAmountInput;
    Image manaFlowAmountInputImage;
    TextMeshProUGUI manaFlowInputText;
    bool showsSpellInputs;

    public SpellSlot(bool showsSpellInputs = false)
    {
        this.showsSpellInputs = showsSpellInputs;
    }

    public void ClearSpell()
    {
        spellType = SpellType.Empty;
        manaResistancePercent = 50;
        manaFlowAmount = 1;
        manaFlowType = ManaFlowType.NoManaFlow;
    }

    public void CopySpellInputs(SpellSlot spellSlot)
    {
        manaResistancePercent = spellSlot.manaResistancePercent;
        manaFlowAmount = spellSlot.manaFlowAmount;
        manaFlowType = spellSlot.manaFlowType;

        if (manaResistanceInput != null)
        {
            manaResistanceInput.text = manaResistancePercent.ToString();
        }
        if (manaFlowAmountInput != null)
        {
            manaFlowAmountInput.text = manaFlowAmount.ToString();
        }

        UpdateManaFlowInputText();
        UpdateManaFlowAmountInputState();
        UpdateManaResistanceInputState();
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

        this.spellType = spellType;
        SetDefaultSpellInputs(spellType);

        CreateSpellIcon(spellType);

        if (showsSpellInputs)
        {
            CreateSpellInputs(spellType);
        }
    }

    void SetDefaultSpellInputs(SpellType spellType)
    {
        if (spellType == SpellType.Ball)
        {
            manaResistancePercent = 10;
            return;
        }

        manaResistancePercent = 50;
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
        if (spellType == SpellType.OpenParenthesisB)
            { tmp.text = "(B"; }
        if (spellType == SpellType.CloseParenthesisB)
            { tmp.text = ")B"; }
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 24;
        tmp.color = Color.black;
        tmp.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 75);
    }

    bool HasManaResistanceInput(SpellType spellType)
    {
        if (spellType == SpellType.Ball)
        { return true; }
        if (spellType == SpellType.EggA)
        { return true; }
        if (spellType == SpellType.EggB)
        { return true; }

        return false;
    }

    bool CanEditManaResistanceInput(SpellType spellType)
    {
        if (spellType == SpellType.Ball)
        { return false; }

        return true;
    }

    bool HasManaFlowInput(SpellType spellType)
    {
        if (spellType == SpellType.OpenParenthesisA)
        { return false; }
        if (spellType == SpellType.CloseParenthesisA)
        { return false; }
        if (spellType == SpellType.OpenParenthesisB)
        { return false; }
        if (spellType == SpellType.CloseParenthesisB)
        { return false; }

        return true;
    }

    void CreateSpellInputs(SpellType spellType)
    {
        if (HasManaFlowInput(spellType))
        {
            CreateManaFlowInput(new Vector2(0, -55));
            CreateManaFlowAmountInput(new Vector2(0, -85));
        }

        if (HasManaResistanceInput(spellType))
        {
            CreateManaResistanceInput(CanEditManaResistanceInput(spellType), new Vector2(0, -115));
        }

        UpdateManaFlowAmountInputState();
        UpdateManaResistanceInputState();
    }

    void CreateManaResistanceInput(bool canEdit, Vector2 anchoredPosition)
    {
        manaResistanceInput = CreateFloatInput(
            "ManaResistanceInput",
            "% MR",
            manaResistancePercent,
            anchoredPosition,
            delegate (float newValue)
            {
                manaResistancePercent = Mathf.Clamp(newValue, -100, 100);
                manaResistanceInput.SetTextWithoutNotify(manaResistancePercent.ToString());
            },
            canEdit
        );
        manaResistanceInputImage = manaResistanceInput.GetComponent<Image>();
        CreateInputLabel("% Mana Resistance", new Vector2(-75, anchoredPosition.y));
    }

    void CreateManaFlowInput(Vector2 anchoredPosition)
    {
        manaFlowInputText = CreateButtonInput(
            "ManaFlowInput",
            GetManaFlowText(),
            anchoredPosition,
            delegate
            {
                CycleManaFlowType();
            }
        );
        CreateInputLabel("Mana Flow Type", new Vector2(-75, anchoredPosition.y));
    }

    void CreateManaFlowAmountInput(Vector2 anchoredPosition)
    {
        manaFlowAmountInput = CreateFloatInput(
            "ManaFlowAmountInput",
            "Mana",
            manaFlowAmount,
            anchoredPosition,
            delegate (float newValue)
            {
                manaFlowAmount = Mathf.Max(0, newValue);
                manaFlowAmountInput.SetTextWithoutNotify(manaFlowAmount.ToString());
            }
        );
        manaFlowAmountInputImage = manaFlowAmountInput.GetComponent<Image>();
        CreateInputLabel("Mana Flow Amount", new Vector2(-75, anchoredPosition.y));
    }

    void CycleManaFlowType()
    {
        if (manaFlowType == ManaFlowType.NoManaFlow)
        {
            manaFlowType = ManaFlowType.ContinuousManaFlow;
        }
        else if (manaFlowType == ManaFlowType.ContinuousManaFlow)
        {
            manaFlowType = ManaFlowType.ManaFlowOnE;
        }
        else if (manaFlowType == ManaFlowType.ManaFlowOnE)
        {
            manaFlowType = ManaFlowType.NoManaFlow;
        }

        UpdateManaFlowInputText();
        UpdateManaFlowAmountInputState();
        UpdateManaResistanceInputState();
    }

    string GetManaFlowText()
    {
        if (manaFlowType == ManaFlowType.NoManaFlow)
        { return "No Flow"; }
        if (manaFlowType == ManaFlowType.ContinuousManaFlow)
        { return "Flow"; }
        if (manaFlowType == ManaFlowType.ManaFlowOnE)
        { return "E Flow"; }

        return "";
    }

    void UpdateManaFlowInputText()
    {
        if (manaFlowInputText != null)
        {
            manaFlowInputText.text = GetManaFlowText();
        }
    }

    void UpdateManaFlowAmountInputState()
    {
        if (manaFlowAmountInput == null)
        { return; }

        if (manaFlowType == ManaFlowType.NoManaFlow)
        {
            manaFlowAmount = 0;
            manaFlowAmountInput.SetTextWithoutNotify("0");
            manaFlowAmountInput.interactable = false;
            if (manaFlowAmountInputImage != null)
            {
                manaFlowAmountInputImage.color = Color.gray;
            }
            return;
        }

        manaFlowAmountInput.interactable = true;
        if (manaFlowAmountInputImage != null)
        {
            manaFlowAmountInputImage.color = Color.white;
        }
    }

    void UpdateManaResistanceInputState()
    {
        if (manaResistanceInput == null)
        { return; }

        if (manaFlowType == ManaFlowType.NoManaFlow)
        {
            SetDefaultSpellInputs(spellType);
            manaResistanceInput.SetTextWithoutNotify(manaResistancePercent.ToString());
            manaResistanceInput.interactable = false;
            if (manaResistanceInputImage != null)
            {
                manaResistanceInputImage.color = Color.gray;
            }
            return;
        }

        manaResistanceInput.interactable = CanEditManaResistanceInput(spellType);
        if (manaResistanceInputImage != null)
        {
            if (CanEditManaResistanceInput(spellType))
            {
                manaResistanceInputImage.color = Color.white;
            }
            else
            {
                manaResistanceInputImage.color = Color.gray;
            }
        }
    }

    TextMeshProUGUI CreateButtonInput(string inputName, string startingText, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
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

        Button button = inputObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        GameObject textObject = new GameObject();
        textObject.name = "Text";
        textObject.transform.SetParent(inputObject.transform, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = startingText;
        text.color = Color.black;
        text.fontSize = 14;
        text.alignment = TextAlignmentOptions.Center;
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return text;
    }

    void CreateInputLabel(string labelText, Vector2 anchoredPosition)
    {
        GameObject labelObject = new GameObject();
        labelObject.name = labelText + "Label";
        labelObject.transform.SetParent(uiObject.transform, false);
        spellInputObjects.Add(labelObject);

        TextMeshProUGUI tmp = labelObject.AddComponent<TextMeshProUGUI>();
        tmp.text = labelText;
        tmp.color = Color.black;
        tmp.fontSize = 12;
        tmp.alignment = TextAlignmentOptions.Right;
        RectTransform rect = labelObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(70, 25);
        rect.anchoredPosition = anchoredPosition;
    }

    TMP_InputField CreateFloatInput(string inputName, string labelText, float startingValue, Vector2 anchoredPosition, System.Action<float> onValueChanged, bool canEdit = true)
    {
        GameObject inputObject = new GameObject();
        inputObject.name = inputName;
        inputObject.SetActive(false);
        inputObject.transform.SetParent(uiObject.transform, false);
        spellInputObjects.Add(inputObject);

        RectTransform rect = inputObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(70, 25);
        rect.anchoredPosition = anchoredPosition;

        Image image = inputObject.AddComponent<Image>();
        if (canEdit)
        {
            image.color = Color.white;
        }
        else
        {
            image.color = Color.gray;
        }

        TMP_InputField input = inputObject.AddComponent<TMP_InputField>();
        input.interactable = canEdit;
        input.contentType = TMP_InputField.ContentType.DecimalNumber;
        input.targetGraphic = image;
        input.textViewport = rect;
        input.customCaretColor = true;
        input.caretColor = Color.black;
        input.caretWidth = 2;
        input.selectionColor = new Color(0.65f, 0.8f, 1f, 0.75f);

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

        // Apparently you need to set it as inactive while setting all the properties then make it active afterwards to prevent bugs...
        inputObject.SetActive(true); 

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
        manaResistanceInputImage = null;
        manaFlowAmountInput = null;
        manaFlowAmountInputImage = null;
        manaFlowInputText = null;
    }
}
