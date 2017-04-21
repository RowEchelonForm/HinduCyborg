using Rotorz.ReorderableList;
using UnityEditor;

/*
 * Making possible the reorderable list editor functionality
 * Credit to Rotorz: https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity
*/
[CustomEditor(typeof(AnimationSelector))]
public class PriorityListEditor : Editor
{

	private SerializedProperty _animationsPriorities;

	private void OnEnable()
	{
		_animationsPriorities = serializedObject.FindProperty("animationPriorities");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		ReorderableListGUI.Title("Animation Priorities");
		ReorderableListGUI.ListField(_animationsPriorities);
		serializedObject.ApplyModifiedProperties();
	}

}