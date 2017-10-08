using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{

    [PostProcessingModelEditor(typeof(GrainModel))]
    public class GrainModelEditor : PostProcessingModelEditor
    {
        SerializedProperty m_Colored;
        SerializedProperty m_Intensity;
        SerializedProperty m_Size;
        SerializedProperty m_LuminanceContribution;

        public override void OnEnable()
        {
            m_Colored = FindSetting((GrainModel.Settings x) => x.colored);
            m_Intensity = FindSetting((GrainModel.Settings x) => x.intensity);
            m_Size = FindSetting((GrainModel.Settings x) => x.size);
            m_LuminanceContribution = FindSetting((GrainModel.Settings x) => x.luminanceContribution);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_Intensity);
            EditorGUILayout.PropertyField(m_LuminanceContribution);
            EditorGUILayout.PropertyField(m_Size);
            EditorGUILayout.PropertyField(m_Colored);
        }
    }
}
