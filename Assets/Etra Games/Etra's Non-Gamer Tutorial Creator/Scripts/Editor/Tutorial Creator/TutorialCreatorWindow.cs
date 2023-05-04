using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;
using System;
using System.Collections.Generic;

using Etra.StarterAssets.Source.Editor;
using Etra.NonGamerTutorialCreator.Level;

using static Etra.StarterAssets.EtraCharacterMainController;

namespace Etra.NonGamerTutorialCreator.TutorialCreator
{
    public class TutorialCreatorWindow : EditorWindow, IHasCustomMenu
    {
        const int PAGE_LIMIT = 4;
        const string PAGE_SESSION_KEY = "etra_nongamer_tutorial_creator_page";
        const float DEFAULT_WINDOW_WIDTH = 500f;
        const float DEFAULT_WINDOW_HEIGHT = 700f;

        int? _page = null;
        int Page
        {
            get
            {
                if (_page == null)
                    _page = SessionState.GetInt(PAGE_SESSION_KEY, 0);
                return _page ?? 0;
            }
            set
            {
                _page = value;
                SessionState.SetInt(PAGE_SESSION_KEY, value);
            }
        }

        TutorialCreatorAbilityTreeView _abilityTreeView;
        TreeViewState _abilityTreeViewState;

        TutorialCreatorLevelBuilder _levelBuilder;

        GameplayType _gameplayType = GameplayType.FirstPerson;
        Model _fpModel = Model.None;
        Model _tpModel = Model.DefaultArmature;

        #region Opening
        [MenuItem("Window/Etra's Tutorial Creator/Tutorial Creator")]
        public static TutorialCreatorWindow OpenWindow()
        {
            TutorialCreatorWindow window = GetWindow<TutorialCreatorWindow>();

            window.titleContent = new GUIContent("Tutorial Creator");

            if (!Preferences.FirstTime)
            {
                Preferences.FirstTime = true;
                window.minSize = new Vector2(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
                window.maxSize = new Vector2(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
            }

            window.minSize = new Vector2(500f, 700f);
            window.maxSize = new Vector2(700f, 1000f);

            window.Page = 0;

            window.Show();

            return window;
        }
        #endregion

        #region Inherited
        public void AddItemsToMenu(GenericMenu menu)
        {

        }
        #endregion

        #region Initialization
        [NonSerialized] bool _init = false;

        private void InitializeIfRequired()
        {
            if (!_init)
                Initialize();
        }

        private void Initialize()
        {
            _abilityTreeViewState = new TreeViewState();
            _abilityTreeView = new TutorialCreatorAbilityTreeView(_abilityTreeViewState);

            _levelBuilder = new TutorialCreatorLevelBuilder();
            _levelBuilder.Initialize();

            _init = true;
        }
        #endregion

        #region GUI
        Vector2 _scroll;

        private void OnGUI()
        {
            InitializeIfRequired();

            using (var scope = new GUILayout.ScrollViewScope(_scroll))
            {
                using (var change = new EditorGUI.ChangeCheckScope())
                {
                    ContentGUI();

                    //if (change.changed)
                    //    SaveStates();
                }

                if (Page != 2)
                    GUILayout.FlexibleSpace();

                _scroll = scope.scrollPosition;
            }

            EtraGUIUtility.HorizontalLineLayout();

            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label($"Page {Page + 1}/{PAGE_LIMIT}", GUILayout.Width(60f));
                    BarGUI();
                }

                //Skip buttons
                using (new GUILayout.VerticalScope(GUILayout.Height(26f)))
                    GUILayout.FlexibleSpace();

                Rect spaceRect = GUILayoutUtility.GetLastRect()
                    .Border(2f, 2f, 0f, 2f);
                Rect previousButtonRect = new Rect(spaceRect)
                    .ResizeToLeft(spaceRect.width / 2f - 2f);
                Rect nextButtonRect = new Rect(spaceRect)
                    .ResizeToRight(spaceRect.width / 2f - 2f);

                using (new EditorGUI.DisabledScope(Page <= 0))
                    if (GUI.Button(previousButtonRect, "<< Previous"))
                        SkipPage(-1);

                switch (Page + 1 < PAGE_LIMIT)
                {
                    case true:
                        if (GUI.Button(nextButtonRect, "Next >>"))
                            SkipPage(1);
                        break;
                    case false:
                        using (new EditorGUI.DisabledScope(Application.isPlaying))
                            if (GUI.Button(nextButtonRect, true ? "Create" : "Modify"))
                                CreateOrModify();
                        break;
                }
            }
        }

        void BarGUI()
        {
            GUILayout.BeginVertical(Styles.Bar, GUILayout.Height(14f));
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            Rect barRect = GUILayoutUtility.GetLastRect();

            EditorGUI.ProgressBar(barRect, (float)Page / (PAGE_LIMIT - 1), string.Empty);
        }

        void ContentGUI()
        {
            switch (Page)
            {
                case 0: //Intro
                    GUILayout.Label("Etra's Non-Gamer Tutorial Creator", Styles.Title);

                    int linkIndex = GUILayout.SelectionGrid(-1, new string[] { "Documentation", "Discord", "Tutorials" }, 3);

                    if (linkIndex != -1)
                    {
                        switch (linkIndex)
                        {
                            case 0:
                                Application.OpenURL("Assets\\Etra Games\\Etra'sStarterAssets\\1-UserAssets\\Etra'sStarterAssets_Documentation.pdf");
                                break;
                            case 1:
                                Application.OpenURL("https://discord.gg/d3AzQDGj4C");
                                break;
                            case 2:
                                Application.OpenURL("https://www.youtube.com/playlist?list=PLvmCfejZtwhO7w1sI0DAMHWqrr6JMABpD");
                                break;
                        }
                    }

                    EditorGUILayout.Space(2f);

                    using (new GUILayout.VerticalScope(Styles.DescriptionBackground))
                        GUILayout.Label("Welcome to the Etra's Starter Assets: Character Creator! \n\nThis setup wizard will allow you to create and modify the character controller, along with its different abilities. \n\nEvery setting is dynamically generated, so your own abilities and items will also show up here. \n\nIf you feel stuck at any point, you can ask for help on our discord server (link above).", Styles.WrappedLabel);

                    break;
                case 1: //Gameplay type selection
                    using (var changeScope = new EditorGUI.ChangeCheckScope())
                    {
                        _gameplayType = EtraGUIUtility.GameplayTypeSelectorLayout(_gameplayType);

                        if (changeScope.changed)
                        {
                            _abilityTreeView.TutorialGameplayType = _gameplayType;
                            _abilityTreeView.GenerateAbilitiesAndItems();
                            _abilityTreeView.Reload();
                        }
                    }

                    EditorGUILayout.Space();

                    switch (_gameplayType)
                    {
                        case GameplayType.FirstPerson:
                            EtraGUIUtility.ModelSelectorLayout("First Person Model", _fpModel, (model) => _fpModel = model, new Model[] { Model.None, Model.Capsule });
                            break;
                        case GameplayType.ThirdPerson:
                            EtraGUIUtility.ModelSelectorLayout("Third Person Model", _tpModel, (model) => _tpModel = model, (Model[])Enum.GetValues(typeof(Model)));
                            break;
                    }

                    break;
                case 2: //Ability selection
                    _abilityTreeView.Width = position.width;
                    DrawTreeView(_abilityTreeView);
                    Repaint();
                    break;
                case 3: //Level builder
                    GUILayout.Label("Level Builder", EtraGUIUtility.Styles.Header);

                    _levelBuilder.OnGUI();
                    break;
            }
        }

        bool Toggle(string label, bool value)
        {
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label(label);
                var backgroundColor = GUI.backgroundColor;

                GUI.backgroundColor = value ? backgroundColor : Color.red;
                if (GUILayout.Toggle(!value, "Off", EditorStyles.miniButtonLeft, GUILayout.Width(40f)) && value)
                    value = false;

                GUI.backgroundColor = value ? Color.green : backgroundColor;
                if (GUILayout.Toggle(value, "On", EditorStyles.miniButtonRight, GUILayout.Width(40f)) && !value)
                    value = true;

                GUI.backgroundColor = backgroundColor;
            }

            return value;
        }

        void DrawTreeView(TreeView tree)
        {
            using (new GUILayout.VerticalScope())
                GUILayout.FlexibleSpace();

            Rect rect = GUILayoutUtility.GetLastRect();
            tree?.OnGUI(rect);
        }
        #endregion

        #region Utility
        void SkipPage(int amount)
        {
            Page += amount;
            Page = Mathf.Clamp(Page, 0, PAGE_LIMIT);
        }
        #endregion

        #region Creation
        public void CreateOrModify()
        {

        }
        #endregion

        private static class Styles
        {
            public static GUIStyle Bar => new GUIStyle()
            {
                margin = new RectOffset(4, 4, 4, 4)
            };

            public static GUIStyle DescriptionBackground => new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(8, 8, 8, 8),
            };

            public static GUIStyle Title = new GUIStyle(EditorStyles.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(4, 4, 4, 0),
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 120f,
                wordWrap = true,
                normal = new GUIStyleState()
                {
                    textColor = Color.white,
                    background = EtraGUIUtility.GenerateColorTexture(new Color(232f/255f, 65f/255f, 24f/255f)),
                }
            };

            public static GUIStyle WrappedLabel = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                fontSize = 13,
            };

            public static GUIStyle GameplayTypeLabel = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.LowerCenter,
                fontSize = 14,
            };
        }

        private static class Preferences
        {
            private const string _KEEP_OPENED_KEY = "etra_nongamer_tutorial_creator_keep_opened";
            private static bool? _keepOpened = null;
            public static bool KeepOpened
            {
                get
                {
                    if (_keepOpened == null)
                        _keepOpened = EditorPrefs.GetBool(_KEEP_OPENED_KEY, false);

                    return _keepOpened ?? false;
                }
                set
                {
                    _keepOpened = value;
                    EditorPrefs.SetBool(_KEEP_OPENED_KEY, value);
                }
            }

            private const string _FIRST_TIME = "etra_nongamer_tutorial_creator_first";
            private static bool? _firstTime = null;
            public static bool FirstTime
            {
                get
                {
                    if (_firstTime == null)
                        _firstTime = EditorPrefs.GetBool(_FIRST_TIME, false);

                    return _firstTime ?? false;
                }
                set
                {
                    _firstTime = value;
                    EditorPrefs.SetBool(_FIRST_TIME, value);
                }
            }
        }
    }
}