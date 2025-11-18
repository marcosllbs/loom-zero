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

    [Header("Card Sprites")]
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private List<Sprite> cardFaceSprites;

    private List<CardModel> cardModelList = new List<CardModel>();
    private List<Card> cardList = new List<Card>();

    float cellWidth;
    float cellHeight;

    private Card firstCard = null;
    private Card secondCard = null;
    // private bool checking = false;

    private int score = 0;

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        int totalCards = rows * columns;

        if (totalCards % 2 != 0)
        {
            Debug.LogWarning("The total of Cards need to be even");
            return;
        }

        cardModelList = GenerateDeck(totalCards);
        BuildBoard();
    }

    public void OnCardClicked(Card card)
    {
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
        // checking = true;


        yield return new WaitForSeconds(0.1f);

        if (a.Id == b.Id)
        {

            yield return StartCoroutine(a.PlayMatchFeedback());
            yield return StartCoroutine(b.PlayMatchFeedback());

            a.SetMatched();
            b.SetMatched();

            score += 100;
            Debug.Log($"MATCH! Score = {score}");
        }
        else
        {

            yield return StartCoroutine(a.PlayMismatchFeedback());
            yield return StartCoroutine(b.PlayMismatchFeedback());

            a.Flip();
            b.Flip();

            Debug.Log("NO MATCH!");
        }

        a.Flip();
        b.Flip();
        //checking = false;
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
}