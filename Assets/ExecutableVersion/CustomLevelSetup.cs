using Etra.NonGamerTutorialCreator.Level;
using Etra.StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Etra.StarterAssets.EtraCharacterMainController;

public class CustomLevelSetup : MonoBehaviour
{

    ExecutableNewLevelDataHolder levelData;
    LevelController levelController;
    OpeningMenu openingMenu;
    EtraCharacterMainController character;
    // Start is called before the first frame update
    void Awake()
    {
        //Get references dynamically
        openingMenu.canBeginGame = false;
        levelController = FindObjectOfType<LevelController>();
        levelData = FindObjectOfType<ExecutableNewLevelDataHolder>();
        character = FindObjectOfType<EtraCharacterMainController>();
        //Set proper settings
        UpdateCharacter();
        UpdateLevel();
        openingMenu.canBeginGame = true;
    }

    void UpdateCharacter()
    {
        character.applyGameplayChanges(levelData.gameplayType, levelData.characterModel);
        character.etraAbilityManager.activateAbilities(levelData.abilitiesToActivate);
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
        newChunks.Reverse();

       levelController.ResetAllChunksPositions(newChunks);
    }


}
