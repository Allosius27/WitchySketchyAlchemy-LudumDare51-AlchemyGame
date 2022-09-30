//
// Updated by Allosius(Yanis Q.) on 7/9/2022.
//

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllosiusDevUtilities.Audio
{
    [CreateAssetMenu(fileName = "New AudioData", menuName = "AllosiusDev/AudioData")]
    public class AudioData : ScriptableObject
    {
        #region Properties

        public AudioClip Clip => clip;

        public float Volume2d => volume2d;
        public float Volume3d => volume3d;
        public float Pitch => pitch;
        public bool Loop => loop;
        public int Priority => priority;
        public int SpacialBlend => spacialBlend;
        public float MaxDistance => _maxDistance;
        public enum TypeSound
        {
            Sfx,
            Music,
            Ambients,
        }

        #endregion

        #region UnityInspector

        [SerializeField] private AudioClip clip;

        public TypeSound typeSound;

        [Range(.1f, 3)]
        [SerializeField] private float pitch = 1.0f;
        [SerializeField] private bool loop;
        [Range(0, 256)]
        [SerializeField] private int priority = 128;


        [SerializeField] private bool parameters2d;

        [ShowIf("parameters2d")]
        [Range(0f, 5f)]
        [SerializeField] private float volume2d = 0.25f;


        [SerializeField] private bool parameters3d;

        [ShowIf("parameters3d")]
        [Range(0f, 5f)]

        [SerializeField] private float volume3d = 0.25f;
        [ShowIf("parameters3d")]
        [Range(0, 1)]

        [SerializeField] private int spacialBlend;
        [ShowIf("parameters3d")]
        [SerializeField] private float _maxDistance = 500.0f;

        #endregion


        #region Behaviour

        public void SetSpacialBlend(int _value)
        {
            this.spacialBlend = _value;
        }

        public void ActiveSpacialBlend(int _value)
        {
            SetSpacialBlend(_value);
        }

        public float SoundIs3D()
        {
            if (SpacialBlend != 0)
            {
                return Volume3d;
            }
            else
            {
                return Volume2d;
            }
        }

        #endregion
    }
}
