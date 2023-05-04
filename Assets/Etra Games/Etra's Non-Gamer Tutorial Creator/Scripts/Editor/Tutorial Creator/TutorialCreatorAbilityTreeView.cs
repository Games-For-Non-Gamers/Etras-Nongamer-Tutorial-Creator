using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;
using System.Linq;

using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Items;
using Etra.StarterAssets.Source.Editor;

using static Etra.StarterAssets.EtraCharacterMainController;

namespace Etra.NonGamerTutorialCreator.TutorialCreator
{
    public class TutorialCreatorAbilityTreeView : TreeView
    {
        const string _ABILITY_HEADER = "Ability";
        const string _CANNOT_HEADER = "Cannot";
        const string _STARTS_WITH_HEADER = "Starts With";
        const string _TAUGHT_TO_HEADER = "Is Taught To";

        const float _CANNOT_COLUMN_WIDTH = 0.3333f;
        const float _STARTS_WITH_COLUMN_WIDTH = 0.3333f;
        const float _TAUGHT_TO_COLUMN_WIDTH = 0.3333f;

        public float Width { get; set; }

        Dictionary<int, Ability> abilities = new Dictionary<int, Ability>();

        public GameplayType TutorialGameplayType { get; set; }

        #region Creation
        public TutorialCreatorAbilityTreeView(TreeViewState state)
            : base(state)
        {
            rowHeight = 20;
            GenerateAbilitiesAndItems();
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(-2, -1);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>();

            rows.Clear();

            TreeViewItem headerItem = new TreeViewItem(-1, -1);
            root.AddChild(headerItem);
            rows.Add(headerItem);

            var abilityCollection = new Dictionary<int, Ability>(abilities);

            foreach (var ability in abilities)
            {
                TreeViewItem treeViewItem = new TreeViewItem(ability.Key, -1, ability.Value.name);
                root.AddChild(treeViewItem);
                rows.Add(treeViewItem);
            }

            return rows;
        }
        #endregion

        #region Abilities
        public void GenerateAbilitiesAndItems()
        {
            //Initialize abilities
            var allAbilities = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>()
                .Select(x => new Ability(x))
                .ToList();

            var fpAbilities = allAbilities
                .Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.FirstPerson))
                .ToList();

            var tpAbilities = allAbilities
                .Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.ThirdPerson))
                .ToList();

            abilities = allAbilities
                .Except(fpAbilities)
                .Except(tpAbilities)
                .Concat(TutorialGameplayType switch
                {
                    GameplayType.ThirdPerson => tpAbilities,
                    GameplayType.FirstPerson => fpAbilities,
                    _ => new List<Ability>(),
                })
                .ToDictionary(x => x.type.GetHashCode());

            if (TutorialGameplayType == GameplayType.FirstPerson)
            {
                var items = EtraGUIUtility.FindAllTypes<EtraFPSUsableItemBaseClass>()
                    .Select(x => new ItemAbility(x) as Ability)
                    .ToDictionary(x => x.type.GetHashCode());

                abilities = abilities
                    .Concat(items)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }

        Ability GetAbility(int key) =>
            abilities.TryGetValue(key, out Ability ability) ?
                ability :
                null;
        #endregion

        #region GUI
        protected override void RowGUI(RowGUIArgs args)
        {
            bool repaint = Event.current.type == EventType.Repaint;

            var ability = GetAbility(args.item.id);

            Rect abilityRect = args.rowRect
                .ResizeToLeft(EditorGUIUtility.labelWidth);

            Rect itemsRect = args.rowRect
                .BorderLeft(EditorGUIUtility.labelWidth);

            Rect cannotRect = itemsRect
                .SetWidth(_CANNOT_COLUMN_WIDTH * itemsRect.width);

            Rect startsWithRect = itemsRect
                .BorderLeft(_CANNOT_COLUMN_WIDTH * itemsRect.width)
                .BorderRight(_TAUGHT_TO_COLUMN_WIDTH * itemsRect.width);

            Rect taughtToRect = itemsRect
                .ResizeToRight(_TAUGHT_TO_COLUMN_WIDTH * itemsRect.width);

            switch (args.item.id)
            {
                case -1:
                    EditorGUI.LabelField(abilityRect, _ABILITY_HEADER, Styles.HeaderLabel);
                    EditorGUI.LabelField(cannotRect, _CANNOT_HEADER, Styles.HeaderLabel);
                    EditorGUI.LabelField(startsWithRect, _STARTS_WITH_HEADER, Styles.HeaderLabel);
                    EditorGUI.LabelField(taughtToRect, _TAUGHT_TO_HEADER, Styles.HeaderLabel);
                    break;
                default:
                    GUIContent abilityContent = new GUIContent(args.label);

                    if (ability is ItemAbility)
                        abilityContent.image = EditorGUIUtility.IconContent("d_FilterByType").image;

                    EditorGUI.LabelField(abilityRect.BorderLeft(EditorGUIUtility.standardVerticalSpacing), abilityContent);
                    if (EditorGUI.Toggle(cannotRect.ResizeWidthToCenter(EditorGUIUtility.singleLineHeight), ability.state == Ability.State.Disabled))
                        ability.state = Ability.State.Disabled;

                    if (EditorGUI.Toggle(startsWithRect.ResizeWidthToCenter(EditorGUIUtility.singleLineHeight), ability.state == Ability.State.StartsWith))
                        ability.state = Ability.State.StartsWith;

                    if (EditorGUI.Toggle(taughtToRect.ResizeWidthToCenter(EditorGUIUtility.singleLineHeight), ability.state == Ability.State.TaughtTo))
                        ability.state = Ability.State.TaughtTo;

                    break;
            }

            if (repaint)
                OnGUIRepaint(args);
        }

        protected override void AfterRowsGUI()
        {
            base.AfterRowsGUI();
            RefreshCustomRowHeights();
        }

        void OnGUIRepaint(RowGUIArgs args)
        {
            Rect separatorRect = new Rect(args.rowRect)
                .ResizeToBottom(1f);

            Styles.Separator.Draw(separatorRect, GUIContent.none, true, true, args.selected, args.focused);
        }

        //Making items unselectable
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            SetSelection(new List<int>());
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item) =>
            item.id switch 
            {
                -1 => GetHeaderHeight(),
                _ => base.GetCustomRowHeight(row, item),
            };

        float GetHeaderHeight()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            float totalWidth = Width - labelWidth;

            float[] heights = new float[]
            {
                Styles.HeaderLabel.CalcHeight(new GUIContent(_ABILITY_HEADER), labelWidth),
                Styles.HeaderLabel.CalcHeight(new GUIContent(_CANNOT_HEADER), totalWidth * _CANNOT_COLUMN_WIDTH),
                Styles.HeaderLabel.CalcHeight(new GUIContent(_STARTS_WITH_HEADER), totalWidth * _STARTS_WITH_COLUMN_WIDTH),
                Styles.HeaderLabel.CalcHeight(new GUIContent(_TAUGHT_TO_HEADER), totalWidth * _TAUGHT_TO_COLUMN_WIDTH),
            };

            return Mathf.Max(heights);
        }
        #endregion

        public class Ability
        {
            public Ability(Type type)
            {
                this.type = type;
                state = State.Disabled;
                GenerateName();
            }

            public enum State
            {
                Disabled,
                StartsWith,
                TaughtTo,
            }

            public Type type;
            public string name;
            public State state;

            public virtual void GenerateName()
            {
                name = type.Name
                    .Split('_')
                    .Last();

                name = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
            }
        }

        public class ItemAbility : Ability
        {
            public ItemAbility(Type type) : base(type) { }
        }

        private static class Styles
        {
            public static GUIStyle HeaderLabel => new GUIStyle(EditorStyles.boldLabel)
            {
                wordWrap = true,
                alignment = TextAnchor.LowerCenter,
                padding = new RectOffset(4, 4, 4, 4),
                fontSize = 16,
            };

            public static GUIStyle Label => new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleLeft 
            };

            public static GUIStyle Separator => new GUIStyle("Label")
                .WithBackground(EtraGUIUtility.BorderTexture);
        }
    }
}