using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Serializable 사용 목적 : 이걸 해줌으로써 유니티 에디터의 인스펙터에서 요소를 보여줄 수 있다.
[System.Serializable]
public enum QuestAnswerBtnType
{
    FIRST = 0,
    SECOND,
    THIRD
}

[System.Serializable]
public class Quest
{
    public string Question;
    public string AnswerNumber01;
    public string AnswerNumber02;
    public string AnswerNumber03;

    public QuestAnswerBtnType correctAnswerNumber;
}

public class Question : MonoBehaviour
{
    [Header("Input Scripts")]
    public Score m_Score = null;        // 화면에 현재 점수를 표시할 UI
    [Header("Input Question")]
    public List<Quest> m_Questions;     // 질문들을 담을 리스트

    private int QIndex = 0;     // 현재 문제 순서
    

    private QuestAnswerBtnType Result;     // 문제 답이 속한 번호 위치

    private int m_correctQuestion = 0;          // 현재 맞춘 문제 카운트
    private int m_maxQuestionCount = -1;

    [Header("Input Text UI")]
    public Text m_QuestionCountText;            //남은 질문 개수를 표시하는 UI
    public Text m_QuestionText;             // 질문 UI
    public Text[] m_AnswerText = new Text[3];       // 답변 UI

    public int GetCurrentQuestionIdx() => QIndex;       // GetCurrentQuestionIdx() { return QIndex; } 와 같다.
    public int GetMaxQuestionCount() => m_maxQuestionCount;     // 최대 문제 개수
    private void CorrectQuestion() { ++m_correctQuestion; }
    public int GetCorrectQuestion() => m_correctQuestion;       // 현재 맞춘 문제 개수

    public List<Quest> GetQuestion() => m_Questions;
    public int GetQuestCount() { return m_Questions.Count;}     // 현재 질문 리스트의 개수를 가져온다.
    public void AppendQuest(Quest quest) => m_Questions.Add(quest);
    public void DeleteAllQuest() => m_Questions.Clear();

    public bool DeleteQuest(Quest quest)
    {
        if (m_Questions.Contains(quest))
        {
            m_Questions.Remove(quest);
            return true;
        }
        return false;
    }
    public bool DeleteQuest(int questionNumebr)
    {
        if (GetQuestCount() > 0)
        {
            m_Questions.RemoveAt(questionNumebr);
            return true;
        }
        Debug.Log("Don't have Questions 1");
        return false;
    }



    /// <summary>
    /// 남은 문제 수 로드
    /// </summary>
    private void LoadQuestionCount()
    {
        m_QuestionCountText.text = string.Format("{0}/{1}", GetCurrentQuestionIdx(), GetMaxQuestionCount());
    }
    public void LoadQuestion()
    {
        if (GetMaxQuestionCount() > GetCurrentQuestionIdx())        // 문제가 남았을 때
        {
            // 질문 & 문제 로드
            m_QuestionText.text = string.Format("Q. {0}", m_Questions[QIndex].Question);     // 질문 텍스트 로드
            m_AnswerText[(int)QuestAnswerBtnType.FIRST].text = m_Questions[QIndex].AnswerNumber01;      // 답변 텍스트 로드
            m_AnswerText[(int)QuestAnswerBtnType.SECOND].text = m_Questions[QIndex].AnswerNumber02;
            m_AnswerText[(int)QuestAnswerBtnType.THIRD].text = m_Questions[QIndex].AnswerNumber03;

            Result = m_Questions[QIndex].correctAnswerNumber;
            QIndex++;
        }
        else     // 문제가 더이상 없을 때
        {
            Debug.Log("Don't have Questions 2");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Rank");     // Rank 씬으로 씬 변경
        }
        LoadQuestionCount();
        m_Score.RenewScore();
    }

    /// <summary>
    /// 답변이 정답인가 아닌가를 판별해주는 함수
    /// </summary>
    /// <param name="AnswerType">답변을 답는 변수</param>
    /// <returns></returns>
    private bool IsCorrectAnswer(QuestAnswerBtnType AnswerType)
    {
        if (Result == AnswerType)     //정답
            return true;
        else    // 오답
            return false;
    }
    /// <summary>
    /// 답변 버튼을 눌렀을 때 실행될 함수
    /// </summary>
    /// <param name="type">눌른 버튼의 위치를 담는 변수(첫번째 : 1, 두번째 : 2, 세번째 : 3)</param>
    public void OnClickSubmitBtn(int type)
    {
        if (IsCorrectAnswer((QuestAnswerBtnType)type))
        {
            m_QuestionText.text = string.Format("정답입니다 !\n {0}", m_QuestionText.text);
            CorrectQuestion();      // 정답일 때 맞춘 문제 카운트를 1 증가시킴
            Invoke("LoadQuestion", 2.0f);       // 2초뒤에 LoadQuestion이라는 함수를 실행시켜 준다.
                                                    //Todo:
            m_Score.Increase(100);      //점수를 100 증가시킨다.
            //
        }
        else
        {
            m_QuestionText.text = string.Format("오답입니다 ! !\n {0}", m_QuestionText.text);
            Invoke("LoadQuestion", 2.0f);
        }
    }
    private void Start()
    {
        m_maxQuestionCount = m_Questions.Count;     // 게임이 시작되었을 때 최대 문제 개수를 maxQuestionCount 변수에 담음
        LoadQuestion();     //문제 로드
    }
}
