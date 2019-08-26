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

            PIANO_C3,

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
        const float THIRD_TONE = 1.25992104989f;        // 3度
        const float FIFTH_TONE = 1.49830707688f;        // 5度



        static readonly float[] DIATONIC_SCALE = new float[]{
            1.0f,               // 1度
            1.12246204831f,     // 2度
            1.25992104989f,     // 3度
            1.33483985417f,     // 4度
            1.49830707688f,     // 5度
            1.68179283051f,     // 6度
            1.88774862536f,     // 7度
            2.0f,               // 8度
        };

        static readonly float[] PENTATONIC_SCALE = new float[]
        {
            1.0f,               // 1度
            1.12246204831f,     // 2度
            1.25992104989f,     // 3度
            1.49830707688f,     // 5度
            1.68179283051f,     // 6度
        };
        static readonly float[] ARABIC_SCALE = new float[]
        {
            1.0f,               // 1度
            1.12246204831f,     // 2度
            1.189207115f,       // 3度(♭)
            1.41421356237f,     // 4度(♯)
            1.49830707688f,     // 5度
            1.58740105197f,     // 6度(♭)
            1.88774862536f,     // 7度
            2.0f,               // 8度
        };

        int index = 0;
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

                    length += obj.GetClipLength( PitchTest.CLIP_TYPE.CODE_Ab );
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_Bb, length, 1.0f);
                }
                if( GUILayout.Button("Play Script"))
                {
                    // double length = AudioSettings.dspTime;
                    float length = 0.0f;

                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_G, length);

                    length += obj.GetClipLength( PitchTest.CLIP_TYPE.CODE_G )*LOWER_HALF_TONE;
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_G, length, UPPER_HALF_TONE);

                    length += obj.GetClipLength( PitchTest.CLIP_TYPE.CODE_G )*LOWER_HALF_TONE*LOWER_HALF_TONE*LOWER_HALF_TONE;
                    obj.PlaySE( PitchTest.CLIP_TYPE.CODE_G, length, UPPER_HALF_TONE*UPPER_HALF_TONE*UPPER_HALF_TONE);

                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if( GUILayout.Button("Play A3"))
                {
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3, 0.0f, 1.0f);
                }
                if( GUILayout.Button("Play 1up"))
                {
                    obj.PlaySE( PitchTest.CLIP_TYPE.SE_1UP, 0.0f, 1.0f);
                }
            }
            EditorGUILayout.EndHorizontal();

            PlayCode( obj );

            PlayScale( obj );
        }
        void PlayCode(PitchTest obj)
        {
            EditorGUILayout.LabelField("PLAY CODE");
            EditorGUI.indentLevel++;
            {
                if( GUILayout.Button("Play A Code"))
                {
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3, 0f, 1.0f);
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3, 0f, THIRD_TONE);
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3, 0f, FIFTH_TONE);
                }
                if( GUILayout.Button("Play C Code"))
                {
                    float pitch = UPPER_HALF_TONE * UPPER_HALF_TONE * UPPER_HALF_TONE;
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3, 0f, pitch);
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3, 0f, pitch * THIRD_TONE);
                    obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_A3, 0f, pitch * FIFTH_TONE);
                }
            }
            EditorGUI.indentLevel--;
        }
        void PlayScale(PitchTest obj)
        {
            EditorGUILayout.LabelField("PLAY SCALE");
            EditorGUI.indentLevel++;
            {
                if( GUILayout.Button("Reset Index"))
                {
                    index = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    if( GUILayout.Button("Play Random(Diatonic)"))
                    {
                        obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_C3, 0f, DIATONIC_SCALE[ UnityEngine.Random.Range(0, DIATONIC_SCALE.Length)]);
                    }
                    if( GUILayout.Button("Play Random(PentaTonic)"))
                    {
                        obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_C3, 0f, PENTATONIC_SCALE[ UnityEngine.Random.Range(0, PENTATONIC_SCALE.Length)]);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if( GUILayout.Button("Play Incr(Diatonic)"))
                    {
                        obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_C3, 0f, DIATONIC_SCALE[ index++ % DIATONIC_SCALE.Length ]);
                    }
                    if( GUILayout.Button("Play Incr(PentaTonic)"))
                    {
                        obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_C3, 0f, PENTATONIC_SCALE[ index++ % PENTATONIC_SCALE.Length ]);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if( GUILayout.Button("Play Incr(ARABIC)"))
                    {
                        obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_C3, 0f, ARABIC_SCALE[ index++ % ARABIC_SCALE.Length ]);
                    }
                    if( GUILayout.Button("Play Decr(ARABIC)"))
                    {
                        index = Mathf.Max( 0, index-1 );
                        obj.PlaySE( PitchTest.CLIP_TYPE.PIANO_C3, 0f, ARABIC_SCALE[ index % ARABIC_SCALE.Length ]);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
        #endif
    }

}
