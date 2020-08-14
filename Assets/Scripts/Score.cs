using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    private static int m_Score = 0;
    /// <summary>
    /// 현재 스코어를 가져온다
    /// </summary>
    public static int Get() { return m_Score; }

    /// <summary>
    /// 스코어를 증가시킨다
    /// </summary>
    /// <param name="score">증가시킬 양</param>
    public void Increase(int score) { m_Score += score; }

    public UnityEngine.UI.Text ScoreText = null;

    /// <summary>
    /// 현재 스코어 갱신
    /// </summary>
    public void RenewScore()
    {
        ScoreText.text = Get().ToString();
    }
}
