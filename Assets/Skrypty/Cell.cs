using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {

    #region Poczatek
    public Image _OutlineImage;
    [HideInInspector]
    public Vector2Int Pozycja_Planszy = Vector2Int.zero;
    [HideInInspector]
    public Board plansza = null;
    [HideInInspector]
    public RectTransform _RectTransform = null;
    [HideInInspector]
    public Bazowa_Figura Aktualna_Figura = null;
    #endregion

    #region Ustawienia
    public void Zaladuj(Vector2Int nowa_Pozycja_Planszy, Board newBoard)
    {
        Pozycja_Planszy = nowa_Pozycja_Planszy;
        plansza = newBoard;

        _RectTransform = GetComponent<RectTransform>();
    }
    #endregion
    public void Usun_Figure()
    {
        if(Aktualna_Figura != null)
        {
            Aktualna_Figura.Zabij();
        }
    }
}
