using Etra.NonGamerTutorialCreator.Level;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class Executable_LevelBuilder : MonoBehaviour
{


    public List<LevelChunk> allLevelChunks;
    public List<LevelChunk> possibleLevelChunks = new List<LevelChunk>();

    public List<LevelChunk> reccomendedLevelChunks;

    private ExecutableNewLevelDataHolder dataHolder;

    [ContextMenu ("TextRun")]
    private void OnEnable()
    {
        if (possibleLevelChunks.Count > 0) // Make a check for changes in previous pages, if so reload
        {
            return;
        }

        dataHolder = GetComponentInParent<ExecutableNewLevelDataHolder>();
        RebuildAvaliableChunksCache();
    }

    public string GenerateName(string fileName)
    {
        string toReturn = fileName;
        toReturn = toReturn.Split('_').Last();

        toReturn = Regex.Replace(toReturn, "([a-z])([A-Z])", "$1 $2");
        Debug.Log(toReturn);
        return toReturn;
    }

    public void RebuildAvaliableChunksCache()
    {
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
                chunk.tempRecommended = false;
            });
        }
    }

    private void ProcessCameraMovementChunks()
    {
        List<LevelChunk> chunksToCompare = possibleLevelChunks
            .Where(l => l.taughtAbilities.Contains("Camera Movement"))
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

    private void CreateUiChunks()
    {

    }
}
