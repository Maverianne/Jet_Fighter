using System.Collections;
using UnityEngine;

public class ScreenWrapObject : MonoBehaviour
{
    private bool _canCheckScreenBounds = true; 
    private Camera _mainCamera;
    private Direction _exitDirection;

    protected virtual void Awake()
    {
        _mainCamera = Camera.main;
    }

    protected bool IsOutsideScreen(float radius)
    {
        if (!_canCheckScreenBounds) return false;
        var position = transform.position;
        
        var positivePos = new Vector2(position.x - radius,position.y - radius);
        var negativePos = new Vector2(position.x + radius,position.y + radius);
        var screenPositivePos = _mainCamera.WorldToScreenPoint (positivePos);
        var screenNegativePos = _mainCamera.WorldToScreenPoint (negativePos);

        if (screenPositivePos.x > Screen.safeArea.xMax)
        {
            _exitDirection = Direction.Right;
            return true;
        }

        if (screenNegativePos.x < Screen.safeArea.xMin)
        {
            _exitDirection = Direction.Left;
            return true;
        }

        if (screenPositivePos.y > Screen.safeArea.yMax)
        {
            _exitDirection = Direction.Up;
            return true;
        }

        if (screenNegativePos.y < Screen.safeArea.yMin)
        {
            _exitDirection = Direction.Down;
            return true;
        }
        return false;
    }

    protected void WrapPosition()
    {
        if(!_canCheckScreenBounds) return;;
        _canCheckScreenBounds = false;
        transform.position = GetWrapPosition(_exitDirection);
        AddScreenOffImpulse();
        StartCoroutine(PerformPlaneExitWrap(_exitDirection));
    }

    protected virtual void AddScreenOffImpulse()
    {
        
    }

    private Vector2 GetWrapPosition(Direction direction)
    {
        var position = transform.position;
        switch (direction)
        {
            case Direction.Left or Direction.Right:
                position.x = -position.x;
                break;
            case Direction.Up or Direction.Down:
                position.y = -position.y;
                break;
        }
        var newPos = position;
        return newPos;
    }

    private IEnumerator PerformPlaneExitWrap(Direction direction)
    {
        while (IsWrapping(direction)) yield return null;
        _canCheckScreenBounds = true;
    }

    private bool IsWrapping(Direction direction)
    {
        var position = _mainCamera.WorldToScreenPoint(transform.position);
        switch (direction)
        {
            case Direction.Right: return position.x < Screen.safeArea.xMin;
            case Direction.Left: return position.x > Screen.safeArea.xMax;
            case Direction.Up: return position.y < Screen.safeArea.yMin;
            case Direction.Down: return position.y > Screen.safeArea.yMax;
        }
        return false;
    }

    private enum Direction
    {
        Right, Left, Up, Down
    }
}
