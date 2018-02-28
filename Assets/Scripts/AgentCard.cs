using UnityEngine;
using UnityEngine.UI;

public class AgentCard : Card
{
    public Image image;

    public Sprite[] redAgents;
    public Sprite[] blueAgents;
    public Sprite deathAgent;
    public Sprite[] neutrals;

    public void SetRedAgent()
    {
        image.sprite = redAgents[Random.Range(0, redAgents.Length)];
        fraction = FractionType.redTeam;
    }

    public void SetBlueAgent()
    {
        image.sprite = blueAgents[Random.Range(0, blueAgents.Length)];
        fraction = FractionType.blueTeam;
    }

    public void SetDeathAgent()
    {
        image.sprite = deathAgent;
        fraction = FractionType.death;
    }

    public void SetNeutralCitizen()
    {
        image.sprite = neutrals[Random.Range(0, neutrals.Length)];
        fraction = FractionType.neutral;
    }
}
