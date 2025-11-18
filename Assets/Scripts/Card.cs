using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Card : MonoBehaviour, IPointerClickHandler
{
  public int Id { get; private set; }
  public bool IsFlipped { get; private set; }
  public bool IsMatched { get; private set; }

  [SerializeField] Image frontImage;
  [SerializeField] Image backImage;

  private GameController controller;

  public void Init(int index, CardModel model)
  {
    Id = model.Id;
    controller = FindObjectOfType<GameController>();


    frontImage.color = new Color(Random.value, Random.value, Random.value);
    SetFlipped(false, instant: true);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (IsMatched) return;
    if (IsFlipped) return;

    controller.OnCardClicked(this);
  }

  public void Flip()
  {
    SetFlipped(!IsFlipped);
  }

  public void SetMatched()
  {
    IsMatched = true;
    gameObject.SetActive(false);
  }

  public void SetFlipped(bool flipped, bool instant = false)
  {
    IsFlipped = flipped;

    if (instant)
    {
      frontImage.gameObject.SetActive(flipped);
      backImage.gameObject.SetActive(!flipped);
      return;
    }

    // ---- animação simples ---
    frontImage.gameObject.SetActive(flipped);
    backImage.gameObject.SetActive(!flipped);
  }
}