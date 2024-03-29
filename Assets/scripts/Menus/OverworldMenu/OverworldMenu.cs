using UnityEngine;

/// <summary>
/// The general menu acessible in the overworld.
/// </summary>
public class OverworldMenu : NavigableMenu{
    [SerializeField] private GameObject _inventoryMenuPrefab;
    [SerializeField] private GameObject _questMenuPrefab; //remove soon
    [SerializeField] private GameObject _statsMenuPrefab;

    private void Start() {
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.ui_opened);
    }

    public override void OnSubmit() {
        if (enabled) {
            switch (_currentRow) {
                case 0:
                    Factory.InstantiateNavigableMenu(_inventoryMenuPrefab, this);
                    enabled = false;
                    break;
                case 1:
                    Factory.InstantiateNavigableMenu(_questMenuPrefab, this);
                    enabled = false;
                    break;
                case 2:
                    Factory.InstantiateNavigableMenu(_statsMenuPrefab, this);
                    enabled = false;
                    break;
            }
        }
    }   
}
