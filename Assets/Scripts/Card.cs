using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
  public int Id { get; private set; }
  public bool IsFlipped { get; private set; }
  public bool IsMatched { get; private set; }

  [SerializeField] private Image frontImage;
  [SerializeField] private Image backImage;

  private GameController controller;
  private bool isAnimating = false;

  private RectTransform rt;


  public void Init(int index, CardModel model, Sprite frontSprite, Sprite backSprite)
  {
    Id = model.Id;

    if (rt == null)
      rt = GetComponent<RectTransform>();



    if (controller == null)
      controller = FindObjectOfType<GameController>();


    frontImage.sprite = frontSprite;
    backImage.sprite = backSprite;


    SetFlipped(false, instant: true);
    IsMatched = false;
    isAnimating = false;
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (IsMatched) return;
    if (IsFlipped) return;
    if (isAnimating) return;

    controller.OnCardClicked(this);
  }

  public void Flip()
  {
    if (isAnimating || IsMatched) return;

    if (controller != null)
      controller.PlayFlipSfx();

    SetFlipped(!IsFlipped);
  }

  public void SetMatched()
  {
    IsMatched = true;


    frontImage.enabled = false;
    backImage.enabled = false;

    frontImage.raycastTarget = false;
    backImage.raycastTarget = false;

    isAnimating = false;
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


    StopAllCoroutines();
    StartCoroutine(FlipRoutine(flipped));
  }

  private System.Collections.IEnumerator FlipRoutine(bool showFront)
  {
    isAnimating = true;

    RectTransform rt = (RectTransform)transform;
    float duration = 0.15f;
    float t = 0f;


    while (t < duration)
    {
      t += Time.deltaTime;
      float k = t / duration;
      float sx = Mathf.Lerp(1f, 0f, k);
      rt.localScale = new Vector3(sx, 1f, 1f);
      yield return null;
    }


    frontImage.gameObject.SetActive(showFront);
    backImage.gameObject.SetActive(!showFront);


    t = 0f;
    while (t < duration)
    {
      t += Time.deltaTime;
      float k = t / duration;
      float sx = Mathf.Lerp(0f, 1f, k);
      rt.localScale = new Vector3(sx, 1f, 1f);
      yield return null;
    }

    rt.localScale = Vector3.one;
    isAnimating = false;
  }

  public System.Collections.IEnumerator PlayMatchFeedback()
  {
    if (rt == null)
      rt = GetComponent<RectTransform>();

    isAnimating = true;

    float upTime = 0.12f;
    float downTime = 0.12f;
    float scaleFactor = 1.15f;

    Vector3 start = rt.localScale;
    Vector3 peak = rt.localScale * scaleFactor;

    float t = 0f;


    while (t < upTime)
    {
      t += Time.deltaTime;
      float k = t / upTime;
      rt.localScale = Vector3.Lerp(start, peak, k);
      yield return null;
    }


    t = 0f;
    while (t < downTime)
    {
      t += Time.deltaTime;
      float k = t / downTime;
      rt.localScale = Vector3.Lerp(peak, start, k);
      yield return null;
    }

    rt.localScale = start;


    frontImage.enabled = false;
    backImage.enabled = false;

    frontImage.raycastTarget = false;
    backImage.raycastTarget = false;

    isAnimating = false;
  }

  public System.Collections.IEnumerator PlayMismatchFeedback()
  {
    RectTransform rect = rt;
    Vector2 startPos = rect.anchoredPosition;

    isAnimating = true;

    float duration = 0.2f;
    float strength = 10f;

    if (rt == null)
      rt = GetComponent<RectTransform>();

    float t = 0f;
    while (t < duration)
    {
      t += Time.deltaTime;
      float k = t / duration;


      float shake = Mathf.Sin(k * Mathf.PI * 10f);
      float offsetX = shake * strength;

      rt.anchoredPosition = startPos + new Vector2(offsetX, 0f);
      yield return null;
    }

    rt.anchoredPosition = startPos;
    isAnimating = false;

  }
}