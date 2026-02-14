# Product Roadmap: BADY

## Phase 1: MVP

* **Core Loop:**
    * Spawn -> Chop Ingredients -> Cook (Fry/Boil) -> Plate -> Deliver.
    * Win/Loss logic based on time limits and star ratings.
* **Networking (Local Only):**
    * Host/Client connection via Local Wi-Fi (Unity Netcode for GameObjects).
    * Lobby syncing (Players appearing in the kitchen).
* **Controls:**
    * Virtual Joystick (Movement).
    * Interact Button (Pickup/Drop).
    * Action Button (Chop/Throw).
* **Content:**
    * **1 Character:** "The Chef" (with 4 color variations).
    * **1 Map:** "Static Kitchen" (No moving trucks/ice yet).
    * **3 Global Recipes:**
        * Simple: "Classic Burger"
        * Medium: "Spicy Ramen" (Boiling mechanic)
        * Complex/Funny: "The Towering Pizza" (Baking mechanic)
* **Feedback:**
    * Basic Sound FX (Chop, Sizzle, Alarm).
    * Visual Feedback (Smoke, Progress Bars, Highlights).

## Phase 2: Post-Launch & Scale

* **Online Multiplayer:**
    * Integration of Unity Relay & Lobby services for remote play.
    * "Join Code" system for inviting friends worldwide.
* **Single Player Mode:**
    * "Chef Swap" mechanic (switch control between two avatars). *Note: Low priority for MVP.*
* **Dynamic Maps:**
    * Moving elements (Trucks that split apart, slippery ice floors).
    * Portals/Conveyor belts.
* **Progression:**
    * Unlockable Chefs (Hats, Skins).
    * Level Selection Map (World 1: Italy, World 2: Japan).
