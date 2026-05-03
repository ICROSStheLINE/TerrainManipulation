using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellMenu : MonoBehaviour
{

    SimpleFPSController simpleFPSController;
    [SerializeField] GameObject buttonGameObjectPrefab;
    static int menuLength = 5;
    static int menuHeight = 3;
    GameObject[,] buttonGameObject = new GameObject[menuLength,menuHeight];


    void Start()
    {
        simpleFPSController = GetComponent<SimpleFPSController>();
        for (int i = 0; i < menuLength; i++)
        {
            for (int j = 0; j < menuHeight; j++)
            {
                // buttonGameObject[i,j] = Instantiate(
                //     buttonGameObjectPrefab, 
                // );
                // TODO: Finish later
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            simpleFPSController.SetCameraState(false);
            
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            simpleFPSController.SetCameraState(true);
            
        }
    }
}
