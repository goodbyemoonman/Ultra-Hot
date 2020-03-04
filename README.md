SUPER HOT 2D

이름 : 슈퍼핫2D
설명 : 슈퍼핫을 2D 탑다운뷰로 옮긴 게임. 
이슈 목록 :
1. 입력과 시간 관리자
2. 맵의 생성과 저장. 
3. 시야(전장의 안개)
4. 무기 및 공격
5. 적 캐릭터와 AI


입력과 시간 관리자
Update함수에서 입력된 축과 마우스 위치에 따라서 TimeScaleManager에게 입력된 종류를 전달. 플레이어 캐릭터에게 입력된 키 (좌클릭, 우클릭, E) 전달.

InputHandlertsm : TimeScaleManager
+player : GameObject
+canInput : bool
-preMousePos : Vector3-Update()
-TimeScaleInput(inputMousePos, inputDir)
-PlayerMoveInput(inputMousePos, inputDir)
-CommandToRotate(angle) 
 Update 내부에서 입력된 축을 Vector2로 만든 direction과 마우스의 위치를 PlayerMoveInput과 TimeScaleInput에 전달합니다. 좌클릭, 우클릭, E키가 눌렸다면 player에게 메시지를 보냅니다. 
 TimeScaleInput은 preMousePos와 매개변수 inputMousePos를 비교하여 마우스의 움직임 유무와 캐릭터 이동 키 유무를 판단하여 입력된 종류를 enum INPUTTYPE을 이용하여 전달합니다.
 PlayerMoveInput은 Command에 입력된 방향과 마우스에 따른 회전을 전달합니다.



 TimeScaleManager-scaler : readonly float
-canScale : bool
-target : float-ControlTimeScale() : IEnumerator
+SetInputType(INPUTTYPE input)
+SetTimeScaleImmediately(float to)
+SetScaleSwitch(bool isTurnOn)
+FixTimeScale(float scale, float duration)
-TurnOnTimer(float duration) : IEnumerator
SetInputType을 통해서 입력종류를 받습니다. 입력이 없을 시 0.05, 마우스 이동 시 0.25, 키보드 입력 시 1로 target을 변경합니다. 
ControlTimeScale 코루틴을 이용하여 현재 TimeScale을 target에 맞춥니다. TimeScale이 감소할 땐 증가할 때 보다 2배 빠르게 감소합니다. Realtime 0.025초마다 0.03씩 lerp 합니다.
FixTimeScale은 TimeScale을 scale값으로 실제시간으로 duration만큼 유지합니다.

InputHandler가 player에게 MoveTo 메시지를 보내면 MoveUnitCommand에서 받아 방향을 저장합니다. MoveUnitCommand는 Update때마다 Execute를 호출하며 MoveHandler에게 MoveToDir 메서드를 호출하고 MoveHandler는 캐릭터의 상태가 움직일 수 있으면 rigidbody2d의 velocity를 조절하여 캐릭터를 움직입니다.
MoveUnitCommand-space : Space
-dir : Vector2
-mh : MoveHandler-Awake()
+MoveUnitCommand(dir, space = Space.World)
-Update()
-Execute()
+MoveTo(direction)MoveHandler-rgbd : Rigidbody2D
-sw : bool
-speed : float-Awake()
-StateObserver(state)
+MoveToWorldDir(dir)
+MoveToSelfDir(dir)
RotateUnitCommand+LookAt(angle)
캐릭터의 회전은 TimeScale과 상관없이 작동합니다.




