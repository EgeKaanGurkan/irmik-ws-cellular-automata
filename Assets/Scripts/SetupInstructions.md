# Unity UI Toolkit Setup Instructions

## Files Created:
1. `CellularAutomataUI.uxml` - UI layout document
2. `CellularAutomataUI.uss` - UI stylesheet
3. `CellularAutomataUIController.cs` - UI controller script
4. `CARuleData.cs` - Data structures for CA rules

## Setup Steps in Unity:

### 1. Create UI Document GameObject
1. In your scene, create an empty GameObject
2. Name it "CellularAutomataUI"
3. Add a `UIDocument` component to it
4. In the UIDocument component:
   - Set **Source Asset** to `CellularAutomataUI.uxml`
   - Set **Panel Settings** to create a new Panel Settings asset

### 2. Add UI Controller Script
1. Add the `CellularAutomataUIController` script to the same GameObject
2. In the inspector, assign:
   - **UI Document**: The UIDocument component on this GameObject
   - **Stack Model**: Your existing StackModel GameObject

### 3. Connect StackModel to UI Controller
1. Select your StackModel GameObject
2. In the StackModel component, assign:
   - **UI Controller**: The CellularAutomataUIController component

### 4. Configure Panel Settings (Optional)
1. Create a new Panel Settings asset in your Project window
2. Configure the Panel Settings as needed for your UI
3. Assign this Panel Settings to the UIDocument component

## How to Use:

### Layer Rule Management:
- **Add New Layer Rule**: Click "Add New Layer Rule" to create a new rule set
- **Configure Rules**: For each rule, set:
  - **Start/End Layer**: Which layers this rule applies to
  - **Survival Min/Max**: How many neighbors an alive cell needs to stay alive
  - **Birth Min/Max**: How many neighbors a dead cell needs to become alive
- **Remove Rules**: Click the "×" button to remove a rule

### Simulation Controls:
- **Start Simulation**: Begins the cellular automata simulation
- **Stop Simulation**: Pauses the simulation
- **Reset Simulation**: Resets the simulation to the initial state

### Real-time Updates:
- The UI shows the current layer and total layers
- Rule changes take effect immediately when you modify parameters
- The simulation uses the UI-configured rules instead of hardcoded ones

## Features:
- **Dynamic Rule Configuration**: Change CA rules for different layer ranges
- **Real-time Parameter Adjustment**: Modify rules while simulation is running
- **Visual Feedback**: See current simulation state and layer information
- **Fallback Support**: If no UI controller is assigned, uses default rules

## Customization:
- Modify `CellularAutomataUI.uss` to change the visual appearance
- Edit `CellularAutomataUI.uxml` to add/remove UI elements
- Extend `CARuleData.cs` to add more rule parameters
- Modify `CellularAutomataUIController.cs` to add new functionality
