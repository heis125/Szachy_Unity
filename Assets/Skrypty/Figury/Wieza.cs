using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Wieza : Bazowa_Figura
{
    public override void Zaladuj(Color kolor_Druzyny, Color32 kolor_Figury, Menager_Figur nowyMenager_Figur)
    {
        base.Zaladuj(kolor_Druzyny, kolor_Figury, nowyMenager_Figur);

        _Ruch = new Vector3Int(7, 7, 0);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Wieza");
    }
}
