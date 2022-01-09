using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    public Gamemanager board;
    public GameObject finestra;
    public Text testo;
    public Text input;
    public Text tempo;

    public void OnMouseDown()
    {
      Debug.Log("ok");
      if (testo.text==input.text) {
        Debug.Log("ok");
        input.text="";
        finestra.SetActive(false);
      }else{
        Debug.Log("no");
        input.text="";
        finestra.SetActive(false);
      }
      board.stop=false;
    }
    float timer = 0.0f;

    void Update()
    {
      timer += Time.deltaTime;
      int seconds = (int) timer % 60;
      int rimanenti= 8 - seconds;
      tempo.text="" + rimanenti;
      if (seconds==0) {
       testo.text=Randomstring();
      }
      if (seconds==8)
      {
        timer = 0.0f;
        board.stop=false;
        if (testo.text==input.text)
        {
          Debug.Log("ok");
          string nuovo="";
          input.text=nuovo;
          board.activecell.mCurrentProp.cariche--;
          finestra.SetActive(false);
        }else{
          Debug.Log("no");
          string nuovo="";
          input.text=nuovo;
          board.killer.PathFindingKiller(board.activecell.mRoomPosition.x,board.activecell.mRoomPosition.y);
          board.killer.sprint=10;
          board.killer.caccia=2;
          SpriteRenderer renderer2 = board.killer.GetComponent<SpriteRenderer>();
          renderer2.color= new Color32(100,100,100,255);
          finestra.SetActive(false);
        }
      }
      if (seconds>5 && testo.text==input.text)
      {
        timer = 0.0f;
        board.stop=false;
        Debug.Log("ok");
        string nuovo="";
        input.text=nuovo;
        board.activecell.mCurrentProp.cariche--;
        if (board.activecell.mCurrentProp.cariche==0) { //engine completato, killer carica e il giocatore ricarica il dodge
          SpriteRenderer renderer = board.activecell.mCurrentProp.GetComponent<SpriteRenderer>();
          renderer.sprite = board.activecell.mCurrentProp.spriteenginedone;
          board.killer.PathFindingKiller(board.activecell.mRoomPosition.x,board.activecell.mRoomPosition.y);
          //board.killer.sprint=10;
          board.killer.caccia=2;
          SpriteRenderer renderer2 = board.killer.GetComponent<SpriteRenderer>();
          renderer2.color= new Color32(100,100,100,255);
          board.mPersonaggio.carichedodge=1;
          board.dodgetext.text="Uncunny dodge (1)";
        }
        finestra.SetActive(false);
      }
    }

    private string Randomstring ()
    {
      string[] carattere = new string[] {"a","b","c","d","e","f","g","h","i","l","m","n","o","p","q","r","s","t","u","v","z","k","j","y","0","1","2","3","4","5","6","7","8","9"};
      string finale="";
      for (int i=0; i<7; i++)
      {
        int random = Random.Range(0,34);
        finale+=carattere[random];
      }
      return finale;
    }

}
