# DIMENSIONAL RACER

#### 2023학년도 2학기 게임소프트웨어 팀3
***
### 프로젝트 팀원

| 이름     | 학과           | 역할         |
| -------- | -------------- | ------------ |
| 곽우진    | 소프트웨어학부 |       서버      |
| 김지환    | 소프트웨어학부 |       UI       |
| 윤재호    | 음악학부       |   클라이언트   |
***

### 개발 주제

우주선을 타고 다양한 차원을 포탈로 넘나들며 결승선까지 도달하는 3D 멀티플레이 레이싱 게임
***

### 기술 스택

Unity 2022.3.9f1 + Photon Engine & PlayFab
***

### 개발 기간

2023.9.14 ~ 2023.12.8
***

### 사용 에셋

> 클라이언트
- Low-Poly Spaceships Set (무료) : 
- Modern UI Pack (유료) :

> 서버
- Photon Unity Networking Free (무료) : Photon 을 활용한 실시간 동기화를 구현하기 위하여 사용했습니다.
- PlayFab SDK (무료) : PlayFab 을 이용한 사용자 계정, 서버, 매치메이킹 관리를 구현하기 위하여 사용했습니다.

### 설치 및 실행 방법

여기는 제가 구글 클라우드에 업로드할게요
***

### 씬 단위 프로젝트 구조 설명
> ### 1. Base 씬

    오브젝트가 파괴되지 않도록 설정하는 OnDestroyOnLoad 기능을 사용하기 위해 제작된 씬입니다.
    모든 씬에 필요한 계정 관리용 AccountManager, 설정값을 갖는 SettingManager 오브젝트에 OnDestroyOnLoad를 부여하고 바로 Sign In 씬으로 이동합니다.

> ### 2. Sign In 씬


> ### 3. Lobby 씬


> ### 4. Ingame 씬
