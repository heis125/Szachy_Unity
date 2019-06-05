using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Menager_Figur : MonoBehaviour
{
    [HideInInspector]
    public bool Czy_krol_zyje = true;

    public GameObject Prefab_Figury;
    private List<Bazowa_Figura> Biale = null;
    private List<Bazowa_Figura> Czarne = null;
    private List<Bazowa_Figura> Promowane = new List<Bazowa_Figura>();

    private Board plansza;

    public bool mat_czarny;
    public bool mat_bialy;

    PossibleMove najlepszy_ruch;

    private string[] Kolejnosc_Figur = new string[16]
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "W", "S", "G", "D", "K", "G", "S", "W"
    };

    public Dictionary<string, Type> Spis_Figur = new Dictionary<string, Type>()
    {
        {"P", typeof(Pionek)},      //1
        {"W", typeof(Wieza)},       //2
        {"S", typeof(Skoczek)},     //3
        {"G", typeof(Goniec)},      //4
        {"D", typeof(Dama)},        //5
        {"K", typeof(Krol)}         //6

    };
    public void Zaladuj(Board board)
    {
        // tworzenie białych figur
        Biale = Stworz_Figure(Color.white, new Color32(80, 124, 159, 255), board);
        //tworzenie czarnych figur
        Czarne = Stworz_Figure(Color.black, new Color32(210, 95, 64, 255), board);

        //ustawianie figur
        Ustaw_Figury(1, 0, Biale, board);
        Ustaw_Figury(6, 7, Czarne, board);
        indeksy(Biale);
        indeksy(Czarne);

        //Biełe zaczynaja 
        Zmiana_Strony(Color.black);

        mat_bialy = false;
        mat_czarny = false;

        plansza = board;

    }

    private Bazowa_Figura Stworz_F(Type typ_figury)
    {
        //Tworzenie nowego obiektu 
        GameObject nowy_Obiekt_Figury = Instantiate(Prefab_Figury);
        nowy_Obiekt_Figury.transform.SetParent(transform);

        //Skiala i pozycja 
        nowy_Obiekt_Figury.transform.localScale = new Vector3(1, 1, 1);
        nowy_Obiekt_Figury.transform.localRotation = Quaternion.identity;

        Bazowa_Figura _nowa_Figura = (Bazowa_Figura)nowy_Obiekt_Figury.AddComponent(typ_figury);

        return _nowa_Figura;
    }
    private List<Bazowa_Figura> Stworz_Figure(Color teamColor, Color32 spriteColor, Board board)
    {
        List<Bazowa_Figura> nowa_Figura = new List<Bazowa_Figura>();

        for (int i = 0; i < Kolejnosc_Figur.Length; i++)
        {

            // załadowanie typu i stworzenie obiektu
            string key = Kolejnosc_Figur[i];
            Type typ_Figury = Spis_Figur[key];

            // przechowywanie nowej figury
            Bazowa_Figura _nowa_Figura = Stworz_F(typ_Figury);
            nowa_Figura.Add(_nowa_Figura);

            //ustawienie figury
            _nowa_Figura.Zaladuj(teamColor, spriteColor, this);
        }
        return nowa_Figura;
    }

    private void Ustaw_Figury(int pionki, int krolewscy, List<Bazowa_Figura> figury, Board board)
    {
        for (int i = 0; i < 8; i++)
        {
            //ustawienie pionków
            figury[i].Miejsce(board.AllCells[i, pionki]);

            // ustawienie lini królewskiejh ;P
            figury[i + 8].Miejsce(board.AllCells[i, krolewscy]);


        }
    }

    public void indeksy(List<Bazowa_Figura> figury)
    {
        for (int i = 0; i < 8; i++)
            figury[i].a = 1;
        figury[8].a = 2;
        figury[9].a = 3;
        figury[10].a = 4;
        figury[11].a = 5;
        figury[12].a = 6;
        figury[13].a = 4;
        figury[14].a = 3;
        figury[15].a = 2;
    }

    private void Aktywnosc_Figur(List<Bazowa_Figura> wszystkie, bool czy)
    {
        foreach (Bazowa_Figura figura in wszystkie)
            figura.enabled = czy;
    }

    public void Zmiana_Strony(Color kolor)
    {
        if (!Czy_krol_zyje)
        {
            //reset figur
            Reset_Figur();

            //wskrzeszanie króla
            Czy_krol_zyje = true;

            kolor = Color.black;
        }
        bool czy_czarne = kolor == Color.white ? true : false;

        // ustawienie aktywnosci 
        Aktywnosc_Figur(Biale, !czy_czarne);
        Aktywnosc_Figur(Czarne, czy_czarne);

        //Promowany 
        foreach (Bazowa_Figura figura in Promowane)
        {
            bool czyCzarny = figura.kolor != Color.white ? true : false;
            bool czy_Druzyna = czyCzarny == true ? czy_czarne : !czy_czarne;
            figura.enabled = czy_Druzyna;
        }
        // ruch komputera 
        if(kolor == Color.white)
         {
            Board newBoard = plansza;
            int ruch = minimax(2, true, newBoard, true); 
            if (najlepszy_ruch != null) {
                Debug.Log(najlepszy_ruch.fromX + " " + najlepszy_ruch.fromY + " " + najlepszy_ruch.toX + " " + najlepszy_ruch.toY);
                plansza.AllCells[najlepszy_ruch.fromX, najlepszy_ruch.fromY].Aktualna_Figura.Pole_Ataku = plansza.AllCells[najlepszy_ruch.toX, najlepszy_ruch.toY];
                plansza.AllCells[najlepszy_ruch.fromX, najlepszy_ruch.fromY].Aktualna_Figura.Ruch();
                Zmiana_Strony(Color.black);
            }
            
         }

    }
    // funkcja zwracajaca ilosc punktów na planszy 
    private int wartosc_planszy(bool czarne, Board newBoard)
    {
        int wartosc = 0;
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (newBoard.AllCells[x, y].Aktualna_Figura)
                {
                    if (czarne)
                    {
                        if (newBoard.AllCells[x, y].Aktualna_Figura.kolor == Color.black)
                            wartosc += newBoard.AllCells[x, y].Aktualna_Figura.wartosc;
                        else
                            wartosc -= newBoard.AllCells[x, y].Aktualna_Figura.wartosc;
                    }
                    else
                    {
                        if (newBoard.AllCells[x, y].Aktualna_Figura.kolor == Color.white)
                            wartosc += newBoard.AllCells[x, y].Aktualna_Figura.wartosc;
                        else
                            wartosc += newBoard.AllCells[x, y].Aktualna_Figura.wartosc;

                    }
                } 
        return wartosc;
    }
    


    // tworzenie jakby drzewa gdzie w każdej gałęzi liczone jest co się najbardziej opłaca
    protected int minimax(int glebokosc, bool czy_Max, Board newBoard, bool czarne)
    {
        if (glebokosc == 0)
        {
            return wartosc_planszy(true, newBoard); //wartość pionków na planszy po ruchach
        }
        if (czy_Max)
        {
            int najlepsza_wartosc = -9999999;
            List<PossibleMove> possibleMoves = newBoard.GetPossibleMoves(czarne); //tworzy listę możliwych ruchów

            possibleMoves.Sort((a, b) => 1 - 2 * UnityEngine.Random.Range(0, 1));
            for (int i = 0; i < possibleMoves.Count; i++) //losowe ułożenie listy
            {
                PossibleMove temp = possibleMoves[i];
                int randomIndex = UnityEngine.Random.Range(i, possibleMoves.Count);
                possibleMoves[i] = possibleMoves[randomIndex];
                possibleMoves[randomIndex] = temp;
            }

            foreach (PossibleMove move in possibleMoves)
            {
                Bazowa_Figura figura = newBoard.tymczasowyRuch(move);
                int wartosc = minimax(glebokosc - 1, !czy_Max, newBoard, !czarne);
                if (wartosc > najlepsza_wartosc) //jak znajdzie lepszą wartość to ją wstawia
                {
                    najlepsza_wartosc = wartosc;
                    if (glebokosc == 2)
                        najlepszy_ruch = move;
                }
                newBoard.cofnijTymczasowyRuch(move, figura);
                //Debug.Log(" Best Move: " + najlepsza_wartosc + " Figura: " + move.fromX + "." + move.fromY);
            }
            return najlepsza_wartosc;
        }
        else
        {
            int najlepsza_wartosc = 9999999;
            List<PossibleMove> possibleMoves = newBoard.GetPossibleMoves(czarne); //tworzy listę możliwych ruchów

            possibleMoves.Sort((a, b) => 1 - 2 * UnityEngine.Random.Range(0, 1));
            for (int i = 0; i < possibleMoves.Count; i++) //losowe ułożenie listy
            {
                PossibleMove temp = possibleMoves[i];
                int randomIndex = UnityEngine.Random.Range(i, possibleMoves.Count);
                possibleMoves[i] = possibleMoves[randomIndex];
                possibleMoves[randomIndex] = temp;
            }

            foreach (PossibleMove move in possibleMoves)
            {
                Bazowa_Figura figura = newBoard.tymczasowyRuch(move);
                int wartosc = minimax(glebokosc - 1, !czy_Max, newBoard, !czarne);
                if (wartosc < najlepsza_wartosc) //jak znajdzie lepszą wartość to ją wstawia
                {
                    najlepsza_wartosc = wartosc;
                    if (glebokosc == 2)
                        najlepszy_ruch = move;
                }
                newBoard.cofnijTymczasowyRuch(move, figura);
                //Debug.Log(" Best Move: " + najlepsza_wartosc + " Figura: " + move.fromX + "." + move.fromY);
            }
            return najlepsza_wartosc;
        }

    }

    public void Reset_Figur()
    {
        foreach(Bazowa_Figura figura in Promowane)
        {
            figura.Zabij();
            Destroy(figura.gameObject);
 
        }
        Promowane.Clear();
        foreach (Bazowa_Figura figura in Biale)
            figura.Reset();

        foreach (Bazowa_Figura figura in Czarne)
            figura.Reset();

    }

    public void Promowanie_Figury(Pionek pionek, Cell cell, Color kolor_Druzyny, Color kolor_Figury)
    {
        // zabic pionka 
        pionek.Zabij();
        //stworzenie nowego 
        Bazowa_Figura Promowany = Stworz_F(typeof(Dama));
        Promowany.Zaladuj(kolor_Druzyny,kolor_Figury,this);
        // postawic 
        Promowany.Miejsce(cell);
        // dodac
        Promowane.Add(Promowany);
    }
   
}

