# Super Hot 2D
**사용 엔진** : [Unity 2018.2.15f1](https://unity3d.com/kr/unity/whatsnew/unity-2018.2.15)   
**사용 언어** : C#   
**타겟 플렛폼** : Windows   
**게임 설명** : 개인 포트폴리오 작업용으로 20년 2월 4일부터 제작된 프로젝트입니다. 게임 [슈퍼 핫](https://store.steampowered.com/app/322500/SUPERHOT/)을 2D 탑다운 뷰 형식으로 옮겨보았습니다. 플레이어가 마우스나 키보드 입력을 하면 시간이 점점 빠르게 흘러가고 입력이 없을 시 점점 느려집니다. 캐릭터들은 모두 기본 공격이 있고 총을 장착하면 6발 사격할 수 있으며 총을 던져 공격할수도 있습니다.    
**구현 설명** : 맵 에디터를 만들어 맵을 만들어 저장하면 저장된 xml데이터를 읽어 타일맵으로 자동으로 맵을 배치합니다. 플레이어는 마우스 포인터를 바라보며 정면 기준 120도 정도의 시야를 얻습니다. 시야에 들어온 적과 총만 볼 수 있으며, 벽 뒤는 시야가 없습니다. 적들은 기본적으로 주위를 돌아다니다가 플레이어나 무기를 발견하면 공격하거나 장착합니다. 가까이 있을수록 우선순위가 높습니다.     

 ## 코드 설명
 ### 맵 에디터와 맵 데이터, 메이커 구현
 게임엔 다양한 스테이지들이 존재합니다. 스테이지를 Excel을 이용하여 csv파일로 맵을 저장, 불러오는 방법도 있겠지만 직접 에디터를 만들어 사용해보고 싶었습니다. 크게 4개의 클래스들이 사용되며, 유니티 에디터에서 사용되는 [MapEditor](Assets/Scripts/Editor/MapEditor.cs), xml을 사용하여 맵 데이터를 저장, 불러오는데 사용되는 [MapDataManager](Assets/Scripts/21.Map/MapDataManager.cs), 맵의 데이터 클래스인 [MapDataSource](Assets/Scripts/21.Map/MapDataSource.cs), 맵 데이터를 불러와 타일을 타일맵에 그려주는 [WorldMaker](Assets/Scripts/21.Map/WorldMaker.cs)가 있습니다. 아직 진행중인 프로젝트기에 그려진 맵을 지우고 다른 맵을 불러오는 기능은 추가될 예정입니다.   
 에디터를 시작으로 순서대로 설명하겠습니다.
 
 ![mapeditor1](https://user-images.githubusercontent.com/24664506/75951748-212d4b80-5ef0-11ea-8753-53d0351ef63a.JPG)   
x, y에 원하는 값을 입력한 후 Generate를 누르면 GenerateNewMap메서드가 실제 데이터를 초기화하여 맵 크기를 할당하고 GenerateBoxGrid메서드를 사용하여 칸을 그리게 됩니다.    

![mapeditor2](https://user-images.githubusercontent.com/24664506/75951750-24283c00-5ef0-11ea-9751-f5a5c971ac49.JPG)    
슬라이드를 옮겨 이동할 수 있으며, OnGUI에서 DrawMap 메서드를 호출하여 그려진 칸 위에 좌표, P, E를 쓰고 색을 입힙니다.     

![mapeditor3](https://user-images.githubusercontent.com/24664506/75951753-268a9600-5ef0-11ea-942c-66e4b56404b3.JPG)   
Wall, PlayerSpawn, EnemySpawn 중 하나를 선택해 원하는 칸을 클릭하면 칸의 내용이 바뀝니다.      
Wall은 빈 칸 클릭 시 파란 칸으로 바꾸며 파란 칸 클릭 시 빈 칸으로 되돌립니다. 파란 칸들은 나중에 벽이 됩니다.    
EnemySpawn은 빈 칸 클릭 시 칸의 내용을 E로 바꾸며 한 번 더 클릭 시 빈 칸으로 바꿉니다. 여러 개 설치할 수 있습니다. 아직 Enemy의 스폰 방식을 정하진 않았습니다. (추가해야할 기능)    
PlayerSpawn은 기본적으로 0, 0으로 설정되어 있으며 빈 칸 클릭 시 기존 칸을 비우고 선택한 칸에 P를 배치합니다. 나중에 플레이어의 생성 위치가 됩니다.    
Save 클릭 시 원하는 곳에 Xml로 맵 정보를 저장합니다. MapDataManager 클래스를 이용해서 저장과 불러오기를 실행하기 때문에 에디터가 아닌 다른 코드에서도 사용가능합니다.    
자세한 소스코드는 [여기](Assets/Scripts/Editor/MapEditor.cs)를 확인하세요.

MapDataManger 클래스는 스크립터블오브젝트입니다. 맵에디터와 다른 코드에서 사용할 수 있도록 제작되었습니다. 기본적인 맵의 저장, 불러오기를 담당합니다. 소스코드는 [여기](Assets/Scripts/21.Map/MapDataManager.cs)를 확인하세요.

MapDataSource는 직렬가능클래스입니다. C#에서 기본적으로 제공하는 Xml.Serialization을 이용하기위해 이렇게 만들었으며 맵의 크기, 이름, walls의 좌표들, enemySpawns의 좌표들, playerSpawn의 좌표를 갖고 있습니다. Clear라는 메서드를 통해 초기화가 가능합니다. 소스코드는 [여기](Assets/Scripts/21.Map/MapDataSource.cs)를 확인하세요.

WorldMaker입니다. 싱글턴 클래스를 상속받아 만들어졌습니다. 단 한 개만 생성되며 나중에 나올 길찾기 알고리즘에서 사용됩니다. MapDataManger에 있는 MapDataSource를 이용하여 벽의 위치에 타일을 배치하여 디자인했던 스테이지에 맞도록합니다. 소스코드는 [여기](Assets/Scripts/21.Map/WorldMaker.cs)를 확인하세요.

### 캐릭터 시야의 구현
 게임이 시작되자마자 모든 적과 무기의 위치를 안다면 스릴이 부족하겠죠. 플레이어는 캐릭터가 바라보는 방향의 전방 120도 정도의 시야만 얻을 수 있습니다. 물론 이 시야는 벽 뒤를 보여주지않습니다. 시야를 구현하는덴 좀 많은 시행착오를 거쳤습니다.
 *이부분부터는 시행착오입니다.* SpriteRenderer에 Mask Interation을 통해 만들어야겠다고 생각한 뒤 원형의 SpriteMask를 갖는 시야 오브젝트를 만들어 캐릭터의 방향에 맞게 배치해야겠다고 결정했습니다. 맵에서 가장 긴 시야가 나오는 경우를 생각하여 넉넉히 (12,0)의 좌표를 60도 만큼 회전시키고 36개의 원형 시야를 배치시킨 다음 0도까지 회전시키면서 원호 길이의 0.3 Unit마다 배치하였습니다. 배치할 때 -60도부터 0도까지도 배치해야하니 반대 부분에도 똑같은 위치에 배치했죠. 그 다음 raycast를 이용하여 중간에 벽이 있다면 시야를 죽이고 풀에 반납했습니다. 완성하고 나니 프레임 드랍이 엄청 심했습니다.
 두 가지 정도의 문제점이 떠올랐습니다. 처음엔 2000개 정도의 시야 오브젝트가 호출하던 Raycast 때문인가 생각했습니다. 맨 끝의 점만 먼저 회전시켜 50개의 위치를 구하고, Raycast를 하여 닿는 point로 위치를 바꾼다음, 플레이어부터 point까지 0.3Unit마다 시야를 배치했습니다. 플레이어에 가까울수록 오브젝트가 중복되지만 RayCast의 숫자는 엄청 줄일 수 있었습니다. 하지만 여전히 프레임이 좋지 않았죠. *여기까지 시행착오입니다.*   
 그래서 생각해둔 두 번째 문제점이 Batch였습니다. SpriteMask를 가진 오브젝트를 수백, 수천개를 배치하다보니 엄청난 드로우콜이 발생했고 그 과정에서오는 프레임 드랍이 아닌가 했습니다. 그래서 끝부분만 원형 시야 오브젝트를 사용하고 그 부분 전까지 박스 시야 오브젝트를 늘려 사용하기로 결정했습니다. 50개의 시야만 사용할 예정이니 100개의 오브젝트만 있으면 되니까요.
 
 SightManager 클래스와 SightData 클래스가 사용됩니다.
 SightManager 클래스부터 설명하자면 박스 시야 오브젝트와 원형 시야 오브젝트 프리팹을 복제하여 새로 생성한 SightData 객체에 할당합니다. 우측 (1,0) 방향을 바라볼 때 (5, 10)의 점을 (5, -10)까지 화전시키며 50개의 방향을 SightData에 할당합니다. 방향을 구하는 메서드입니다.  
 ```C#
     Vector3[] GetFogTargetPos(Vector3 baseLinePos)
    {
        int count = maxCount;
        if (count == 0)
            return null;
        float rotateAngle = baseRotateAngleRad / count;
        float cos = Mathf.Cos(-rotateAngle);
        float sin = Mathf.Sin(-rotateAngle);
        Vector3[] result = new Vector3[count];
        result[0] = baseLinePos;
        result[result.Length - 1] = baseLinePos;
        result[result.Length - 1].y *= -1;
        for (int i = 1; i < count * 0.5f; i++)
        {
            result[i] = new Vector3(
                cos * result[i - 1].x - sin * result[i - 1].y,
                sin * result[i - 1].x + cos * result[i - 1].y);
            //count가 홀수 일 때 연산을 두번 하게 되지만 큰 상관은 없는것같음. 
            result[result.Length - 1 - i] = new Vector3(
                result[i].x, -result[i].y);

        }

        return result;
    }
 ```
 0도를 중심으로 양쪽이 고르게 나오게 하기 위해서 오브젝트를 한 번에 두 개씩, y를 뒤집어 배치하였습니다. 만들어진 박스 시야 오브젝트와 원형 시야 오브젝트는 Player GameObject의 자식인 parent 오브젝트를 부모 오브젝트로 할당하기 때문에 Player GameObject의 위치와 회전을 상속합니다.      
 SightData 클래스에서 RefreshSight 메서드를 호출하여 Raycast를 통해 벽의 위치를 파악한 후 그에 맞게 박스 시야 오브젝트의 크기와 원형 시야 오브젝트의 위치를 조절합니다.     
 RefreshSight는 Player의 방향이 갱신될 때마다 호출됩니다.     
 두 클래스의 코드는 [여기](Assets/Scripts/01.Manager/SightManager.cs)에서 확인할 수 있습니다. 