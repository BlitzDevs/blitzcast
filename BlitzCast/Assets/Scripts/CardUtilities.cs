using System;
using System.Collections.Generic;
using UnityEngine;

public static class CardUtilities
{

    public static List<Card> Clone(List<Card> original)
    {
        List<Card> newList = new List<Card>(original.Count);

        original.ForEach((item) =>
        {
            newList.Add(item);
        });

        return newList;
    }

    public static void Shuffle(this List<Card> deck, System.Random random)
    {
        for (int i = 0; i < deck.Count; i++)
            deck.Swap(i, random.Next(i, deck.Count));
    }

    public static void Swap(this List<Card> deck, int i, int j)
    {
        var temp = deck[i];
        deck[i] = deck[j];
        deck[j] = temp;
    }

}
