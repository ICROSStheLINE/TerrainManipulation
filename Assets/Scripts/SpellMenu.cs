using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellMenu : MonoBehaviour
{

    SimpleFPSController simpleFPSController;
    [SerializeField] GameObject buttonGameObjectPrefab;
    static int spellMenuWidth = 5;
    static int spellMenuHeight = 3;
    float buttonWidth = 100;
    float buttonHeight = 100;
    float menuStartingPointX = 100;
    float menuStartingPointY = 0;
    SpellSlot[,] spellMenuButtonMap = new SpellSlot[spellMenuHeight,spellMenuWidth];
    [SerializeField] Transform canvasTransform;
    static int inventoryWidth = 1;
    static int inventoryHeight = 3;
    float inventoryStartingPointX = -600;
    float inventoryStartingPointY = 0;
    SpellSlot[,] spellInventoryMap = new SpellSlot[inventoryHeight,inventoryWidth];


    void Start()
    {
        simpleFPSController = GetComponent<SimpleFPSController>();
        PopulateSpellMenuMap();
        PopulateSpellInventoryMap();
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
            }
        }
    }

    void PopulateSpellMenuMap()
    {
        for (int i = 0; i < spellMenuHeight; i++)
        {
            for (int j = 0; j < spellMenuWidth; j++)
            {
                spellMenuButtonMap[i,j] = new SpellSlot();
                spellMenuButtonMap[i,j].uiObject = Instantiate(buttonGameObjectPrefab);
                spellMenuButtonMap[i,j].uiObject.transform.SetParent(canvasTransform, false);
                RectTransform rect = spellMenuButtonMap[i,j].uiObject.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    menuStartingPointX + (buttonWidth * j),
                    menuStartingPointY + (buttonHeight * i)
                );
                spellMenuButtonMap[i,j].uiObject.SetActive(false);
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
        for (int i = 0; i < spellMenuHeight; i++) {
            for (int j = 0; j < spellMenuWidth; j++) {
                spellMenuButtonMap[i,j].uiObject.SetActive(openState);
            }
        }
    }
}

public class SpellSlot
{
    public GameObject uiObject;
    public enum SpellType { Empty, Ball, Cube }
}
