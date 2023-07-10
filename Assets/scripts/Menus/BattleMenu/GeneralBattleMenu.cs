using UnityEngine;
using UnityEngine.InputSystem;

public class GeneralBattleMenu : NavigableMenu {
    [SerializeField] private GameObject _fightMenuPrefab;
    [SerializeField] private GameObject _actMenuPrefab; //remove soon
    [SerializeField] private GameObject _itemMenuPrefab;

    private void Awake() {
        _currentColumn = 0;
    }

    public override void OnNavigate(InputValue value) {
        Vector2 controlDirection = value.GetCallBackContext().ReadValue<Vector2>();
        
        if(_currentRow != -1) _columns[_currentColumn].Rows[_currentRow].color = _colorIdle;

        if (controlDirection.y > 0) { //W
            if (_currentRow != 0) _currentRow = 0;
            else if (enabled) {
                Factory.InstantiateNavigableMenu(_fightMenuPrefab, this);
                enabled = false;
            }
        }
        else if(controlDirection.x < 0){ //A
            if (_currentRow != 1) _currentRow = 1;
            else if (enabled) {
                Factory.InstantiateNavigableMenu(_actMenuPrefab, this);
                enabled = false;
            }
        }
        else if(controlDirection.y < 0) { //S
            if (_currentRow != 2) _currentRow = 2;
            else if (enabled) {
                Factory.InstantiateNavigableMenu(_itemMenuPrefab, this);
                enabled = false;
            }
        }
        else if(controlDirection.x > 0) { //D
            if (_currentRow != 3) _currentRow = 3;
            else if (enabled) {
                flee();
            }
        }
        _columns[_currentColumn].Rows[_currentRow].color = _colorHighlight;
    }

    private void flee() {

    }
}
