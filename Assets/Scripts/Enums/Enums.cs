
public enum Orientation
{
    north,
    east,
    south,
    west,
    none
}

public enum ChestSpawnEvent {
    onRoomEntry,
    onEnemiesDefeated
}

public enum ChestSpawnPosition {
    atSpawnerPosition,
    atPlayerPosition
}

public enum ChestState {
    closed,
    healthPotionItem,
    weaponItem,
    pillItem
}

public enum GameState {
    gameStarted,
    playingLevel,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    dungeonOverviewMap,
    restartGame
}
