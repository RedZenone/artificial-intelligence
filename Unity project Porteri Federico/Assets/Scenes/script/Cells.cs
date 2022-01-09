using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class Cells : MonoBehaviour
{
    public int ablemovimento=0;
    public bool range=false;

    public string tipocella = "";

    public GameObject mPropPrefab;

    public Sprite terra;
    public Sprite muro;

    [HideInInspector]
    public Vector2Int mRoomPosition = Vector2Int.zero; //posizione da 0 a 26 nin tutta la plancia
    [HideInInspector]
    public Vector2Int posizioneRelativa = Vector2Int.zero; //posizione da 0 a 8 nella stanza
    [HideInInspector]
    public Room mRoom = null;
    [HideInInspector]
    public Transform mTransform = null;
    [HideInInspector]
    public Personaggio mCurrentPiece = null;
    [HideInInspector]
    public Killer mCurrentKiller = null;
    [HideInInspector]
    public Prop mCurrentProp = null;

    [HideInInspector]
    public int gCost;
    [HideInInspector]
    public int hCost;
    [HideInInspector]
    public Cells parent;

    [HideInInspector]
    public int deepsearch = 0;

    public void Setup(Vector2Int newRoomPosition, Room newRoom)
    {
      mRoomPosition = newRoomPosition;
      mRoom = newRoom;
      mTransform = GetComponent<Transform>();
    }

    public void OnMouseDown()
    {
      if (!EventSystem.current.IsPointerOverGameObject())
      {
        if (!mRoom.mMegaboard.stop) {
          if (mCurrentProp!=null)
          {
            mCurrentProp.Obiettivo();
          }
          if(range==true)
          {
            mRoom.mMegaboard.mPersonaggio.Move(this);
            mRoom.mMegaboard.mPersonaggio.ClearCells();
            mRoom.mMegaboard.mPersonaggio.CheckPathing();
          }
          mRoom.mMegaboard.killer.Passo();
        }
      }
    }

    public int fCost
    {
      get {
        return gCost + hCost;
      }
    }

    public void Stampa (){
      Debug.Log("-x: " + mRoomPosition.x + " y: " +  mRoomPosition.y + " / tipo: " + tipocella);
    }

    public void PlaceEngine()
    {
      GameObject newProp = Instantiate(mPropPrefab, transform);
      mCurrentProp = newProp.GetComponent<Prop>();
      mCurrentProp.Place(this,"engine");
      SpriteRenderer renderer = mCurrentProp.GetComponent<SpriteRenderer>();
      renderer.sprite = mCurrentProp.spriteengine;
    }

    public void PlaceLocket()
    {
      GameObject newProp = Instantiate(mPropPrefab, transform);
      mCurrentProp = newProp.GetComponent<Prop>();
      mCurrentProp.Place(this,"locket");
      SpriteRenderer renderer = mCurrentProp.GetComponent<SpriteRenderer>();
      renderer.sprite = mCurrentProp.spritelocket;
    }

}
