using UnityEngine;
using System;

public class PlayerInputManager : MonoBehaviour
{
    public static event Action<Vector2> OnMouseClick;

    private Camera m_MainCamera;


    private void Awake()
    {
        m_MainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            OnMouseClick?.Invoke(mousePosition);
        }
    }
}