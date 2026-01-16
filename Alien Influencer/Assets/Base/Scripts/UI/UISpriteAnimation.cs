using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public int _frameIndex;
    public float _timer;
    [Header("References")]
    [SerializeField] private Sprite[] sprites;

    [Header("Animation")]
    [Tooltip("Seconds per frame")]
    [SerializeField] private float frameDuration = 0.02f;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        _timer = frameDuration;
    }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;

        if (_timer >= frameDuration)
        {
            _timer -= frameDuration;
            AdvanceFrame();
        }
    }

    void AdvanceFrame()
    {
        if (_frameIndex >= sprites.Length)
            _frameIndex = 0;

        image.sprite = sprites[_frameIndex];
        _frameIndex++;
    }
}
