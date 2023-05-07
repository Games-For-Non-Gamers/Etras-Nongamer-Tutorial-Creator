using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Etra.StarterAssets.Source.Editor;
using Etra.NonGamerTutorialCreator.Level;

namespace Etra.NonGamerTutorialCreator.TutorialCreator
{
    public class TutorialCreatorLevelBuilder
    {
        const float _IMAGE_WIDTH = 64f;

        /// <summary>List of abilities that the player already knows</summary>
        public List<Type> TaughtAbilities { get; set; } = new List<Type>();
        /// <summary>List of abilities that the player is going to learn</summary>
        public List<Type> NewAbilities { get; set; } = new List<Type>();

        #region Initialize
        [NonSerialized] bool _init = false;

        public void InitializeIfNeeded()
        {
            if (!_init)
                Initialize();
        }

        public void Initialize()
        {
            reorderableList = new ReorderableList(_chunks, typeof(LevelChunk));
            reorderableList.drawHeaderCallback += ReorderableList_DrawHeaderCallback;
            reorderableList.drawElementCallback += ReorderableList_DrawElementCallback;
            reorderableList.elementHeightCallback += ReorderableList_ElementHeightCallback;
            reorderableList.onAddDropdownCallback += ReorderableList_OnAddDropdownCallback;
            reorderableList.onCanRemoveCallback += ReorderableList_OnCanRemoveCallback;
            reorderableList.onCanAddCallback += ReorderableList_OnCanAddCallback;

            Reload();

            _init = true;
        }
        #endregion

        #region Reloading
        private bool _autoReload = true;
        /// <summary>If true, the level builder will reload it's data automatically whenever something changes (eg. an asset gets created or deleted)</summary>
        public bool AutoReload
        {
            get => _autoReload;
            set
            {
                _autoReload = value;
                switch (value)
                {
                    case true:
                        LevelChunk.OnAssetValidation += _ => Reload();
                        break;
                    case false:
                        LevelChunk.OnAssetValidation -= _ => Reload();
                        break;
                }
            }
        }

        /// <summary>Reloads level builder's data. If <see cref="AutoReload"/> is set to true, this method will get called automatically</summary>
        public void Reload()
        {
            RebuildAvaliableChunksCache();
            CheckChunksList();
        }
        #endregion

        #region GUI
        ReorderableList reorderableList;

        /// <summary>Method for drawing GUI</summary>
        public void OnGUI()
        {
            InitializeIfNeeded();

            using (new GUILayout.VerticalScope(Styles.List))
                reorderableList.DoLayoutList();
        }

        private void ReorderableList_DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, "Chunks");
        }

        private void ReorderableList_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            LevelChunk chunk = _chunks[index];

            Rect textRect = rect
                .ResizeWHeightToCenter(34f);

            Rect labelRect = new Rect(textRect)
                .ResizeToTop(18f);

            Rect abilitiesRect = new Rect(textRect)
                .ResizeToBottom(12f)
                .MoveY(-4f);

            Rect imageRect = new Rect(rect)
                .ResizeToRight(_IMAGE_WIDTH);

            GUI.Label(labelRect, chunk.chunkName, Styles.ChunkName);
            GUI.Label(abilitiesRect, string.Join(", ", chunk.abilitiesToTeach.Select(x => x.Split('.').Last())), Styles.Abilities);
            GUI.Label(imageRect, GUIContent.none, EditorStyles.helpBox);

            if (chunk.icon != null)
                GUI.DrawTexture(imageRect.Border(2f), chunk.icon.texture);
        }

        private float ReorderableList_ElementHeightCallback(int index)
        {
            LevelChunk chunk = _chunks[index];

            if (chunk.icon == null)
                return _IMAGE_WIDTH;

            return _IMAGE_WIDTH * chunk.icon.texture.height / chunk.icon.texture.width;
        }

        private void ReorderableList_OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();

            foreach (var item in AvaliableChunks)
            {
                menu.AddItem(new GUIContent(string.IsNullOrWhiteSpace(item.chunkName) ? item.name : item.chunkName), false, () =>
                {
                    _chunks.Insert(0, item);
                });
            }

            menu.ShowAsContext();
        }

        private bool ReorderableList_OnCanRemoveCallback(ReorderableList list)
        {
            var selectedChunk = _chunks[list.index];
            var type = selectedChunk.GetType();

            int duplicatesCount = _chunks
                .Select(x => x.GetType())
                .Where(x => x == type)
                .Count();

            return !selectedChunk.required ||
                duplicatesCount > 1;
        }

        private bool ReorderableList_OnCanAddCallback(ReorderableList list)
        {
            return AvaliableChunks
                .Where(x => !x.useSingle || !_chunks.Contains(x))
                .Any();
        }
        #endregion

        #region Chunks
        private List<LevelChunk> _avaliableChunks = null;
        /// <summary>A list of every Level Chunk asset in the project</summary>
        public List<LevelChunk> AvaliableChunks
        {
            get
            {
                if (_avaliableChunks == null)
                    RebuildAvaliableChunksCache();

                return _avaliableChunks;
            }
        }

        List<LevelChunk> _chunks = new List<LevelChunk>();

        /// <summary>Rebuilds the cache of <see cref="AvaliableChunks"/></summary>
        public void RebuildAvaliableChunksCache()
        {
            var taughtAbilitiesPaths = TaughtAbilities
                .Select(x => x.FullName)
                .ToList();

            var newAbilitiesPaths = NewAbilities
                .Select(x => x.FullName)
                .ToList();

            _avaliableChunks = AssetDatabase.FindAssets($"t:{typeof(LevelChunk).Name}")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<LevelChunk>(x))
                .Where(x => !x.taughtAbilities.Except(taughtAbilitiesPaths).Any()) //If all chunks taught abilities have been selected
                .Where(x => !x.abilitiesToTeach.Except(newAbilitiesPaths).Except(taughtAbilitiesPaths).Any()) //If all chunks new abilities have been selected as new or taught
                .ToList();
        }

        public void CheckChunksList()
        {
            _chunks = _chunks
                .Intersect(AvaliableChunks)
                .ToList();

            var requiredChunks = AvaliableChunks
                .Where(x => x.required);

            _chunks.AddRange(requiredChunks.Except(_chunks));

            reorderableList.list = _chunks;
        }
        #endregion

        #region Creation
        public void CreateOrModify()
        {

        }
        #endregion

        private static class Styles
        {
            public static GUIStyle List => new GUIStyle()
            {
                margin = new RectOffset(4, 4, 4, 4),
            };

            public static GUIStyle ChunkName => new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
            };

            public static GUIStyle Abilities => new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
            };
        }
    }
}