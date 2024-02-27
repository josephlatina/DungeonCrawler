
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
    pillItem,
    teethItem,
    empty
}

public enum GameState {
    gameStarted,
    playingLevel,
    engagingEnemies,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    dungeonOverviewMap,
    restartGame
}
