using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour
{
    [SerializeField] Image manaBar;
    public float manaMax = 100f;
    public float manaAmount;


    void Start()
    {
        manaAmount = manaMax;
    }

    void Update()
    {
        
    }

    public void LoseMana(float manaLost)
    {
        manaAmount -= manaLost;
        manaAmount = Mathf.Clamp(manaAmount, 0, manaMax);
        manaBar.fillAmount = manaAmount / manaMax;
    }

    public void GainMana(float manaGained)
    {
        manaAmount += manaGained;
        manaAmount = Mathf.Clamp(manaAmount, 0, manaMax);
        manaBar.fillAmount = manaAmount / manaMax;
    }
}
