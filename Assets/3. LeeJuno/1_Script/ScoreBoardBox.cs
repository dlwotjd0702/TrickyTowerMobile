using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardBox : MonoBehaviour
{
    [HideInInspector] public PlayerRef owner;
   // [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform[] medalSlots;

    public void Initialize(PlayerRef p)
    {
        owner = p;
        ResetBox();
    }

    public void ResetBox()
    {
       // scoreText.text = "0";
        foreach (var slot in medalSlots)
        foreach (Transform c in slot)
            Destroy(c.gameObject);
    }

    public void UpdateBox(int medalCount, GameObject medalPrefab)
    {
        // 1) 점수 세팅
        int score = GameClearManager.Instance.GetPlayerScore(owner);
       // scoreText.text = score.ToString();                    

        // 2) 메달 세팅
        for (int i = 0; i < medalCount && i < medalSlots.Length; i++)
            Instantiate(medalPrefab, medalSlots[i].position, Quaternion.identity, medalSlots[i]);
    }
}
