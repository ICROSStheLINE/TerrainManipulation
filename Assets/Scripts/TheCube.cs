using UnityEngine;

public class TheCube : MonoBehaviour
{
    Material material;
    World world;
    PhysicalProperties physicalProperties;
    float previousManaCharge = 0;
    bool materialized = false;
    

    void Start()
    {
        GameObject worldGameObject = GameObject.FindWithTag("World");
        world = worldGameObject.transform.GetComponent<World>();
        material = GetComponent<Renderer>().material;
        physicalProperties = GetComponent<PhysicalProperties>();
        previousManaCharge = physicalProperties.manaCharge;
    }


    void Update()
    {
        float manaChargeDelta = physicalProperties.manaCharge - previousManaCharge;

        if (manaChargeDelta != 0 && !materialized)
        {
            Materialize();
        }
        else if (manaChargeDelta == 0 && materialized)
        {
            Dematerialize();
        }


        previousManaCharge = physicalProperties.manaCharge;
    }

    void Materialize()
    {
        // if OverlappingCubes > 0 then call Latch()
    }

    void Dematerialize()
    {
        
    }

    void Latch()
    {
        // I have no idea what to do here.
        // I guess take in the cube position and information as arguments?
        // Then move them accordingly and have them only be able to move in a spot that an air block was?
        // Maybe find a way to store information on where the overlapping cube was relative to this object, then constantly keep it moving towards that relative position
    }

    Block.BlockType[] OverlappingCubes()
    {
        // Find all the whole number Vector3 coordinates that this cube overlaps
        //      - Note that this will depend on the size/bounds of the cube, so account for that accordingly
        //      - EDIT: Actually whole number coordinates are the CORNERS of cubes. I guess then I must find out which half number (x = 0.5, y = 0.5) coordinates it overlaps?
        //          - EDIT2: Well it gets floored to int anyways in the next function that gets called, so does it even matter?
        //              - EDIT3: Actually it probably does matter. It would make sense if a cube is detected to be overlapped when this object touches the centre of it instead of a random corner.
        // Use world.ConvertWorldPositionToCubeInChunk(...) for each coordinate found.
        // Find the info on what blocktype that cube and chunk position refers to
        // Return an array of blocktypes that aren't air blocks.
        // Profit
        return null;
    }
}
