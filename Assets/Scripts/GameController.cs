using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Board Config")]
    [SerializeField] int rows;
    [SerializeField] int columns;

    [Header("References")]
    [SerializeField] RectTransform boardRoot;
    [SerializeField] Card card;

    private List<CardModel> models = new List<CardModel>();
    private List<Card> cards = new List<Card>();

    void Start()
    {

    }

}
