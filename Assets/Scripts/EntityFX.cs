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

    private void Start()
    {
        sr = GetComponentsInChildren<SpriteRenderer>()[1];

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

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color color = sr.color;
        sr.color = Color.red;

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
