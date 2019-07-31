using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cova8bitdots
{
    public class PitchTest : MonoBehaviour
    {
        //----------------------------------------
        // 定数関連
        //----------------------------------------
        #region ===== CONSTS =====
        public enum CLIP_TYPE
        {
            PIANO_A3,

            CODE_G,
            CODE_Ab,
            CODE_Bb,

            SE_1UP,


            MAX,
        }
        #endregion //) ===== CONSTS =====

        //----------------------------------------
        // メンバ変数
        //----------------------------------------
        #region ===== MEMBER_VARIABLES =====

        [SerializeField]
        AudioClip[] m_clips = new AudioClip[(int)CLIP_TYPE.MAX];
        public float GetClipLength( CLIP_TYPE _type ){ return m_clips[ (int)_type].length; }

        List<AudioSource> m_sourceList = new List<AudioSource>();
        AudioSource m_source = null;
        Queue<AudioSource> m_emptyList = new Queue<AudioSource>();
        #endregion //) ===== MEMBER_VARIABLES =====


        void OnEnable()
        {
            for (int i = 0; i < 3; i++)
            {
                var s = this.gameObject.AddComponent<AudioSource>();
                m_emptyList.Enqueue( s );
                m_sourceList.Add( s );
            }

            m_source = m_sourceList[0];
        }

        void OnDisable()
        {
            foreach (var item in m_sourceList)
            {
                Destroy( item );
            }
            m_sourceList.Clear();
            m_emptyList.Clear();
            m_source = null;
        }

        public float Pitch
        {
            get{ return m_source == null ? 1.0f : m_source.pitch; }
            set{ if( m_source == null )return; m_source.pitch = Mathf.Clamp( value, -3.0f, 3.0f);}
        }
        public void PlaySE( CLIP_TYPE _type)
        {
            if( m_source == null )
            {
                return;
            }
            var s = m_source;
            s.clip = m_clips[(int)_type];
            s.loop = false;
            s.Play();
        }
        public void PlaySE( CLIP_TYPE _type, float _delay)
        {
            if( m_emptyList.Count < 1 )
            {
                return;
            }
            var s = m_emptyList.Dequeue();
            s.clip = m_clips[(int)_type];
            s.loop = false;
            s.PlayDelayed(  _delay );
        }
        public void PlaySE( CLIP_TYPE _type, float _delay, float _pitch)
        {
            if( m_emptyList.Count < 1 )
            {
                return;
            }
            var s = m_emptyList.Dequeue();
            s.clip = m_clips[(int)_type];
            s.loop = false;
            s.pitch = _pitch;
            s.PlayDelayed( _delay );
        }

        void Update()
        {
            for (int i = 0; i < m_sourceList.Count; i++)
            {
                if( m_emptyList.Contains(m_sourceList[i]) )
                {
                    continue;
                }
                if( !m_sourceList[i].isPlaying )
                {
                    Debug.Log($"{i}: End");
                    m_emptyList.Enqueue( m_sourceList[i] );
                }
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(PitchTest))]
    [CanEditMultipleObjects]
    public class PitchTestEditor : Editor
    {
        const float UPPER_HALF_TONE = 1.05946309436f;   // 2^(1/12)
        const float LOWER_HALF_TONE = 0.94387431268f;   // 1/UPPER_HALF_TONE;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var obj = target as PitchTest;
            EditorGUILayout.BeginHorizontal();
            {
                if( GUILayout.Button("--"))
                {
                    obj.Pitch = obj.Pitch * LOWER_HALF_TONE* LOWER_HALF_TONE;
                }
                if( GUILayout.Button("-"))
                {
                    obj.Pitch = obj.Pitch * LOWER_HALF_TONE;
                }
                if( GUILayout.Button("R"))
                {
                    obj.Pitch = 1.0f;
                }
                if( GUILayout.Button("+"))
                {
                    obj.Pitch = obj.Pitch * UPPER_HALF_TONE;
                }
                if( GUILayout.Button("++"))
                {
                    obj.Pitch = obj.Pitch * UPPER_HALF_TONE * UPPER_HALF_TONE;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if( GUILayout.Button("Play Original"))
                {
                    // double length = AudioSettings.dspTime;
                    float length = 0.0f;
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_G, length, 1.0f);

                    length += obj.GetClipLength( PitchTest.CLIP_TYPE.CODE_G );
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_Ab, length, 1.0f);
                    Debug.Log($"Delay:{length}");

                    length += obj.GetClipLength( PitchTest.CLIP_TYPE.CODE_Ab );
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_Bb, length, 1.0f);
                    Debug.Log($"Delay:{length}");
                }
                if( GUILayout.Button("Play Script"))
                {
                    // double length = AudioSettings.dspTime;
                    float length = 0.0f;

                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_G, length);

                    length += obj.GetClipLength( PitchTest.CLIP_TYPE.CODE_G )*LOWER_HALF_TONE;
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_G, length, UPPER_HALF_TONE);
                    Debug.Log($"Delay:{length}");

                    length += obj.GetClipLength( PitchTest.CLIP_TYPE.CODE_G )*LOWER_HALF_TONE*LOWER_HALF_TONE*LOWER_HALF_TONE;
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_G, length, UPPER_HALF_TONE*UPPER_HALF_TONE*UPPER_HALF_TONE);

                    Debug.Log($"Delay:{length}");

                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if( GUILayout.Button("Play A3"))
                {
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3);
                }
                if( GUILayout.Button("Play 1up"))
                {
                    obj.PlaySE( PitchTest.CLIP_TYPE.SE_1UP);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        #endif
    }

}
