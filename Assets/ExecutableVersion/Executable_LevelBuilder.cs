using Etra.NonGamerTutorialCreator.Level;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class Executable_LevelBuilder : MonoBehaviour
{


    public List<LevelChunk> allLevelChunks;
    public List<LevelChunk> possibleLevelChunks = new List<LevelChunk>();

    public List<LevelChunk> reccomendedLevelChunks;

    private ExecutableNewLevelDataHolder dataHolder;

    public bool resetBuilder = true;


    private void OnEnable()
    {
        if (resetBuilder)
        {

        }
        else if (possibleLevelChunks.Count > 0)
        {
            return;
        }

        dataHolder = FindObjectOfType<ExecutableNewLevelDataHolder>();
        LoadChunks(true);
    }

    public string GenerateName(string fileName)
    {
        string toReturn = fileName;
        toReturn = toReturn.Split('_').Last();

        toReturn = Regex.Replace(toReturn, "([a-z])([A-Z])", "$1 $2");
        return toReturn;
    }

    public bool applyReccomendationsState = true;
    public void LoadChunks(bool applyReccomendations )
    {
        applyReccomendationsState = applyReccomendations;
        possibleLevelChunks = new List<LevelChunk>();

        List<string> taughtAbilities = dataHolder.abilitiesInLevel.Except(dataHolder.abilitiesToActivate).ToList();
        //type to english for all tested abilities. Put in same filter.

        foreach (LevelChunk lc in allLevelChunks)
        {
            if (lc.testedAbilities.Any(chunkAbility => dataHolder.abilitiesInLevel.Contains(GenerateName(chunkAbility))))//May need to rework to differentiate for checking for full word vs part
            {
                if (!possibleLevelChunks.Contains(lc))
                {
                    possibleLevelChunks.Add(lc);
                }
            }

            if (lc.taughtAbilities.Any(chunkAbility => taughtAbilities.Contains(GenerateName(chunkAbility)))) //May need to rework to differentiate for checking for full word vs part
            {
                if (!possibleLevelChunks.Contains(lc))
                {
                    possibleLevelChunks.Add(lc);
                }
            }

        }


        if (applyReccomendations)
        {
            //Set the temporary reccomended state for teaching priority below
            foreach (LevelChunk lc in possibleLevelChunks)
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
        else
        {
            foreach (LevelChunk lc in possibleLevelChunks)
            {
                lc.tempRecommended = true;
            }
        }


        LoadRecommendedChunksList();
    }


    public void LoadRecommendedChunksList()
    {
        reccomendedLevelChunks = new List<LevelChunk>();
        ProcessRequiredChunks();
        ProcessTaughtAbilities();
        ApplyRecommendations();
        SortChunks();
        CreateUiChunks();
    }


    private void ProcessRequiredChunks()
    {
        var requiredChunks = allLevelChunks.Where(x => x.required);
        reccomendedLevelChunks.AddRange(requiredChunks.Except(reccomendedLevelChunks));
    }

    private void ProcessTaughtAbilities()
    {
        List<string> taughtAbilities = dataHolder.abilitiesInLevel.Except(dataHolder.abilitiesToActivate).ToList();
        foreach (string taughtAbility in taughtAbilities)
        {
            if (taughtAbility != "Camera Movement")
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
        List<LevelChunk> chunksToCompare = possibleLevelChunks
            .Where(l => l.taughtAbilities.Contains(taughtAbility))
            .ToList();

        if (chunksToCompare.Count > 1)
        {
            chunksToCompare.Sort((a, b) => b.teachingPriority.CompareTo(a.teachingPriority));
            chunksToCompare.Skip(1).ToList().ForEach(chunk =>
            {
                if (applyReccomendationsState)
                {
                    chunk.tempRecommended = false;
                }
            });
        }
    }

    private void ProcessCameraMovementChunks()
    {
        List<LevelChunk> chunksToCompare = possibleLevelChunks
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
        var recommendedChunks = possibleLevelChunks.Where(x => x.tempRecommended);
        reccomendedLevelChunks.AddRange(recommendedChunks.Except(reccomendedLevelChunks));
    }

    private void SortChunks()
    {
        reccomendedLevelChunks = reccomendedLevelChunks.OrderByDescending(chunk => chunk.orderPriority).ToList();
    }



    public GameObject prefabToDuplicate;
    public GameObject entryParent;
    public bool abilityChoicesChanged;
    private void CreateUiChunks()
    {
        int childCount = entryParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = entryParent.transform.GetChild(i);
            Destroy(child.gameObject);
        }


        foreach (LevelChunk lc in reccomendedLevelChunks)
        {
            CreateUiChunk(lc);
        }
    }

    void CreateUiChunk(LevelChunk lc)
    {
        GameObject entry = Instantiate(prefabToDuplicate, entryParent.transform, false);
        entry.name = lc.name;
        DragController script = entry.GetComponent<DragController>();
        script.text.text = lc.chunkName;
        script.icon.sprite = lc.icon;
        if (lc.required)
        {
            script.trashButton.SetActive(false);
        }

    }

    GameObject savedUndoBackup;
    int undoIndex;
    public void SaveGameObjectForUndo(GameObject deletedBlock, int index)
    {
        if (savedUndoBackup != null)
        {
            Destroy(savedUndoBackup); savedUndoBackup = null;
        }

        savedUndoBackup = deletedBlock;
        savedUndoBackup.SetActive(false);
        UpdateListDataFromChildrenPosition();
    }
    

    public void UndoDelete()
    {
        if (savedUndoBackup!= null)
        {
            savedUndoBackup.SetActive(true);
            savedUndoBackup = null;
        }

    }

    public void UpdateListDataFromChildrenPosition()
    {

        dataHolder.levelChunks = new List<string>();


        int childCount = entryParent.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (entryParent.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                dataHolder.levelChunks.Add(entryParent.transform.GetChild(i).name);
            }
        }

        //list of chunks casual names in order
    }

}
