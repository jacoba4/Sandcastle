using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontMovement : MonoBehaviour
{
    public GameObject Font;
    public float _currentScale = InitScale;
    public const float TargetScale = 2.3f;
    public const float InitScale = 2f;
    public const int FramesCount = 100;
    public const float AnimationTimeSeconds = 1;
    public float _deltaTime = AnimationTimeSeconds / FramesCount;
    public float _dx = (TargetScale - InitScale) / FramesCount;
    public bool _upScale = true;
    private IEnumerator Breath()
    {
        while (true)
        {
            while (_upScale)
            {
                _currentScale += _dx;
                if (_currentScale > TargetScale)
                {
                    _upScale = false;
                    _currentScale = TargetScale;
                }
                Font.transform.localScale = Vector2.one * _currentScale;
                yield return new WaitForSeconds(_deltaTime);
            }

            while (!_upScale)
            {
                _currentScale -= _dx;
                if (_currentScale < InitScale)
                {
                    _upScale = true;
                    _currentScale = InitScale;
                }
                Font.transform.localScale = Vector2.one * _currentScale;
                yield return new WaitForSeconds(_deltaTime);
            }
        }
    }
    private void Start()
    {
        StartCoroutine(Breath());
    }
}