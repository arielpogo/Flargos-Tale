using UnityEngine;

public static class Factory{
    /// <summary>
    /// Custom instantiation for NavigableMenus where you pass what menu called you & what gamestate to return to when fully closed.
    /// </summary>
    public static Object InstantiateNavigableMenu(Object prefab, NavigableMenu previousMenu, GameState returnGamestate = GameState.overworld) {
        GameObject newGO = Object.Instantiate(prefab, GameObject.FindGameObjectWithTag("PlayerMenu").transform) as GameObject;
        NavigableMenu newNavMenu = newGO.GetComponent<NavigableMenu>();
        newNavMenu.Setup(previousMenu, returnGamestate);
        return newGO;
    }
}
