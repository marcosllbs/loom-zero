using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Board Config")]
    [SerializeField] private int rows = 2;
    [SerializeField] private int columns = 3;

    [Header("Layout")]
    [SerializeField] private float margin = 20f;
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float minCardSize = 200f;
    [SerializeField] private float maxCardSize = 300f;


    [Header("References")]
    [SerializeField] private RectTransform boardRoot;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI comboText;

    [Header("Card Sprites")]
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private List<Sprite> cardFaceSprites;

    [Header("Start Reveal")]
    [SerializeField] private bool revealAtStart = true;
    [SerializeField] private float startRevealDuration = 2f;
    [SerializeField] private float revealFlipDelay = 0.05f;

    private bool canInteract = false;

    private List<CardModel> cardModelList = new List<CardModel>();
    private List<Card> cardList = new List<Card>();

    float cellWidth;
    float cellHeight;

    private Card firstCard = null;
    private Card secondCard = null;
    // private bool checking = false;

    [SerializeField] private int baseScore = 100;
    [SerializeField] private int comboBonusPerMatch = 25;

    private int score = 0;
    private int combo = 0;

    private int matchedPairs = 0;
    private int totalPairs = 0;

    private void Start()
    {
        if (SaveManager.HasSave())
        {
            LoadGameFromSave();
        }
        else
        {
            StartNewGame();
        }
    }

    public void StartNewGame()
    {
        int totalCards = rows * columns;

        if (totalCards % 2 != 0)
        {
            Debug.LogWarning("The total of Cards need to be even");
            return;
        }

        totalPairs = totalCards / 2;
        matchedPairs = 0;
        firstCard = null;
        secondCard = null;

        cardModelList = GenerateDeck(totalCards);
        BuildBoard();

        if (revealAtStart)
        {
            StartCoroutine(StartRevealRoutine());
        }
        else
        {
            canInteract = true;
        }
    }

    public void ResetAndStartNewGame()
    {
        score = 0;
        combo = 0;
        matchedPairs = 0;


        // SaveManager.Delete(); // Clear last save Data

        StartNewGame();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (comboText != null)
            comboText.text = combo > 1 ? $"Combo x{combo}" : "";
    }

    public void OnCardClicked(Card card)
    {
        if (!canInteract)
            return;

        Debug.Log($"CARD CLICKED → ID {card.Id}");
        //if (checking) return;

        card.Flip();

        if (firstCard == null)
        {
            firstCard = card;
            return;
        }

        if (secondCard == null)
        {
            secondCard = card;

            Card a = firstCard;
            Card b = secondCard;


            firstCard = null;
            secondCard = null;
            StartCoroutine(CheckMatch(a, b));
        }


    }

    private System.Collections.IEnumerator CheckMatch(Card a, Card b)
    {

        yield return new WaitForSeconds(0.1f);

        if (a.Id == b.Id)
        {
            // MATCH
            combo++;
            matchedPairs++;

            int gained = baseScore + (combo - 1) * comboBonusPerMatch;
            score += gained;

            Debug.Log($"MATCH! Combo = {combo} | +{gained} pontos | Score total = {score}");


            yield return StartCoroutine(a.PlayMatchFeedback());
            yield return StartCoroutine(b.PlayMatchFeedback());

            a.SetMatched();
            b.SetMatched();


            if (matchedPairs >= totalPairs)
            {
                Debug.Log("Todas as cartas foram encontradas! Reiniciando com novo baralho.");
                yield return new WaitForSeconds(0.5f);
                ResetAndStartNewGame();
                yield break;
            }
        }
        else
        {
            // ERROR
            combo = 0;
            Debug.Log("NO MATCH! Combo resetado.");


            yield return StartCoroutine(a.PlayMismatchFeedback());
            yield return StartCoroutine(b.PlayMismatchFeedback());


            a.Flip();
            b.Flip();
        }
    }


    private List<CardModel> GenerateDeck(int totalCards)
    {
        int pairCount = totalCards / 2;
        var list = new List<CardModel>(totalCards);


        for (int id = 0; id < pairCount; id++)
        {
            list.Add(new CardModel(id));
            list.Add(new CardModel(id));
        }


        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }


    private void BuildBoard()
    {


        for (int i = boardRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(boardRoot.GetChild(i).gameObject);
        }
        cardList.Clear();

        LayoutRebuilder.ForceRebuildLayoutImmediate(boardRoot);


        var grid = boardRoot.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;

            Vector2 size = boardRoot.rect.size;

            float reference = Mathf.Min(size.x, size.y);


            float marginFactor = 0.05f;
            float spacingFactor = 0.02f;

            float marginPx = reference * marginFactor;
            float spacingPx = reference * spacingFactor;


            float availableWidth =
                size.x - 2f * marginPx - spacingPx * (columns - 1);
            float availableHeight =
                size.y - 2f * marginPx - spacingPx * (rows - 1);

            cellWidth = availableWidth / columns;
            cellHeight = availableHeight / rows;


            float baseSize = Mathf.Min(cellWidth, cellHeight);

            float clampedSize = Mathf.Clamp(baseSize, minCardSize, maxCardSize);


            float finalWidth = clampedSize;
            float finalHeight = clampedSize;



            grid.cellSize = new Vector2(finalWidth, finalHeight);
            grid.spacing = new Vector2(spacing, spacing * columns);
            grid.padding = new RectOffset(
                Mathf.RoundToInt(margin),
                Mathf.RoundToInt(margin),
                Mathf.RoundToInt(margin),
                Mathf.RoundToInt(margin)
            );
        }

        for (int i = 0; i < cardModelList.Count; i++)
        {
            var model = cardModelList[i];


            Sprite frontSprite = cardFaceSprites[model.Id];
            Sprite backSprite = cardBackSprite;

            var cardInstance = Instantiate(cardPrefab, boardRoot);
            cardInstance.Init(i, model, frontSprite, backSprite);
            cardList.Add(cardInstance);


            var rt = cardInstance.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localScale = Vector3.one;
        }



    }

    public void SaveGame()
    {
        if (cardList == null || cardList.Count == 0)
        {
            Debug.LogWarning("No cards to save.");
            return;
        }

        SaveData data = new SaveData();
        data.rows = rows;
        data.columns = columns;
        data.score = score;
        data.combo = combo;

        int count = cardList.Count;
        data.cardIds = new List<int>(count);
        data.matched = new List<bool>(count);
        data.revealed = new List<bool>(count);

        for (int i = 0; i < count; i++)
        {
            Card c = cardList[i];
            data.cardIds.Add(c.Id);
            data.matched.Add(c.IsMatched);
            data.revealed.Add(c.IsFlipped);
        }

        SaveManager.Save(data);
    }

    public void LoadGameFromSave()
    {
        SaveData data = SaveManager.Load();
        if (data == null)
        {
            Debug.LogWarning("Cannot load game: no save data.");
            return;
        }


        rows = data.rows;
        columns = data.columns;
        score = data.score;
        combo = data.combo;

        cardModelList.Clear();


        for (int i = 0; i < data.cardIds.Count; i++)
        {
            int id = data.cardIds[i];
            var model = new CardModel(id);
            model.IsMatched = data.matched[i];
            model.IsRevealed = data.revealed[i];
            cardModelList.Add(model);
        }


        BuildBoard();


        for (int i = 0; i < cardList.Count; i++)
        {
            Card c = cardList[i];
            bool matched = data.matched[i];
            // bool revealed = data.revealed[i];


            //  c.SetFlipped(revealed, instant: true);


            if (matched)
            {
                c.SetMatched();
            }
        }

        Debug.Log("Game loaded from save.");
        canInteract = true;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application quitting, saving game...");
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("Application paused (background), saving game...");
            SaveGame();
        }
    }

    private System.Collections.IEnumerator StartRevealRoutine()
    {
        canInteract = false;


        foreach (var card in cardList)
        {
            card.SetFlipped(true, instant: true);
        }


        yield return new WaitForSeconds(startRevealDuration);


        foreach (var card in cardList)
        {
            card.SetFlipped(false, instant: false);
            yield return new WaitForSeconds(revealFlipDelay);
        }


        canInteract = true;
    }
}