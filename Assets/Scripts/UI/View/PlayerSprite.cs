using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteUI : MonoBehaviour
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Image mainWeaponImage;
    [SerializeField] private Image deputyWeaponImage;

    private Vector3 mainWeaponPos = new(-17, -3, 0);
    private Vector3 deputyWeaponPos = Vector3.zero;

    private void Start()
    {
        mainWeaponImage.transform.position = playerImage.transform.TransformPoint(mainWeaponPos);
        // deputyWeaponImage.transform.position = deputyWeaponPos;
    }

    public void UpdateSprite(Sprite playerImage, Sprite mainWeaponImage, Sprite deputyWeaponImage, bool bRotation)
    {
        this.playerImage.sprite = playerImage;
        this.mainWeaponImage.sprite = mainWeaponImage;
        this.mainWeaponImage.transform.rotation = bRotation ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, 0);
        // this.deputyWeaponImage.sprite = deputyWeaponImage;
    }
    
    public void UpdateSprite(Sprite playerImage)
    {
        this.playerImage.sprite = playerImage;
        // this.deputyWeaponImage.sprite = deputyWeaponImage;
    }
}
