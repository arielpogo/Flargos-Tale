using TMPro;
using UnityEngine.InputSystem;

public class StatusMenu : NavigableMenu{
    public void Awake() {
        //child 1 is strength label, its child is the value
        gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SaveManager.PlayerData.strength.ToString();
        //child 1 is stealth label, its child is the value
        gameObject.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SaveManager.PlayerData.stealth.ToString();
        //might cause issues with floating point rounding
        gameObject.transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SaveManager.PlayerData.dollars.ToString() + "." + ((SaveManager.PlayerData.cents < 10) ? "0" : string.Empty) + SaveManager.PlayerData.cents.ToString();
    }

    public override void OnNavigate(InputValue value) {
        return;
    }
}
