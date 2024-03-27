using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemUI : MonoBehaviour
{
    public int itemCount;
    [SerializeField] TMP_Text itemCountText;
    public itemStats item;

    private void Start()
    {
        itemCountText.outlineColor = Color.black;
        itemCountText.outlineWidth = 0.2f;
    }

}
