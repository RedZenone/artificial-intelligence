using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{

  public Astar astar;
  public Gamemanager board;
  public Personaggio personaggio;

  [HideInInspector]
  public int sprint=0;
  private int stun=0;
  [HideInInspector]
  public int caccia=0; //1: inseguimento -- 2: verso i generatori falliti o finiti -- 3: ricerca del pg nei lockets
  [HideInInspector]
  public Cells mTargetCells = null;
  [HideInInspector]
  public List<Cells> path = new List<Cells>();
  private List<Cells> pathbackup = new List<Cells>();
  [HideInInspector]
  public List<Room> stanzeVisitate = new List<Room>();
  [HideInInspector]
  public List<Room> stanzeNonVisitate = new List<Room>();
  [HideInInspector]
  public Cells mCurrentCells = null;
  private List<Cells> locketslist = new List<Cells>();

  protected Cells mOriginalCells = null;

  public LayerMask mylayer;

  public virtual void Place(Cells newCells)
  {
    ////Debug.Log("killer a " + newCells.mRoomPosition.x + "||" + newCells.mRoomPosition.y);
    mCurrentCells = newCells;
    mOriginalCells = newCells;
    mCurrentCells.mCurrentKiller = this;
    transform.position = newCells.transform.position;
    gameObject.SetActive(true);
    for (int x=0; x<3; x++)
    {
      for (int y=0; y<3; y++)
      {
      stanzeNonVisitate.Add(board.mAllRooms[x,y]);
      }
    }

  }

  public void Reset()
  {
    mCurrentCells.mCurrentPiece = null;
    gameObject.SetActive(false);
    Place(mOriginalCells);
  }

  private int Max (int a, int b)
  {
    if (a>b) {
      return a;
    }else{
      return b;
    }
  }

  private void ColoraPath()
  {
    foreach(Cells cella in path)
    {
      SpriteRenderer renderer = cella.GetComponent<SpriteRenderer>();
      renderer.color= new Color32(63,63,63,255);
    }
  }

  protected void deColorCells()
  {
    foreach (Cells Cell in path)
    {
      SpriteRenderer renderer = Cell.GetComponent<SpriteRenderer>();
      renderer.color= new Color32(255,255,255,255);
    }
  }

  public void Passo()
  {
    Uccidi();
    if (stun>0) {
      stun--;
      if (stun==0) {
        SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
        renderer.color= new Color32(255,255,255,255);
      }
      return;
    }
    if (sprint>0)
    {
      for (int i=0; i<2; i++)
      {
        Scelta();
        Cells target= path[0];
        Move(target);
      }
      sprint--;
    }else{
      Scelta();
      Cells target= path[0];
      Move(target);
    }
    Uccidi();
  }

  private void Uccidi()
  {
    if (caccia==1||caccia==3)
    {
      List<Cells> celle = new List<Cells>();
      celle=board.GetNeighbours(mCurrentCells);
      celle.Add(mCurrentCells);
      if (celle.Count>0)
      {
        foreach(Cells cell in celle)
        {
          if (cell.mCurrentPiece!=null)
          {
            cell.mCurrentPiece.vita--;
            stun = 5;
            caccia=0;
            sprint=0;
            SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
            renderer.color= new Color32(255,0,0,255);
            if (cell.mCurrentPiece.nascosto==1)
            {
              SpriteRenderer colore = cell.mCurrentPiece.GetComponent<SpriteRenderer>();
              colore.color= new Color32(255,255,255,255);
              cell.mCurrentPiece.nascosto=0;
              cell.mCurrentPiece.mMovement.x=1;
              cell.mCurrentPiece.mMovement.y=1;
              cell.mCurrentPiece.CheckPathing();
            }
            if (cell.mCurrentPiece.vita==0) {
              SpriteRenderer renderer2 = cell.mCurrentPiece.GetComponent<SpriteRenderer>();
              renderer2.color= new Color32(200,30,0,255);
              cell.mCurrentPiece.mMovement.x=0;
              cell.mCurrentPiece.mMovement.y=0;
              cell.mCurrentPiece.ClearCells();
            }
          }
          if (cell.mCurrentProp!=null) {
            if (cell.mCurrentProp.tipo=="locket") {
              cell.mCurrentProp.integrità=5;
              SpriteRenderer renderer = cell.mCurrentProp.GetComponent<SpriteRenderer>();
              renderer.sprite = cell.mCurrentProp.spritelocketrotto;
              if (caccia==3)
              {
                path.Clear();
              }
            }
          }
        }
      }
    }
  }

  private void Scelta()
  {
    Visione();
    if (path.Count<1) {
      if (caccia==2 || caccia==3)
      {
        caccia=1;
      }
      if (caccia==1)
      {
        if (locketslist.Count==0)
        {
          locketslist=Locketvicini();
          caccia=3;
        }
        if (locketslist.Count==0)
        {
          //Debug.Log("lockets = vuoti " + locketslist.Count);
          caccia=0;
          SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
          renderer.color= new Color32(255,255,255,255);
          ProssimaStanza();
        }else{
          //Debug.Log("lockets = " + locketslist.Count);
          int random = Random.Range(0,locketslist.Count);
          Cells targetlocket=locketslist[random];
          int x=targetlocket.mRoomPosition.x;
          int y=targetlocket.mRoomPosition.y;
          locketslist.Remove(targetlocket);
          PathFindingKiller(x,y);
        }
      }else{
        ProssimaStanza();
      }
    }
  }

  private List<Cells> Locketvicini()
  {
    List<Cells> lockets = new List<Cells>();
    for (int x=-3; x<4; x++)
    {
      for (int y=-3; y<4; y++)
      {
        int checkx=mCurrentCells.mRoomPosition.x+x;
        int checky=mCurrentCells.mRoomPosition.y+y;
        if (checky >= 0 && checky < (board.dimensione*3) && checkx >= 0 && checkx < (board.dimensione*3) && board.mAllCells[checkx,checky].mCurrentProp!=null)
        {
          if (board.mAllCells[checkx,checky].mCurrentProp.tipo=="locket" && board.mAllCells[checkx,checky].mCurrentProp.integrità==0)
          {
            lockets.Add(board.mAllCells[checkx,checky]);
          }
        }
      }
    }
    return lockets;
  }

  private void ProssimaStanza ()
  {
    int corridoio = Random.Range(0,4);
    int[] direzione = new int[8] {0,-1,-1,0,0,1,1,0}; //differenza in x e y della stanza di arrivo
    Room stanza= mCurrentCells.mRoom;
    stanzeVisitate.Add(stanza);
    stanzeNonVisitate.Remove(stanza);
    Room stanzadiarrivo = stanza;

    for (int i=0; i<4; i++) {
      if (stanzeVisitate.Contains(stanzadiarrivo)) {
        while (stanza.collegamenti[corridoio]!=1)
        {
          corridoio++;
          if (corridoio==4)
          {
            corridoio=0;
          }
        }
        stanzadiarrivo=board.mAllRooms[stanza.mGlobalPosition.x+direzione[corridoio*2],stanza.mGlobalPosition.y+direzione[(corridoio*2)+1]];
        corridoio++;
        if (corridoio==4)
        {
          corridoio=0;
        }
      }
    }
    if (stanzeVisitate.Contains(stanzadiarrivo)) {
      int stanzarandom = Random.Range(0,stanzeNonVisitate.Count);
      //Debug.Log(stanzeNonVisitate.Count + " || " + stanzarandom);
      stanzadiarrivo = stanzeNonVisitate[stanzarandom];
    }

    int x=stanzadiarrivo.roomCells[4,4].mRoomPosition.x;
    int y=stanzadiarrivo.roomCells[4,4].mRoomPosition.y;
    while(!PathFindingKiller(x,y))
    {
      int randomx = Random.Range(-1,2);
      int randomy = Random.Range(-1,2);
      x+=randomx;
      y+=randomy;
    }
    //stanzeVisitate.Add(stanzadiarrivo);

    //Debug.Log("___");
    for (int i=0; i<stanzeVisitate.Count; i++) {
      stanzeVisitate[i].Stampa();
    }
    if(stanzeVisitate.Count==8)
    {
      stanzeVisitate.Clear();
      for (int x2=0; x2<3; x2++)
      {
        for (int y2=0; y2<3; y2++)
        {
        stanzeNonVisitate.Add(board.mAllRooms[x2,y2]);
        }
      }
    }
  }

  private bool ControlloViciniNonVisitati()
  {
    foreach (Room room in board.GetNeighbours(mCurrentCells.mRoom))
    {
      if (stanzeNonVisitate.Contains(room))
      {
        return true;
      }
    }
    return false;
  }

  public bool PathFindingKiller(int x, int y)
  {
    bool risultato=false;
    astar.FindPath(mCurrentCells.mRoomPosition.x,mCurrentCells.mRoomPosition.y,x,y,board);
    if (path.Contains(board.mAllCells[x,y]))
    {
      mTargetCells=board.mAllCells[x,y];
      risultato=true;
    }
    foreach(Cells cella in path)
    {
      if (cella.tipocella!="")
      {
        risultato=false;
      }
    }
    return risultato;
  }

  public void Move(Cells mTargetCells)
  {
    SpriteRenderer renderer = mCurrentCells.GetComponent<SpriteRenderer>();
    renderer.color= new Color32(255,255,255,255);
    ////Debug.Log("da " + mCurrentCells.mRoomPosition.x + " " + mCurrentCells.mRoomPosition.y);
    mCurrentCells.mCurrentKiller=null;
    mCurrentCells=mTargetCells;
    mCurrentCells.mCurrentKiller=this;
    path.Remove(mCurrentCells);

    transform.position=mCurrentCells.transform.position;
    mTargetCells=null;
  }

  private Transform player;

  private void Visione()
  {
    if (personaggio.nascosto==0)
    {
      int deltax= Mathf.Abs(mCurrentCells.mRoomPosition.x - personaggio.mCurrentCells.mRoomPosition.x);
      int deltay= Mathf.Abs(mCurrentCells.mRoomPosition.y - personaggio.mCurrentCells.mRoomPosition.y);
      if (deltax<8 && deltay<5)
      {
        Transform player = personaggio.GetComponent<Transform>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position-transform.position,Vector3.Distance(transform.position, player.position), mylayer); //mylayer=muri

        if (hit.collider == null)
        {
        Debug.Log("in vista");
          //Debug.DrawLine(transform.position, player.position, Color.green, 2.5f);
          PathFindingKiller(board.mPersonaggio.mCurrentCells.mRoomPosition.x,board.mPersonaggio.mCurrentCells.mRoomPosition.y);
          SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
          renderer.color= new Color32(100,100,100,255);
          //ColoraPath();
          caccia=1;
          locketslist.Clear();
        }else{
          Debug.Log("non in vista", hit.transform.gameObject);
          //Debug.DrawLine(transform.position, player.position, Color.red, 2.5f);
        }
      }
    }
    /*
    if (personaggio.nascosto==0)
    {
      int deltax= Mathf.Abs(mCurrentCells.mRoomPosition.x - personaggio.mCurrentCells.mRoomPosition.x);
      int deltay= Mathf.Abs(mCurrentCells.mRoomPosition.y - personaggio.mCurrentCells.mRoomPosition.y);
      if (deltax<8 && deltay<5 && path.Count<=Max(deltax,deltay))
      {
        pathbackup=path;
        PathFindingKiller(board.mPersonaggio.mCurrentCells.mRoomPosition.x,board.mPersonaggio.mCurrentCells.mRoomPosition.y);
        if (path.Count<=Max(deltax,deltay))
        {
          deColorCells();
          SpriteRenderer renderer = this.GetComponent<SpriteRenderer>();
          renderer.color= new Color32(100,100,100,255);
          caccia=1;
          locketslist.Clear();
        }
        else
        {
          path=pathbackup;
        }
      }
    }*/
  }

}
