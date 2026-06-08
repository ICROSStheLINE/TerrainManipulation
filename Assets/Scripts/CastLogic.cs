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
        CastStartClose();
        CastStartMiddle();
        CastStartFar();
        CastStartLeft();
        CastStartRight();
        CastStartUp();
        CastStartDown();
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
            if (manaObject == null)
            { continue; }
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

    void CastStartClose()
    {
        Vector3 closeCastOffset = handTransform.forward;
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castCloseWidth; i++)
        {
            if (spellMenu.castCloseMap[i].spellType == SpellSlot.SpellType.Empty)
            { continue; }
            if (spellMenu.castCloseMap[i].spellType == SpellSlot.SpellType.Ball)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                GameObject ballObject = Instantiate(ballPrefab, spawnPosition + closeCastOffset, transform.rotation);
                PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                ballPhysicalProperties.manaResistance = spellMenu.castCloseMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castCloseMap[i];
                manaManager.LoseMana(5);
            }
            if (IsEggSpell(spellMenu.castCloseMap[i].spellType)) // if spell is an egg, then 
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                SpellSlot.SpellType eggSpellType = spellMenu.castCloseMap[i].spellType;
                SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition + closeCastOffset, transform.rotation);
                spawnedEgg.transform.name = GetEggName(eggSpellType);
                PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                physicalProperties.manaResistance = spellMenu.castCloseMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castCloseMap[i];
                manaManager.LoseMana(5);
                int remainingIndices = spellMenu.castCloseMap.Count - 1 - i;
                if (remainingIndices <= 2)
                { continue; }
                int openParenthesesPassed = 0;
                int closeParenthesesPassed = 0;
                int closeParenthesisIndex = 0;
                for (int eggIndex = i + 1; eggIndex < spellMenu.castCloseMap.Count; eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                {
                    if (spellMenu.castCloseMap[eggIndex].spellType == openParenthesis)
                    {
                        openParenthesesPassed++;
                    }
                    if (spellMenu.castCloseMap[eggIndex].spellType == closeParenthesis)
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
                    if (spellMenu.castCloseMap[eggIndex].spellType == SpellSlot.SpellType.Empty)
                    { continue; }
                    if (spellMenu.castCloseMap[eggIndex].spellType == SpellSlot.SpellType.Ball)
                    {
                        if (manaManager.manaAmount <= 0) 
                        { continue; }
                        GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                        PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                        ballPhysicalProperties.manaResistance = spellMenu.castCloseMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castCloseMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (spellMenu.castCloseMap[eggIndex].spellType == SpellSlot.SpellType.Spark)
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                        sparkObject.transform.SetParent(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castCloseMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (IsEggSpell(spellMenu.castCloseMap[eggIndex].spellType))
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                        PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                        innerEggPhysicalProps.manaResistance = spellMenu.castCloseMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castCloseMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                }
                i = closeParenthesisIndex;
            }
            if (spellMenu.castCloseMap[i].spellType == SpellSlot.SpellType.Spark)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                Instantiate(sparkPrefab, spawnPosition + closeCastOffset, transform.rotation);
                manaManager.LoseMana(5);
            }
        }
    }

    void CastStartMiddle()
    {
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castMiddleWidth; i++)
        {
            if (spellMenu.castMiddleMap[i].spellType == SpellSlot.SpellType.Empty)
            { continue; }
            if (spellMenu.castMiddleMap[i].spellType == SpellSlot.SpellType.Ball)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                GameObject ballObject = Instantiate(ballPrefab, spawnPosition, transform.rotation);
                PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                ballPhysicalProperties.manaResistance = spellMenu.castMiddleMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castMiddleMap[i];
                manaManager.LoseMana(5);
            }
            if (IsEggSpell(spellMenu.castMiddleMap[i].spellType)) // if spell is an egg, then 
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                SpellSlot.SpellType eggSpellType = spellMenu.castMiddleMap[i].spellType;
                SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition, transform.rotation);
                spawnedEgg.transform.name = GetEggName(eggSpellType);
                PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                physicalProperties.manaResistance = spellMenu.castMiddleMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castMiddleMap[i];
                manaManager.LoseMana(5);
                int remainingIndices = spellMenu.castMiddleMap.Count - 1 - i;
                if (remainingIndices <= 2)
                { continue; }
                int openParenthesesPassed = 0;
                int closeParenthesesPassed = 0;
                int closeParenthesisIndex = 0;
                for (int eggIndex = i + 1; eggIndex < spellMenu.castMiddleMap.Count; eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                {
                    if (spellMenu.castMiddleMap[eggIndex].spellType == openParenthesis)
                    {
                        openParenthesesPassed++;
                    }
                    if (spellMenu.castMiddleMap[eggIndex].spellType == closeParenthesis)
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
                    if (spellMenu.castMiddleMap[eggIndex].spellType == SpellSlot.SpellType.Empty)
                    { continue; }
                    if (spellMenu.castMiddleMap[eggIndex].spellType == SpellSlot.SpellType.Ball)
                    {
                        if (manaManager.manaAmount <= 0) 
                        { continue; }
                        GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                        PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                        ballPhysicalProperties.manaResistance = spellMenu.castMiddleMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castMiddleMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (spellMenu.castMiddleMap[eggIndex].spellType == SpellSlot.SpellType.Spark)
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                        sparkObject.transform.SetParent(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castMiddleMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (IsEggSpell(spellMenu.castMiddleMap[eggIndex].spellType))
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                        PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                        innerEggPhysicalProps.manaResistance = spellMenu.castMiddleMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castMiddleMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                }
                i = closeParenthesisIndex;
            }
            if (spellMenu.castMiddleMap[i].spellType == SpellSlot.SpellType.Spark)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                Instantiate(sparkPrefab, spawnPosition, transform.rotation);
                manaManager.LoseMana(5);
            }
        }
    }

    void CastStartFar()
    {
        Vector3 farCastOffset = -handTransform.forward + (-handTransform.forward/2);
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castFarWidth; i++)
        {
            if (spellMenu.castFarMap[i].spellType == SpellSlot.SpellType.Empty)
            { continue; }
            if (spellMenu.castFarMap[i].spellType == SpellSlot.SpellType.Ball)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                GameObject ballObject = Instantiate(ballPrefab, spawnPosition + farCastOffset, transform.rotation);
                PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                ballPhysicalProperties.manaResistance = spellMenu.castFarMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castFarMap[i];
                manaManager.LoseMana(5);
            }
            if (IsEggSpell(spellMenu.castFarMap[i].spellType)) // if spell is an egg, then 
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                SpellSlot.SpellType eggSpellType = spellMenu.castFarMap[i].spellType;
                SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition + farCastOffset, transform.rotation);
                spawnedEgg.transform.name = GetEggName(eggSpellType);
                PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                physicalProperties.manaResistance = spellMenu.castFarMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castFarMap[i];
                manaManager.LoseMana(5);
                int remainingIndices = spellMenu.castFarMap.Count - 1 - i;
                if (remainingIndices <= 2)
                { continue; }
                int openParenthesesPassed = 0;
                int closeParenthesesPassed = 0;
                int closeParenthesisIndex = 0;
                for (int eggIndex = i + 1; eggIndex < spellMenu.castFarMap.Count; eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                {
                    if (spellMenu.castFarMap[eggIndex].spellType == openParenthesis)
                    {
                        openParenthesesPassed++;
                    }
                    if (spellMenu.castFarMap[eggIndex].spellType == closeParenthesis)
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
                    if (spellMenu.castFarMap[eggIndex].spellType == SpellSlot.SpellType.Empty)
                    { continue; }
                    if (spellMenu.castFarMap[eggIndex].spellType == SpellSlot.SpellType.Ball)
                    {
                        if (manaManager.manaAmount <= 0) 
                        { continue; }
                        GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                        PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                        ballPhysicalProperties.manaResistance = spellMenu.castFarMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castFarMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (spellMenu.castFarMap[eggIndex].spellType == SpellSlot.SpellType.Spark)
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                        sparkObject.transform.SetParent(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castFarMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (IsEggSpell(spellMenu.castFarMap[eggIndex].spellType))
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                        PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                        innerEggPhysicalProps.manaResistance = spellMenu.castFarMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castFarMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                }
                i = closeParenthesisIndex;
            }
            if (spellMenu.castFarMap[i].spellType == SpellSlot.SpellType.Spark)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                Instantiate(sparkPrefab, spawnPosition + farCastOffset, transform.rotation);
                manaManager.LoseMana(5);
            }
        }
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

    void CastStartRight()
    {
        Vector3 rightCastOffset = -handTransform.right;
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castRightWidth; i++)
        {
            if (spellMenu.castRightMap[i].spellType == SpellSlot.SpellType.Empty)
            { continue; }
            if (spellMenu.castRightMap[i].spellType == SpellSlot.SpellType.Ball)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                GameObject ballObject = Instantiate(ballPrefab, spawnPosition + rightCastOffset, transform.rotation);
                PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                ballPhysicalProperties.manaResistance = spellMenu.castRightMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castRightMap[i];
                manaManager.LoseMana(5);
            }
            if (IsEggSpell(spellMenu.castRightMap[i].spellType)) // if spell is an egg, then 
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                SpellSlot.SpellType eggSpellType = spellMenu.castRightMap[i].spellType;
                SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition + rightCastOffset, transform.rotation);
                spawnedEgg.transform.name = GetEggName(eggSpellType);
                PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                physicalProperties.manaResistance = spellMenu.castRightMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castRightMap[i];
                manaManager.LoseMana(5);
                int remainingIndices = spellMenu.castRightMap.Count - 1 - i;
                if (remainingIndices <= 2)
                { continue; }
                int openParenthesesPassed = 0;
                int closeParenthesesPassed = 0;
                int closeParenthesisIndex = 0;
                for (int eggIndex = i + 1; eggIndex < spellMenu.castRightMap.Count; eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                {
                    if (spellMenu.castRightMap[eggIndex].spellType == openParenthesis)
                    {
                        openParenthesesPassed++;
                    }
                    if (spellMenu.castRightMap[eggIndex].spellType == closeParenthesis)
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
                    if (spellMenu.castRightMap[eggIndex].spellType == SpellSlot.SpellType.Empty)
                    { continue; }
                    if (spellMenu.castRightMap[eggIndex].spellType == SpellSlot.SpellType.Ball)
                    {
                        if (manaManager.manaAmount <= 0) 
                        { continue; }
                        GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                        PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                        ballPhysicalProperties.manaResistance = spellMenu.castRightMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castRightMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (spellMenu.castRightMap[eggIndex].spellType == SpellSlot.SpellType.Spark)
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                        sparkObject.transform.SetParent(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castRightMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (IsEggSpell(spellMenu.castRightMap[eggIndex].spellType))
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                        PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                        innerEggPhysicalProps.manaResistance = spellMenu.castRightMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castRightMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                }
                i = closeParenthesisIndex;
            }
            if (spellMenu.castRightMap[i].spellType == SpellSlot.SpellType.Spark)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                Instantiate(sparkPrefab, spawnPosition + rightCastOffset, transform.rotation);
                manaManager.LoseMana(5);
            }
        }
    }

    void CastStartUp()
    {
        Vector3 upCastOffset = handTransform.up;
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castUpWidth; i++)
        {
            if (spellMenu.castUpMap[i].spellType == SpellSlot.SpellType.Empty)
            { continue; }
            if (spellMenu.castUpMap[i].spellType == SpellSlot.SpellType.Ball)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                GameObject ballObject = Instantiate(ballPrefab, spawnPosition + upCastOffset, transform.rotation);
                PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                ballPhysicalProperties.manaResistance = spellMenu.castUpMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castUpMap[i];
                manaManager.LoseMana(5);
            }
            if (IsEggSpell(spellMenu.castUpMap[i].spellType)) // if spell is an egg, then 
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                SpellSlot.SpellType eggSpellType = spellMenu.castUpMap[i].spellType;
                SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition + upCastOffset, transform.rotation);
                spawnedEgg.transform.name = GetEggName(eggSpellType);
                PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                physicalProperties.manaResistance = spellMenu.castUpMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castUpMap[i];
                manaManager.LoseMana(5);
                int remainingIndices = spellMenu.castUpMap.Count - 1 - i;
                if (remainingIndices <= 2)
                { continue; }
                int openParenthesesPassed = 0;
                int closeParenthesesPassed = 0;
                int closeParenthesisIndex = 0;
                for (int eggIndex = i + 1; eggIndex < spellMenu.castUpMap.Count; eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                {
                    if (spellMenu.castUpMap[eggIndex].spellType == openParenthesis)
                    {
                        openParenthesesPassed++;
                    }
                    if (spellMenu.castUpMap[eggIndex].spellType == closeParenthesis)
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
                    if (spellMenu.castUpMap[eggIndex].spellType == SpellSlot.SpellType.Empty)
                    { continue; }
                    if (spellMenu.castUpMap[eggIndex].spellType == SpellSlot.SpellType.Ball)
                    {
                        if (manaManager.manaAmount <= 0) 
                        { continue; }
                        GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                        PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                        ballPhysicalProperties.manaResistance = spellMenu.castUpMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castUpMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (spellMenu.castUpMap[eggIndex].spellType == SpellSlot.SpellType.Spark)
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                        sparkObject.transform.SetParent(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castUpMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (IsEggSpell(spellMenu.castUpMap[eggIndex].spellType))
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                        PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                        innerEggPhysicalProps.manaResistance = spellMenu.castUpMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castUpMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                }
                i = closeParenthesisIndex;
            }
            if (spellMenu.castUpMap[i].spellType == SpellSlot.SpellType.Spark)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                Instantiate(sparkPrefab, spawnPosition + upCastOffset, transform.rotation);
                manaManager.LoseMana(5);
            }
        }
    }

    void CastStartDown()
    {
        Vector3 downCastOffset = -handTransform.up;
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castDownWidth; i++)
        {
            if (spellMenu.castDownMap[i].spellType == SpellSlot.SpellType.Empty)
            { continue; }
            if (spellMenu.castDownMap[i].spellType == SpellSlot.SpellType.Ball)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                GameObject ballObject = Instantiate(ballPrefab, spawnPosition + downCastOffset, transform.rotation);
                PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                ballPhysicalProperties.manaResistance = spellMenu.castDownMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = ballObject.transform.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castDownMap[i];
                manaManager.LoseMana(5);
            }
            if (IsEggSpell(spellMenu.castDownMap[i].spellType)) // if spell is an egg, then 
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                SpellSlot.SpellType eggSpellType = spellMenu.castDownMap[i].spellType;
                SpellSlot.SpellType openParenthesis = GetOpenParenthesisForEgg(eggSpellType);
                SpellSlot.SpellType closeParenthesis = GetCloseParenthesisForEgg(eggSpellType);
                GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition + downCastOffset, transform.rotation);
                spawnedEgg.transform.name = GetEggName(eggSpellType);
                PhysicalProperties physicalProperties = spawnedEgg.GetComponent<PhysicalProperties>();
                physicalProperties.manaResistance = spellMenu.castDownMap[i].manaResistancePercent / 100f;
                ManaObject manaObject = spawnedEgg.GetComponent<ManaObject>();
                manaObject.AttachToHand(handTransform);
                activeManaObjects.Add(manaObject);
                manaObject.spellSlotInfo = spellMenu.castDownMap[i];
                manaManager.LoseMana(5);
                int remainingIndices = spellMenu.castDownMap.Count - 1 - i;
                if (remainingIndices <= 2)
                { continue; }
                int openParenthesesPassed = 0;
                int closeParenthesesPassed = 0;
                int closeParenthesisIndex = 0;
                for (int eggIndex = i + 1; eggIndex < spellMenu.castDownMap.Count; eggIndex++) // do another loop to iterate through the the spells until you get to a an amount of closed parentheses passed that is equal to the amount of open parentheses passed (including the first open parenthesis).
                {
                    if (spellMenu.castDownMap[eggIndex].spellType == openParenthesis)
                    {
                        openParenthesesPassed++;
                    }
                    if (spellMenu.castDownMap[eggIndex].spellType == closeParenthesis)
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
                    if (spellMenu.castDownMap[eggIndex].spellType == SpellSlot.SpellType.Empty)
                    { continue; }
                    if (spellMenu.castDownMap[eggIndex].spellType == SpellSlot.SpellType.Ball)
                    {
                        if (manaManager.manaAmount <= 0) 
                        { continue; }
                        GameObject ballObject = Instantiate(ballPrefab, spawnedEgg.transform.position, transform.rotation); // Any spells within the parentheses should be spawned in the egg
                        PhysicalProperties ballPhysicalProperties = ballObject.GetComponent<PhysicalProperties>();
                        ballPhysicalProperties.manaResistance = spellMenu.castDownMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = ballObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castDownMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (spellMenu.castDownMap[eggIndex].spellType == SpellSlot.SpellType.Spark)
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject sparkObject = Instantiate(sparkPrefab, spawnedEgg.transform.position, transform.rotation);
                        sparkObject.transform.SetParent(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castDownMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                    if (IsEggSpell(spellMenu.castDownMap[eggIndex].spellType))
                    {
                        if (manaManager.manaAmount <= 0)
                        { continue; }
                        GameObject innerEggObject = Instantiate(innerEggPrefab, spawnedEgg.transform.position, transform.rotation);
                        PhysicalProperties innerEggPhysicalProps = innerEggObject.GetComponent<PhysicalProperties>();
                        innerEggPhysicalProps.manaResistance = spellMenu.castDownMap[eggIndex].manaResistancePercent / 100f;
                        ManaObject innerManaObject = innerEggObject.GetComponent<ManaObject>();
                        innerManaObject.AttachToEgg(spawnedEgg.transform);
                        manaObject.spellSlotInfo = spellMenu.castDownMap[eggIndex];
                        manaManager.LoseMana(5);
                    }
                }
                i = closeParenthesisIndex;
            }
            if (spellMenu.castDownMap[i].spellType == SpellSlot.SpellType.Spark)
            {
                if (manaManager.manaAmount <= 0)
                { continue; }
                
                Instantiate(sparkPrefab, spawnPosition + downCastOffset, transform.rotation);
                manaManager.LoseMana(5);
            }
        }
    }

    void CastStart()
    {
        Vector3 spawnPosition = handTransform.position + handTransform.up;
        for (int i = 0; i < SpellMenu.castStartHeight; i++)
        {
            Vector3 spawnOffset = Vector3.zero;
            for (int j = 0; j < SpellMenu.castStartWidth; j++)
            {
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.Empty)
                { continue; }
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.Ball)
                {
                    if (manaManager.manaAmount <= 0)
                    { continue; }
                    
                    GameObject ballObject = Instantiate(ballPrefab, spawnPosition + spawnOffset, transform.rotation);
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
                    GameObject spawnedEgg = Instantiate(eggPrefab, spawnPosition + spawnOffset, transform.rotation);
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
                    
                    Instantiate(sparkPrefab, spawnPosition + spawnOffset, transform.rotation);
                    manaManager.LoseMana(5);
                }
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.XOpenParenthesis)
                {
                    spawnOffset += -handTransform.right;
                }
                if (spellMenu.castStartMap[i,j].spellType == SpellSlot.SpellType.XCloseParenthesis)
                {
                    spawnOffset += handTransform.right;
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
