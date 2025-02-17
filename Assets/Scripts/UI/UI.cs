using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private GameObject skillUI;
    
    private void Start()
    {
        characterUI.SetActive(false);
        SwitchCharacterUITo(equipmentUI);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TurnOrderManager.instance.GetCurrentEntity().enabled = characterUI.activeSelf;
            characterUI.SetActive(!characterUI.activeSelf);
            Inventory.instance.UpdateModelToUI();
        }
    }

    public void SwitchCharacterUITo(GameObject menu)
    {
        equipmentUI.SetActive(false);
        skillUI.SetActive(false);
        menu.SetActive(true);
    }
}
