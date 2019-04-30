﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMgr : MonoBehaviour
{   //4x3 행렬으로 맵을만듬(y,x)형태
    private Room[,] rooms = new Room[3, 4];

    //private은 Inspector에 안보이지만 [SerializeField]를하면 인스펙터에 보이게된다.
    //보통 캡슐화를 하고싶은데 Inspector에 무언가 집어넣고 싶을떄 사용
    //미니맵용 프리팹을 집어넣기 위해 사용.
    [SerializeField]
    private GameObject[] minimapPrefab;
    [SerializeField]
    private GameObject BossRoomImg;
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
    int currentXpos, currentYpos;
   

    private void Awake()
    {
        //맵(미로)를 만드는 함수
        CreateMaze();
        //미니맵 프리팹을 만들어진 미로에 바인딩 시키는 함수
        MiniMapGeneration();
    }

    private void CreateMaze()
    {
        //2차원 배열 레퍼런스를 위에 생성해두고
        //2차원 배열의 객체들을 생성하는 과정
        for (int y = 0; y < rooms.GetLength(0); y++)
            for (int x = 0; x < rooms.GetLength(1); x++)
            {
                rooms[y, x] = new Room();
                //기본적으로 방의 타입은 노멀타입이다.
                rooms[y, x].roomType = Room.ROOMTYPE.NORMAL;
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
        // 이거는 제대로 좌표에 룸타입이 바인딩 됐는지 확인하는용~ DEBUG
        //for (int i = 0; i < rooms.GetLength(0); i++)
        //    for (int j = 0; j < rooms.GetLength(1); j++)
        //    {
        //        Debug.Log(i + "," + j + " " + rooms[i, j].roomType);
        //    }

       

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
                        //현재 방은 위에 방이 있고 연결된걸 확인했으니 top의 index를 해당 방의 index로
                        //바꿔준다.
                        rooms[currentYpos, currentXpos].TopRoomIndex = currentRoomNum+1;
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
                        rooms[currentYpos, currentXpos+1].LeftRoomIndex = currentRoomNum;
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
                        rooms[currentYpos, currentXpos-1].RightRoomIndex = currentRoomNum;
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
    private void MiniMapGeneration()
    {
        //DFS에서 어느방향이 길로뚫려있는지를 설정했다
        //이제 길이 뚫린 방향에 따라 ROOMDIR을 바인딩시킨다.
        //앞에서도 말했듯이 디폴트 값으로 4방향의 값은 전부 -1
        //하지만 탐색을 통해 길이 뚫려있다면 양수의 형태로 나타날 것이다.
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
                    //7은 딱히 의미있는 숫자가아니라 Inspector에 들어간 T에대한 prefab 번호다
                    rooms[y, x].MiniMapRoom = minimapPrefab[7];
                }

                //TR
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                   && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TR;
                    rooms[y, x].MiniMapRoom = minimapPrefab[11];
                }

                //TL
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex < 0
                   && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TL;
                    rooms[y, x].MiniMapRoom = minimapPrefab[10];
                }

                //TB
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex < 0
                   && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TB;
                    rooms[y, x].MiniMapRoom = minimapPrefab[8];
                }
                //TRL
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TRL;
                    rooms[y, x].MiniMapRoom = minimapPrefab[14];
                }
                //TRB
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TRB;
                    rooms[y, x].MiniMapRoom = minimapPrefab[12];
                }
                //TLB
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TLB;
                    rooms[y, x].MiniMapRoom = minimapPrefab[9];
                }
                //TRBL
                else if (rooms[y, x].TopRoomIndex > -1 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.TRBL;
                    rooms[y, x].MiniMapRoom = minimapPrefab[13];
                }
                //R
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.R;
                    rooms[y, x].MiniMapRoom = minimapPrefab[3];
                }
                //RL
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.RL;
                    rooms[y, x].MiniMapRoom = minimapPrefab[5];
                }

                //RB
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.RB;
                    rooms[y, x].MiniMapRoom = minimapPrefab[4];
                }

                //RLB
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex > -1
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.RBL;
                    rooms[y, x].MiniMapRoom = minimapPrefab[6];
                }

                //BL
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.BL;
                    rooms[y, x].MiniMapRoom = minimapPrefab[1];
                }

                //B
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex < 0 && rooms[y, x].BottomRoomIndex > -1)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.B;
                    rooms[y, x].MiniMapRoom = minimapPrefab[0];
                }

                //L
                else if (rooms[y, x].TopRoomIndex < 0 && rooms[y, x].RightRoomIndex < 0
                  && rooms[y, x].LeftRoomIndex > -1 && rooms[y, x].BottomRoomIndex < 0)
                {
                    rooms[y, x].roomDir = Room.DIRECTION.L;
                    rooms[y, x].MiniMapRoom = minimapPrefab[2];
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
                    Debug.Log(x + " " + y);
                    Instantiate(BossRoomImg,new Vector3(x*5+2.5f,-y*5,1),Quaternion.identity,GameObject.Find("MiniMap").transform);
                }
                if(rooms[y,x].roomType != Room.ROOMTYPE.BLOCK)
                     Instantiate(rooms[y, x].MiniMapRoom, new Vector3(rooms[y, x].xPos *5, -rooms[y, x].yPos *5, 0), Quaternion.identity, GameObject.Find("MiniMap").transform);
                

            }


    }
}
          
