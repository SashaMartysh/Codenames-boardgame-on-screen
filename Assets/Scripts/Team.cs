using UnityEngine;

[CreateAssetMenu(menuName = "Team")]
public class Team : ScriptableObject
{
    public string name;
    public FractionType FractionType;
    public Color color;
    public int scores;
    public GameObject[] agentCards;
    public int agentsLeft;
    public Deck deck;
}


