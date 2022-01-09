using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Personaggio : EventTrigger
{
    [HideInInspector]
    public int vita=2;
    protected Cells mOriginalCells = null;
    [HideInInspector]
    public Cells mCurrentCells = null;

    protected Cells mTargetCells = null;
    [HideInInspector]
    public Vector3Int mMovement = new Vector3Int(1,1,0);
    [HideInInspector]
    public int dodge = 0;
    protected List<Cells> mHighlightedCells = new List<Cells>();

    [HideInInspector]
    public int nascosto = 0;
    [HideInInspector]
    public int carichedodge = 1;

    public virtual void Place(Cells newCells)
    {
      mCurrentCells = newCells;
      mOriginalCells = newCells;
      mCurrentCells.mCurrentPiece = this;

      transform.position = newCells.transform.position;
      gameObject.SetActive(true);
      mCurrentCells.mRoom.mMegaboard.camerafollow.transform.position = transform.position;

      CheckPathing();
    }

    public void Reset()
    {
      mCurrentCells.mCurrentPiece = null;
      gameObject.SetActive(false);
      Place(mOriginalCells);
    }

    private void CreateCellsPath(int xDirection, int yDirection, int movement)
    {
      int currentX = mCurrentCells.mRoomPosition.x;
      int currentY = mCurrentCells.mRoomPosition.y;

      for (int i=1; i<=movement;i++)
      {
        currentX += xDirection;
        currentY += yDirection;

        CellsState mCellsState = CellsState.None;
        mCellsState = mCurrentCells.mRoom.ValidateCells(currentX,currentY, this);

        if (mCellsState != CellsState.Free)
          break;


        mHighlightedCells.Add(mCurrentCells.mRoom.mMegaboard.mAllCells[currentX, currentY]);
      }
      ShowCells();
    }

    public virtual void CheckPathing()
    {
      //horizontal
      CreateCellsPath(1,0, mMovement.x);
      CreateCellsPath(-1,0, mMovement.x);

      //vertical
      CreateCellsPath(0,1, mMovement.y);
      CreateCellsPath(0,-1, mMovement.y);

      //upper diagonal
      CellsState mCellsState = CellsState.None;
      CellsState mCellsState2 = CellsState.None;
      mCellsState = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x, mCurrentCells.mRoomPosition.y+1, this);
      mCellsState2 = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x+1, mCurrentCells.mRoomPosition.y, this);
      if(mCellsState == CellsState.Free || mCellsState2 == CellsState.Free) CreateCellsPath(1,1, mMovement.z);
      mCellsState = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x, mCurrentCells.mRoomPosition.y+1, this);
      mCellsState2 = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x-1, mCurrentCells.mRoomPosition.y, this);
      if(mCellsState == CellsState.Free || mCellsState2 == CellsState.Free) CreateCellsPath(-1,1, mMovement.z);

      //lower diagonal
      mCellsState = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x, mCurrentCells.mRoomPosition.y-1, this);
      mCellsState2 = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x+1, mCurrentCells.mRoomPosition.y, this);
      if(mCellsState == CellsState.Free || mCellsState2 == CellsState.Free) CreateCellsPath(1,-1, mMovement.z);
      mCellsState = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x, mCurrentCells.mRoomPosition.y-1, this);
      mCellsState2 = mCurrentCells.mRoom.ValidateCells(mCurrentCells.mRoomPosition.x-1, mCurrentCells.mRoomPosition.y, this);
      if(mCellsState == CellsState.Free || mCellsState2 == CellsState.Free) CreateCellsPath(-1,-1, mMovement.z);
    }

    protected void ShowCells()
    {
      foreach (Cells Cell in mHighlightedCells)
      {
        SpriteRenderer renderer = Cell.GetComponent<SpriteRenderer>();
        renderer.color= new Color32(100,100,100,255);
        Cell.range = true;
      }
    }

    public void ClearCells()
    {
      foreach (Cells Cell in mHighlightedCells)
      {
        SpriteRenderer renderer = Cell.GetComponent<SpriteRenderer>();
        renderer.color= new Color32(255,255,255,255);
        Cell.range=false;
      }

      mHighlightedCells.Clear();
    }

    public void Move(Cells mTargetCells)
    {
      mCurrentCells.mCurrentPiece=null;
      mCurrentCells=mTargetCells;
      mCurrentCells.mCurrentPiece=this;

      transform.position=mCurrentCells.transform.position;
      mCurrentCells.mRoom.mMegaboard.camerafollow.transform.position = transform.position;
      mTargetCells=null;
      if (dodge>0) {
        dodge--;
      }else{
        if (mMovement.z!=0)
        {
          mMovement.z=0;
        }
      }
    }

    public void OnMouseDown()
    {
      if(mCurrentCells.mCurrentProp.tipo=="locket" && nascosto==0)
      {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        renderer.color= new Color32(100,100,100,255);
        nascosto=1;
      }
      if(mCurrentCells.mCurrentProp.tipo=="locket" && nascosto==1)
      {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        renderer.color= new Color32(255,255,255,255);
        nascosto=0;
      }
      mCurrentCells.OnMouseDown();
      Debug.Log("personaggio");
    }
}
