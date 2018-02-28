using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject cardPlaceHolderPrefab;

    public GameObject[] cardPlaceHolders;

    public GameObject deckPlace;
    public Deck redDeck;
    public Deck blueDeck;

    public void SetupPlaceholders()
    {
        cardPlaceHolders = new GameObject[GlobalOptions.CardNumber];
        for (int i = 0; i < GlobalOptions.CardNumber; i++)
        {
            cardPlaceHolders[i] = Instantiate(cardPlaceHolderPrefab, gameObject.transform);
        }
        Debug.Log("cardPlaceHolders instantiated.");
    }
}
