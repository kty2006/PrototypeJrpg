using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class Button : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private readonly Vector3 originSize = Vector3.one;
    private readonly float sizeChangeSpeed = 0.45f;

    public UnityEvent OnUpEvent;
    public UnityEvent OnDownEvent;
    public UnityEvent OnClickEvent;

    [SerializeField] private float downSize = 0.95f;
    [SerializeField] private float upSize = 1.01f;

    private Coroutine _sizeRoutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        InvokeClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InvokeDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InvokeUp();
    }

    public void AddOnClick(UnityAction action)
    {
        OnClickEvent.AddListener(action);
    }

    public void AddOnUp(UnityAction action)
    {
        OnUpEvent.AddListener(action);
    }

    public void AddOnDown(UnityAction action)
    {
        OnDownEvent.AddListener(action);
    }

    public void SetOnClick(UnityAction action)
    {
        OnClickEvent.RemoveAllListeners();
        OnClickEvent.AddListener(action);
    }

    public void SetOnUp(UnityAction action)
    {
        OnUpEvent.RemoveAllListeners();
        OnUpEvent.AddListener(action);
    }

    public void SetOnDown(UnityAction action)
    {
        OnDownEvent.RemoveAllListeners();
        OnDownEvent.AddListener(action);
    }

    private void InvokeClick()
    {
        OnClickEvent?.Invoke();
    }

    private void InvokeUp()
    {
        OnUpEvent?.Invoke();
        if (_sizeRoutine != null)
        {
            StopCoroutine(_sizeRoutine);
        }
        _sizeRoutine = StartCoroutine(ReSize(upSize, true));
    }

    private void InvokeDown()
    {
        OnDownEvent?.Invoke();
        if (_sizeRoutine != null)
        {
            StopCoroutine(_sizeRoutine);
        }
        _sizeRoutine = StartCoroutine(ReSize(downSize, false));
    }

    IEnumerator ReSize(float size, bool isBackToOrigin)
    {
        while (Mathf.Abs(transform.localScale.x - size) > 0.01f)
        {
            transform.localScale = originSize * Mathf.Lerp(transform.localScale.x, size, sizeChangeSpeed);
            yield return null;
        }
        transform.localScale = originSize * size;

        if (isBackToOrigin)
        {
            while (Mathf.Abs(transform.localScale.x - originSize.x) > 0.01f)
            {
                transform.localScale = originSize * Mathf.Lerp(transform.localScale.x, originSize.x, sizeChangeSpeed);
                yield return null;
            }
            transform.localScale = originSize;
        }
    }
}