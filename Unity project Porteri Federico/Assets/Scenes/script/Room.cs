using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellsState
{
    None,
    Free,
    OutOfBounds,
    Muro
}

public class Room : MonoBehaviour
{

    public GameObject mCellPrefab;

    [HideInInspector]
    public Vector2Int mGlobalPosition = Vector2Int.zero;
    [HideInInspector]
    public Transform mTransformroom = null;
    [HideInInspector]
    public Gamemanager mMegaboard = null;
    [HideInInspector]
    public Cells[,] roomCells;
    private int dimensione;

    //bool per sapere se è raggiungibile dal centro
    public int collegataalcentro = 0;

    // array per indicare dove sono i corridoi. sono giu, sin, su, dex
    public int[] collegamenti;

    [HideInInspector]
    private float posizione = 1;

    public void Create(Vector2Int newGlobalPosition, Gamemanager newMegaboard, int newdimensione)
    {
      collegamenti= new int [4];
      //crea una nuova Board
      mGlobalPosition = newGlobalPosition;
      mMegaboard=newMegaboard;
      dimensione=newdimensione;
      roomCells = new Cells[dimensione,dimensione];
      mTransformroom = GetComponent<Transform>();
      //Debug.Log(collegamenti.lenght);
      //cra la cella

      for (int y=0; y<dimensione; y++)
      {
        for (int x=0; x<dimensione; x++)
        {
          GameObject newCell = Instantiate(mCellPrefab, transform);

          Transform mtransform = newCell.GetComponent<Transform>();
          mtransform.position = new Vector2((x-3)*posizione+(dimensione*(mGlobalPosition.x-1)), (y-3)*posizione+(dimensione*(mGlobalPosition.y-1)));

          mMegaboard.mAllCells[x+(mGlobalPosition.x * dimensione), y+(mGlobalPosition.y * dimensione)] = newCell.GetComponent<Cells>();
          mMegaboard.mAllCells[x+(mGlobalPosition.x * dimensione), y+(mGlobalPosition.y * dimensione)].Setup(new Vector2Int(x+(mGlobalPosition.x * dimensione), y+(mGlobalPosition.y * dimensione)), this);
          roomCells[x,y]=mMegaboard.mAllCells[x+(mGlobalPosition.x * dimensione), y+(mGlobalPosition.y * dimensione)];
          roomCells[x,y].posizioneRelativa.x=x;
          roomCells[x,y].posizioneRelativa.y=y;
        }
      }
      Murare();
    }

    public CellsState ValidateCells(int targetX, int targetY, Personaggio checkingPiece)
    {
        int outofbounts = (int) ((dimensione*3)-1);
        if (targetX < 0 || targetX >outofbounts)
          return CellsState.OutOfBounds;
        if (targetY < 0 || targetY >outofbounts)
          return CellsState.OutOfBounds;

        Cells targetCell = mMegaboard.mAllCells[targetX,targetY];

        if(targetCell.mCurrentPiece!=null)
        {
          return CellsState.Free;
        }
        if(targetCell.tipocella=="m" || targetCell.tipocella=="m2")
        {
          return CellsState.Muro;
        }

        return CellsState.Free;
    }

    public void Murare ()
    {
      for (int i=0; i<dimensione; i++) {
        roomCells[i,0].tipocella = "m";
        roomCells[i,0].gameObject.layer = 8;
        SpriteRenderer renderer = roomCells[i,0].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].muro;
      }

      for (int i=0; i<dimensione; i++) {
        int murox = Random.Range(0,(dimensione-1));
        roomCells[i,(dimensione-1)].tipocella = "m";
        roomCells[i,(dimensione-1)].gameObject.layer = 8;
        SpriteRenderer renderer = roomCells[i,(dimensione-1)].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].muro;
      }

      for (int i=0; i<dimensione; i++) {
        int muroy = Random.Range(0,(dimensione-1));
        roomCells[0,i].tipocella = "m";
        roomCells[0,i].gameObject.layer = 8;
        SpriteRenderer renderer = roomCells[0,i].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].muro;
      }

      for (int i=0; i<dimensione; i++) {
        int muroy = Random.Range(0,(dimensione-1));
        roomCells[(dimensione-1),i].tipocella = "m";
        roomCells[(dimensione-1),i].gameObject.layer = 8;
        SpriteRenderer renderer = roomCells[(dimensione-1),i].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].muro;
      }
    }

    public void Corridoi (int giu, int sin, int su, int dex)
    {
      if (giu==1) {
        roomCells[(dimensione-1)/2,0].tipocella = "";
        roomCells[(dimensione-1)/2,0].gameObject.layer = 0;
        SpriteRenderer renderer = roomCells[(dimensione-1)/2,0].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].terra;
        mMegaboard.mAllRooms[mGlobalPosition.x,mGlobalPosition.y-1].roomCells[(dimensione-1)/2,(dimensione-1)].tipocella = "";
        mMegaboard.mAllRooms[mGlobalPosition.x,mGlobalPosition.y-1].roomCells[(dimensione-1)/2,(dimensione-1)].gameObject.layer = 0;
        SpriteRenderer renderer2 = mMegaboard.mAllRooms[mGlobalPosition.x,mGlobalPosition.y-1].roomCells[(dimensione-1)/2,(dimensione-1)].GetComponent<SpriteRenderer>();
        renderer2.sprite = roomCells[0,0].terra;
      }
      if (sin==1) {
        roomCells[0,(dimensione-1)/2].tipocella = "";
        roomCells[0,(dimensione-1)/2].gameObject.layer = 0;
        SpriteRenderer renderer = roomCells[0,(dimensione-1)/2].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].terra;
        mMegaboard.mAllRooms[mGlobalPosition.x-1,mGlobalPosition.y].roomCells[(dimensione-1),(dimensione-1)/2].tipocella = "";
        mMegaboard.mAllRooms[mGlobalPosition.x-1,mGlobalPosition.y].roomCells[(dimensione-1),(dimensione-1)/2].gameObject.layer = 0;
        SpriteRenderer renderer2 = mMegaboard.mAllRooms[mGlobalPosition.x-1,mGlobalPosition.y].roomCells[(dimensione-1),(dimensione-1)/2].GetComponent<SpriteRenderer>();
        renderer2.sprite = roomCells[0,0].terra;
      }
      if (su==1) {
        roomCells[(dimensione-1)/2,(dimensione-1)].tipocella = "";
        roomCells[(dimensione-1)/2,(dimensione-1)].gameObject.layer = 0;
        SpriteRenderer renderer = roomCells[(dimensione-1)/2,(dimensione-1)].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].terra;
        mMegaboard.mAllRooms[mGlobalPosition.x,mGlobalPosition.y+1].roomCells[(dimensione-1)/2,0].tipocella = "";
        mMegaboard.mAllRooms[mGlobalPosition.x,mGlobalPosition.y+1].roomCells[(dimensione-1)/2,0].gameObject.layer = 0;
        SpriteRenderer renderer2 = mMegaboard.mAllRooms[mGlobalPosition.x,mGlobalPosition.y+1].roomCells[(dimensione-1)/2,0].GetComponent<SpriteRenderer>();
        renderer2.sprite = roomCells[0,0].terra;
      }
      if (dex==1) {
        roomCells[(dimensione-1),(dimensione-1)/2].tipocella = "";
        roomCells[(dimensione-1),(dimensione-1)/2].gameObject.layer = 0;
        SpriteRenderer renderer = roomCells[(dimensione-1),(dimensione-1)/2].GetComponent<SpriteRenderer>();
        renderer.sprite = roomCells[0,0].terra;
        mMegaboard.mAllRooms[mGlobalPosition.x+1,mGlobalPosition.y].roomCells[0,(dimensione-1)/2].tipocella = "";
        mMegaboard.mAllRooms[mGlobalPosition.x+1,mGlobalPosition.y].roomCells[0,(dimensione-1)/2].gameObject.layer = 0;
        SpriteRenderer renderer2 = mMegaboard.mAllRooms[mGlobalPosition.x+1,mGlobalPosition.y].roomCells[0,(dimensione-1)/2].GetComponent<SpriteRenderer>();
        renderer2.sprite = roomCells[0,0].terra;
      }
    }


    //i vicini, diagonale esclusa
    public List<Cells> GetNeighboursRoom(Cells cell)
    {
      List<Cells> neighbours = new List<Cells>();

      for (int x = -1; x <= 1; x=x+2) {
        int checkX = cell.mRoomPosition.x + x;

        if (checkX >= (0+(dimensione*mGlobalPosition.x)) && checkX < (dimensione+(dimensione*mGlobalPosition.x)) && mMegaboard.mAllCells[checkX,cell.mRoomPosition.y].tipocella=="") {
          neighbours.Add(mMegaboard.mAllCells[checkX,cell.mRoomPosition.y]);
        }
      }
      for (int y = -1; y <= 1; y=y+2) {
        int checkY = cell.mRoomPosition.y + y;

        if (checkY >= (0+(dimensione*mGlobalPosition.y)) && checkY < (dimensione+(dimensione*mGlobalPosition.y)) && mMegaboard.mAllCells[cell.mRoomPosition.x,checkY].tipocella=="") {
          neighbours.Add(mMegaboard.mAllCells[cell.mRoomPosition.x,checkY]);
        }
      }

      return neighbours;
    }

    public void Stampa(){
      //Debug.Log("x:"+ mGlobalPosition.x + " y:" +mGlobalPosition.y);
    }

    public List<Cells> path;
}
