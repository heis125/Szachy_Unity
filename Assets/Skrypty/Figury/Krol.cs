using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Krol : Bazowa_Figura
{
    public override void Zaladuj(Color kolor_Druzyny, Color32 kolor_Figury, Menager_Figur nowyMenager_Figur)
    {
        base.Zaladuj(kolor_Druzyny, kolor_Figury, nowyMenager_Figur);

        _Ruch = new Vector3Int(1, 1, 1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Krol");
    }

    public override void Zabij()
    {
        base.Zabij();
        _Menager_Figur.Czy_krol_zyje = false;
    }
}
