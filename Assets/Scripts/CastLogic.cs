using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastLogic : MonoBehaviour
{
    SpellMenu spellMenu;
    [SerializeField] GameObject ballPrefab;
    ManaManager manaManager;
    [SerializeField] GameObject eggPrefab;
    [SerializeField] GameObject innerEggPrefab;
    [SerializeField] GameObject sparkPrefab;
    Coroutine castCoroutine;
    bool casting = false;
    [SerializeField] Transform handTransform;
    List<ManaObject> activeManaObjects = new List<ManaObject>();
    float prepulsionStrength = 20f;

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
        if (Input.GetKeyDown(KeyCode.Mouse0) && casting)
        {
            casting = false;
            if (castCoroutine != null) StopCoroutine(castCoroutine);

            PropelCurrentSpell();
        }
    }

    void PropelCurrentSpell()
    {
        foreach (ManaObject obj in activeManaObjects)
        {
            if (obj != null)
            {
                obj.Propel(transform.forward, prepulsionStrength);
            }
        }

        activeManaObjects.Clear();
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
        CastStartLeft();
        CastStart();
        yield return new WaitForSeconds(0.25f);

        while (casting)
        {
            CastContinuous();
            FlowManaIntoActiveSpells();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void FlowManaIntoActiveSpells()
    {
        foreach (ManaObject manaObject in activeManaObjects)
        {
            PhysicalProperties physicalProperties = manaObject.GetComponent<PhysicalProperties>();
            SpellSlot spellSlotInfo = manaObject.spellSlotInfo;
            if (physicalProperties == null)
            { continue; }
            if (spellSlotInfo == null)
            { continue; }

            if (spellSlotInfo.manaFlowType == SpellSlot.ManaFlowType.NoManaFlow)
            { continue; }
            if (spellSlotInfo.manaFlowType == SpellSlot.ManaFlowType.ManaFlowOnE &&
                !Input.GetKey(KeyCode.E))
            { continue; }
            if (spellSlotInfo.manaFlowAmount <= 0)
            { continue; }
            if (manaManager.manaAmount <= 0)
            { continue; }

            manaManager.LoseMana(spellSlotInfo.manaFlowAmount);
            physicalProperties.AddMana(spellSlotInfo.manaFlowAmount);
        }
    }

    bool IsEggSpell(SpellSlot.SpellType spellType)
    {
        if (spellType == SpellSlot.SpellType.EggA)
        { return true; }
        if (spellType == SpellSlot.SpellType.EggB)
        { return true; }

        return false;
    }

    bool IsOpenParenthesisSpell(SpellSlot.SpellType spellType)
    {
        if (spellType == SpellSlot.SpellType.OpenParenthesisA)
        { return true; }
        if (spellType == SpellSlot.SpellType.OpenParenthesisB)
        { return true; }

        return false;
    }

    SpellSlot.SpellType GetOpenParenthesisForEgg(SpellSlot.SpellType spellType)
    {
        if (spellType == SpellSlot.SpellType.EggA)
        { return SpellSlot.SpellType.OpenParenthesisA; }

        if (spellType == SpellSlot.SpellType.EggB)
        { return SpellSlot.SpellType.OpenParenthesisB; }

        return SpellSlot.SpellType.Empty;
    }

    SpellSlot.SpellType GetCloseParenthesisForEgg(SpellSlot.SpellType spellType)
    {
        if (spellType == SpellSlot.SpellType.EggA)
        { return SpellSlot.SpellType.CloseParenthesisA; }

        if (spellType == SpellSlot.SpellType.EggB)
        { return SpellSlot.SpellType.CloseParenthesisB; }

        return SpellSlot.SpellType.Empty;
    }

    SpellSlot.SpellType GetCloseParenthesisForOpenParenthesis(SpellSlot.SpellType spellType)
    {
        if (spellType == SpellSlot.SpellType.OpenParenthesisA)
        { return SpellSlot.SpellType.CloseParenthesisA; }

        if (spellType == SpellSlot.SpellType.OpenParenthesisB)
        { return SpellSlot.SpellType.CloseParenthesisB; }

        return SpellSlot.SpellType.Empty;
    }

    string GetEggName(SpellSlot.SpellType spellType)
    {
        if (spellType == SpellSlot.SpellType.EggA)
        { return "EggA"; }

        if (spellType == SpellSlot.SpellType.EggB)
        { return "EggB"; }

        return null;
    }

    string GetEggNameForOpenParenthesis(SpellSlot.SpellType spellType)
    {
        if (spellType == SpellSlot.SpellType.OpenParenthesisA)
        { return "EggA"; }

        if (spellType == SpellSlot.SpellType.OpenParenthesisB)
        { return "EggB"; }

        return null;
    }

    void CastStartLeft()
    {
        Vector3 leftCastOffset = handTransform.right;
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castLeftWidth; i++)
        {
            if (spellMenu.castLeftMap[i].spellType == SpellSlot.SpellType.Empty)
            { continue; }
            if (spellMenu.castLeftMap[i].spellType == SpellSlot.SpellType.Ball)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                GameObject ballObject = Instantiate(ballPrefab, spawnPosition + leftCastOffset, transform.rotation);
                PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                ballPhysicalProperties.manaResistance = spellMenu.castLeftMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castLeftMap[i];
                manaManager.LoseMana(5);
            }
            if (IsEggSpell(spellMenu.castLeftMap[i].spellType)) // if spell is an egg, then 
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                SpellSlot.SpellType eggSpellType = spellMenu.castLeftMap[i].spellType;
                SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition + leftCastOffset, transform.rotation);
                spawnedEgg.transform.name = GetEggName(eggSpellType);
                PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                physicalProperties.manaResistance = spellMenu.castLeftMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castLeftMap[i];
                manaManager.LoseMana(5);
                int remainingIndices = spellMenu.castLeftMap.Count - 1 - i;
                if (remainingIndices <= 2)
                { continue; }
                int openParenthesesPassed = 0;
                int closeParenthesesPassed = 0;
                int closeParenthesisIndex = 0;
                for (int eggIndex = i + 1; eggIndex < spellMenu.castLeftMap.Count; eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                {
                    if (spellMenu.castLeftMap[eggIndex].spellType == openParenthesis)
                    {
                        openParenthesesPassed++;
                    }
                    if (spellMenu.castLeftMap[eggIndex].spellType == closeParenthesis)
                    {
                        closeParenthesesPassed++;
                        closeParenthesisIndex = eggIndex;
                    }
                    if (closeParenthesesPassed > openParenthesesPassed)
                    { break; }
                    if (openParenthesesPassed == closeParenthesesPassed && openParenthesesPassed > 0)
                    { break; }
                }
                if (openParenthesesPassed != closeParenthesesPassed)
                { continue; }
                if (openParenthesesPassed == 0)
                { continue; }
                for (int eggIndex = i + 1; eggIndex < closeParenthesisIndex; eggIndex++) // If so, iterate through the rest of the spells between parentheses until you reach the closed parenthesis.
                {
                    if (spellMenu.castLeftMap[eggIndex].spellType == SpellSlot.SpellType.Empty)
                    { continue; }
                    if (spellMenu.castLeftMap[eggIndex].spellType == SpellSlot.SpellType.Ball)
                    {
                        if (manaManager.manaAmount <= 0) 
                        { continue; }
                        GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                        PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                        ballPhysicalProperties.manaResistance = spellMenu.castLeftMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castLeftMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (spellMenu.castLeftMap[eggIndex].spellType == SpellSlot.SpellType.Spark)
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                        sparkObject.transform.SetParent(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castLeftMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (IsEggSpell(spellMenu.castLeftMap[eggIndex].spellType))
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                        PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                        innerEggPhysicalProps.manaResistance = spellMenu.castLeftMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castLeftMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                }
                i = closeParenthesisIndex;
            }
            if (spellMenu.castLeftMap[i].spellType == SpellSlot.SpellType.Spark)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                Instantiate(sparkPrefab, spawnPosition + leftCastOffset, transform.rotation);
                manaManager.LoseMana(5);
            }
        }
    }

    void CastStart()
    {
        Vector3 spawnPosition = handTransform.position + handTransform.up;
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
                    
                    GameObject ballObject = Instantiate(ballPrefab, spawnPosition, transform.rotation);
                    PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                    ballPhysicalProperties.manaResistance = spellMenu.castStartMap[i,j].manaResistancePercent / 100f;
                    ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                    manaObject.AttachToHand(handTransform);
                    activeManaObjects.Add(manaObject);
                    manaObject.spellSlotInfo = spellMenu.castStartMap[i,j];
                    manaManager.LoseMana(5);
                }
                if (IsEggSpell(spellMenu.castStartMap[i,j].spellType)) // if spell is an egg, then 
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }

                    SpellSlot.SpellType eggSpellType = spellMenu.castStartMap[i,j].spellType;
                    SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                    SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                    GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition, transform.rotation);
                    spawnedEgg.transform.name = GetEggName(eggSpellType);
                    PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                    physicalProperties.manaResistance = spellMenu.castStartMap[i,j].manaResistancePercent / 100f;
                    ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                    manaObject.AttachToHand(handTransform);
                    activeManaObjects.Add(manaObject);
                    manaObject.spellSlotInfo = spellMenu.castStartMap[i,j];
                    manaManager.LoseMana(5);

                    int remainingIndices = spellMenu.castStartMap.GetLength(1) - 1 - j;
                    if (remainingIndices <= 2)
                    { continue; }

                    int openParenthesesPassed = 0;
                    int closeParenthesesPassed = 0;
                    int closeParenthesisIndex = 0;
                    for (int eggIndex = j + 1; eggIndex < spellMenu.castStartMap.GetLength(1); eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                    {
                        if (spellMenu.castStartMap[i,eggIndex].spellType == openParenthesis)
                        {
                            openParenthesesPassed++;
                        }
                        if (spellMenu.castStartMap[i,eggIndex].spellType == closeParenthesis)
                        {
                            closeParenthesesPassed++;
                            closeParenthesisIndex = eggIndex;
                        }

                        if (closeParenthesesPassed > openParenthesesPassed)
                        { break; }

                        if (openParenthesesPassed == closeParenthesesPassed && openParenthesesPassed > 0)
                        { break; }
                    }
                    if (openParenthesesPassed != closeParenthesesPassed)
                    { continue; }
                    if (openParenthesesPassed == 0)
                    { continue; }

                    for (int eggIndex = j + 1; eggIndex < closeParenthesisIndex; eggIndex++) // If so, iterate through the rest of the spells between parentheses until you reach the closed parenthesis.
                    {
                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.Empty)
                        { continue; }

                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.Ball)
                        {
                            if (manaManager.manaAmount <= 0) 
                            { continue; }
                            GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                            PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                            ballPhysicalProperties.manaResistance = spellMenu.castStartMap[i,eggIndex].manaResistancePercent / 100f;
                            ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                            innerManaObject.AttachToEgg(spawnedEgg.transform);
                            manaObject.spellSlotInfo = spellMenu.castStartMap[i,eggIndex];
                            manaManager.LoseMana(5);
                        }
                        if (spellMenu.castStartMap[i,eggIndex].spellType == SpellSlot.SpellType.Spark)
                        {
                            if (manaManager.manaAmount <= 0)
                            { continue; }
                            GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                            sparkObject.transform.SetParent(spawnedEgg.transform);
                            manaObject.spellSlotInfo = spellMenu.castStartMap[i,eggIndex];
                            manaManager.LoseMana(5);
                        }
                        if (IsEggSpell(spellMenu.castStartMap[i,eggIndex].spellType))
                        {
                            if (manaManager.manaAmount <= 0)
                            { continue; }
                            GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                            PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                            innerEggPhysicalProps.manaResistance = spellMenu.castStartMap[i,eggIndex].manaResistancePercent / 100f;
                            ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                            innerManaObject.AttachToEgg(spawnedEgg.transform);
                            manaObject.spellSlotInfo = spellMenu.castStartMap[i,eggIndex];
                            manaManager.LoseMana(5);
                        }
                    }
                    j = closeParenthesisIndex;
                }
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.Spark)
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }
                    
                    Instantiate(sparkPrefab, spawnPosition, transform.rotation);
                    manaManager.LoseMana(5);
                }
            }
        }
    }

    void CastContinuous()
    {
        Vector3 spawnPosition = handTransform.position + handTransform.up;
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
                    
                    GameObject ballObject = Instantiate(ballPrefab, spawnPosition, transform.rotation);
                    // ManaObject manaObject = ballObject.GetComponent<ManaObject>();
                    // manaObject.AttachToHand(handTransform);
                    // activeManaObjects.Add(manaObject);
                    manaManager.LoseMana(5);
                }
                if (IsEggSpell(spellMenu.castContinuousMap[i,j].spellType)) // if spell is an egg, then 
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }

                    SpellSlot.SpellType eggSpellType = spellMenu.castContinuousMap[i,j].spellType;
                    SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                    SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                    GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition, transform.rotation);
                    spawnedEgg.transform.name = GetEggName(eggSpellType);
                    // ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                    // manaObject.AttachToHand(handTransform);
                    // activeManaObjects.Add(manaObject);
                    manaManager.LoseMana(5);

                    int remainingIndices = spellMenu.castContinuousMap.GetLength(1) - 1 - j;
                    if (remainingIndices <= 2)
                    { continue; }

                    int openParenthesesPassed = 0;
                    int closeParenthesesPassed = 0;
                    int closeParenthesisIndex = 0;
                    for (int eggIndex = j + 1; eggIndex < spellMenu.castContinuousMap.GetLength(1); eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                    {
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == openParenthesis)
                        {
                            openParenthesesPassed++;
                        }
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == closeParenthesis)
                        {
                            closeParenthesesPassed++;
                            closeParenthesisIndex = eggIndex;
                        }

                        if (closeParenthesesPassed > openParenthesesPassed)
                        { break; }

                        if (openParenthesesPassed == closeParenthesesPassed && openParenthesesPassed > 0)
                        { break; }
                    }
                    if (openParenthesesPassed != closeParenthesesPassed)
                    { continue; }
                    if (openParenthesesPassed == 0)
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
                    
                    Instantiate(sparkPrefab, spawnPosition, transform.rotation);
                    manaManager.LoseMana(5);
                }
                if (IsOpenParenthesisSpell(spellMenu.castContinuousMap[i,j].spellType))
                {
                    int remainingIndices = spellMenu.castContinuousMap.GetLength(1) - 1 - j;
                    if (remainingIndices <= 1)
                    { continue; }

                    SpellSlot.SpellType openParenthesis = spellMenu.castContinuousMap[i,j].spellType;
                    SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForOpenParenthesis(openParenthesis);
                    string eggName = GetEggNameForOpenParenthesis(openParenthesis);
                    int openParenthesesPassed = 0;
                    int closeParenthesesPassed = 0;
                    int closeParenthesisIndex = 0;
                    for (int eggIndex = j; eggIndex < spellMenu.castContinuousMap.GetLength(1); eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                    {
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == openParenthesis)
                        {
                            openParenthesesPassed++;
                        }
                        if (spellMenu.castContinuousMap[i,eggIndex].spellType == closeParenthesis)
                        {
                            closeParenthesesPassed++;
                            closeParenthesisIndex = eggIndex;
                        }

                        if (closeParenthesesPassed > openParenthesesPassed)
                        { break; }

                        if (openParenthesesPassed == closeParenthesesPassed && openParenthesesPassed > 0)
                        { break; }
                    }
                    if (openParenthesesPassed != closeParenthesesPassed)
                    { continue; }

                    foreach (ManaObject obj in activeManaObjects)
                    {
                        if (obj == null)
                        { continue; }

                        if (obj.transform.name != eggName)
                        { continue; }

                        for (int eggIndex = j + 1; eggIndex < closeParenthesisIndex; eggIndex++) // If so, iterate through the rest of the spells between parentheses until you reach the closed parenthesis.
                        {
                            if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.Empty)
                            { continue; }

                            if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.Ball)
                            {
                                if (manaManager.manaAmount <= 0) 
                                { continue; }
                                GameObject ballObject = Instantiate(ballPrefab, obj.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                                ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                                innerManaObject.AttachToEgg(obj.transform);
                                manaManager.LoseMana(5);
                            }
                            if (spellMenu.castContinuousMap[i,eggIndex].spellType == SpellSlot.SpellType.Spark)
                            {
                                if (manaManager.manaAmount <= 0)
                                { continue; }
                                Instantiate(sparkPrefab, obj.transform.position, transform.rotation);
                                manaManager.LoseMana(5);
                            }
                        }
                        j = closeParenthesisIndex;
                        break;
                    }
                }
            }
        }
    }
}
