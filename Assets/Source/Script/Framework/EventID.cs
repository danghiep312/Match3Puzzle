public enum EventID {
    None = 0,
    ClickToTile,
    AddToQueue,
    CombineComplete,
    // For build map
    Delete,
    Return,
    Restart,
    Retry,
    
    PlayGame,
    Win,
    GameOver,
    //Use spell
    Spell,
    
    // UI Flow
    GoHome,
    PrepareGoHome,
    Revive,
    AddSlot,
}