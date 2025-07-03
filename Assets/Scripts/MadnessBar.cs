using Unity.VisualScripting;
using UnityEngine;

public class MadnessBar : MonoBehaviour
{
    
    [SerializeField] private ai _aiScript;
    [SerializeField] private Vector3 _offset = new Vector3(0, 3, 0);
    [SerializeField] private Vector3 _textOffset = new Vector3(0, 3.5f, 0);
    [SerializeField] private Vector2 size = new Vector2(60, 10);
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    void OnGUI() {
        if (!_aiScript) return ;
        Vector3 worldPos = transform.position + _offset;
        Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);

        if (screenPos.z > 0)
        {
            float fill =  Mathf.Clamp(_aiScript._madness, 0, 150) / 150f;
            Rect bgRect = new Rect(screenPos.x - size.x / 2, Screen.height - screenPos.y, size.x, size.y);
            GUI.color = Color.black;
            GUI.DrawTexture(bgRect, Texture2D.whiteTexture);
            GUI.color = Color.Lerp(Color.green, Color.red, fill);
            GUI.DrawTexture(new Rect(bgRect.x + 1, bgRect.y + 1, (size.x - 2) * fill, size.y - 2), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        
        // AI state text
        Vector3 textWorldPos = transform.position + _textOffset;
        Vector3 textScreenPos = _camera.WorldToScreenPoint(textWorldPos);

        if (textScreenPos.z > 0)
        {
            string state = _aiScript._state;
            Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(state));
            Rect textRect = new Rect(textScreenPos.x - textSize.x / 2, Screen.height - textScreenPos.y, textSize.x, textSize.y);
            GUI.color = Color.white;
            // float fill = Mathf.Clamp(_aiScript._madness, 0, 150) / 150f;
            // GUI.color = Color.Lerp(Color.green, Color.red, fill);
            GUI.Label(textRect, state);
        }
    }
    
}
