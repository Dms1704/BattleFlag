using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;
    
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private GameObject skillUI;
    [SerializeField] private GameObject BottomUI;
    [SerializeField] private GameObject WinOrLoseUI;
    private TextMeshProUGUI text;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 确保只有一个实例
        }
    }

    private void Start()
    {
        characterUI.SetActive(false);
        SwitchCharacterUITo(equipmentUI);
        
        text = WinOrLoseUI.GetComponentInChildren<TextMeshProUGUI>();
        WinOrLoseUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            characterUI.SetActive(!characterUI.activeSelf);
            Inventory.instance.UpdateModelToUI();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            BoardManager.instance.CurrentOperateOver();
        }
    }

    public void UpdateUI(Entity entity)
    {
        BottomUI.SetActive(entity.isAlly);
    }

    public void WinOrLose(bool isWin)
    {
        WinOrLoseUI.SetActive(true);
        text.color = isWin ? Color.green : Color.red;
        text.text = isWin ? "Win" : "Lose";
        
        // BoardManager.instance.QuitGame();
    }

    public void SwitchCharacterUITo(GameObject menu)
    {
        equipmentUI.SetActive(false);
        skillUI.SetActive(false);
        menu.SetActive(true);
    }
}
