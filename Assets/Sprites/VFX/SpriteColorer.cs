using UnityEngine;
using Mirror;

// Properties: Color
// Methods: SetColor, SetTeamColor
public class SpriteColorer : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer[] spritesToColor;
    [SyncVar]
    [SerializeField] private Color color;
    [SerializeField] private TeamsConfig teamsConfig;

    public Color Color { get { return color; } }

    private void Start()
    {
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        this.color = color;
        foreach (var sprite in spritesToColor)
            sprite.color = color;
    }

    public void SetTeamColor(Team team)
    {
        if (team == Team.Left)
            color = teamsConfig.LeftTeamColor;
        else if (team == Team.Right)
            color = teamsConfig.RightTeamColor;
        SetColor(color);
    }
}
