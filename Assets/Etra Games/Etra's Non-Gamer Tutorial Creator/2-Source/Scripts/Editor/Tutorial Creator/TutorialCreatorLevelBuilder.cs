using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Etra.StarterAssets.Source.Editor;
using Etra.NonGamerTutorialCreator.Level;

using UObject = UnityEngine.Object;
using Etra.StarterAssets.Source;
using Codice.Client.BaseCommands;

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
            LoadRecommendedChunksList();
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
                GUILayout.Label("Options", EditorStyles.boldLabel);
                if (Target != null && GUILayout.Button("Return to recommended layout"))
                    LoadRecommendedChunksList();
                if (Target != null && GUILayout.Button("Add ALL possible chunks"))
                    LoadAllPossibleChunks();
            }
            /*
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Debug", EditorStyles.boldLabel);
                if (GUILayout.Button("Verify Target Chunks"))
                    VerifyTargetLevelChunks();
                if (Target != null && GUILayout.Button("Reset Chunks Positions"))
                    Target.ResetAllChunksPositions();
            }
            */
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
                    //_chunks is level builder list
                    _chunks.Insert(1, item); // inser below star

                    //chunks is actual chunk array builder list
                    if (Target != null)
                    {
                        Target.chunks.Insert(1, (CreateChunkObject(item)));
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
            var targetIndex = Target.chunks.Count - list.index - 1;
            _chunks.RemoveAt(list.index);

            if (Target != null)
            {
                
                var chunkObject = Target.chunks[list.index];
                Vector3 chunkTotalOffset = chunkObject.endConnectionPoint - chunkObject.startConnectionPoint;

                Target.chunks.RemoveAt(list.index);
                UObject.DestroyImmediate(chunkObject.gameObject);

                Target.ResetAllChunksPositions();
            }
        }

        private void ReorderableList_OnReorderCallbackWithDetails(ReorderableList list, int oldIndex, int newIndex)
        {
         //   Debug.Log("Old " + oldIndex + "\nNew " + newIndex);
            if (Target != null)
            {
                var targetChunks = Target.chunks;

                // Swap elements within the list
                var selectedChunk = targetChunks[oldIndex];
                targetChunks.RemoveAt(oldIndex);
                targetChunks.Insert(newIndex, selectedChunk);

                // Reset the positions of all chunks
                Target.ResetAllChunksPositions();

                // Rearrange in the hierarchy if both chunks are children of the target
                if (selectedChunk.transform.parent == Target.transform)
                {
                    // Get the sibling index of the selected chunk
                    var selectedChunkSiblingIndex = selectedChunk.transform.GetSiblingIndex();

                    // Get the starting index for shifting other chunks
                    var shiftStartIndex = oldIndex < newIndex ? oldIndex : newIndex;

                    // Shift the sibling indices of the chunks below the moved chunk
                    for (int i = shiftStartIndex; i < targetChunks.Count; i++)
                    {
                        if (i != selectedChunkSiblingIndex)
                        {
                            // Determine the new sibling index for each chunk
                            int newSiblingIndex = i < newIndex ? i + 1 : i - 1;
                            targetChunks[i].transform.SetSiblingIndex(newSiblingIndex);
                        }
                    }
                }

                // Reset the positions of all chunks again after rearranging hierarchy
                Target.ResetAllChunksPositions();
            }

        }

        private void ReorderableList_OnSelectCallback(ReorderableList list)
        {
            if (Target != null && !Target.isPreview)
            {
                Selection.objects = new UObject[] { Target.chunks[list.index].gameObject };
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

        GameObject manager;
        public LevelController CreateNewLevelController(bool isPreview = false, string name = "Level Controller")
        {
            LevelController levelController;
            if (GameObject.Find("NonGamerTutorialManager"))
            {
                manager = GameObject.Find("NonGamerTutorialManager");
            }
            else
            {
                manager = EtrasResourceGrabbingFunctions.addPrefabFromResourcesByName("NonGamerTutorialManager");
            }

            if (GameObject.Find("Level Controller"))
            {
                levelController = GameObject.Find("Level Controller").gameObject.GetComponent<LevelController>();
            }
            else
            {
                GameObject controller = new GameObject("Level Controller");
                controller.transform.parent = manager.transform;
                controller.transform.SetAsFirstSibling();
                controller.transform.localPosition = Vector3.zero;
                controller.transform.localScale = Vector3.one;
                controller.AddComponent<LevelController>();
                levelController = controller.GetComponent<LevelController>();
            }

            levelController.chunks = new List<LevelChunkObject>();
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
        /// 
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

            //Set the temporary reccomended state for teaching priority below
            foreach (LevelChunk lc in _avaliableChunks)
            {
                if (lc.recommended)
                {
                    lc.tempRecommended = true;
                }
                else
                {
                    lc.tempRecommended = false;
                }
            }

        }

        public void LoadRecommendedChunksList()
        {
            InitializeChunks();
            ProcessRequiredChunks();
            ProcessTaughtAbilities();
            ApplyRecommendations();
            SortChunks();
            UpdateUI();
            ResetTargetChunks();
            VerifyTargetLevelChunks();
            if (Target != null)
            {
                Target.ResetAllChunksPositions();
            }

        }

        private void InitializeChunks()
        {
            _chunks = new List<LevelChunk>();
            _chunks = AvaliableChunks.Intersect(_chunks).ToList();
        }

        private void ProcessRequiredChunks()
        {
            var requiredChunks = AvaliableChunks.Where(x => x.required);
            _chunks.AddRange(requiredChunks.Except(_chunks));
        }

        private void ProcessTaughtAbilities()
        {
            var taughtAbilitiesPaths = TaughtAbilities.Select(x => x.FullName).ToList();

            foreach (string taughtAbility in taughtAbilitiesPaths)
            {
                if (taughtAbility != "Etra.StarterAssets.Abilities.ABILITY_CameraMovement")
                {
                    ProcessTaughtAbilityChunks(taughtAbility);
                }
                else
                {
                    ProcessCameraMovementChunks();
                }
            }
        }

        private void ProcessTaughtAbilityChunks(string taughtAbility)
        {
            List<LevelChunk> chunksToCompare = AvaliableChunks
                .Where(l => l.taughtAbilities.Contains(taughtAbility))
                .ToList();

            if (chunksToCompare.Count > 1)
            {
                chunksToCompare.Sort((a, b) => b.teachingPriority.CompareTo(a.teachingPriority));
                chunksToCompare.Skip(1).ToList().ForEach(chunk =>
                {
                    chunk.tempRecommended = false;
                });
            }
        }

        private void ProcessCameraMovementChunks()
        {
            List<LevelChunk> chunksToCompare = AvaliableChunks
                .Where(l => l.taughtAbilities.Contains("Etra.StarterAssets.Abilities.ABILITY_CameraMovement"))
                .ToList();

            ProcessCameraAxisChunks(chunksToCompare, "LookX");
            ProcessCameraAxisChunks(chunksToCompare, "LookY");
        }

        private void ProcessCameraAxisChunks(List<LevelChunk> chunksToCompare, string axis)
        {
            List<LevelChunk> axisChunks = chunksToCompare.Where(l => l.name.Contains(axis)).ToList();

            if (axisChunks.Count > 1)
            {
                axisChunks.Sort((a, b) => b.teachingPriority.CompareTo(a.teachingPriority));

                axisChunks.Skip(1).ToList().ForEach(chunk =>
                {
                    chunk.tempRecommended = false;
                });
            }
        }

        private void ApplyRecommendations()
        {
            var recommendedChunks = AvaliableChunks.Where(x => x.tempRecommended);
            _chunks.AddRange(recommendedChunks.Except(_chunks));
        }

        private void SortChunks()
        {
            _chunks = _chunks.OrderByDescending(chunk => chunk.orderPriority).ToList();
        }

        private void UpdateUI()
        {
            reorderableList.list = _chunks;
            VerifyTargetLevelChunks();
        }

        private void ResetTargetChunks()
        {
            if (Target != null)
            {
                Target.ResetAllChunksPositions();
            }
        }


        public void LoadAllPossibleChunks()
        {
            _chunks = new List<LevelChunk>();
            _chunks = _chunks
    .Intersect(AvaliableChunks)
    .ToList();

            var requiredChunks = AvaliableChunks
                .Where(x => x.required);

            _chunks.AddRange(AvaliableChunks);

            //put recommended here
            /*
            var recommendedChunks = AvaliableChunks
            .Where(x => x.fpsRecommended);

            _chunks.AddRange(recommendedChunks.Except(_chunks));
            */
            _chunks = _chunks.OrderByDescending(chunk => chunk.orderPriority).ToList();


            reorderableList.list = _chunks;

            VerifyTargetLevelChunks();
            Target.ResetAllChunksPositions();

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