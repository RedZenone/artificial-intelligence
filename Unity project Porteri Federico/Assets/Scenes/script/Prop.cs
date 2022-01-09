using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
  public Sprite spriteengine;
  public Sprite spriteenginedone;
  public Sprite spritelocket;
  public Sprite spritelocketrotto;

  protected Cells mOriginalCells = null;
  [HideInInspector]
  public Cells mCurrentCells = null;

  private Gamemanager board;

  [HideInInspector]
  public Vector2Int mPosition = Vector2Int.zero;
  [HideInInspector]
  public string tipo = "";
  [HideInInspector]
  public int cariche = 5;
  [HideInInspector]
  public int integrità = 0;

  public virtual void Place(Cells newCells, string newtipo)
  {
    mCurrentCells = newCells;
    mOriginalCells = newCells;
    mCurrentCells.mCurrentProp = this;
    mPosition=mCurrentCells.mRoomPosition;
    //Debug.Log(mPosition.x + "||" + mPosition.y);
    tipo=newtipo;
    board=mCurrentCells.mRoom.mMegaboard;
    //transform.position = newCells.transform.position;
    Transform mtransform = this.GetComponent<Transform>();
    mtransform.position = new Vector3(newCells.transform.position.x, newCells.transform.position.y,-1);
    gameObject.SetActive(true);
  }

  public void OnMouseDown()
  {
    Obiettivo();
    mCurrentCells.OnMouseDown();
  }

  public void Obiettivo()
  {
    if(!board.stop)
    {
      if (mCurrentCells.mCurrentPiece!=null)
      {
        if (mCurrentCells.mCurrentPiece.nascosto==0 && tipo=="locket" && integrità==0)
        {
          SpriteRenderer renderer = mCurrentCells.mCurrentPiece.GetComponent<SpriteRenderer>();
          renderer.color= new Color32(100,100,100,255);
          mCurrentCells.mCurrentPiece.nascosto=1;
          mCurrentCells.mCurrentPiece.mMovement.x=0;
          mCurrentCells.mCurrentPiece.mMovement.y=0;
          mCurrentCells.mCurrentPiece.ClearCells();
        }
        else if (mCurrentCells.mCurrentPiece.nascosto==1 && tipo=="locket"  && integrità==0)
        {
          SpriteRenderer renderer = mCurrentCells.mCurrentPiece.GetComponent<SpriteRenderer>();
          renderer.color= new Color32(255,255,255,255);
          mCurrentCells.mCurrentPiece.nascosto=0;
          mCurrentCells.mCurrentPiece.mMovement.x=1;
          mCurrentCells.mCurrentPiece.mMovement.y=1;
          mCurrentCells.mCurrentPiece.CheckPathing();
        }
        if (tipo=="locket"  && integrità>0) {
          integrità--;
          if (integrità==0) {
            SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
            renderer.sprite = spritelocket;
          }
        }
        if (mCurrentCells.mCurrentPiece!=null && tipo=="engine" && cariche>0)
        {
          board.stop=true;
          board.activecell=mCurrentCells;
          board.finestra.SetActive(true);
        }
        if (mCurrentCells.mCurrentPiece!=null && tipo=="engine" && cariche==0)
        {
          SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
          renderer.sprite = spriteenginedone;
          board.killer.PathFindingKiller(mCurrentCells.mRoomPosition.x,mCurrentCells.mRoomPosition.y);
          board.killer.sprint=10;
          board.killer.caccia=2;
          SpriteRenderer renderer2 = board.killer.GetComponent<SpriteRenderer>();
          renderer2.color= new Color32(100,100,100,255);
        }
      }
    }
  }

}
