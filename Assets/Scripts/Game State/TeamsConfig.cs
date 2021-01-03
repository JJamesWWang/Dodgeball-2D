using UnityEngine;

[CreateAssetMenu(fileName = "Teams Config", menuName = "Teams Config")]
public class TeamsConfig : ScriptableObject
{
    [Header("Team Colors")]
    [SerializeField] private Color leftTeamColor;
    [SerializeField] private Color rightTeamColor;
    public Color LeftTeamColor { get { return leftTeamColor; } }
    public Color RightTeamColor { get { return rightTeamColor; } }
}