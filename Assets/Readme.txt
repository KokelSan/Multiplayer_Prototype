Théo Kokel's test for Atelier Daruma.

Made on Unity 2020.3.8f1 with the online package Mirror.

Controls:
Move : ZQSD & arrows
Show/hide chat : Tab
Show/hide in-game information panel : Esc

Explication of the game:
- When connected, the players are spawned in a default scene 
- Two portals/proximity checkers are placed at each extremity of the scene. When first touched by a player, each client loads the corresponding scene
- The count is stocked in the SubSceneManager script in a dictionnary <SceneName, PlayersNbInIt>. It is a synchronised dictionnary : when the server modifies it, each client receives the modification
- A callback is called each time the dictionnary is modified, which leads to check which scene should be loaded or not

- A chat and a notification panel are implemented to exchange messages. The chat is a player-to-player message system when the notification panel is more like game-to-player
- The chat is updated each time a message is receive regarless of its visibility, so that when oppening it the player sees all the messages sent since its connection
- The notification panel works in the same way as the chat, excepts that the player has no control on it. It appears when hidden and fade out when the display time is above the notifDisplayDuration variable

- A spawn manager spawns randomly collectibles on the floor of loaded scenes. They are child of the default scene, but when spawned we keep a reference of the floor they are spawned on
- This allows to synchronise their position and visibility state for new players (if objects were attached to a sub scene, they would all be destructed when unloading the scene)
- Those objects were mainly implemented to demonstrate the notification service (a notif is sent on spawn and on collection by a player)