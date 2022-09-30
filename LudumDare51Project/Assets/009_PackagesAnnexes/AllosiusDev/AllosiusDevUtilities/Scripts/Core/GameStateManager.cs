//
// Updated by Allosius(Yanis Q.) on 11/9/2022.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AllosiusDevUtilities.Core
{
    public class GameStateManager : Singleton<GameStateManager>
    {
        #region Fields

        private long m_SessionStartTime;

        private float m_FPS;

        #endregion

        #region Properties

        public static bool gameIsPaused = false;

        public long sessionStartTime
        {
            get
            {
                return m_SessionStartTime;
            }
        }

        public float fps
        {
            get
            {
                return m_FPS;
            }
        }

        #endregion

        #region Behaviour

        [Button(ButtonSizes.Medium)]
        public void GetGameIsPaused()
        {
            Debug.Log("Game Is Paused : " + gameIsPaused);
        }

        protected override void Awake()
        {
            base.Awake();

            Configure();
        }

        private void Update()
        {
            if(gameIsPaused)
            {
                return;
            }
            m_FPS = Time.frameCount / Time.time;
        }

        private void Configure()
        {
            StartSession();
        }

        private void StartSession()
        {
            m_SessionStartTime = EpochSeconds();
        }

        private long EpochSeconds()
        {
            var _epoch = new System.DateTimeOffset(System.DateTime.UtcNow);
            return _epoch.ToUnixTimeSeconds();
        }

        #endregion
    }
}
