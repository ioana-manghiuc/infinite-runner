# Fox Infinity-Runner
Infinity Runner game implemented in Unity. \
This project generates an endless track for the runner, as well as obstacles to jump over or slide under.

### **_FEATURES_**
- _fox_ : using keyboard input, the runner (fox) will turn (**A** for left, **D** for right), jump (using the space bar) or slide (**Shift** or **S** keys). Animations and sound effect are played for the jump and slide actions, as well as for when the fox collides with an obstacle.
- _camera movement_ : the runner has a virtual camera attached which follows its movements
- _score_ : calculated and stored in PlayerPrefs. It stores the score of the last game, the highest score, and the previous highest score. This way, if the player obtains a new high score, upon the game's end, the user will see the previous high score as well as the fact that they achieved a new one.
- _collision detection_ : if the fox collides with an obstacle, an animation and sound effect are played, and then the Game Over scene is loaded which is accompanied by a sound effect. The player will see the score they obtained in the run, and has the option to **_Retry_** or **_Exit_** to the Game Start scene.
- _game start_ : when the game is started, a soundtrack is faded in. When/If a collision occurs, the soundtrack is faded out smoothly.
- _skybox_, in which our scene is placed

**Assets used:**
- https://assetstore.unity.com/packages/3d/characters/animals/toon-fox-183005
- https://assetstore.unity.com/packages/3d/vegetation/trees/lemon-trees-200372
- https://assetstore.unity.com/packages/3d/environments/fantasy-landscape-103573
- https://assetstore.unity.com/packages/3d/vegetation/trees/polygon-trees-224068

**Soundtrack** \
Fast Feel Banana Peel by Alexander Nakarada | https://creatorchords.com
Music promoted on https://www.chosic.com/free-music/all/
Creative Commons Attribution 4.0 International (CC BY 4.0)
https://creativecommons.org/licenses/by/4.0/
