using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum PORTAL_DIR
    {
        LEFT,
        TOP,
        RIGHT,
        BOTTOM,
        MIDDLEBOSS,
        BOSS,
    }

    private RoomMgr roomMgr;
    private Player player;
    public PORTAL_DIR portal_Dir;
   
    //potal 타는데 딜레이를 안주면 방을 여러개 뛰어넘는 현상이 일어남.. 방지하려고 약간의 딜레이를둠.
    float potalTime = 0.0f;
  
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.Find("MainChracter").GetComponent<Player>();
        roomMgr = GameObject.FindGameObjectWithTag("GameController").GetComponent<RoomMgr>();
    }

    private void FixedUpdate()
    {
        potalTime += Time.deltaTime;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.UpArrow))
        {
           
            if (potalTime >0.1f)
            {
                potalTime = 0;

                switch (portal_Dir)
                {
                    
                    case PORTAL_DIR.BOTTOM:
                        if (roomMgr.StartYpos+1 < 3)
                        {
                            if (!roomMgr.rooms[roomMgr.StartYpos + 1, roomMgr.StartXpos].dungeon.activeInHierarchy)
                            {
                                roomMgr.rooms[roomMgr.StartYpos + 1, roomMgr.StartXpos].SwitchingMapDependPortal(true);
                            }


                            if (roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.activeInHierarchy)
                                roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].SwitchingMapDependPortal(false);

                            roomMgr.StartYpos += 1;
                            player.transform.position = roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.transform.Find("TopPortal").GetChild(0).position;
                          
                        }
                      
                        break;
                    case PORTAL_DIR.LEFT:

                        if (roomMgr.StartXpos - 1 >= 0)
                        {
                            if (!roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos - 1].dungeon.activeInHierarchy)
                            {
                                roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos - 1].SwitchingMapDependPortal(true);
                            }

                            if (roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.activeInHierarchy)
                                roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].SwitchingMapDependPortal(false);
                            roomMgr.StartXpos -= 1;
                            player.transform.position = roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.transform.Find("RightPortal").GetChild(0).position;
                            
                        }
                       
                        break;
                    case PORTAL_DIR.RIGHT:

                        if (roomMgr.StartXpos + 1 <= 4)
                        {
                            if (!roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos + 1].dungeon.activeInHierarchy)
                            {
                                roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos + 1].SwitchingMapDependPortal(true);
                            }
                            if (roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.activeInHierarchy)
                                roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].SwitchingMapDependPortal(false);
                            roomMgr.StartXpos += 1;
                            player.transform.position = roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.transform.Find("LeftPortal").GetChild(0).position;
                        
                        }
                       
                        break;
                    case PORTAL_DIR.TOP:

                        if (roomMgr.StartYpos - 1 >= 0)
                        {
                            if (!roomMgr.rooms[roomMgr.StartYpos - 1, roomMgr.StartXpos].dungeon.activeInHierarchy)
                            {
                                roomMgr.rooms[roomMgr.StartYpos - 1, roomMgr.StartXpos].SwitchingMapDependPortal(true);
                            }

                            if (roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.activeInHierarchy)
                                roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].SwitchingMapDependPortal(false);

                            roomMgr.StartYpos -= 1;
                            player.transform.position = roomMgr.rooms[roomMgr.StartYpos, roomMgr.StartXpos].dungeon.
                                transform.Find("BottomPortal").GetChild(0).position;
                           
                        }
                       
                        break;
                    default:
                        break;
                }
            }
            
        }
    }
    
}
