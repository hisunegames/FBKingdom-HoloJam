using UnityEngine;

[CreateAssetMenu(fileName = "MascotMessage", menuName = "GameData/MascotMessage")]
public class MascotMessageSO : ScriptableObject
{
    public string[] MessageList;

    /// <summary>
    /// Should have five buffs.
    /// </summary>
    public string[] BuffList;
}
