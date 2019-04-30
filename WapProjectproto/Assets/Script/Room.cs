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

    private void Awake()
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
        LeftRoomIndex = -1;
        RightRoomIndex = -1;
        TopRoomIndex = -1;
        BottomRoomIndex = -1;
    }
}
