using UnityEngine;
using UnityEngine.Profiling;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        [SerializeField, Range(1, 6)] private int _colorsInGame;

        private static GameManager ms_Instance;


        private void Awake()
        {
            InitializationSingleton();

            UnityEditorInternal.ProfilerDriver.enabled = true;
            Profiler.enabled = true;
            Profiler.SetAreaEnabled(ProfilerArea.CPU, true);
        }

        private void InitializationSingleton()
        {
            if (!ms_Instance)
            {
                ms_Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public static int GetRows()
        {
            return ms_Instance._rows;
        }

        public static int GetColumns()
        {
            return ms_Instance._columns;
        }

        public static int GetColorsInGame()
        {
            return ms_Instance._colorsInGame;
        }
    }
}