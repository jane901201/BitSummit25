using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] private int score;
    
    public int GetScore() => score;
    public void SetScore(int scoreValue) => score = scoreValue;
}
