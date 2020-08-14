using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class User
{
    public int score;

    public User(int score)
    {
        this.score = score;
    }
}

public class DataManager : MonoBehaviour
{
    //데이터베이스 참조 레퍼런스
    private DatabaseReference reference;
    public DatabaseReference GetReference => reference;

    // 유저 정보를 담는 리스트
    public List<int> m_userInfo { get; private set; }

    public UnityEngine.UI.Text[] RankTexts = new UnityEngine.UI.Text[5];

    // 스코어가 로드된 상태인가 아닌가
    public bool m_bIsScoreLoaded = false;

    /// <summary>
    /// 데이터베이스를 초기화 시켜주는 함수
    /// </summary>
    private void DatabaseInit()
    {
        m_userInfo = new List<int>();
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://funnystudy-d7b8c.firebaseio.com");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    /// <summary>
    /// 유저정보를 데이터베이스에 추가시켜주는 함수
    /// </summary>
    /// <param name="score">유저 스코어</param>
    public void AppendUser(int score)
    {
        // score값을 담는 User 클래스 생성
        User user = new User(score);

        // user 오브젝트를 JSON 포멧으로 변환
        string json = JsonUtility.ToJson(user);

        // 변환된 JSON 자료를 users의 하위에 넣어준다.
        GetReference.Child("users").Push().SetRawJsonValueAsync(json);      
    }
    /// <summary>
    /// 데이터베이스로부터 불러온 유저 스코어를 유저 리스트에 담아 주는 함수
    /// </summary>
    public void AppendScoreToDatabase()
    {
        //users 안에 있는 값들을 가져온다.
        GetReference.Child("users").GetValueAsync().ContinueWith(task =>
        {
            //값들이 정상적으로 불러와 졌을 때
            if (task.IsCompleted)
            {
                m_userInfo.Clear();     // 유저 스코어 리스트 초기화

                //불러와진 결과값을 DataSnapshot 형태의 변수에 담는다.
                DataSnapshot snapshot = task.Result;

                // snapshot 하위의 모든 정보들을 순차적으로 data에 담아서 모두 돌때까지 반복
                foreach (DataSnapshot data in snapshot.Children)
                {
                    IDictionary userInfo = (IDictionary)data.Value;
                    // 유저의 스코어 정보를 정수형(int)로 변환하여 유저의 스코어를 담는 리스트에 추가해준다.
                    m_userInfo.Add(System.Convert.ToInt32(userInfo["score"]));
                }
                m_bIsScoreLoaded = true;
            }
        });
    }

    // 게임 실행 시 한번만 실행되는 함수
    void Start()
    {
        DatabaseInit();
        if (Score.Get() > 0)
            AppendUser(Score.Get());
        AppendScoreToDatabase();
    }

    // 매 프레임 마다 게속 호출되는 함수
    void Update()
    {
        // 만약 스코어가 로드된 상태라면
        if (m_bIsScoreLoaded)
        {
            // 오름차순 정렬 (ex : 300,500,600,900,1000..)
            m_userInfo.Sort();

            // 값을 거꾸로 뒤집는다.( 오름차순의 반대 -> 내림차순 정렬 : (ex : 1000,900,600,500,300..)
            m_userInfo.Reverse();

            // 불러와진 유저의 수만큼 반복해서 돈다.
            for (int i = 0; i < RankTexts.Length; i++)
            {
                if (i < m_userInfo.Count)
                {
                    // 순위와 점수를 콘솔에 출력해준다.
                    RankTexts[i].text = $"{m_userInfo[i]}점";
                }
            }
            m_bIsScoreLoaded = false;
        }
    }
}
