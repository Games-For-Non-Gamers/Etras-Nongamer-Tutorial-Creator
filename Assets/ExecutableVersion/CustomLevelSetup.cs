using Etra.NonGamerTutorialCreator.Level;
using Etra.StarterAssets;
using Etra.StarterAssets.Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Etra.StarterAssets.EtraCharacterMainController;

public class CustomLevelSetup : MonoBehaviour
{

    ExecutableNewLevelDataHolder levelData;
    LevelController levelController;
    OpeningMenu openingMenu;
    OpeningScene openeningScene;
    EtraCharacterMainController character;
    // Start is called before the first frame update
    void Awake()
    {
        //Get references dynamically
        openingMenu = FindObjectOfType<OpeningMenu>();
        openingMenu.canBeginGame = false;
        levelController = FindObjectOfType<LevelController>();
        levelData = FindObjectOfType<ExecutableNewLevelDataHolder>();
        character = FindObjectOfType<EtraCharacterMainController>();

        //Set proper settings
        UpdateCharacter();
        UpdateLevel();
        openeningScene = FindObjectOfType<OpeningScene>();
        openeningScene.GetReferenceVariables();
        StartCoroutine(updateItems());
        openingMenu.canBeginGame = true;
    }

    IEnumerator updateItems()
    {
        yield return new WaitForSeconds(0.01f);
        character.etraFPSUsableItemManager.updateUsableItemsArray();
    }


    void UpdateCharacter()
    {
        character.applyGameplayChanges(levelData.gameplayType, levelData.characterModel);
        character.etraAbilityManager.activateAbilities(levelData.abilitiesToActivate);
        character.etraFPSUsableItemManager.activateAbilities(levelData.abilitiesToActivate);
    }


    //Maybe different approach of loading in non vital chunks from resources.load in the future?
    void UpdateLevel()
    {
        List<LevelChunkObject> newChunks = new List<LevelChunkObject>();

        foreach (string chunkName in levelData.levelChunks)
        {
            foreach (LevelChunkObject lc in levelController.chunks)
            {
                if (chunkName == lc.gameObject.name) {
                    newChunks.Add(lc);
                }
            }

        }

        //Delete unused blocks
        List<LevelChunkObject> uniqueBlocks = levelController.chunks.Except(newChunks).ToList();
        foreach (LevelChunkObject lc in uniqueBlocks)
        {
            Destroy(lc.gameObject);
        }


        levelController.ResetAllChunksPositions(newChunks);
    }


}
