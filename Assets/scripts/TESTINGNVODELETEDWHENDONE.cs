using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TESTINGNVODELETEDWHENDONE : MonoBehaviour
{
    [Serializable]
    private class PageInfo {
        [SerializeField] private ColumnInfo[] Columns;
    }

    [Serializable]
    private class ColumnInfo {
        [SerializeField] private TextMeshProUGUI[] Rows;
    }

    [SerializeField] private PageInfo[] pagesArray;
}
