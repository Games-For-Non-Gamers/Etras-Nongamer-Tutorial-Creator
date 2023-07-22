using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Text.RegularExpressions;
using System.Linq;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Items;
using Etra.StarterAssets.Source.Editor;
using static Etra.StarterAssets.EtraCharacterMainController;
using Etra.NonGamerTutorialCreator.Level;
using static Etra.NonGamerTutorialCreator.TutorialCreator.TutorialCreatorAbilityTreeView;
#endif
//Editor
public class ExecutableListUpdater : MonoBehaviour
{
#if UNITY_EDITOR
    //e
    public ExecutableAbilitySelection abilitySelection;
    public Executable_TeachSelection teachSelection;
    public Executable_LevelBuilder levelBuilder;

    private List<string> abilitiesOrItemsThatHaveTeachingChunks;

    [ContextMenu("Update Lists")]
    void UpdateLists()
    {
        abilitySelection.standardAbilities = GetAllStandardAbilities();
        abilitySelection.allFpsAbilities = GetAllFpsAbilities();
        abilitySelection.allFpsItems = GetAllFpsItems();
        abilitySelection.allTpsAbilities = GetAllTpsAbilities();
        teachSelection.abilitiesWithTeachingBlocks = GetAbilitiesWithTeachingChunks();
        levelBuilder.allLevelChunks = AssetDatabase.FindAssets($"t:{typeof(LevelChunk).Name}").Select(x => AssetDatabase.GUIDToAssetPath(x)) .Select(x => AssetDatabase.LoadAssetAtPath<LevelChunk>(x)).ToList();

    }


    public List<string> GetAllStandardAbilities()
    {
        List<Ability> allAbilities = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();
        List<Ability> standardAbilities = allAbilities.Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.All)).ToList();


        List<Ability> tempList = new List<Ability>();


        tempList.AddRange(standardAbilities);


        List<string> returnedList = new List<string>();

        foreach (var ability in tempList)
        {
            returnedList.Add(ability.name);
        }
        return returnedList;
    }


    public List<string> GetAllFpsAbilities()
    {
        List<Ability> allAbilities = EtraGUIUtility.FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();
        List<Ability> fpsAbilities = allAbilities.Where(x => EtraGUIUtility.CheckForUsage(x.type, GameplayTypeFlags.FirstPerson)).ToList();


        List<Ability> tempList = new List<Ability>();


        tempList.AddRange(fpsAbilities);


        List<string> returnedList = new List<string>();

        foreach (var ability in tempList)
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
       List<LevelChunk> _avaliableChunks = AssetDatabase.FindAssets($"t:{typeof(LevelChunk).Name}").Select(x => AssetDatabase.GUIDToAssetPath(x)).Select(x => AssetDatabase.LoadAssetAtPath<LevelChunk>(x)).ToList();

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

#endif


}
