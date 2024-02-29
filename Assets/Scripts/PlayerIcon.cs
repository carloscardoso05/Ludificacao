using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteResolver))]
[RequireComponent(typeof(SpriteLibrary))]
public class PlayerIcon : MonoBehaviour {
    [SerializeField] private SpriteResolver resolver;
    
    private void Start() {
        GameManager.I.OnTurnChanged += ChangeIconColor;
    }

    private void ChangeIconColor(object sender, GameColor newColor) {
        resolver.SetCategoryAndLabel("player", newColor.ToString());
    }
}