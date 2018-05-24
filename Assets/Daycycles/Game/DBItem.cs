using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Daycycles/Item")]
public class DBItem : ScriptableObject {

    public Sprite icon;

    public DBItemCategory[] categories;

    public override string ToString()
    {
        return name;
    }

}
