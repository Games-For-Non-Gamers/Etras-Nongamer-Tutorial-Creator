using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Etra.StarterAssets.Source.Editor;
using Etra.NonGamerTutorialCreator.Level;

using UObject = UnityEngine.Object;

namespace Etra.NonGamerTutorialCreator.TutorialCreator
{
    public class TutorialCreatorLevelBuilder : IDisposable
    {
        const float _IMAGE_WIDTH = 64f;

        /// <summary>List of abilities that the player already knows</summary>
        public List<Type> TestedAbilities { get; set; } = new List<Type>();
        /// <summary>List of abilities that the player is going to learn</summary>
        public List<Type> TaughtAbilities { get; set; } = new List<Type>();

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
            reorderableList.onRemoveCallback += ReorderableList_OnRemoveCallback;
            reorderableList.onReorderCallbackWithDetails += ReorderableList_OnReorderCallbackWithDetails;
            reorderableList.onSelectCallback += ReorderableList_OnSelectCallback;

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
            VerifyTargetLevelChunks();
            CheckForTarget();
            Target.ResetAllChunksPositions();
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

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Debug", EditorStyles.boldLabel);
                if (GUILayout.Button("Verify Target Chunks"))
                    VerifyTargetLevelChunks();

                if (Target != null && GUILayout.Button("Reset Chunks Positions"))
                    Target.ResetAllChunksPositions();
            }
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
            GUI.Label(abilitiesRect, string.Join(", ", chunk.taughtAbilities.Select(x => x.Split('.').Last())), Styles.Abilities);
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
                    
                    if (Target != null)
                    {
                        Target.chunks.Add(CreateChunkObject(item));
                        Target.ResetAllChunksPositions();
                    }
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

        private void ReorderableList_OnRemoveCallback(ReorderableList list)
        {
            _chunks.RemoveAt(list.index);

            if (Target != null)
            {
                var targetIndex = Target.chunks.Count - list.index - 1;

                var chunkObject = Target.chunks[targetIndex];
                Vector3 chunkTotalOffset = chunkObject.endConnectionPoint - chunkObject.startConnectionPoint;

                Target.chunks.RemoveAt(targetIndex);
                UObject.DestroyImmediate(chunkObject.gameObject);

                Target.ResetAllChunksPositions();
            }
        }

        private void ReorderableList_OnReorderCallbackWithDetails(ReorderableList list, int oldIndex, int newIndex)
        {
            if (Target != null)
            {
                Target.chunks.Reverse();
                var targetOldIndex = Target.chunks.Count - oldIndex - 1;
                var targetNewIndex = Target.chunks.Count - newIndex - 1;

                var selectedChunk = Target.chunks[targetOldIndex];
                var toReplaceChunk = Target.chunks[targetNewIndex];
                Target.chunks[targetOldIndex] = toReplaceChunk;
                Target.chunks[targetNewIndex] = selectedChunk;
                Target.chunks.Reverse();
                Target.ResetAllChunksPositions();

                //Rearrange in the hierarchy if both chunks are children of the target
                if (selectedChunk.transform.parent == Target.transform &&
                    toReplaceChunk.transform.parent == Target.transform)
                {
                    var selectedChunkSiblingIndex = selectedChunk.transform.GetSiblingIndex();
                    var toReplaceChunkSiblingIndex = toReplaceChunk.transform.GetSiblingIndex();

                    selectedChunk.transform.SetSiblingIndex(toReplaceChunkSiblingIndex);
                    toReplaceChunk.transform.SetSiblingIndex(selectedChunkSiblingIndex);
                }
                Target.ResetAllChunksPositions();
            }

        }

        private void ReorderableList_OnSelectCallback(ReorderableList list)
        {
            if (Target != null && !Target.isPreview)
            {
                var targetIndex = Target.chunks.Count - list.index - 1;
                Selection.objects = new UObject[] { Target.chunks[targetIndex].gameObject };
            }
        }
        #endregion

        #region Target
        public LevelController Target { get; set; }

        public void CheckForTarget()
        {
            if (Target != null)
                return;

            var levelController = UObject.FindObjectOfType<LevelController>();

            if (levelController == null)
                levelController = CreateNewLevelController(true, "Level Controller (Temp)");

            if (Target == levelController) return;

            Target = levelController;
            VerifyTargetLevelChunks();
            Target.ResetAllChunksPositions();
        }

        public LevelController CreateNewLevelController(bool isPreview = false, string name = "Level Controller")
        {
            GameObject go = new GameObject(name);

            var levelController = go.AddComponent<LevelController>();
            levelController.isPreview = isPreview;

            return levelController;
        }

        public void VerifyTargetLevelChunks()
        {
            if (Target == null)
                return;

            if (_chunks.Count != Target.chunks.Count)
            {
                var difference = Mathf.Abs(_chunks.Count - Target.chunks.Count);

                switch (_chunks.Count > Target.chunks.Count)
                {
                    //Target has too little
                    case true:
                        for (int i = 0; i < difference; i++)
                            Target.chunks.Add(null);
                        break;
                    //Target has too much
                    case false:
                        for (int i = 0; i < difference; i++)
                        {
                            var index = Target.chunks.Count - 1;
                            UObject.DestroyImmediate(Target.chunks[index].gameObject);
                            Target.chunks.RemoveAt(index);
                        }
                        break;
                }
            }

            for (int i = 0; i < _chunks.Count; i++)
            {
                //Ignore if it's using the correct prefab
                if (_chunks[i].chunkObject == null && Target.chunks[i] == null)
                    continue;

                if (Target.chunks[i] != null && 
                    PrefabUtility.GetCorrespondingObjectFromSource(Target.chunks[i]) == _chunks[i].chunkObject)
                    continue;

                //Remove fake prefab
                if (Target.chunks[i] != null)
                    UObject.DestroyImmediate(Target.chunks[i].gameObject);

                //Create
                var prefab = CreateChunkObject(_chunks[i]);
                Target.chunks[i] = prefab.GetComponent<LevelChunkObject>();
            }
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
            var testedAbilitiesPaths = TestedAbilities
                .Select(x => x.FullName)
                .ToList();

            var taughtAbilitiesPaths = TaughtAbilities
                .Select(x => x.FullName)
                .ToList();

            _avaliableChunks = AssetDatabase.FindAssets($"t:{typeof(LevelChunk).Name}")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<LevelChunk>(x))
                .Where(x => !x.testedAbilities.Except(testedAbilitiesPaths).Except(taughtAbilitiesPaths).Any()) //If all chunks taught abilities have been selected
                .Where(x => !x.taughtAbilities.Except(taughtAbilitiesPaths).Any()) //If all chunks new abilities have been selected as new or taught
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

            //put recommended here

            var recommendedChunks = AvaliableChunks
            .Where(x => x.recommended);

            _chunks.AddRange(recommendedChunks.Except(_chunks));

            _chunks = _chunks.OrderByDescending(chunk => chunk.orderPriority).ToList();


            reorderableList.list = _chunks;
           
        }

        public LevelChunkObject CreateChunkObject(LevelChunk chunk)
        {
            var prefab = (GameObject)PrefabUtility.InstantiatePrefab(chunk.chunkObject.gameObject, Target.transform);
            prefab.name = chunk.chunkName;

            return prefab.GetComponent<LevelChunkObject>();
        }
        #endregion

        #region Creation
        public void CreateOrModify()
        {
            if (Target == null)
                return;

            if (Target.isPreview)
            {
                Target.name = "Level Controller";
                Target.isPreview = false;
            }


        }

        public void Dispose()
        {
            if (Target != null &&
                Target.isPreview == true)
            {
                UObject.DestroyImmediate(Target.gameObject);
            }
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