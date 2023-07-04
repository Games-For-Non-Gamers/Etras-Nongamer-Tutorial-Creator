using UnityEngine;
using UnityEditor;
using System;
using Etra.StarterAssets.Source.Editor;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Items;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Etra.NonGamerTutorialCreator.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(AbilitySelectorAttribute))]
    public class AbilitySelectorAttributeDrawer : PropertyDrawer
    {
        private static Type[] _abilityTypes = null;
        private static Type[] _AbilityTypes
        {
            get
            {
                if (_abilityTypes == null)
                {
                    _abilityTypes = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>()
                        .ToArray();
                }

                return _abilityTypes;
            }
        }

        private static Type[] _abilityAndItemTypes;
        private static Type[] _AbilityAndItemTypes
        {
            get
            {
                if (_abilityAndItemTypes == null)
                {
                    var abilities = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>();
                    var items = EtraGUIUtility.FindAllTypes<EtraFPSUsableItemBaseClass>();

                    _abilityAndItemTypes = abilities
                        .Concat(items)
                        .ToArray();
                }

                return _abilityAndItemTypes;
            }
        }

        int _abilityIndex = -1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, "Use Ability Selector on string field only"); ;
                return;
            }

            bool selectItems = (attribute as AbilitySelectorAttribute).ShowItems;
            var types = selectItems ? _AbilityAndItemTypes : _AbilityTypes;

            var abilityPaths = types
                .Select(x => x.FullName)
                .Prepend(string.Empty)
                .ToList();

            //Get the current ability index
            
            if (_abilityIndex == -1)
            {
                _abilityIndex = 0;
                if (abilityPaths.Contains(property.stringValue))
                    _abilityIndex = abilityPaths.IndexOf(property.stringValue);
            }
            

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            var abilityNames = types
                .Select(x => new KeyValuePair<Type, string>(x, x.Name.Split('_').Last())) //Remove prefixes
                .Select(x => new KeyValuePair<Type, string>(x.Key, Regex.Replace(x.Value, "([a-z])([A-Z])", "$1 $2"))) //Add spaces between capital letters
                .Select(x => typeof(EtraFPSUsableItemBaseClass).IsAssignableFrom(x.Key) ?
                    $"ITEM: {x.Value}" :
                    x.Value) //Add the 'ITEM' prefix to items
                .Select(x => new GUIContent(x))
                .Prepend(new GUIContent("None"))
                .ToArray();

            
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                _abilityIndex = EditorGUI.Popup(position, _abilityIndex, abilityNames);

                if (changeCheck.changed)
                {
                    property.stringValue = abilityPaths[_abilityIndex];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            
        }
    }




}