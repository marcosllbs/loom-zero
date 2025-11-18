using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Board Config")]
    [SerializeField] int rows = 2;
    [SerializeField] int columns = 4;

    [Header("References")]
    [SerializeField] RectTransform boardRoot;
    [SerializeField] Card cardPrefab;

    private List<CardModel> cardModelsList = new List<CardModel>();
    private List<Card> cardsPrefabList = new List<Card>();

    void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        int totalCards = rows * columns;

        if (totalCards % 2 != 0)
        {
            Debug.LogWarning("The total of cards need to be even");
            return;
        }

        cardModelsList = GenerateDeck(totalCards);
        BuildBoard();
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

        cardsPrefabList.Clear();

        var grid = boardRoot.GetComponent<GridLayoutGroup>();

        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;


            var size = boardRoot.rect.size;

            float cellWidth =
                (size.x - grid.padding.left - grid.padding.right - grid.spacing.x * (columns - 1))
                / columns;

            float cellHeight =
                (size.y - grid.padding.top - grid.padding.bottom - grid.spacing.y * (rows - 1))
                / rows;

            grid.cellSize = new Vector2(cellWidth, cellHeight);
        }


        for (int i = 0; i < cardModelsList.Count; i++)
        {
            var cardInstance = Instantiate(cardPrefab, boardRoot);
            cardInstance.Init(i, cardModelsList[i]);
            cardsPrefabList.Add(cardInstance);
        }
    }

}

