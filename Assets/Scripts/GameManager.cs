using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Variables and Operables")]
    [HideInInspector] public List<string> unusedWords;
    public CardWithWord[] cardsWithWord;
    public bool isSomeCardMoving = false;  // TODO HIDE IN INSPECTOR
    public bool isSelectionAllowed = false;
    int hintNumber; // Какое количество слов подразумевает капитан команды.
    int hintRemain;
    public float timeRemain;
    float timeOfRoundStart;
    public GameStatus gameStatus;

    [Header("Teams")]
    public Team redTeam;
    public Team blueTeam;
    Deck redDeck;
    Deck blueDeck;
    Team currentTeam; // Who plays now.

    [Header("Prefabs")]
    public GameObject cardWithWordPrefab;
    public GameObject agentCardPrefab;

    [Header("Links and UI Elements")]
    public Table table;
    public WordsLibrary wordsLibrary;
    public TextMeshProUGUI defeatLabel;
    public TextMeshProUGUI statusLabel;
    public Transform containerForMovedCards; // Последний в иерархии объект, чтоб двигаемые карты были выше остальных объектов и были видны.
    public GameObject panelWithWordsNumber;
    public TextMeshProUGUI timeLabel ;
    public GameObject scoreScreen;

    [Header("Sound")]
    public SoundPlayer soundPlayer;

    // Use this for initialization
    void Start ()
    {
        Initialization();
        StartNewRound();
    }

    private void Initialization()
    {
        Instance = this;
        table.SetupPlaceholders();
        redTeam.deck = table.redDeck;
        blueTeam.deck = table.blueDeck;
        unusedWords = wordsLibrary.words.ToList();
    }

    void Update()
    {
        if (gameStatus == GameStatus.TeamSelectsWords)
        {
            timeRemain = GlobalOptions.RoundTime - (Time.realtimeSinceStartup - timeOfRoundStart);
            if (timeRemain <= 0f)
            {
                // TODO END TURN
                soundPlayer.PlayTimeOverSound();
                ChangeTeam();
            }
        }
    }

    public void StartNewRound()
    {
        ClearGameTable();
        panelWithWordsNumber.SetActive(false);
        timeLabel.gameObject.SetActive(false);
        statusLabel.gameObject.SetActive(false);

        if (Random.Range(0, 2) == 0)
            currentTeam = redTeam;
        else currentTeam = blueTeam;
        currentTeam.agentsLeft = 9;
        NotCurrentTeam().agentsLeft = 8;

        cardsWithWord = new CardWithWord[table.cardPlaceHolders.Length];
        StartCoroutine("CardDistribution");
    }


    
    IEnumerator CardDistribution()
    {
        List<FractionType> fractionsPool = new List<FractionType>();
        for (int i = 0; i < redTeam.agentsLeft; i++)
            fractionsPool.Add(redTeam.FractionType);
        for (int i = 0; i < blueTeam.agentsLeft; i++)
            fractionsPool.Add(blueTeam.FractionType);
        fractionsPool.Add(FractionType.death);
        while   (fractionsPool.Count < GlobalOptions.CardNumber)
            fractionsPool.Add(FractionType.neutral);

        for (int i = 0; i < table.cardPlaceHolders.Length; i++)
        {
            string word = unusedWords[Random.Range(0, unusedWords.Count)];
            unusedWords.Remove(word);

            CardWithWord card = cardsWithWord[i] = Instantiate(cardWithWordPrefab, table.deckPlace.transform).GetComponent<CardWithWord>();
            cardsWithWord[i].wordLabel.text = word;
            cardsWithWord[i].SetRandomAngle();
            int rnd = Random.Range(0, fractionsPool.Count);
            cardsWithWord[i].fraction = fractionsPool[rnd];
            fractionsPool.RemoveAt(rnd);

            card.PlaySound();
            LeanTween.move(cardsWithWord[i].gameObject, table.cardPlaceHolders[i].transform, 0.5f);
            
            if (cardsWithWord[i].fraction == FractionType.blueTeam)
                cardsWithWord[i].gameObject.GetComponent<Image>().color = Color.blue;
            if (cardsWithWord[i].fraction == FractionType.redTeam)
                cardsWithWord[i].gameObject.GetComponent<Image>().color = Color.red;
            if (cardsWithWord[i].fraction == FractionType.death)
                cardsWithWord[i].gameObject.GetComponent<Image>().color = Color.gray;

            yield return new WaitForSeconds(0.25f);
        }

        for (int i = 0; i < redTeam.agentsLeft; i++)
        {
            AgentCard agentCard = Instantiate(agentCardPrefab, table.redDeck.transform).GetComponent<AgentCard>();
            agentCard.SetRedAgent();
            agentCard.SetRandomAngle(5f);
            agentCard.SetRandomOffset(40f);
            agentCard.PlaySound();
            table.redDeck.cards.Add(agentCard);
            yield return new WaitForSeconds(0.2f);
        }
        for (int i = 0; i < blueTeam.agentsLeft; i++)
        {
            AgentCard agentCard = Instantiate(agentCardPrefab, table.blueDeck.transform).GetComponent<AgentCard>();
            agentCard.SetBlueAgent();
            agentCard.SetRandomAngle(5f);
            agentCard.SetRandomOffset(40f);
            agentCard.PlaySound();
            table.blueDeck.cards.Add(agentCard);
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("Card Distribution done.");

        StartCaptainTurn();
    }

    public void SelectCard (Card card)
    {
        if (gameStatus != GameStatus.TeamSelectsWords)
            return;
        if (isSomeCardMoving)
            return;
        if (isSelectionAllowed == false)
            return;

        card.GetComponent<Button>().interactable = false;

        // Если выбрана карточка, скрывающая агента красной или синей команды.
        if (card.fraction == FractionType.blueTeam || card.fraction == FractionType.redTeam)
        {
            Team team = GetTeamByFraction(card.fraction);
            team.agentsLeft--;
            Card agentCard = team.deck.cards[team.deck.cards.Count - 1];
            team.deck.cards.Remove(agentCard);
            agentCard.MoveToNewLocation(card.transform, true);

            // Если свой агент.
            if (card.fraction == currentTeam.FractionType)
            {
                // TODO KEEP SELECTING IF FREE WORDS AVAILABLE OR END TURN IF NO FREE WORDS
                soundPlayer.PlayApplauseSound();
                if (currentTeam.agentsLeft == 0)
                    StartCoroutine("VictoryDemonstration");
                else
                {
                    hintRemain--;
                    statusLabel.text = currentTeam.name + ": УГАДЫВАЙТЕ! ВАМ ОСТАЛОСЬ " + hintRemain + " СЛОВА.";
                    if (hintRemain <= 0)
                        ChangeTeam();
                }
            }
            // Если чужой агент.
            else
            {
                // TODO END TURN
                soundPlayer.PlayBooSound();
                if (NotCurrentTeam().agentsLeft > 0)
                    ChangeTeam();
                else StartCoroutine("VictoryDemonstration");
            }

        }

        //if (card.fraction == FractionType.blueTeam)
        //{
        //    blueTeam.agentsLeft --;
        //    Card agentCard = table.blueDeck.cards[table.blueDeck.cards.Count - 1];
        //    table.blueDeck.cards.Remove(agentCard);
        //    agentCard.MoveToNewLocation (card.transform, true);
        //    //LeanTween.move(agentCard.gameObject, card.gameObject.transform, 0.5f);
        //}
        //else if (card.fraction == FractionType.redTeam)
        //{
        //    redTeam.agentsLeft--;
        //    Card agentCard = table.redDeck.cards[table.redDeck.cards.Count - 1];
        //    table.redDeck.cards.Remove(agentCard);
        //    agentCard.MoveToNewLocation(card.transform, true);
        //    //LeanTween.move(agentCard.gameObject, card.gameObject.transform, 0.5f);
        //}
        else if (card.fraction == FractionType.death)
        {
            isSelectionAllowed = false;
            AgentCard deathCard = Instantiate(agentCardPrefab, card.transform).GetComponent<AgentCard>();
            deathCard.SetDeathAgent();
            deathCard.SetRandomOffset(10f);
            deathCard.SetRandomAngle(2f);
            NotCurrentTeam().scores++;
            soundPlayer.PlayDefeatSound();
            StartCoroutine("DefeatDemonstration");
        }
        else if (card.fraction == FractionType.neutral)
        {
            AgentCard neutralCard = Instantiate(agentCardPrefab, card.transform).GetComponent<AgentCard>();
            neutralCard.SetNeutralCitizen();
            neutralCard.SetRandomOffset(10f);
            neutralCard.SetRandomAngle(2f);
            soundPlayer.PlayBooSound();
            ChangeTeam();
        }
    }

    IEnumerator DefeatDemonstration()
    {
        yield return new WaitForSeconds(2f);
        defeatLabel.gameObject.SetActive(true);
        defeatLabel.text = currentTeam.name + " ПРОИГРАЛА";
        yield return new WaitForSeconds(5f);
        defeatLabel.gameObject.SetActive(false);
        scoreScreen.SetActive(true);
    }

    IEnumerator VictoryDemonstration()
    {
        isSelectionAllowed = false;
        yield return new WaitForSeconds(2f);
        defeatLabel.gameObject.SetActive(true);
        Team victoryTeam = currentTeam.agentsLeft == 0 ? currentTeam : NotCurrentTeam();
        defeatLabel.text = victoryTeam.name + " ВЫИГРАЛА";
        yield return new WaitForSeconds(5f);
        defeatLabel.gameObject.SetActive(false);
        scoreScreen.SetActive(true);
    }

    void ChangeTeam()
    {
        currentTeam = NotCurrentTeam();
        StartCaptainTurn();
    }

    // Капитан выбирает намеки.
    void StartCaptainTurn()
    {
        gameStatus = GameStatus.CaptainsChoose;
        isSelectionAllowed = false;
        panelWithWordsNumber.SetActive(true);
        timeLabel.gameObject.SetActive(false);
        statusLabel.gameObject.SetActive(true);
        statusLabel.text = currentTeam.name + ": КАПИТАН, ДЕЛАЙТЕ ПОДСКАЗКУ";
    }

    public void ClearGameTable()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        for (int i = 0; i < cards.Length; i++)
        {
            Destroy(cards[i].gameObject);
        }
        for (int i = 0; i < table.redDeck.cards.Count; i++)
        {
            Destroy(table.redDeck.cards[i].gameObject);
        }
        for (int i = 0; i < table.blueDeck.cards.Count; i++)
        {
            Destroy(table.blueDeck.cards[i].gameObject);
        }
    }

    Team NotCurrentTeam()
    {
        if (currentTeam == redTeam)
            return blueTeam;
        else return redTeam;
    }

    Team GetTeamByFraction (FractionType fraction)
    {
        if (fraction == redTeam.FractionType)
            return redTeam;
        else return blueTeam;
    }

    // Команлда начинает выбирать слова.
    public void StartCardSelection(int numberOfHints)
    {
        isSelectionAllowed = true;
        panelWithWordsNumber.SetActive(false);
        hintNumber = numberOfHints;
        hintRemain = hintNumber + 1;
        statusLabel.text = currentTeam.name + ": УГАДЫВАЙТЕ! ВАМ ОСТАЛОСЬ " + hintRemain + " СЛОВА.";
        timeRemain = GlobalOptions.RoundTime;
        timeOfRoundStart = Time.realtimeSinceStartup;
        timeLabel.gameObject.SetActive(true);
        gameStatus = GameStatus.TeamSelectsWords;
    }

    public enum GameStatus
    {
        CaptainsChoose = 1,
        TeamSelectsWords = 2
    }
}
