using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellMenu : MonoBehaviour
{

    SimpleFPSController simpleFPSController;
    [SerializeField] GameObject buttonGameObjectPrefab;
    static int menuWidth = 5;
    static int menuHeight = 3;
    float buttonWidth = 100;
    float buttonHeight = 100;
    float startingPointX = 100;
    float startingPointY = 000;
    GameObject[,] spellMenuButtonMap = new GameObject[menuHeight,menuWidth];
    [SerializeField] Transform canvasTransform;
    GameObject[] spellInventoryMap = new GameObject[3];


    void Start()
    {
        simpleFPSController = GetComponent<SimpleFPSController>();
        PopulateSpellMenuMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            OpenSpellMenu(true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            OpenSpellMenu(false);
        }
    }

    void PopulateSpellMenuMap()
    {
        for (int i = 0; i < menuHeight; i++)
        {
            for (int j = 0; j < menuWidth; j++)
            {
                spellMenuButtonMap[i,j] = Instantiate(buttonGameObjectPrefab);
                spellMenuButtonMap[i,j].transform.SetParent(canvasTransform, false);
                spellMenuButtonMap[i,j].transform.localPosition = new Vector3(startingPointX + (buttonWidth * j), startingPointY + (buttonHeight * i), 0);
                spellMenuButtonMap[i,j].SetActive(false);
            }
        }
    }
    
    void OpenSpellMenu(bool openState)
    {
        simpleFPSController.SetCameraUseState(!openState);
        for (int i = 0; i < menuHeight; i++) {
            for (int j = 0; j < menuWidth; j++) {
                spellMenuButtonMap[i,j].SetActive(openState);
            }
        }
    }
}
