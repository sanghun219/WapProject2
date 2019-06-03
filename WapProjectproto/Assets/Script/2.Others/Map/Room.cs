using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //방의 타입
    public enum ROOMTYPE
    {
        BOSS,
        START,
        MIDDLE,
        NORMAL,
        BLOCK,
        END
    }
    //이 방이 어떤 방향으로 이동가능한지에 대한 것
    public enum DIRECTION
    {
        T,
        R,
        B,
        L,
        TR,
        TB,
        TL,
        RB,
        RL,
        BL,
        TRB,
        TRL,
        TLB,
        RBL,
        TRBL,
        BLOCK,
    }

    public ROOMTYPE roomType;
    public DIRECTION roomDir;
    public int LeftRoomIndex = -1;
    public int RightRoomIndex = -1;
    public int TopRoomIndex = -1;
    public int BottomRoomIndex = -1;

    public int currentRoomIndex;
    public bool IsSearched;
    public bool IsCreated;
    public int xPos;
    public int yPos;
    public GameObject MiniMapRoom;
    public Sprite MiniMapSprite;
    public GameObject dungeon;

    // 해당 방의 몬스터 정보 받기 
    public GameObject[] Monsters;
    public int NumofMonsters;
    public bool IsEmptyMonster = false;



    private void DungeonMonstersInit()
    {
        if (dungeon.transform.Find("Monsters"))
        {
            NumofMonsters = dungeon.transform.Find("Monsters").childCount;
            Monsters = new GameObject[NumofMonsters];
            for (int i = 0; i < NumofMonsters; i++)
            {             
                Monsters[i] = dungeon.transform.Find("Monsters").GetChild(i).gameObject;
                Monsters[i].GetComponent<Monster>().InitMonster();
            }
            if (Monsters.Length != 0)
            {              
                IsEmptyMonster = false;                
            }

           
        }
        else
        {
            
            IsEmptyMonster = true;
        }
    }

    public void SwitchingMapDependPortal(bool isActivateDungeon)
    {
        dungeon.SetActive(isActivateDungeon);
    }

    public void InitRoom()
    {
        IsSearched = false;
        IsCreated = false;
       
    }

    //RoomMgr에서 DFS조건을 만족 못시켰을 때마다 모든 걸 초기화시키기 위해 만든 함수이다.
    //초기화해주지않으면 방의 정보가 남아있어 검색을 다시해도 똑같은 결과값이 나오게됨.
    public void ReInitialize()
    {
        IsSearched = false;
        IsCreated = false;
        roomType = ROOMTYPE.NORMAL;
        currentRoomIndex = 0;
        LeftRoomIndex = -1;
        RightRoomIndex = -1;
        BottomRoomIndex = -1;
        TopRoomIndex = -1;

    }

    public void InitializeDungeon()
    {
        DungeonMonstersInit(); //몬스터 - 맵 설정
        if (dungeon.activeInHierarchy != false)
        {       
            dungeon.SetActive(false);
        }
        
    }
   

    public void CheckIsClearRoom()
    {
        if (dungeon.transform.Find("Monsters"))
        {
            if (dungeon.transform.Find("Monsters").transform.childCount == 0)
            {
                IsEmptyMonster = true;
            }
            else
            {
                IsEmptyMonster = false;
            }
        }
       
    }

    

    public void UpdateDungeon()
    {
       

        if (dungeon.transform.Find("Monsters"))
        {
           
            foreach (Monster m in dungeon.transform.Find("Monsters").GetComponentsInChildren<Monster>())
            {                 
                 m.GetComponent<Monster>().UpdateMonster();
            }
        }
    }
}
