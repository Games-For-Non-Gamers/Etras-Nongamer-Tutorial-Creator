using Etra.NonGamerTutorialCreator;
using UnityEditor;
using UnityEngine;


    //[CustomEditor(typeof(EtraUiAnimation)), CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(EtraUiAnimation))]
    public class EtraUiAnimationEditor : PropertyDrawer
    {

    const float min = 0.1f; // this is the minimum number for the float slider of speed
    const float max = 10;   // this is the maximum number for the float slider of speed
    const int ControlHeight = 16; // this is the height of each control line


    // we check if we are adding a new class to the inspector gui, if so, we add the control height
    // so that the next control goes bellow of our second line of previous control
    public override float GetPropertyHeight(SerializedProperty prop,
                                             GUIContent label)
    {
        return 50;
        //return base.GetPropertyHeight(prop, label) + ControlHeight;
    }

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        // we go one indent inside to make things nicer
        EditorGUI.indentLevel = 1;
        // we find the same Speed float of the animationClass
        
        // we find the same Animation variable on animationClass
        SerializedProperty anim = prop.FindPropertyRelative("Animation");
        // we check for any change on GUI
        EditorGUI.BeginChangeCheck();
        // Draw Speed slider
       // EditorGUI.Slider(new Rect(pos.x, pos.y, pos.width, 15), speed, min, max, label);
        // Now add a lable beside slider so that we know what it do
     //   EditorGUI.LabelField(new Rect(pos.x + pos.width / 2 - 65, pos.y, 60, pos.height), "Speed :");
        //define a new rect
        Rect ExtraPosition = EditorGUI.IndentedRect(pos);
        // now we want to put things in second line
        ExtraPosition.y += ControlHeight;
        ExtraPosition.height = ControlHeight + 5;
        // now we can add the other stuff here but to make it simpler and more coder friendly
        // for future, i put them in another method
        DrawAnimControls(ExtraPosition, anim, prop, label);
    }

    void DrawAnimControls(Rect position, SerializedProperty prop1, SerializedProperty fps, GUIContent label)
    {
        // we check if things changed
        EditorGUI.BeginChangeCheck();
        // we need to devide the width of inspector gui to 2, so that we can add 2 fields there
        position.width = position.width / 2;
        // Draw Animation Field
        
        /*
        EditorGUI.PropertyField(
            position,
            prop1, GUIContent.none);
        position.x += position.width;
        // now we add another label so that we know what the enum is about

        EditorGUI.LabelField(new Rect(position.x, position.y, 50, position.height), "FPS :");

        */

        // and finally we add the enum to GUI
        // remember that you can't address the enum variable of animClass directly,
        // but if you defiend a variable of type of that enum, then you can search for the
        // name of that variable, here i had : "public AnimationFPS FPS"
        // and with using "fps.FindPropertyRelative("FPS")" i can find that variable
        // and make a property field for that !
        EditorGUI.PropertyField(new Rect(position.x + 55, position.y, position.width - 55, position.height), fps.FindPropertyRelative("FPS"), GUIContent.none);

        AnimationFPS state = (AnimationFPS)fps.FindPropertyRelative("FPS").enumValueIndex;
       

        SerializedProperty speed = prop1.FindPropertyRelative("Speed");
        switch (state)
        {
            case 0:

                
                // Now add a lable beside slider so that we know what it do
                  EditorGUI.LabelField(new Rect(position.x + position.width / 2 - 65, position.y, 60, position.height), "Speed :");
                Debug.Log("Speed");
                EditorGUI.PropertyField(new Rect(position.x + 55, position.y +10, position.width - 55, position.height), fps.FindPropertyRelative("FPS"), GUIContent.none);
                //EditorGUI.Slider(new Rect(position.x, position.y, position.width, 15), speed, min, max, label);
                //   EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
                //    EditorGUILayout.IntSlider(valForA_Prop, 0, 10, new GUIContent("valForA"));
                //      EditorGUILayout.IntSlider(valForAB_Prop, 0, 100, new GUIContent("valForAB"));
                break;

            case AnimationFPS.Film:

               // EditorGUI.Slider(new Rect(position.x, position.y, position.width, 15), speed, min, max, label);
                //  EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
                //   EditorGUILayout.IntSlider(valForAB_Prop, 0, 100, new GUIContent("valForAB"));
                break;

            case AnimationFPS.PAL:
                //  EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
                //  EditorGUILayout.IntSlider(valForC_Prop, 0, 100, new GUIContent("valForC"));
                break;

        }

    }

    /*
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 16f * 20;
    }
    



    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {

        var positionProperty = property.FindPropertyRelative("Position");
        var normalProperty = property.FindPropertyRelative("NormalRotation");

        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = 70;
        rect.height /= 2;
        positionProperty.vector3Value = EditorGUI.Vector3Field(rect, "Position", positionProperty.vector3Value);
        rect.y += rect.height;
        normalProperty.vector3Value = EditorGUI.Vector3Field(rect, "Normal", normalProperty.vector3Value);


        SerializedProperty
        state_Prop,
        valForAB_Prop,
        valForA_Prop,
        valForC_Prop,
        controllable_Prop;

        state_Prop = property.FindPropertyRelative("state");
        valForAB_Prop = property.FindPropertyRelative("valForAB");
        valForA_Prop = property.FindPropertyRelative("valForA");
        valForC_Prop = property.FindPropertyRelative("valForC");
        controllable_Prop = property.FindPropertyRelative("controllable");


       

        // EditorGUILayout.PropertyField(state_Prop);

        EtraUiAnimation.Status st = (EtraUiAnimation.Status)state_Prop.enumValueIndex;

        switch (st)
        {
            case EtraUiAnimation.Status.A:

                valForAB_Prop.intValue = EditorGUI.IntField(rect, "A Test pog", valForAB_Prop.intValue);

                //   EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
                //    EditorGUILayout.IntSlider(valForA_Prop, 0, 10, new GUIContent("valForA"));
                //      EditorGUILayout.IntSlider(valForAB_Prop, 0, 100, new GUIContent("valForAB"));
                break;

            case EtraUiAnimation.Status.B:
              //  EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
             //   EditorGUILayout.IntSlider(valForAB_Prop, 0, 100, new GUIContent("valForAB"));
                break;

            case EtraUiAnimation.Status.C:
              //  EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
              //  EditorGUILayout.IntSlider(valForC_Prop, 0, 100, new GUIContent("valForC"));
                break;

        }

    }
    */
    /*


       public override void OnInspectorGUI()
       {

           serializedObject.Update();

          EditorGUILayout.PropertyField(state_Prop);

          EtraUiAnimation.Status st = (EtraUiAnimation.Status)state_Prop.enumValueIndex;

          switch (st)
          {
              case EtraUiAnimation.Status.A:
                  EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
                  EditorGUILayout.IntSlider(valForA_Prop, 0, 10, new GUIContent("valForA"));
                  EditorGUILayout.IntSlider(valForAB_Prop, 0, 100, new GUIContent("valForAB"));
                  break;

              case EtraUiAnimation.Status.B:
                  EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
                  EditorGUILayout.IntSlider(valForAB_Prop, 0, 100, new GUIContent("valForAB"));
                  break;

              case EtraUiAnimation.Status.C:
                  EditorGUILayout.PropertyField(controllable_Prop, new GUIContent("controllable"));
                  EditorGUILayout.IntSlider(valForC_Prop, 0, 100, new GUIContent("valForC"));
                  break;

          }
            serializedObject.ApplyModifiedProperties();
    }
      */

}


