using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Start
    public Board plansza;
    public Menager_Figur _Menager_Figur;
    void Start()
    {

        //Tworzenie planszy
        plansza.Stworz();

        //Tworzenie figur
        _Menager_Figur.Zaladuj(plansza);
    }
    #endregion


}
