using System.Collections;
using UnityEngine;

/**
 * 特效
 */
public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("元素颜色")]
    [SerializeField] private Color chilledColor;
    [SerializeField] private Color[] ignitedColor;
    [SerializeField] private Color[] shockedColor;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        originalMat = sr.material;
    }

    public void MakeTransparent(bool transparent)
    {
        if (transparent)
        {
            sr.color = Color.clear;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    public void ChillFXFor(float seconds)
    {
        InvokeRepeating("ChilledColorFX", 0, .5f);
        sr.color = chilledColor;
        Invoke("CancelBlink", seconds);
    }

    public void IgniteFXFor(float seconds)
    {
        InvokeRepeating("IgniteColorFX", 0, .5f);
        Invoke("CancelBlink", seconds);
    }

    public void ShockFXFor(float seconds)
    {
        InvokeRepeating("ShockedColorFX", 0, .5f);
        Invoke("CancelBlink", seconds);
    }

    private void IgniteColorFX()
    {
        if (sr.color != ignitedColor[0])
        {
            sr.color = ignitedColor[0];
        }
        else
        {
            sr.color = ignitedColor[1];
        }
    }

    private void ShockedColorFX()
    {
        if (sr.color != shockedColor[0])
        {
            sr.color = shockedColor[0];
        }
        else
        {
            sr.color = shockedColor[1];
        }
    }

    private void ChilledColorFX()
    {
        sr.color = chilledColor;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color color = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(.2f);

        sr.color = color;
        sr.material = originalMat;
    }

    private void RedColorBlink() => sr.color = sr.color != Color.white ? Color.white : Color.red;

    private void CancelBlink()
    {
        CancelInvoke();
        sr.color = Color.white;
    }
}
