using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastLogic : MonoBehaviour
{
    SpellMenu spellMenu;
    [SerializeField] GameObject ballPrefab;
    ManaManager manaManager;
    [SerializeField] GameObject eggPrefab;
    [SerializeField] GameObject sparkPrefab;
    Coroutine castCoroutine;
    bool casting = false;
    [SerializeField] Transform handTransform;
    List<ManaObject> activeManaObjects = new List<ManaObject>();

    void Start()
    {
        spellMenu = GetComponent<SpellMenu>();
        manaManager = GameObject.Find("ManaManager").transform.GetComponent<ManaManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            casting = true;
            castCoroutine = StartCoroutine("Cast");
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            casting = false;
            if (castCoroutine != null) StopCoroutine(castCoroutine);

            ReleaseCurrentSpell();
        }
    }

    void ReleaseCurrentSpell()
    {
        foreach (ManaObject obj in activeManaObjects)
        {
            if (obj != null)
            {
                obj.Release();
            }
        }

        activeManaObjects.Clear();
    }

    IEnumerator Cast()
    {
        CastStart();
        yield return new WaitForSeconds(0.25f);

        while (casting)
        {
            CastContinuous();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void CastStart()
    {
        for (int i = 0; i < SpellMenu.castStartHeight; i++)
        {
            for (int j = 0; j < SpellMenu.castStartWidth; j++)
            {
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.Empty)
                { continue; }
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.Ball)
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }
                    
                    GameObject ballObject = Instantiate(ballPrefab, handTransform.position + handTransform.up, transform.rotation);
                    ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                    manaObject.AttachToHand(handTransform);
                    activeManaObjects.Add(manaObject);
                    manaManager.LoseMana(5);
                }
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.Egg) // if spell is an egg, then 
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }

                    GameObject spawnedEgg = Instantiate(eggPrefab, handTransform.position + handTransform.up, transform.rotation);
                    ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                    manaObject.AttachToHand(handTransform);
                    activeManaObjects.Add(manaObject);
                    manaManager.LoseMana(5);

                    int remainingIndices = spellMenu.castStartMap.GetLength(1) - 1 - j;
                    if (remainingIndices == 0)
                    { continue; }
                    if (spellMenu.castStartMap[i,j+1].spellType != SpellSlot.SpellType.OpenParenthesis) // check to see if the next spell is an open parenthesis.
                    { continue; }

                    int openParenthesesPassed = 0;
                    int closeParenthesesPassed = 0;
                    int closeParenthesisIndex = 0;
                    for (int eggIndex = j + 1; eggIndex < spellMenu.castStartMap.GetLength(1); eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                    {
                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.OpenParenthesis)
                        {
                            openParenthesesPassed++;
                        }
                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.CloseParenthesis)
                        {
                            closeParenthesesPassed++;
                            closeParenthesisIndex = eggIndex;
                        }

                        if (openParenthesesPassed == closeParenthesesPassed)
                        { break; }
                    }
                    if (openParenthesesPassed != closeParenthesesPassed) 
                    { continue; }

                    for (int eggIndex = j + 1; eggIndex < closeParenthesisIndex; eggIndex++) // If so, iterate through the rest of the spells between parentheses until you reach the closed parenthesis.
                    {
                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.Empty)
                        { continue; }

                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.Ball)
                        {
                            if (manaManager.manaAmount <= 0) 
                            { continue; }
                            Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                            manaManager.LoseMana(5);
                        }
                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.Spark)
                        {
                            if (manaManager.manaAmount <= 0)
                            { continue; }
                            Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                            manaManager.LoseMana(5);
                        }
                    }
                    j = closeParenthesisIndex;
                }
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.Spark)
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }
                    
                    Instantiate(sparkPrefab, transform.position + transform.forward, transform.rotation);
                    manaManager.LoseMana(5);
                }
            }
        }
    }

    void CastContinuous()
    {
        for (int i = 0; i < SpellMenu.castContinuousHeight; i++)
        {
            for (int j = 0; j < SpellMenu.castContinuousWidth; j++)
            {
                if (spellMenu.castContinuousMap[i,j].spellType == SpellSlot.SpellType.Empty)
                { continue; }
                if (spellMenu.castContinuousMap[i,j].spellType == SpellSlot.SpellType.Ball)
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }
                    
                    GameObject ballObject = Instantiate(ballPrefab, handTransform.position + handTransform.up, transform.rotation);
                    ManaObject manaObject = ballObject.GetComponent<ManaObject>();
                    manaObject.AttachToHand(handTransform);
                    activeManaObjects.Add(manaObject);
                    manaManager.LoseMana(5);
                }
                if (spellMenu.castContinuousMap[i,j].spellType == SpellSlot.SpellType.Egg) // if spell is an egg, then 
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }

                    GameObject spawnedEgg = Instantiate(eggPrefab, handTransform.position + handTransform.up, transform.rotation);
                    ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                    manaObject.AttachToHand(handTransform);
                    activeManaObjects.Add(manaObject);
                    manaManager.LoseMana(5);

                    int remainingIndices = spellMenu.castContinuousMap.GetLength(1) - 1 - j;
                    if (remainingIndices == 0)
                    { continue; }
                    if (spellMenu.castContinuousMap[i,j+1].spellType != SpellSlot.SpellType.OpenParenthesis) // check to see if the next spell is an open parenthesis.
                    { continue; }

                    int openParenthesesPassed = 0;
                    int closeParenthesesPassed = 0;
                    int closeParenthesisIndex = 0;
                    for (int eggIndex = j + 1; eggIndex < spellMenu.castContinuousMap.GetLength(1); eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                    {
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.OpenParenthesis)
                        {
                            openParenthesesPassed++;
                        }
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.CloseParenthesis)
                        {
                            closeParenthesesPassed++;
                            closeParenthesisIndex = eggIndex;
                        }

                        if (openParenthesesPassed == closeParenthesesPassed)
                        { break; }
                    }
                    if (openParenthesesPassed != closeParenthesesPassed) 
                    { continue; }

                    for (int eggIndex = j + 1; eggIndex < closeParenthesisIndex; eggIndex++) // If so, iterate through the rest of the spells between parentheses until you reach the closed parenthesis.
                    {
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.Empty)
                        { continue; }

                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.Ball)
                        {
                            if (manaManager.manaAmount <= 0) 
                            { continue; }
                            Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                            manaManager.LoseMana(5);
                        }
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.Spark)
                        {
                            if (manaManager.manaAmount <= 0)
                            { continue; }
                            Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                            manaManager.LoseMana(5);
                        }
                    }
                    j = closeParenthesisIndex;
                }
                if (spellMenu.castContinuousMap[i,j].spellType == SpellSlot.SpellType.Spark)
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }
                    
                    Instantiate(sparkPrefab, transform.position + transform.forward, transform.rotation);
                    manaManager.LoseMana(5);
                }
            }
        }
    }
}
