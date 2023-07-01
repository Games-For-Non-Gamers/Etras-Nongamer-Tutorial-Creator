using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;
using System;
using System.Collections.Generic;

using Etra.StarterAssets.Source.Editor;
using Etra.NonGamerTutorialCreator.Level;

using static Etra.StarterAssets.EtraCharacterMainController;
using System.Linq;
using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Items;
using static Etra.StarterAssets.Items.EtraFPSUsableItemManager;
using static UnityEditor.Progress;
using Etra.StarterAssets.Interactables;

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
            menu.AddItem(new GUIContent("Prefs/Keep Opened"), Preferences.KeepOpened, () => Preferences.KeepOpened = !Preferences.KeepOpened);
        }

        private void OnDestroy()
        {
            if (_levelBuilder != null)
            {
                _levelBuilder.Dispose();
            }
            
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

            OnChangePage();

            _init = true;
        }
        #endregion
        
        #region GUI
        Vector2 _scroll;
        private void OnGUI()
        {
            if (Preferences.LevelCreated)
            {
                LevelController temp = FindObjectOfType<LevelController>();
                if (temp != null)
                {
                }
                else
                {
                    InitializeIfRequired();
                }
            }
            else
            {
                InitializeIfRequired();
            }

            using (var scope = new GUILayout.ScrollViewScope(_scroll))
            {
                using (var change = new EditorGUI.ChangeCheckScope())
                {
                    if (Preferences.LevelCreated)
                    {
                        LevelController temp = FindObjectOfType<LevelController>();
                        if (temp != null)
                        {
                            Page = PAGE_LIMIT;
                        }
                    }

                    ContentGUI();

                    //if (change.changed)
                    //    SaveStates();
                }

                if (Page != 2)
                    GUILayout.FlexibleSpace();

                _scroll = scope.scrollPosition;
            }

            EtraGUIUtility.HorizontalLineLayout();

            //If on reset page don't display the pages bar
            if (Page == PAGE_LIMIT)
            {
                return; //e
            }

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
                            {
                                CreateOrModify();
                                Preferences.LevelCreated = true;
                            }
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

                case PAGE_LIMIT: //Reset Page


                    GUILayout.Label("Etra's Non-Gamer Tutorial Creator", Styles.Title);

                    int linkIndex1 = GUILayout.SelectionGrid(-1, new string[] { "Documentation", "Discord", "Tutorials" }, 3);

                    if (linkIndex1 != -1)
                    {
                        switch (linkIndex1)
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

                    GUILayout.Label("Reset Level Builder", EtraGUIUtility.Styles.Header);

                    EditorGUILayout.Space(2f);

                    using (new GUILayout.VerticalScope(Styles.DescriptionBackground))
                        GUILayout.Label("To reuse the Level Builder you must delete the current level and character. \n\nIf you want to modify or add to the current level and chararacter you can... \n\n-Add, remove, or modify level chunks from the Level Controller Object. \n-Go to (Window->StarterAssets->Etra's Character Creator) to modify the current character.", Styles.WrappedLabel);

                    GUILayout.Space(10);
                    // Create a GUIStyle for the button
                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                    buttonStyle.fixedHeight = 100;
                    buttonStyle.fontSize = 18;

                    // Calculate the position to center the button
                    float centerX = position.width / 2 - buttonStyle.fixedWidth / 2;
                    float centerY = position.height / 2 - buttonStyle.fixedHeight / 2;
                    Rect buttonRect = new Rect(centerX+10, centerY, position.width, buttonStyle.fixedHeight);

                    // Draw the button using the GUIStyle
                    if (GUILayout.Button("Delete The Level and Character", buttonStyle, GUILayout.Width(buttonRect.width -10), GUILayout.Height(buttonStyle.fixedHeight)))
                    {
                        Preferences.LevelCreated = false;
                        Page = 0;
                        if (GameObject.FindGameObjectWithTag("Player"))
                        {
                            DestroyImmediate(GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject);
                        }
                        
                    }





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
            var previousPage = Page;
            Page += amount;
            Page = Mathf.Clamp(Page, 0, PAGE_LIMIT);

            if (Page != previousPage)
                OnChangePage();
        }

        void OnChangePage()
        {
            switch (Page)
            {
                case 3:
                    _levelBuilder.TestedAbilities = _abilityTreeView.GetTaughtAbilities();
                    _levelBuilder.TaughtAbilities = _abilityTreeView.GetNewAbilities();
                    _levelBuilder.Reload();
                    _levelBuilder.CheckForTarget();
                    break;
            }
        }
        #endregion

        #region Creation
        public void CreateOrModify()
        {

            _levelBuilder.CreateOrModify();

            //Get player from this
            EtraCharacterMainController character = EtraCharacterCreatorCreateOrModify.CreateOrModify(_gameplayType, _fpModel, _tpModel, _abilityTreeView.GetTaughtAbilities(), _abilityTreeView.GetNewAbilities());

            //disable abilities with _abilityTreeView.GetNewAbilities()

            //Combine the Ability lists and get their names
            List<string> abilitiesToTeach = new List<string>();
            foreach (Type abil in _abilityTreeView.GetNewAbilities())
            {
                abilitiesToTeach.Add(abil.Name);
            }

            //Disable all abilities
            character.disableAllActiveAbilitiesAndSubAblities();

            //Activate what is not in the learn array
            foreach (EtraAbilityBaseClass ability in character.etraAbilityManager.characterAbilityUpdateOrder)
            {
                if (!abilitiesToTeach.Contains(ability.GetType().Name))
                {
                    ability.abilityEnabled = true;

                    for (int i = 0; i < ability.subAbilityUnlocks.Length; i++)
                    {
                        ability.subAbilityUnlocks[i].subAbilityEnabled = true;
                    }
                    ability.abilityCheckSubAbilityUnlocks();
                }
            }

            //Also activate the camera movement for a 3pa bug. If locked the x and y look should fix things anyway.
            if (character.etraAbilityManager.GetComponent<ABILITY_CameraMovement>())
            {
                character.etraAbilityManager.GetComponent<ABILITY_CameraMovement>().abilityEnabled = true;
            }



            //Remove taught weapons
            if (character.GetComponentInChildren<EtraFPSUsableItemManager>())
            {
                EtraFPSUsableItemManager itemManager = character.GetComponentInChildren<EtraFPSUsableItemManager>();

                for (int i = 0; i < itemManager.usableItems.Length; i++)
                {
                    if (abilitiesToTeach.Contains(itemManager.usableItems[i].script.GetType().Name))
                    {
                        DestroyImmediate(itemManager.GetComponent(itemManager.usableItems[i].script.GetType()));
                        itemManager.usableItems[i].script = null;
                        itemManager.updateUsableItemsArray();
                    }
                }
            }

            if (_levelBuilder.Target.chunks.Count ==1 || _levelBuilder.Target.chunks[_levelBuilder.Target.chunks.Count - 1].name != "End Chunk")
            {
                _levelBuilder.Target.chunks[_levelBuilder.Target.chunks.Count - 1].makePlayerSpawn();
            }

            if (!Preferences.KeepOpened)
                Close();

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


            private const string _LEVEL_CREATED = "etra_nongamer_tutorial_creator_created";
            private static bool? _levelCreated = null;
            public static bool LevelCreated
            {
                get
                {
                    if (_levelCreated == null)
                        _levelCreated = EditorPrefs.GetBool(_LEVEL_CREATED, false);

                    return _levelCreated ?? false;
                }
                set
                {
                    _levelCreated = value;
                    EditorPrefs.SetBool(_LEVEL_CREATED, value);
                }
            }
        }
    }
}