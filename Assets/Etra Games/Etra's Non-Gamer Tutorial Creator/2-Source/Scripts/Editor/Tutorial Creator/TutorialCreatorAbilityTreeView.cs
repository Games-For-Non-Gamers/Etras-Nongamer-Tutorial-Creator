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
using Etra.NonGamerTutorialCreator.Level;

namespace Etra.NonGamerTutorialCreator.TutorialCreator
{
    public class TutorialCreatorAbilityTreeView : TreeView
    {
        const string _ABILITY_HEADER = "Ability";
        const string _DISABLED_ABILITY_HEADER = "Cannot";
        const string _TAUGHT_ABILITY_HEADER = "Already Knows";
        const string _NEW_ABILITY_HEADER = "Is Taught To";

        const float _DISABLED_ABILITY_COLUMN_WIDTH = 0.3333f;
        const float _TAUGHT_ABILITY_COLUMN_WIDTH = 0.3333f;
        const float _NEW_ABILITY_COLUMN_WIDTH = 0.3333f;

        const string _SELECT_ALL_BUTTON_TEXT = "Select All";

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
        public List<Type> GetDisabledAbilities() =>
            abilities
            .Select(x => x.Value)
            .Where(x => x.state == Ability.State.Disabled)
            .Select(x => x.type)
            .ToList();

        public List<Type> GetTaughtAbilities() =>
            abilities
            .Select(x => x.Value)
            .Where(x => x.state == Ability.State.Taught)
            .Select(x => x.type)
            .ToList();

        public List<Type> GetNewAbilities() =>
            abilities
            .Select(x => x.Value)
            .Where(x => x.state == Ability.State.New)
            .Select(x => x.type)
            .ToList();

        private List<LevelChunk> _avaliableChunks = null;
        private List<string> abilitiesOrItemsThatHaveTeachingChunks;


        public List<string> GetAllFpsAbilities()
        {
            List<Ability> allAbilities = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();
            List<Ability> fpsAbilities = allAbilities.Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.FirstPerson)).ToList();



            List<Ability> tempList = new List<Ability>();


            tempList.AddRange(fpsAbilities);

            List<string> returnedList = new List<string>();

            foreach (var ability in allAbilities)
            {
                returnedList.Add(ability.name);
            }
            return returnedList;
        }

        public List<string> GetAllFpsItems()
        {
            List<Ability> fpsItems = EtraGUIUtility.FindAllTypes<EtraFPSUsableItemBaseClass>().Select(x => new ItemAbility(x) as Ability).ToList();

            List<Ability> tempList = new List<Ability>();

            tempList.AddRange(fpsItems);

            List<string> returnedList = new List<string>();

            foreach (var ability in tempList)
            {
                returnedList.Add(ability.name);
            }
            return returnedList;
        }

        public List<string> GetAllTpsAbilities()
        {
            List<Ability> allAbilities = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();
            List<Ability> tpsAbilities = allAbilities.Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.ThirdPerson)).ToList();


            List<Ability> tempList = new List<Ability>();


            tempList.AddRange(tpsAbilities);


            List<string> returnedList = new List<string>();

            foreach (var ability in tempList)
            {
                returnedList.Add(ability.name);
            }
            return returnedList;
        }

        public List<string> GetAbilitiesWithTeachingChunks()
        {
            List<string> tempStringList = new List<string>();

            //Load All level chunks
            _avaliableChunks = AssetDatabase.FindAssets($"t:{typeof(LevelChunk).Name}").Select(x => AssetDatabase.GUIDToAssetPath(x)).Select(x => AssetDatabase.LoadAssetAtPath<LevelChunk>(x)).ToList();

            foreach (LevelChunk chunk in _avaliableChunks)
            {
                foreach (string n in chunk.taughtAbilities)
                {
                    if (!tempStringList.Contains(n))
                    {
                        tempStringList.Add(n);
                    }
                }
            }

            abilitiesOrItemsThatHaveTeachingChunks = new List<string>();

            foreach (string name in tempStringList)
            {
                string nameTemp = name;
                nameTemp = nameTemp.Split('_').Last();
                nameTemp = Regex.Replace(nameTemp, "([a-z])([A-Z])", "$1 $2");
                abilitiesOrItemsThatHaveTeachingChunks.Add(nameTemp);
            }
            return abilitiesOrItemsThatHaveTeachingChunks;
        }

        private List<string> GetAllAbilities(GameplayType gameplayType)
        {
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
                .Concat(gameplayType switch
                {
                    GameplayType.ThirdPerson => tpAbilities,
                    GameplayType.FirstPerson => fpAbilities,
                    _ => new List<Ability>(),
                })
                .ToDictionary(x => x.type.GetHashCode());

            if (gameplayType == GameplayType.FirstPerson)
            {
                var items = EtraGUIUtility.FindAllTypes<EtraFPSUsableItemBaseClass>()
                    .Select(x => new ItemAbility(x) as Ability)
                    .ToDictionary(x => x.type.GetHashCode());

                abilities = abilities
                    .Concat(items)
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            List<string> tempStringList = new List<string>();

            foreach (var a in abilities)
            {
                tempStringList.Add(a.Value.ToString());
            }
            return tempStringList;
        }

        public void GenerateAbilitiesAndItems()
        {
            GetAbilitiesWithTeachingChunks();
            GetAllAbilities(TutorialGameplayType);
        }

        Ability GetAbility(int key) =>
            abilities.TryGetValue(key, out Ability ability) ?
                ability :
                null;
        #endregion

        #region GUI
        bool noTeachingChunk = true;
        protected override void RowGUI(RowGUIArgs args)
        {
            bool repaint = Event.current.type == EventType.Repaint;

            var ability = GetAbility(args.item.id);

            Rect abilityRect = args.rowRect
                .ResizeToLeft(EditorGUIUtility.labelWidth);

            Rect itemsRect = args.rowRect
                .BorderLeft(EditorGUIUtility.labelWidth);

            Rect cannotRect = itemsRect
                .SetWidth(_DISABLED_ABILITY_COLUMN_WIDTH * itemsRect.width);

            Rect startsWithRect = itemsRect
                .BorderLeft(_DISABLED_ABILITY_COLUMN_WIDTH * itemsRect.width)
                .BorderRight(_NEW_ABILITY_COLUMN_WIDTH * itemsRect.width);

            Rect taughtToRect = itemsRect
                .ResizeToRight(_NEW_ABILITY_COLUMN_WIDTH * itemsRect.width);

            //Don't allow enabling of those that don't have a teaching chunk to be taught.
            //Have wierd double negatives cause thats how it decided to work.
            if (abilitiesOrItemsThatHaveTeachingChunks.Contains(args.item.displayName))
            {
                noTeachingChunk = false;
            }
            else
            {
                noTeachingChunk = true;
            }


            switch (args.item.id)
            {
                case -1:
                    var bottomSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    EditorGUI.LabelField(abilityRect.BorderBottom(bottomSpace), _ABILITY_HEADER, Styles.HeaderLabel);
                    EditorGUI.LabelField(cannotRect.BorderBottom(bottomSpace), _DISABLED_ABILITY_HEADER, Styles.HeaderLabel);
                    EditorGUI.LabelField(startsWithRect.BorderBottom(bottomSpace), _TAUGHT_ABILITY_HEADER, Styles.HeaderLabel);
                    EditorGUI.LabelField(taughtToRect.BorderBottom(bottomSpace), _NEW_ABILITY_HEADER, Styles.HeaderLabel);

                    EditorStyles.miniButton.CalcMinMaxWidth(new GUIContent(_SELECT_ALL_BUTTON_TEXT), out float buttonWidth, out _);

                    //I hate this
                    Rect cannotSelectAllRect = cannotRect
                        .ResizeToBottom(bottomSpace)
                        .BorderBottom(EditorGUIUtility.standardVerticalSpacing)
                        .ResizeWidthToCenter(buttonWidth);

                    Rect startsWithSelectAllRect = startsWithRect
                        .ResizeToBottom(bottomSpace)
                        .BorderBottom(EditorGUIUtility.standardVerticalSpacing)
                        .ResizeWidthToCenter(buttonWidth);

                    Rect taughtToSelectAllRect = taughtToRect
                        .ResizeToBottom(bottomSpace)
                        .BorderBottom(EditorGUIUtility.standardVerticalSpacing)
                        .ResizeWidthToCenter(buttonWidth);

                    if (GUI.Button(cannotSelectAllRect, _SELECT_ALL_BUTTON_TEXT))
                        SelectAll(Ability.State.Disabled);

                    if (GUI.Button(startsWithSelectAllRect, _SELECT_ALL_BUTTON_TEXT))
                        SelectAll(Ability.State.Taught);

                    if (GUI.Button(taughtToSelectAllRect, _SELECT_ALL_BUTTON_TEXT))
                        SelectAll(Ability.State.New);
                    break;
                default:
                    GUIContent abilityContent = new GUIContent(args.label);

                    
                    if (ability is ItemAbility)
                        abilityContent.image = EditorGUIUtility.IconContent("d_FilterByType").image;

                    EditorGUI.LabelField(abilityRect.BorderLeft(EditorGUIUtility.standardVerticalSpacing), abilityContent);
                    if (EditorGUI.Toggle(cannotRect.ResizeWidthToCenter(EditorGUIUtility.singleLineHeight), ability.state == Ability.State.Disabled))
                        ability.state = Ability.State.Disabled;

                    if (EditorGUI.Toggle(startsWithRect.ResizeWidthToCenter(EditorGUIUtility.singleLineHeight), ability.state == Ability.State.Taught))
                        ability.state = Ability.State.Taught;

                    EditorGUI.BeginDisabledGroup(noTeachingChunk);
                    if (EditorGUI.Toggle(taughtToRect.ResizeWidthToCenter(EditorGUIUtility.singleLineHeight), ability.state == Ability.State.New))
                        ability.state = Ability.State.New;
                    EditorGUI.EndDisabledGroup();
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
                Styles.HeaderLabel.CalcHeight(new GUIContent(_DISABLED_ABILITY_HEADER), totalWidth * _DISABLED_ABILITY_COLUMN_WIDTH),
                Styles.HeaderLabel.CalcHeight(new GUIContent(_TAUGHT_ABILITY_HEADER), totalWidth * _TAUGHT_ABILITY_COLUMN_WIDTH),
                Styles.HeaderLabel.CalcHeight(new GUIContent(_NEW_ABILITY_HEADER), totalWidth * _NEW_ABILITY_COLUMN_WIDTH),
            };

            return Mathf.Max(heights) + //Headers
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; //"Select all" functions
        }
        #endregion

        #region Utility
        public void SelectAll(Ability.State state)
        {

            foreach (var item in abilities)
            {

                if (state == Ability.State.New)
                {
                    //only set State.New to true if possible/ has a teaching chunk
                    if (abilitiesOrItemsThatHaveTeachingChunks.Contains(item.Value.name))
                    {
                        item.Value.state = state;
                    }

                }
                else
                {
                    item.Value.state = state;
                }

            }
                
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
                Taught,
                New,
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