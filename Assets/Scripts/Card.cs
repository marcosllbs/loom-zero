using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour
{

  [SerializeField] Image frontImage;
  [SerializeField] Image backImage;
  [SerializeField] Button cardButtom;


  private int _index;
  private CardModel _model;

  public void Init(int index, CardModel model)
  {
    _index = index;
    _model = model;
  }

}
