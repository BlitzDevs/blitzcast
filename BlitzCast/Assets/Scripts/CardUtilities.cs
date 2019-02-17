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


    public static void Draw(this List<Card> deck, List<Card> hand, int amount)
    {
        for (int i = 0; i < amount; i++)
            Draw(deck, hand);
    }

    public static void Draw(this List<Card> deck, List<Card> hand)
    {
        Card newCard = deck[0].Clone();
        newCard.status = Card.CardStatus.Held;

        deck.RemoveAt(0);

        if (hand.Count < 4)
            hand.Add(newCard);
        else
        {
            int redrawCardSlot = hand.FindIndex(card => card.status == Card.CardStatus.Deck);
            if (redrawCardSlot != -1)
                hand[redrawCardSlot] = newCard;
            else
                Debug.LogError("Disaster in CardUtilities::Draw()");
        }

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
