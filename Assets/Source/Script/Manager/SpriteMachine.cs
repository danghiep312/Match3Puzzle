using Sirenix.OdinInspector;
using UnityEngine;

public class SpriteMachine : Singleton<SpriteMachine>
{
    public Sprite[] itemSprites;
    public Sprite[] blocks;
    
    [FoldoutGroup("Button Level Sprite")]
    public Sprite buttonLevelPlaying;
    [FoldoutGroup("Button Level Sprite")]
    public Sprite buttonLevelLocked;
    [FoldoutGroup("Button Level Sprite")]
    public Sprite buttonLevelPassed;

    public override void Awake()
    {
        base.Awake();
        blocks = Resources.LoadAll<Sprite>("Sprites/Block");
        itemSprites = blocks;
    }
}
