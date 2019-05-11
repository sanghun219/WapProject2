using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private RoomMgr roomMgr;
    [SerializeField]
    private Player player;
  
    private void Awake()
    {
        roomMgr = GetComponent<RoomMgr>();
       

        //1.맵 생성
        roomMgr.CreateMaze();
        roomMgr.MiniMapGeneration();
        roomMgr.DungeonInitAndCreatePrefab();
        roomMgr.DungeonActivate();

        //2.플레이어 초기화
        player.PlayerInit();
        
    }

    //물리적인 동작에 대한 Update()
    private void FixedUpdate()
    {
        //2.플레이어 동작
        player.FixedUpdatePlayerMovement();     
        player.UpdatePlayerAnimation();
        player.UpdatePlayerMinimapOnOff();
    }
    private void Update()
    {
        //1.미니맵표시
        roomMgr.UpdatePlayerPosinMinimap();

        
    }
}
