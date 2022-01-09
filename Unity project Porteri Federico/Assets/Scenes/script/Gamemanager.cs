using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public Personaggio mPersonaggio;
    public GameObject mRoomPrefab;
    public int dimensione= 11;
    public int celledamettere=30;
    public Astar astar;
    public Killer killer;
    public GameObject finestra;
    public GameObject killercambutton;
    [HideInInspector]
    public Room[,] mAllRooms= new Room[3,3];
    [HideInInspector]
    public Cells[,] mAllCells = new Cells[100,100];
    [HideInInspector]
    public bool stop=false;
    [HideInInspector]
    public int killercam=3;

    [HideInInspector]
    public Cells activecell=null;

    public Text grandezza;
    public Text riempimento;

    void Start()
    {
      finestra.SetActive(false);
      killercambutton.SetActive(false);
    }

    public void Inizio()
    {
      if (grandezza.text=="9") {
        if (riempimento.text=="Vuoto") {
          dimensione=9;
          celledamettere=0;
        }
        if (riempimento.text=="Basso") {
          dimensione=9;
          celledamettere=8;
        }
        if (riempimento.text=="Medio") {
          dimensione=9;
          celledamettere=11;
        }
        if (riempimento.text=="Fitto") {
          dimensione=9;
          celledamettere=14;
        }
      }
      if (grandezza.text=="11") {
        if (riempimento.text=="Vuoto") {
          dimensione=11;
          celledamettere=0;
        }
        if (riempimento.text=="Basso") {
          dimensione=11;
          celledamettere=20;
        }
        if (riempimento.text=="Medio") {
          dimensione=11;
          celledamettere=25;
        }
        if (riempimento.text=="Fitto") {
          dimensione=11;
          celledamettere=30;
        }
      }
      if (grandezza.text=="13") {
        if (riempimento.text=="Vuoto") {
          dimensione=13;
          celledamettere=0;
        }
        if (riempimento.text=="Basso") {
          dimensione=13;
          celledamettere=30;
        }
        if (riempimento.text=="Medio") {
          dimensione=13;
          celledamettere=40;
        }
        if (riempimento.text=="Fitto") {
          dimensione=13;
          celledamettere=50;
        }
      }
      if (grandezza.text=="15") {
        if (riempimento.text=="Vuoto") {
          dimensione=15;
          celledamettere=0;
        }
        if (riempimento.text=="Basso") {
          dimensione=15;
          celledamettere=40;
        }
        if (riempimento.text=="Medio") {
          dimensione=15;
          celledamettere=50;
        }
        if (riempimento.text=="Fitto") {
          dimensione=15;
          celledamettere=60;
        }
      }
      myStart();
    }

    void myStart()
    {
      Create();
      mAllRooms[1,1].collegataalcentro=1;
      ProceduraleCorridoi();
      Riempimento();
      int tredici=(((dimensione*3)-1)/2);
      mPersonaggio.Place(mAllCells[tredici,tredici]);
      mAllCells[tredici,tredici].tipocella = "";
      mAllCells[tredici,tredici].gameObject.layer = 0;
      SpriteRenderer renderer = mAllCells[tredici,tredici].GetComponent<SpriteRenderer>();
      renderer.sprite = mAllCells[0,0].terra;
      ClearVicini(mAllCells[tredici,tredici]);
      Placekiller();
      PlaceProps();
      mPersonaggio.CheckPathing();
      killercambutton.SetActive(true);
    }


    public void Create()
    {
      for (int y=0; y<3; y++)
      {
        for (int x=0; x<3; x++)
        {
          GameObject newRoom = Instantiate(mRoomPrefab, transform);

          Transform mTransform = newRoom.GetComponent<Transform>();
          mTransform.position = new Vector2(dimensione*(x-1), dimensione*(y-1));

          mAllRooms[x, y] = newRoom.GetComponent<Room>();
          mAllRooms[x, y].Create(new Vector2Int(x,y), this,dimensione);
        }
      }
    }

    //celle vicini, diagonale compresa, la diagonale non verrà contata se di mezzo c'è un muro
    public List<Cells> GetNeighbours(Cells cell)
    {
      List<Cells> neighbours = new List<Cells>();

      for (int x = -1; x <= 1; x=x+2) {
        int checkX = cell.mRoomPosition.x + x;

        if (checkX >= 0 && checkX < (dimensione*3) && mAllCells[checkX,cell.mRoomPosition.y].tipocella=="") {
          neighbours.Add(mAllCells[checkX,cell.mRoomPosition.y]);
        }
      }
      for (int y = -1; y <= 1; y=y+2) {
        int checkY = cell.mRoomPosition.y + y;

        if (checkY >= 0 && checkY < (dimensione*3) && mAllCells[cell.mRoomPosition.x,checkY].tipocella=="") {
          neighbours.Add(mAllCells[cell.mRoomPosition.x,checkY]);
        }
      }

      for (int x = -1; x <= 1; x=x+2) {
        for (int y = -1; y <= 1; y=y+2) {
          int checkX = cell.mRoomPosition.x + x;
          int checkY = cell.mRoomPosition.y + y;

          if (checkY >= 0 && checkY < (dimensione*3) && checkX >= 0 && checkX < (dimensione*3) && mAllCells[checkX,checkY].tipocella=="") {
            if (mAllCells[cell.mRoomPosition.x,checkY].tipocella=="" && mAllCells[checkX,cell.mRoomPosition.y].tipocella=="") {
              neighbours.Add(mAllCells[checkX,checkY]);
            }
          }
        }
      }
      return neighbours;
    }

    public List<Room> GetNeighbours(Room room)
    {
      List<Room> neighbours = new List<Room>();
      for (int x = -1; x <= 1; x=x+2)
      {
        int checkX = room.mGlobalPosition.x + x;
        if (checkX >= 0 && checkX < 3)
        {
          neighbours.Add(mAllRooms[checkX,room.mGlobalPosition.y]);
        }
      }
      for (int y = -1; y <= 1; y=y+2)
      {
        int checkY = room.mGlobalPosition.y + y;
        if (checkY >= 0 && checkY < 3)
        {
          neighbours.Add(mAllRooms[room.mGlobalPosition.x,checkY]);
        }
      }
      ////Debug.Log(neighbours.Count);
      return neighbours;
    }

//______________________________________________________________________________
//    CREAZIONE DEI CORRIDOI PROCEDURALI CON ANNESSO CONTROLLO SolvibilitaCorridoi
//______________________________________________________________________________
    private void ProceduraleCorridoi()
    {
      while(true)
      {
        Randomcorridoi();
        SolvibilitaCorridoi(1,1,0);
        if(ControlloSolvibilitaCorridoi()==9)
        {
          return;
        }
        Resetcorridoi();
      }
    }

    public void Randomcorridoi()
    {
      //stanza centrale, ha 3 o 4 corridoi random
      bool verifica=false;
      while(!verifica)
      {
        int giu = Random.Range(0,2);
        int sin = Random.Range(0,2);
        int su = Random.Range(0,2);
        int dex = Random.Range(0,2);
        if(giu+su+sin+dex>2)
        {
          //verifico che ci siano almeno 3 corridoi
          verifica=true;
          //////Debug.Log("giu " + giu + " sin "+ sin + " su " + su + " dex" + dex);
          mAllRooms[1,1].Corridoi( giu, sin, su, dex);
          Vettorializza(giu, sin, su, dex, 1, 1);
          //////Debug.Log(mAllRooms[1,1].collegamenti);
        }
      }
      verifica=false;

      //creo almeno 1 corridoi dalle stanze laterali alle loro rispoettive stanze sopra o sotto
      while(!verifica)
      {
        int giu = Random.Range(0,2);
        int su = Random.Range(0,2);
        if(giu+su>0)
        {
          //verifico che ci siano almeno 1 corridoi
          verifica=true;
          for (int i=0; i<3; i+=2) {
            mAllRooms[i,1].Corridoi( giu, 0, su, 0);
            Vettorializza(giu, 0, su, 0, i, 1);
            //////Debug.Log(mAllRooms[i,1].collegamenti);
          }
        }
      }
      verifica=false;

      //creo almeno 1 corridoi dalle stanze sopra e sotto alle loro rispoettive stanze laterali
      while(!verifica)
      {
        int sin = Random.Range(0,2);
        int dex = Random.Range(0,2);
        if(sin+dex>0)
        {
          //verifico che ci siano almeno 1 corridoi
          verifica=true;
          for (int i=0; i<3; i+=2) {
            mAllRooms[1,i].Corridoi( 0, sin, 0, dex);
            Vettorializza( 0, sin, 0, dex, 1, i);
            //////Debug.Log(mAllRooms[1,i].collegamenti);
          }
        }
      }
    }

    public void Vettorializza(int giu, int sin, int su, int dex, int roomx, int roomy)
    {
      //inizializza il vettore collegamenti
      if (giu==1)
      {
        mAllRooms[roomx, roomy].collegamenti[0]=1;
        mAllRooms[roomx, roomy-1].collegamenti[2]=1;
      }
      if (sin==1)
      {
        mAllRooms[roomx, roomy].collegamenti[1]=1;
        mAllRooms[roomx-1, roomy].collegamenti[3]=1;
      }
      if (su==1)
      {
        mAllRooms[roomx, roomy].collegamenti[2]=1;
        mAllRooms[roomx, roomy+1].collegamenti[0]=1;
      }
      if (dex==1)
      {
        mAllRooms[roomx, roomy].collegamenti[3]=1;
        mAllRooms[roomx+1, roomy].collegamenti[1]=1;
      }
    }

    private void SolvibilitaCorridoi(int roomx, int roomy, int i)
    {
      //collega al centro tutte le stanze collegate a questa
      //ricorsivo per 4 volte centro-> laterali centrali-> angoli-> laterali centrali
      if(i<4)
      {
          if (mAllRooms[roomx,roomy].collegataalcentro==1)
          {
              //giu
              if (mAllRooms[roomx, roomy].collegamenti[0]==1)
              {
                mAllRooms[roomx,roomy-1].collegataalcentro=1;
                SolvibilitaCorridoi(roomx,roomy-1,i+1);
              }
              //sin
              if (mAllRooms[roomx, roomy].collegamenti[1]==1)
              {
                mAllRooms[roomx-1,roomy].collegataalcentro=1;
                SolvibilitaCorridoi(roomx-1,roomy,i+1);
              }
              //su
              if (mAllRooms[roomx, roomy].collegamenti[2]==1)
              {
                mAllRooms[roomx,roomy+1].collegataalcentro=1;
                SolvibilitaCorridoi(roomx,roomy+1,i+1);
              }
              //dex
              if (mAllRooms[roomx, roomy].collegamenti[3]==1)
              {
                mAllRooms[roomx+1,roomy].collegataalcentro=1;
                SolvibilitaCorridoi(roomx+1,roomy,i+1);
              }
          }
      }
      return;
    }

    private int ControlloSolvibilitaCorridoi()
    {
      int risultato=0;
      for (int i=0; i<3; i++)
      {
        for (int l=0; l<3; l++)
        {
          risultato=risultato+mAllRooms[i,l].collegataalcentro;
        }
      }

      return risultato;
    }

    private void Resetcorridoi()
    {
      for (int i=0; i<3; i++)
      {
        for (int l=0; l<3; l++)
        {
          mAllRooms[i,l].collegataalcentro=0;
          mAllRooms[i,l].Murare();
          for (int k=0; k<4; k++)
          {
            mAllRooms[i,l].collegamenti[k]=0;
          }
        }
      }
      mAllRooms[1,1].collegataalcentro=1;
    }

    //______________________________________________________________________________


    //______________________________________________________________________________
    //              RIEMPIMENTO DELLE STANZE E CONTROLLO DELLA SOLVIBILITA'
    //______________________________________________________________________________
    private void Riempimento()
    {

      for (int i=0; i<3; i++)
      {
        for (int l=0; l<3; l++)
        {
          ////Debug.Log("_______________" + i + "||" + l + "_______________");
          int somma=0; // controllo se esiste un solo corridoio nella stanza
          for (int k=0; k<4; k++)
          {
            somma+=mAllRooms[i,l].collegamenti[k];
          }
          Randomcelle(i,l);
          Gameoflife(i,l);
          if(!Solvibilità(i,l))
          {
            Resettastanza(i,l);
            l--;
          }
          if(i==1 && l==1) //controllo che dalla cella centrale della stanza centrale sia risolvibile, basta raggiungere anche un solo corridoio
          {
            if(!Solvibilitàdalcentro(1,1))
            {
              Resettastanza(1,1);
              l=0;
            }
          }
          if (somma==1)
          {
            ////Debug.Log("un solo corridoio");
            if(!Solvibilitàdalcentro(i,l))
            {
              Resettastanza(i,l);
              l--;
            }
          }
        }
      }
      //secondo giro di controlli
      for (int i=0; i<10; i++) {
        Controllosecondo();
      }
    }

    private void Controllosecondo()
    {
      for (int i=0; i<3; i++)
      {
        for (int l=0; l<3; l++)
        {
          if(!Solvibilità(i,l))
          {
            Resettastanza(i,l);
            Randomcelle(i,l);
            Gameoflife(i,l);
            l--;
          }
        }
      }
      while(!Solvibilità(1,1) || !Solvibilitàdalcentro(1,1))
      {
        Resettastanza(1,1);
        Randomcelle(1,1);
        Gameoflife(1,1);
      }
    }

    private void Resettastanza (int roomx, int roomy)
    {
      for (int i=1; i<(dimensione-1); i++) {
        for (int l=1; l<(dimensione-1); l++) {
          mAllRooms[roomx,roomy].roomCells[i,l].tipocella = "";
          mAllRooms[roomx,roomy].roomCells[i,l].gameObject.layer = 0;
          SpriteRenderer renderer = mAllRooms[roomx,roomy].roomCells[i,l].GetComponent<SpriteRenderer>();
          renderer.sprite = mAllCells[0,0].terra;
        }
      }
    }

    private void Randomcelle(int roomx, int roomy)
    {
      for (int i=0; i<celledamettere; i++)
      {
        int cellax = Random.Range(1,(dimensione-1));
        int cellay = Random.Range(1,(dimensione-1));
        mAllRooms[roomx,roomy].roomCells[cellax,cellay].tipocella = "m";
        mAllRooms[roomx,roomy].roomCells[cellax,cellay].gameObject.layer = 8;
        SpriteRenderer renderer = mAllRooms[roomx,roomy].roomCells[cellax,cellay].GetComponent<SpriteRenderer>();
        renderer.sprite = mAllCells[0,0].muro;
      }
    }

    private void Gameoflife(int roomx, int roomy)
    {
      for (int i=1; i<(dimensione-1); i++)
      {
        for (int l=1; l<(dimensione-1); l++) {
          if (Controllovicini(i,l,roomx,roomy)==1) {
            mAllRooms[roomx,roomy].roomCells[i,l].tipocella = "m";
            mAllRooms[roomx,roomy].roomCells[i,l].gameObject.layer = 8;
            SpriteRenderer renderer = mAllRooms[roomx,roomy].roomCells[i,l].GetComponent<SpriteRenderer>();
            renderer.sprite = mAllCells[0,0].muro;
            //renderer.renderer2.sprite = mAllCells[0,0].muro;
          }
          if (Controllovicini(i,l,roomx,roomy)==2) {
            mAllRooms[roomx,roomy].roomCells[i,l].tipocella = "";
            mAllRooms[roomx,roomy].roomCells[i,l].gameObject.layer = 0;
            SpriteRenderer renderer = mAllRooms[roomx,roomy].roomCells[i,l].GetComponent<SpriteRenderer>();
            renderer.sprite = mAllCells[0,0].terra;
          }
        }
      }
    }

    //controlla quanti muri sono confinanti
    private int Controllovicini(int cellax, int cellay, int roomx, int roomy)
    {
      int contatorevicini = 0;
      for (int i=-1; i<2; i+=2)
      {
        if (mAllRooms[roomx,roomy].roomCells[cellax+i,cellay].tipocella == "m")
          contatorevicini++;
      }
      for (int l=-1; l<2; l+=2) {
        if (mAllRooms[roomx,roomy].roomCells[cellax,cellay+l].tipocella == "m")
          contatorevicini++;
      }
      if (contatorevicini>2) {
        return 1;
      }
      if (contatorevicini==0) {
        return 2;
      }
      return 0;
    }

    //controlla la solvibilità, che esista un percorso da ogni corridoio aperto a tutti gli altri corridoi
    private bool Solvibilità(int x, int y)
    {
      //elenco in ordine delle coordinate dei corridoi {x,y,x,y,x,y,x,y,x,y} gli ultimi due servono per ritornare al primo punto
      int[] coordinate = new int[8] {((dimensione-1)/2),0,0,((dimensione-1)/2),((dimensione-1)/2),(dimensione-1),(dimensione-1),((dimensione-1)/2)};
      //le combinazioni di corridoi
      Room room = mAllRooms[x,y];
      for (int l=0; l<3; l++)
      {
        int startx= coordinate[(l*2)];
        int starty= coordinate[((l*2)+1)];
        if (room.collegamenti[l]==0)
        {
          continue;
        }
        for (int i=1; i<(4-l); i++)
        {
          int targetx=coordinate[(i*2)+(2*l)];
          int targety=coordinate[((i*2)+1)+(2*l)];
          if (room.collegamenti[i+l]==0)
          {
            continue;
          }
          if(!Solvibilità(startx,starty,targetx,targety,room))
          {
            return false;
          }
        }
      }
      return true;
    }
    private bool Solvibilità (int sx, int sy, int tx, int ty, Room room)
    {
      astar.FindPathInterno(sx,sy,tx,ty,room);
      foreach(Cells cella in room.path)
      {
        if (cella.tipocella!="")
        {
          return false;
        }
      }
      /*if (room.path.Contains(room.roomCells[tx,ty]))
      {
        return true;
      }*/
      if (room.path.Contains(room.roomCells[tx,ty]))
      {
        foreach(Cells cella in room.path)
        {
          if (cella.tipocella!="") {
            return false;
          }
          cella.tipocella = "";
          cella.gameObject.layer = 0;
          SpriteRenderer renderer = cella.GetComponent<SpriteRenderer>();
          renderer.sprite = cella.terra;
          /*renderer.color= new Color32(63,63,63,255);
          renderer.sprite = cella.terra;*/
        }
        return true;
      }
      return false;
    }
    private bool Solvibilitàdalcentro (int x, int y)
    {
      if(Solvibilità(((dimensione-1)/2),((dimensione-1)/2),((dimensione-1)/2),0,mAllRooms[x,y]) || Solvibilità(((dimensione-1)/2),((dimensione-1)/2),0,((dimensione-1)/2),mAllRooms[x,y]) || Solvibilità(((dimensione-1)/2),((dimensione-1)/2),(dimensione-1),((dimensione-1)/2),mAllRooms[x,y]) || Solvibilità(((dimensione-1)/2),((dimensione-1)/2),((dimensione-1)/2),(dimensione-1),mAllRooms[x,y]))
      {
        return true;
      }
      return false;
    }
    private bool Solvibilitàdaengine (int x, int y, Room room)
    {
      if(Solvibilità(x,y,((dimensione-1)/2),0,room) || Solvibilità(x,y,0,((dimensione-1)/2),room) || Solvibilità(x,y,(dimensione-1),((dimensione-1)/2),room) || Solvibilità(x,y,((dimensione-1)/2),(dimensione-1),room))
      {
        return true;
      }
      return false;
    }


    //______________________________________________________________________________

    //______________________________________________________________________________
    //                            GESTIONE KILLER
    //______________________________________________________________________________

    private void Placekiller()
    {
      int roomx = Random.Range(0,2);
      int roomy = Random.Range(0,2);

      Cells killercell;
      killercell=mAllRooms[roomx*2,roomy*2].roomCells[((dimensione-1)/2),((dimensione-1)/2)];
      int controllo=0;
      while(controllo==0)
      {
        if (Solvibilitàdalcentro(roomx*2,roomy*2) && Solvibilità(roomx*2,roomy*2))
        {
          controllo=1;
          killercell.tipocella = "";
          killercell.gameObject.layer = 0;
          SpriteRenderer renderer = killercell.GetComponent<SpriteRenderer>();
          renderer.sprite = killercell.terra;
          ClearVicini(killercell);
          killer.Place(killercell);
        }
        else
        {
          Resettastanza(roomx*2,roomy*2);
          Randomcelle(roomx*2,roomy*2);
          Gameoflife(roomx*2,roomy*2);
        }
      }
    }

    private void ClearVicini(Cells cella)
    {
      for (int x = -1; x <= 1; x=x+2) {
        int checkX = cella.mRoomPosition.x + x;

        if (checkX >= 0 && checkX < (dimensione*3)) {
          mAllCells[checkX,cella.mRoomPosition.y].tipocella = "";
          mAllCells[checkX,cella.mRoomPosition.y].gameObject.layer = 0;
          SpriteRenderer renderer2 = mAllCells[checkX,cella.mRoomPosition.y].GetComponent<SpriteRenderer>();
          renderer2.sprite = mAllCells[checkX,cella.mRoomPosition.y].terra;
        }
      }
      for (int y = -1; y <= 1; y=y+2) {
        int checkY = cella.mRoomPosition.y + y;

        if (checkY >= 0 && checkY < (dimensione*3)) {
          mAllCells[cella.mRoomPosition.x,checkY].tipocella = "";
          mAllCells[cella.mRoomPosition.x,checkY].gameObject.layer = 0;
          SpriteRenderer renderer2 = mAllCells[cella.mRoomPosition.x,checkY].GetComponent<SpriteRenderer>();
          renderer2.sprite = mAllCells[cella.mRoomPosition.x,checkY].terra;
        }
      }
    }

    //______________________________________________________________________________

    //______________________________________________________________________________
    //                    GESTIONE OBIETTIVI E LOCKETS
    //______________________________________________________________________________

    private void PlaceProps()
    {
      //in ogni stanza con un engine c'è almeno un locket
      List<Room> stanzeconengine = new List<Room>();
      //prendo una stanza in un angolo
      int x = Random.Range(0,2);
      int y = Random.Range(0,2);
      PlaceEngine(mAllRooms[x*2,y*2]);
      PlaceLocket(mAllRooms[x*2,y*2]);
      stanzeconengine.Add(mAllRooms[x*2,y*2]);
      //prendo una delle due stanze laterali centrali, opposte a quella precedente (es: 1° 2|0 --> 1|2 o 0|1)
      List<Room> stanzepossibili = new List<Room>();
      if (x==0) {
        stanzepossibili.Add(mAllRooms[2,1]);
      }else{
        stanzepossibili.Add(mAllRooms[0,1]);
      }
      if (y==0) {
        stanzepossibili.Add(mAllRooms[1,2]);
      }else{
        stanzepossibili.Add(mAllRooms[1,0]);
      }
      int random = Random.Range(0,2);
      PlaceEngine(stanzepossibili[random]);
      PlaceLocket(stanzepossibili[random]);
      stanzeconengine.Add(stanzepossibili[random]);
      //poi ne prendo una a caso non centrale, che non sia già stata scelta
      int x2 = Random.Range(0,3);
      int y2 = Random.Range(0,3);
      while(stanzeconengine.Contains(mAllRooms[x2,y2]) || (x2==1 && y2==1))
      {
        x2 = Random.Range(0,3);
        y2 = Random.Range(0,3);
      }
      while(stanzeconengine.Contains(mAllRooms[x2,y2]) || (x2==1 && y2==1))
      {
        x2 = Random.Range(0,3);
        y2 = Random.Range(0,3);
      }
      PlaceEngine(mAllRooms[x2,y2]);
      PlaceLocket(mAllRooms[x2,y2]);

      //ora i locket
      while(numerolocket>0)
      {
        for (int i=0; i<3; i++)
        {
          for (int l=0; l<3; l++)
          {
            int random2 = Random.Range(0,2);
            if (random2==1 && numerolocket>0)
              PlaceLocket(mAllRooms[i,l]);
          }
        }
      }
    }

    private int numerolocket=7;

    public void PlaceEngine(Room room)
    {
      while(true)
      {
        int x = Random.Range(1,(dimensione-1));
        int y = Random.Range(1,(dimensione-1));
        if (room.roomCells[x,y].tipocella=="" && Solvibilitàdaengine(x,y,room))
        {
          for (int i=0; i<10; i++) {
            if (!Solvibilitàdaengine(x,y,room)) {
              continue;
            }
          }
          room.roomCells[x,y].PlaceEngine();
          break;
        }
      }
    }

    public bool Murovicino(int x, int y, Room room)
    {
      foreach (Cells cell in room.GetNeighboursRoom(room.roomCells[x,y]))
      {
        if (cell.tipocella!="") {
          return true;
        }
      }
      return false;
    }

    public void PlaceLocket(Room room)
    {
      int x = Random.Range(1,(dimensione-1));
      int y = Random.Range(1,(dimensione-1));
      while(room.roomCells[x,y].mCurrentProp!=null || room.roomCells[x,y].tipocella!="") // && !Murovicino(x,y,room)
      {
        int newx = Random.Range(-1,2);
        int newy = Random.Range(-1,2);
        x+=newx;
        y+=newy;
        if (x<1) {
          x=1;
        }
        if (x>(dimensione-1)) {
          x=(dimensione-1);
        }
        if (y<1) {
          y=1;
        }
        if (y>(dimensione-1)) {
          y=(dimensione-1);
        }
      }
      while(room.roomCells[x,y].mCurrentProp!=null || room.roomCells[x,y].tipocella!="") // && !Murovicino(x,y,room)
      {
        int newx = Random.Range(-1,2);
        int newy = Random.Range(-1,2);
        x+=newx;
        y+=newy;
        if (x<1) {
          x=1;
        }
        if (x>(dimensione-1)) {
          x=(dimensione-1);
        }
        if (y<1) {
          y=1;
        }
        if (y>(dimensione-1)) {
          y=(dimensione-1);
        }
      }
      ////Debug.Log("controllo= " + room.roomCells[x,y].mRoomPosition.x + "||" + room.roomCells[x,y].mRoomPosition.y + " _ vicino a un muro?= " + Murovicino(x,y,room));
      if (room.roomCells[x,y].mCurrentProp==null && room.roomCells[x,y].tipocella=="")
      {
        room.roomCells[x,y].PlaceLocket();
        numerolocket--;
      //  //Debug.Log(numerolocket);
      }
    }

    [HideInInspector]
    public bool killercambool=true; //per sapere chi sta guardando

    public GameObject camerafollow;
    public Text killercameratext;

    public void KillerCam()
    {
      if (killercambool)
      {
        if (killercam>0)
        {
          killercam--;
          killercambool=false;
          camerafollow.transform.position=killer.transform.position;
          killercameratext.text= "Killer Cam (" + killercam + ")";
        }
      }else{
        killercambool=true;
        camerafollow.transform.position=mPersonaggio.transform.position;
      }
    }

    public Text dodgetext;
    public void Dodge()
    {
      if (mPersonaggio.carichedodge==1)
      {
        mPersonaggio.mMovement.z=1;
        mPersonaggio.dodge=4;
        dodgetext.text="Uncanny dodge (0)";
        mPersonaggio.carichedodge--;
        mPersonaggio.CheckPathing();
      }
    }
}
