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
    }

    void UpdateCharacter()
    {
        character.applyGameplayChanges(levelData.gameplayType, levelData.characterModel);



    }

    void UpdateLevel()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Header("Current Values")]
    public GameplayType gameplayType = GameplayType.FirstPerson; //Set by toggle
    public Model characterModel; //Set by dropdown
    public List<string> abilitiesInLevel; //Set by Teach Selection <--- Don't need this in data
    public List<string> abilitiesToActivate; //Set by Teach Selection
    public List<string> levelChunks; //Set by Level builder

}
