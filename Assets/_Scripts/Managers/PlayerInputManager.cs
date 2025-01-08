using System;
using UnityEngine;

namespace Managers
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static event Action<Vector2> OnMouseClick;

        private Camera m_MainCamera;
        private PlayerInput m_PlayerInput;


        private void Awake()
        {
            m_MainCamera = Camera.main;
            m_PlayerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            m_PlayerInput.Enable();
        }

        private void Update()
        {
            HandleMouseInputNewInputSystem();       // Ney input system
            // HandleMouseInputOldInputSystem();    // Old input system

        }

        private void OnDisable()
        {
            m_PlayerInput.Disable();
        }


        private void HandleMouseInputNewInputSystem()
        {
            if (m_PlayerInput.Board.SelectBlock.WasPressedThisFrame())
            {
                ProcessMouseClick();
            }
        }

        private void HandleMouseInputOldInputSystem()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            Vector2 mousePosition = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            OnMouseClick?.Invoke(mousePosition);
        }
    }
}