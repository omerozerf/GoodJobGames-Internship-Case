using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private float _orthographicSizeMultiplier;
        private Camera m_MainCamera;


        private void Awake()
        {
            m_MainCamera = Camera.main;
            Board.OnInitializeBoard += HandleOnInitializeBoard;
        }

        private void OnDestroy()
        {
            Board.OnInitializeBoard -= HandleOnInitializeBoard;
        }


        private void HandleOnInitializeBoard(int rows, int columns)
        {
            CenterCamera(rows, columns);
        }


        private void CenterCamera(int rows, int columns)
        {
            var centerX = (columns - 1) * 0.5f;
            var centerY = (rows - 1) * 0.5f;

            m_MainCamera.transform.position = new Vector3(centerX
                , centerY
                , m_MainCamera.transform.position.z);

            var aspectRatio = m_MainCamera.aspect;
            float boardHeight = rows;
            float boardWidth = columns;

            m_MainCamera.orthographicSize = Mathf.Max(boardHeight / 2, boardWidth / (2 * aspectRatio))
                                            * _orthographicSizeMultiplier;
        }
    }
}