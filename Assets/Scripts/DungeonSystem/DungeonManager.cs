using UnityEngine;
using System.Collections;


//this class manage monsterStageLevel, sumon, player sumon, player death;
public class DungeonManager : MonoBehaviour
{//MonsterController change-> DungeonManager;
    private const int MaxPlayerNum = 4;
    private GameObject[] players = new GameObject[MaxPlayerNum];
    public GameObject[] Players { get { return players; } }

    DataSender dataSender;

    public void InitializeManager()
    {
        dataSender = GameObject.Find("NetworkManager").GetComponent<DataSender>();
    }

    protected bool modeForm; //false  -> normalBattle, true -> Defence; 
    public bool ModeForm
    {
        get { return modeForm; }
        set { modeForm = value; }
    }

    public Section[] section;

    [SerializeField]
    int mapNumber;

    //defence mode, normal mode
    public void ModeChange(bool modeForm)
    {
        if (!modeForm)
        {
            modeForm = true;
            //player1,player2 ->  nextScene; 
            //respwanstart;
        }

        if (modeForm)
        {
            //(close socket) send to otherclient socket;
        }
    }

    public void mapNumberChange()
    {
        if (mapNumber < section.Length)
        {
            mapNumber += 1;
        }

        if (mapNumber == section.Length)
        {
            ModeChange(modeForm);
        }
    }

	void DungeonConstruct()
    {
        for (int i = 0; i <= section.Length - 1; i++)
        {

            section[i].MonsterSet();
            section[i].GateNumber = i;
            section[i].ModeForm = modeForm;
            //			defenceWave[i].StartDefenceMonsterSet ();
        }
    }

    void Start()
    {
        DungeonConstruct();
        mapNumber = 0;
        modeForm = false;
        ModeChange(modeForm);//client get modeform and ingame play ;

    }

    void Update()
    {
        if (!modeForm)
        {
            for (int i = 0; i < section.Length; i++)
            {
                section[i].UpdateConduct(); //this.method change //section [mapNumber].UpdateConduct ();
            }
        }

        //if (modeForm)
        //{
        //    for (int i = 0; i < defenceWave.Length; i++)
        //    {
        //        defenceWave[i].WaveRespwan();
        //    }
        //}
    }

    public void GateNext()
    {

    }

    //	public void MonsterArrayAliveCheck(GameObject monsterArray){
    //		for (int i = 0; i < flock.Length; i++) {	
    //			flock [i].UpdateConduct();
    //		}
    //	}



    public GameObject CreatePlayer(int CharacterId)
    {
        //여기서는 플레이어 캐릭터 딕셔너리 -> 각 직업에 따른 플레이어 스탯과 능력치, 스킬, 이름을 가지고 있음
        //딕셔너리를 사용하여 그에 맞는 캐릭터를 소환해야 하지만 Prototype 진행 시에는 고정된 플레이어를 소환하도록 함.

        GameObject player = Instantiate(Resources.Load("Warrior")) as GameObject;
        player.transform.position = Vector3.zero;
        player.GetComponent<CharacterManager>().enabled = true;
        player.name = "Warrior";
        player.tag = "Player";
        players[0] = player;

        dataSender.CreateUnitSend(0, player.transform.position);

        return player;
    }

    public GameObject CreateUnit(int unitId, Vector3 newPosition)
    {
        //위와 같은 생성이지만 이곳에서는 다른 플레이어의 캐릭터를 생성한다.
        //DataHandler 에서 데이타를 받아서 실행된다.

        GameObject unit = Instantiate(Resources.Load("Warrior")) as GameObject;
        unit.transform.position = newPosition;
        unit.name = "Warrior";

        players[1] = unit;

        return unit;
    }
}