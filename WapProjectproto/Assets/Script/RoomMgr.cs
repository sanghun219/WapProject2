using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMgr : MonoBehaviour
{   //4x3 행렬으로 맵을만듬(y,x)형태
    public Room[,] rooms = new Room[3, 4];
    public Transform playerTransform;

    public GameObject[] roomsPrefab_T;
    public GameObject[] roomsPrefab_TR;
    public GameObject[] roomsPrefab_TB;
    public GameObject[] roomsPrefab_TL;
    public GameObject[] roomsPrefab_TRB;
    public GameObject[] roomsPrefab_TRL;
    public GameObject[] roomsPrefab_TBL;
    public GameObject[] roomsPrefab_TRBL;
    public GameObject[] roomsPrefab_R;
    public GameObject[] roomsPrefab_RB;
    public GameObject[] roomsPrefab_RL;
    public GameObject[] roomsPrefab_RBL;
    public GameObject[] roomsPrefab_B;
    public GameObject[] roomsPrefab_BL;
    public GameObject[] roomsPrefab_L;

    public GameObject roomsPrefab_T_Boss;
    public GameObject roomsPrefab_TR_Boss;
    public GameObject roomsPrefab_TB_Boss;
    public GameObject roomsPrefab_TL_Boss;
    public GameObject roomsPrefab_TRB_Boss;
    public GameObject roomsPrefab_TRL_Boss;
    public GameObject roomsPrefab_TBL_Boss;
    public GameObject roomsPrefab_TRBL_Boss;
    public GameObject roomsPrefab_R_Boss;
    public GameObject roomsPrefab_RB_Boss;
    public GameObject roomsPrefab_RL_Boss;
    public GameObject roomsPrefab_RBL_Boss;
    public GameObject roomsPrefab_B_Boss;
    public GameObject roomsPrefab_BL_Boss;
    public GameObject roomsPrefab_L_Boss;


    //private은 Inspector에 안보이지만 [SerializeField]를하면 인스펙터에 보이게된다.
    //보통 캡슐화를 하고싶은데 Inspector에 무언가 집어넣고 싶을떄 사용
    [SerializeField]
    private Sprite[] minimapSprites;
    [SerializeField]
    private Sprite minimapPlayer;
    [SerializeField]
    private Sprite BossRoomImg;
    //플레이어가 접근할 수 있는 최대 방의 개수
    private int maxRoomNums = 8;
    //접근할 수 없는 방의 최대 개수(우리는 방8개정도를 생각하고 있음, 따라서 4x3=12 ,12-4 =8을 위함.)
    private int Maxblock = 4;
    //현재 생성된 접근불가 방(CreateMaze()에 사용됨)
    private int currentblock = 0;
    //보스방이 만들어 졌는지 확인
    private bool IsCreatedBoss = false;
    //시작 지점방이 만들어 졌는지 확인
    private bool IsCreatedStart = false;
    //중간 보스방이 만들어 졌는지 확인
    private bool IsCreatedMiddle = false;
    //최근에 만들어진 방의 인덱스
    int currentRoomNum = 0;
    //길이 막혔을 때 차례로 돌아오면서 탐색하기 위해 stack 사용, DFS()에 사용됨
    Stack<Room> roomPath = new Stack<Room>();
    //최근 만들어진 방의 Position
    public int currentXpos, currentYpos;

    public int StartXpos, StartYpos;
    public GameObject[] roomstest;




    private void Start()
    {
        playerTransform = GameObject.Find("MiniMap").transform;
    }

    public void DungeonInitAndCreatePrefab()
    {
        //모든 방의 프리팹을 미리 만들어 놓고 InitializeDungeon함수 안의 SetActive로 비활성화 시켜둔다.
        for (int y = 0; y < rooms.GetLength(0); y++)
            for (int x = 0; x < rooms.GetLength(1); x++)
            {
                
                if (rooms[y, x].roomType != Room.ROOMTYPE.BLOCK)
                {
                   
                    rooms[y, x].dungeon = Instantiate(rooms[y, x].dungeon, GameObject.Find("Dungeon").transform);
                    rooms[y, x].InitializeDungeon();
                    rooms[y, x].DungeonMonstersInit();
                }
                
            }
    }

    public void DungeonActivate()
    {
        //현재 시작 방만을 활성화 시킨다.
        rooms[StartYpos, StartXpos].dungeon.SetActive(true);
    }

    public void UpdatePlayerPosinMinimap()
    {
        //플레이어 포탈 이동시 미니맵에 표시되는 플레이어의 아이콘의 위치를 변경해줌.
        if (playerTransform.gameObject.activeInHierarchy)
        {
            playerTransform.Find("Player").GetComponent<RectTransform>().localPosition =
          new Vector3(-24 + rooms[StartYpos, StartXpos].xPos * GameObject.Find("MiniMap").transform.Find("Player").GetComponent<RectTransform>().rect.width / 1.3f
          , 57 - 1 * rooms[StartYpos, StartXpos].yPos * GameObject.Find("MiniMap").transform.Find("Player").GetComponent<RectTransform>().rect.height / 1.3f, 1.0f);
        }
       
    }
    
  
    public void CreateMaze()
    {
        //2차원 배열 레퍼런스를 위에 생성해두고
        //2차원 배열의 객체들을 생성하는 과정
        for (int y = 0; y < rooms.GetLength(0); y++)
            for (int x = 0; x < rooms.GetLength(1); x++)
            {
                rooms[y, x] = new Room();
                //기본적으로 방의 타입은 노멀타입이다.
                rooms[y, x].roomType = Room.ROOMTYPE.NORMAL;
                rooms[y, x].InitRoom();
            }

        //block이 4개가 될 때까지(4x3배열는 12방 -4방 = 8방 <- 우리가 설정한방개수)
        //랜덤으로 블락을 해준다.
        //그리고 블락된 방을 제외한 나머지방에서 보스,중간보스,노말방을 정해준다.
        while (true)
        {
            int randXpos = Random.Range(0, 4);
            int randYpos = Random.Range(0, 3);
            //현재 블락된 방의 개수가 최대 블락개수보다 적고, 랜덤으로 가져온 좌표의 방이
            //만들어져 있지 않다면
            if (currentblock < Maxblock && !rooms[randYpos, randXpos].IsCreated)
            {
                //블락된 방의 개수를 증가 시키고
                currentblock++;
                //이 좌표의 방의 타입을 블락으로 바꾸며, 생성됨을 true로 이 방의 좌표를 바인딩
                rooms[randYpos, randXpos].roomType = Room.ROOMTYPE.BLOCK;
                rooms[randYpos, randXpos].IsCreated = true;
                rooms[randYpos, randXpos].xPos = randXpos;
                rooms[randYpos, randXpos].yPos = randYpos;
            }
            //우리가 지정한 최대 블락수가 됐을경우 나머지 타입의 방 생성
            if (currentblock == Maxblock)
            {
                //현재 좌표의 방이 생성된적이 없고 시작지점방이 생성된적이 없다면
                //->결국 블락된 방과 겹치지 않으며 단 한 번 생성하기 위한 조건
                if (rooms[randYpos, randXpos].IsCreated == false
                   && !IsCreatedStart)
                {
                    //시작지점이 만들어짐을 알리고, 데이터들을 바인딩
                    IsCreatedStart = true;
                    rooms[randYpos, randXpos].IsCreated = true;
                    rooms[randYpos, randXpos].roomType = Room.ROOMTYPE.START;
                    rooms[randYpos, randXpos].xPos = randXpos;
                    rooms[randYpos, randXpos].yPos = randYpos;
                    // 처음 시작 위치를 알아야 하기 떄문.(DFS에 이 포지션을 기점으로 길 탐색을 한다.)
                    currentXpos = randXpos;
                    currentYpos = randYpos;

                    StartXpos = currentXpos;
                    StartYpos = currentYpos;

                    //시작 지점방은 이미 검색됐다는 가정하에 DFS()에서 이용할 예정
                    //왜냐하면 시작방 다음에서 검색할 때 이미 검색된 방이어야 이방을 제외하고 검색하기 떄문에
                    rooms[randYpos, randXpos].IsSearched = true;
                }

                //현재 방의 위치와 겹치는 방이없고 보스방이 만들어진적이 없다면
                if (rooms[randYpos, randXpos].IsCreated == false
                   && !IsCreatedBoss)
                {
                    //위에 START와 같음.
                    IsCreatedBoss = true;
                    rooms[randYpos, randXpos].IsCreated = true;
                    rooms[randYpos, randXpos].roomType = Room.ROOMTYPE.BOSS;
                    rooms[randYpos, randXpos].xPos = randXpos;
                    rooms[randYpos, randXpos].yPos = randYpos;
                }

                //중간보스방에대한 것들 .. 위에 내용과 같다.
                if (rooms[randYpos, randXpos].IsCreated == false
                  && !IsCreatedMiddle)
                {
                    IsCreatedMiddle = true;
                    rooms[randYpos, randXpos].IsCreated = true;
                    rooms[randYpos, randXpos].roomType = Room.ROOMTYPE.MIDDLE;
                    rooms[randYpos, randXpos].xPos = randXpos;
                    rooms[randYpos, randXpos].yPos = randYpos;
                }



            }
            // 필수적인 방(시작,중간보스,보스)이 모두 만들어 졌다면
            if (IsCreatedBoss && IsCreatedMiddle && IsCreatedStart)
            {
                //특수 방을 제외한 NORMAL방들에 좌표를 설정해준다.
                for (int y = 0; y < rooms.GetLength(0); y++)
                    for (int x = 0; x < rooms.GetLength(1); x++)
                    {
                        if (rooms[y, x].roomType == Room.ROOMTYPE.NORMAL)
                        {
                            rooms[y, x].xPos = x;
                            rooms[y, x].yPos = y;
                        }
                    }

                //이 알고리즘은 수정할 필요가 있다.. DFS()와 관련된 구문들이 밑에 따로 놀고있음.(작동엔 문제X)

                //깊이 탐색을 한다 못가는 방이 없는지 탐색하는데
                DFS();
                //만약 우리가 검색한 방의 개수와 최대값으로 설정한값과 같다면
                //제대로 맵생성이 됐으니 while문을 탈출한다.
                if (currentRoomNum == maxRoomNums - 1)
                {
                  
                    break;
                }
                //하지만 장애물의 배치떄문에 모든 방을 못갔을시에는
                //모든 방의 정보와, 방배치에 필요한 정보들을 전부 초기치로 돌린다.
                else
                {
                    //ReInitialize()함수는 방을방문했는가,방을생성했는가의 정보를 false로 만든다.
                    for (int i = 0; i < rooms.GetLength(0); i++)
                        for (int j = 0; j < rooms.GetLength(1); j++)
                        {
                            rooms[i, j].ReInitialize();
                        }
                    //장애물 수를 0 으로만든다.
                    currentblock = 0;
                    //현재 방의 번호를 0으로 만든다.
                    currentRoomNum = 0;
                    //보스방,중간보스방,시작지점을 한 번만드는 변수를 다시 false로 둔다.
                    IsCreatedBoss = false;
                    IsCreatedMiddle = false;
                    IsCreatedStart = false;


                }

            }

        }

    }
    private void DFS()
    {
      


        //방을 다 탐색 할 떄까지 반복된다.
        while (true)
        {

            //검색하려는 룸의 좌표가 3x4배열 밖인지 아닌지에 대한 검사.
            //우리는 top,right,bottom,left 순으로 방을 검색해 나갈 것임.

            //Top 검사
            if (currentYpos - 1 >= 0)
            {
                //현재 포지션의 Y값 보다 1 작은(바로 위의 방)방이 탐색된적 없다면
                if (!rooms[currentYpos - 1, currentXpos].IsSearched)
                {
                    //그 방을 탐색됐다고 알리고
                    rooms[currentYpos - 1, currentXpos].IsSearched = true;
                    //그 방의 타입이 BLOCK이 아니라면
                    if (rooms[currentYpos - 1, currentXpos].roomType != Room.ROOMTYPE.BLOCK)
                    {
                        //현재 방의 정보를 스택에 쌓아두고(나중에 돌아오기 위해)
                        roomPath.Push(rooms[currentYpos, currentXpos]);
                        //그 방의 밑의 방 인덱스를 현재 방으로 설정해둔다.
                        //이건 여러가지 이유가 있는데, 나중에 포탈을 생성했을 때 어떤 방으로
                        //이동할건지에 대해 방의 인덱스를 통해 확인할 수 있고
                        //room의 property로 존재하는 bottom,top,right,left는 -1로 초기화되어있는데
                        //이것을 어떤 인덱스로 대입을 하면(index >=0) 양수가 된다. 따라서
                        //현재 방의 포털이 어떤 방향으로 몇개 뚤려있는지 동시에 확인할 수 있다.
                        rooms[currentYpos - 1, currentXpos].BottomRoomIndex = currentRoomNum;
                        rooms[currentYpos, currentXpos].currentRoomIndex = currentRoomNum;
                        //현재 방은 위에 방이 있고 연결된걸 확인했으니 top의 index를 해당 방의 index로
                        //바꿔준다.
                        rooms[currentYpos, currentXpos].TopRoomIndex = currentRoomNum + 1;
                        //이제 현재 y포지션을 -1해주고(실제로는 윗칸의 맵으로 이동한 것)
                        currentYpos -= 1;
                        //현재 검색된 방의 개수를 하나 증가 시켜준다.
                        //이것은 현재 검색된 방의개수가 나중에 최대 방의 개수와 같게되면 탐색을 중지하게
                        //하기 위함이다.(현재방의개수 != 최대방의 개수)라면 block된 방에의해
                        //모든 경로를 탐색하지 못했다는것을 의미함.
                        currentRoomNum += 1;
                        //이 조건을 만족시키지 않을 때 까지 반복문을 돌리려고..
                        continue;
                    }
                }

            }
            //위의 top검사와 똑같다, 단 우리가 top,right,bottom,left의 시계방향으로 탐색하기로
            // 규칙을 만들었기 때문에 2번째로 right을 검사하게된다.
            //Right 검사
            if (currentXpos + 1 < rooms.GetLength(1))
            {


                if (!rooms[currentYpos, currentXpos + 1].IsSearched)
                {
                    rooms[currentYpos, currentXpos + 1].IsSearched = true;
                    if (rooms[currentYpos, currentXpos + 1].roomType != Room.ROOMTYPE.BLOCK)
                    {

                        roomPath.Push(rooms[currentYpos, currentXpos]);
                        rooms[currentYpos, currentXpos + 1].LeftRoomIndex = currentRoomNum;
                        rooms[currentYpos, currentXpos].currentRoomIndex = currentRoomNum;
                        rooms[currentYpos, currentXpos].RightRoomIndex = currentRoomNum + 1;
                        currentXpos += 1;
                        currentRoomNum += 1;
                        continue;

                    }
                }

            }
            if (currentYpos + 1 < rooms.GetLength(0))
            {
                //BOTTOM 검사


                if (!rooms[currentYpos + 1, currentXpos].IsSearched)
                {
                    rooms[currentYpos + 1, currentXpos].IsSearched = true;
                    if (rooms[currentYpos + 1, currentXpos].roomType != Room.ROOMTYPE.BLOCK)
                    {

                        roomPath.Push(rooms[currentYpos, currentXpos]);
                        rooms[currentYpos + 1, currentXpos].TopRoomIndex = currentRoomNum;
                        rooms[currentYpos, currentXpos].currentRoomIndex = currentRoomNum;
                        rooms[currentYpos, currentXpos].BottomRoomIndex = currentRoomNum + 1;
                        currentYpos += 1;
                        currentRoomNum += 1;
                        continue;

                    }
                }


            }
            if (currentXpos - 1 >= 0)
            {
                //LEFT 검사

                if (!rooms[currentYpos, currentXpos - 1].IsSearched)
                {
                    rooms[currentYpos, currentXpos - 1].IsSearched = true;
                    if (rooms[currentYpos, currentXpos - 1].roomType != Room.ROOMTYPE.BLOCK)
                    {

                        roomPath.Push(rooms[currentYpos, currentXpos]);
                        rooms[currentYpos, currentXpos - 1].RightRoomIndex = currentRoomNum;
                        rooms[currentYpos, currentXpos].currentRoomIndex = currentRoomNum;
                        rooms[currentYpos, currentXpos].LeftRoomIndex = currentRoomNum + 1;
                        currentXpos -= 1;
                        currentRoomNum += 1;
                        continue;

                    }
                }



            }
            //위의 조건들중 하나라도 만족시켰다면 continue에 의해 계속 반복됐을 것이다.
            //하지만, 네개의 모든 조건을 만족 못했다는 것은 더 이상 갈 수있는 길이 없다는것.
            //이제부터는 backTracking을 통해서 지나왔던 방들 중에 검사하지 않은 방향들을
            //탐색할 것이다.

            Room room = new Room();
            //돌아갈 방이 있다면
            if (roomPath.Count != 0)
            {
                //가장 최근에 놓여진 방의 정보를 room에 전달하고
                room = roomPath.Pop();
                //그 좌표들을 현재좌표로 바꿔준다.
                currentXpos = room.xPos;
                currentYpos = room.yPos;


            }
            //stack에 더 이상 쌓여있는 room이 없다면(즉 모든 경로를 돌고 더 이상 탐색할 방이 없다는 뜻)
            else
            {
                // 이제 dfs를 종료하면 됨.
                break;
            }




        }
    }
    public void MiniMapGeneration()
    {
        //DFS에서 어느방향이 길로뚫려있는지를 설정했다
        //이제 길이 뚫린 방향에 따라 ROOMDIR을 바인딩시킨다.
        //앞에서도 말했듯이 디폴트 값으로 4방향의 값은 전부 -1
        //하지만 탐색을 통해 길이 뚫려있다면 양수의 형태로 나타날 것이다.

        // 이거는 제대로 좌표에 룸타입이 바인딩 됐는지 확인하는용~ DEBUG
        for (int i = 0; i < rooms.GetLength(0); i++)
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                if (rooms[i, j].roomType == Room.ROOMTYPE.BLOCK)
                {
                    rooms[i, j].TopRoomIndex = -1;
                    rooms[i, j].RightRoomIndex = -1;
                    rooms[i, j].LeftRoomIndex = -1;
                    rooms[i, j].BottomRoomIndex = -1;
                }
            }
        for (int y = 0; y < rooms.GetLength(0); y++)
            for (int x = 0; x < rooms.GetLength(1); x++)
            {
               
              
                //T
                if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex < 0
                    && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex < 0)
                {
                    //roomDir은 지금 당장은 필요없지만 혹시 방향이 필요할까봐 넣어둠.
                    rooms[y, x].roomDir = Room.DIRECTION.T;
                    //minimapPrefab은 Inspector에 넣어준 prefab들중에서 하나를 가져올 것이다.
                   
                   
                    rooms[y, x].MiniMapSprite = minimapSprites[0];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_T_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_T.Length);
                        rooms[y, x].dungeon = roomsPrefab_T[rand];
                    }
                   
                    
                }

                //TR
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                   && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TR;
                   
                    rooms[y, x].MiniMapSprite = minimapSprites[4];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_TR_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_TR.Length);
                        rooms[y, x].dungeon = roomsPrefab_TR[rand];
                    }
                   
                }

                //TL
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex < 0
                   && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TL;
                 
                    rooms[y, x].MiniMapSprite = minimapSprites[6];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_TL_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_TL.Length);
                        rooms[y, x].dungeon = roomsPrefab_TL[rand];
                    }
                    
                }

                //TB
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex < 0
                   && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TB;
                  
                    rooms[y, x].MiniMapSprite = minimapSprites[5];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_TB_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_TB.Length);
                        rooms[y, x].dungeon = roomsPrefab_TB[rand];
                    }
                    
                }
                //TRL
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TRL;
                   
                    rooms[y, x].MiniMapSprite = minimapSprites[8];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_TRL_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_TRL.Length);
                        rooms[y, x].dungeon = roomsPrefab_TRL[rand];
                    }
                    
                }
                //TRB
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TRB;
                 
                    rooms[y, x].MiniMapSprite = minimapSprites[7];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_TRB_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_TRB.Length);
                        rooms[y, x].dungeon = roomsPrefab_TRB[rand];
                    }
                 
                }
                //TLB
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TLB;
                  
                    rooms[y, x].MiniMapSprite = minimapSprites[9];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_TBL_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_TBL.Length);
                        rooms[y, x].dungeon = roomsPrefab_TBL[rand];
                    }
                   
                }
                //TRBL
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TRBL;
                  
                    rooms[y, x].MiniMapSprite = minimapSprites[10];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_TRBL_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_TRBL.Length);
                        rooms[y, x].dungeon = roomsPrefab_TRBL[rand];
                    }
                   
                }
                //R
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.R;
                   
                    rooms[y, x].MiniMapSprite = minimapSprites[1];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_R_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_R.Length);
                        rooms[y, x].dungeon = roomsPrefab_R[rand];
                    }
                  
                }
                //RL
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.RL;
                  
                    rooms[y, x].MiniMapSprite = minimapSprites[12];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_RL_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_RL.Length);
                        rooms[y, x].dungeon = roomsPrefab_RL[rand];
                    }
                   
                }

                //RB
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.RB;
                  
                    rooms[y, x].MiniMapSprite = minimapSprites[11];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_RB_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_RB.Length);
                        rooms[y, x].dungeon = roomsPrefab_RB[rand];
                    }
                   
                }

                //RLB
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.RBL;
                 
                    rooms[y, x].MiniMapSprite = minimapSprites[13];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_RBL_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_RBL.Length);
                        rooms[y, x].dungeon = roomsPrefab_RBL[rand];
                    }
                   
                }

                //BL
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.BL;
                   
                    rooms[y, x].MiniMapSprite = minimapSprites[14];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_BL_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_BL.Length);
                        rooms[y, x].dungeon = roomsPrefab_BL[rand];
                    }
                   
                }

                //B
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.B;
                   
                    rooms[y, x].MiniMapSprite = minimapSprites[2];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_B_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_B.Length);
                        rooms[y, x].dungeon = roomsPrefab_B[rand];
                    }
                
                }

                //L
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.L;
                
                    rooms[y, x].MiniMapSprite = minimapSprites[3];
                    if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                    {
                        rooms[y, x].dungeon = roomsPrefab_L_Boss;
                    }
                    else
                    {
                        int rand = Random.Range(0, roomsPrefab_L.Length);
                        rooms[y, x].dungeon = roomsPrefab_L[rand];
                    }
                
                }
                //모든 방향에 대해 조건을 만족못하는 경우는 block 한 경우 뿐이고
                //이런 경우는 무시하고 넘어가기 위해 continue를 써줬다.
                //참고로 continue를 반복문에서 쓰게되면 현재 요소를 뛰어 넘어 다음 요소를 검색하게된다.
                else
                {
                    continue;
                }


            }


        // 모든 방에대해 방향과 해당 프리팹이 바인딩됐고 이제 그에대해
        // 미리 만들어둔 프리팹들을 적절한 곳에 위치시켜 미니맵처럼 만들게한다.
        // 단 block방은 없는방이니 아무것도 나타나지않게한다.
        for (int y = 0; y < rooms.GetLength(0); y++)
            for (int x = 0; x < rooms.GetLength(1); x++)
            {


                if (rooms[y, x].roomType == Room.ROOMTYPE.BOSS)
                {
                    GameObject Go = new GameObject();
                    Go.name = "Boss";
                    Image newImage = Go.AddComponent<Image>();
                   
                    newImage.sprite = BossRoomImg;
                    newImage.GetComponent<RectTransform>().SetParent(GameObject.Find("MiniMap").transform);

                    newImage.gameObject.SetActive(false);
                    newImage.rectTransform.localPosition = new Vector3(-30 + rooms[y, x].xPos * newImage.rectTransform.rect.width / 1.3f, 60 - 1 * rooms[y, x].yPos
                        * newImage.rectTransform.rect.height / 1.3f, 1.0f);
                    newImage.rectTransform.localScale = new Vector3(0.4f, 0.4f);
                }

                if (rooms[y,x].yPos == StartYpos && rooms[y,x].xPos == StartXpos)
                {
                    GameObject Go = new GameObject();
                    Go.name = "Player";
                    Image newImage = Go.AddComponent<Image>();

                    newImage.sprite = minimapPlayer;
                    newImage.GetComponent<RectTransform>().SetParent(GameObject.Find("MiniMap").transform);

                    newImage.gameObject.SetActive(false);
                    newImage.rectTransform.localPosition = new Vector3(-24 + rooms[y, x].xPos * newImage.rectTransform.rect.width / 1.3f, 57 - 1 * rooms[y, x].yPos
                        * newImage.rectTransform.rect.height / 1.3f, 1.0f);
                    newImage.rectTransform.localScale = new Vector3(0.2f, 0.2f);
                }

                if (rooms[y, x].roomType != Room.ROOMTYPE.BLOCK)
                {
                    GameObject Go = new GameObject();
                    Go.name = y + "," + x;
                    Image newImage = Go.AddComponent<Image>();
                    newImage.sprite = rooms[y, x].MiniMapSprite;
                    newImage.GetComponent<RectTransform>().SetParent(GameObject.Find("MiniMap").transform);
                   
                    newImage.gameObject.SetActive(false);
                    newImage.rectTransform.localPosition = new Vector3(-30+rooms[y,x].xPos *newImage.rectTransform.rect.width/1.3f,60-1*rooms[y,x].yPos 
                        * newImage.rectTransform.rect.height/1.3f, 1.0f);
                    newImage.rectTransform.localScale = new Vector3(0.7f, 0.7f);

                   //Instantiate(rooms[y, x].MiniMapSprite, new Vector3(rooms[y, x].xPos * 1.1f, -rooms[y, x].yPos * 1.1f, 0), Quaternion.identity, GameObject.Find("MiniMap").transform);
                    //Instantiate(rooms[y, x].MiniMapRoom, new Vector3(rooms[y, x].xPos * 1.1f, -rooms[y, x].yPos * 1.1f, 0),Quaternion.identity, GameObject.Find("MiniMap").transform);                
                }


            }


        //보스 이미지나 플레이어 이미지는 UI 제일 앞에 배치되어야 해서 Minimap 자식오브젝트 중 가장 마지막에 배치시킨다.
        GameObject.Find("MiniMap").transform.Find("Boss").transform.SetAsLastSibling();
        GameObject.Find("MiniMap").transform.Find("Player").transform.SetAsLastSibling();

       

    }

    private void VisualizedCurrentRoom()
    {

    }
  
}
          
