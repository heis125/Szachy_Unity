using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Okreslenie_Pola
{
    None,
    Przyjaciel,
    Wrog,
    Wolne,
    Za_Plansza

}
public enum Okreslenie_Figury
{
    Dama,
    Goniec,
    Krol,
    Skoczek,
    Wieza,
    Pionek,
    None
}
public class Board : MonoBehaviour
{
   
    public GameObject CellPrefab;

    [HideInInspector]
    public Cell[,] AllCells = new Cell[8, 8];


 
    public void Stworz()
    {
      
        for(int y=0;y<8;y++)
        { 
            for(int x=0;x<8;x++)
            {
                // Tworzenie nowego pola
                GameObject newCell = Instantiate(CellPrefab, transform);

                //Pozycja
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((x * 100) + 50, (y * 100) + 50);

                //zaladowanie
                AllCells[x, y] = newCell.GetComponent<Cell>();
                AllCells[x, y].Zaladuj(new Vector2Int(x, y), this);
            }
        }
    

    
        for (int x = 0; x < 8; x += 2) 
        {
            for (int y = 0; y < 8; y++) 
            {
                //offset dla każdej lini
                int offset = (y % 2 != 0) ? 0 : 1;
                int finalX = x + offset;

                //Kolorowanie
                AllCells[finalX, y].GetComponent<Image>().color = new Color32(230, 220, 187, 255);
            }
        }
       
    }
  
    
    public Okreslenie_Pola Sprawdzenie_Pola(int cel_x, int cel_y, Bazowa_Figura sprawdzanie)
    {
        // Czy na planszy?
        if (cel_x < 0 || cel_x > 7)
            return Okreslenie_Pola.Za_Plansza;

        if (cel_y < 0 || cel_y > 7)
            return Okreslenie_Pola.Za_Plansza;

        Cell Atak = AllCells[cel_x, cel_y];

        // Sprawdzenie czy jest wróc albo przyjaciel
        if(Atak.Aktualna_Figura != null)
        {
            if (sprawdzanie.kolor == Atak.Aktualna_Figura.kolor)
                return Okreslenie_Pola.Przyjaciel;

            if (sprawdzanie.kolor != Atak.Aktualna_Figura.kolor)
            {
                return Okreslenie_Pola.Wrog;
            }
        }
        return Okreslenie_Pola.Wolne;
    }

    // sprawdzenie jaka figura jest na danym polu 
    public Okreslenie_Figury jaka_figura(int x,int y)
    {
        // Czy na planszy?
        if (x < 0 || x > 7)
            return Okreslenie_Figury.None;

        if (y < 0 || y > 7)
            return Okreslenie_Figury.None;

        Cell cell = AllCells[x, y];

        if (cell.Aktualna_Figura != null)
        {
            if (cell.Aktualna_Figura.a == 1)
                return Okreslenie_Figury.Pionek;
            if (cell.Aktualna_Figura.a == 2)
                return Okreslenie_Figury.Wieza;
            if (cell.Aktualna_Figura.a == 3)
                return Okreslenie_Figury.Goniec;
            if (cell.Aktualna_Figura.a == 4)
                return Okreslenie_Figury.Skoczek;
            if (cell.Aktualna_Figura.a == 5)
                return Okreslenie_Figury.Krol;
            if (cell.Aktualna_Figura.a == 6)
                return Okreslenie_Figury.Dama;
            return Okreslenie_Figury.None;
        }
           return Okreslenie_Figury.None;

        
    }
    // Znalezienie możliwych ruchów 
    public List<PossibleMove> GetPossibleMoves(bool czarne)
    {
        List<PossibleMove> possibleMoves = new List<PossibleMove>();
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (jaka_figura(x, y) != Okreslenie_Figury.None)
                {
                    if (czarne)
                    {
                        if (AllCells[x, y].Aktualna_Figura.kolor == Color.black)
                            AllCells[x, y].Aktualna_Figura.Sprawdzenie_drogi(true, possibleMoves);
                    }
                    else
                        if (AllCells[x, y].Aktualna_Figura.kolor == Color.white)
                            AllCells[x, y].Aktualna_Figura.Sprawdzenie_drogi(true, possibleMoves);
                }
            
        return possibleMoves;
    }
    // tymczasowe zasymulowanie wykonania ruchu 
    public Bazowa_Figura tymczasowyRuch(PossibleMove possibleMove)
    {
        Bazowa_Figura figura = AllCells[possibleMove.toX, possibleMove.toY].Aktualna_Figura;

        AllCells[possibleMove.toX, possibleMove.toY].Aktualna_Figura = AllCells[possibleMove.fromX, possibleMove.fromY].Aktualna_Figura;
        AllCells[possibleMove.toX, possibleMove.toY].Aktualna_Figura._CurrentCell = AllCells[possibleMove.toX, possibleMove.toY];
        AllCells[possibleMove.fromX, possibleMove.fromY].Aktualna_Figura = null;
        return figura;
    }
    // Cofniecie symulacji wykonania ruchu 
    public void cofnijTymczasowyRuch(PossibleMove possibleMove, Bazowa_Figura figura)
    {
        AllCells[possibleMove.fromX, possibleMove.fromY].Aktualna_Figura = AllCells[possibleMove.toX, possibleMove.toY].Aktualna_Figura;
        AllCells[possibleMove.fromX, possibleMove.fromY].Aktualna_Figura._CurrentCell = AllCells[possibleMove.fromX, possibleMove.fromY];

        AllCells[possibleMove.toX, possibleMove.toY].Aktualna_Figura = figura;
        if (figura != null)
            AllCells[possibleMove.toX, possibleMove.toY].Aktualna_Figura._CurrentCell = AllCells[possibleMove.toX, possibleMove.toY];
    }
}
