using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//게임 내에서 메인으로 돌아가는 함수, SCENE-MENU와는 상관없는 클래스
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RoomMgr roomMgr;
    [SerializeField]
    private Player player;


  
    private void Awake()
    {
        roomMgr = GetComponent<RoomMgr>();

        //0.모든 이벤트 초기화
        EventManager.GetInst().EventInit();

        //1.맵 생성
        roomMgr.CreateMaze();
        roomMgr.MiniMapGeneration();
        roomMgr.DungeonInitAndCreatePrefab();
        roomMgr.DungeonActivate();

        //2.플레이어 초기화
        player.PlayerInit();

        //이벤트,사운드 초기화
        SoundManager.GetInst().InitSoundinAwake();
        
    }

    private void Start()
    {
        //잠시 쓰는용
        SoundManager.GetInst().PlayMusic("MainTheme");
    }

    //물리적인 동작에 대한 Update()
    private void FixedUpdate()
    {

        if (EventManager.GetInst().IsPause) return;
        //2.플레이어 동작
        player.FixedUpdatePlayerMovement();     
        player.UpdateSpecialKey();
    }
    private void Update()
    {
      

        //1.미니맵표시
        roomMgr.UpdatePlayerPosinMinimap();

        //2.플레어이 키 입력
        player.UpdatePlayerMovement();

        //1.현재 방 몬스터 탐색
        roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].CheckIsClearRoom();
        roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].UpdateDungeon();
        roomMgr.FindClearRoom();



        //InGame UI 업데이트
        UIinformationMgr.GetInst().InGameUIupdate();
    }
}
