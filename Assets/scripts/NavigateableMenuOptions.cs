using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavigateableMenuOptions : MonoBehaviour{
    [Serializable]
    private class ColumnInfo {
        [SerializeField] private TextMeshProUGUI[] rows;
    }

    [SerializeField] private ColumnInfo[] pagesArray;
}
