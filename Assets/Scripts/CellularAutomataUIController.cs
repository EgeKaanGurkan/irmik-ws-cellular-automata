using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class CellularAutomataUIController : MonoBehaviour
{
    [Header("UI References")]
    public UIDocument uiDocument;

    [Header("CA Model Reference")]
    public StackModel stackModel;

    [Header("Rule Configuration")]
    public CARuleSet ruleSet = new CARuleSet();

    private VisualElement root;
    private VisualElement uiContent;
    private VisualElement layerControlsContainer;
    private VisualElement layerList;
    private VisualElement simulationControls;
    private Button toggleUiBtn;
    private Button addLayerBtn;
    private Button startBtn;
    private Button stopBtn;
    private Button stepBtn;
    private Button resetBtn;
    private Label layerInfoLabel;

    // Dialog elements
    private VisualElement ruleDialog;
    private TextField ruleNameField;
    private IntegerField startLayerField;
    private IntegerField endLayerField;
    private IntegerField survivalMinField;
    private IntegerField survivalMaxField;
    private IntegerField birthMinField;
    private IntegerField birthMaxField;
    private Button dialogCancelBtn;
    private Button dialogConfirmBtn;

    private List<VisualElement> layerRuleElements = new List<VisualElement>();
    private bool isSimulationRunning = false;
    private bool isUiVisible = true;
    private bool isDialogMode = false;

    private void Start()
    {
        InitializeUI();
        SetupEventHandlers();
        RefreshLayerList();
        UpdateSimulationInfo();

        // Force white text on all elements
        ForceWhiteText();
    }

    private void ForceWhiteText()
    {
        // Force all text elements to be white
        var allLabels = root.Query<Label>().ToList();
        foreach (var label in allLabels)
        {
            label.style.color = Color.white;
        }

        var allButtons = root.Query<Button>().ToList();
        foreach (var button in allButtons)
        {
            button.style.color = Color.white;
        }

        Debug.Log("Forced white text on all UI elements");
    }

    private void InitializeUI()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        root = uiDocument.rootVisualElement;

        // Get UI elements
        uiContent = root.Q<VisualElement>("ui-content");
        layerControlsContainer = root.Q<VisualElement>("layer-controls-container");
        layerList = root.Q<VisualElement>("layer-list");
        simulationControls = root.Q<VisualElement>("simulation-controls");
        toggleUiBtn = root.Q<Button>("toggle-ui-btn");
        addLayerBtn = root.Q<Button>("add-layer-btn");
        startBtn = root.Q<Button>("start-btn");
        stopBtn = root.Q<Button>("stop-btn");
        stepBtn = root.Q<Button>("step-btn");
        resetBtn = root.Q<Button>("reset-btn");
        layerInfoLabel = root.Q<Label>("layer-info-label");

        // Get dialog elements
        ruleDialog = root.Q<VisualElement>("rule-dialog");
        ruleNameField = root.Q<TextField>("rule-name-field");
        startLayerField = root.Q<IntegerField>("start-layer-field");
        endLayerField = root.Q<IntegerField>("end-layer-field");
        survivalMinField = root.Q<IntegerField>("survival-min-field");
        survivalMaxField = root.Q<IntegerField>("survival-max-field");
        birthMinField = root.Q<IntegerField>("birth-min-field");
        birthMaxField = root.Q<IntegerField>("birth-max-field");
        dialogCancelBtn = root.Q<Button>("dialog-cancel-btn");
        dialogConfirmBtn = root.Q<Button>("dialog-confirm-btn");
    }

    private void SetupEventHandlers()
    {
        toggleUiBtn.clicked += ToggleUI;
        addLayerBtn.clicked += ShowAddRuleDialog;
        startBtn.clicked += StartSimulation;
        stopBtn.clicked += StopSimulation;
        stepBtn.clicked += StepSimulation;
        resetBtn.clicked += ResetSimulation;

        // Dialog event handlers
        dialogCancelBtn.clicked += HideAddRuleDialog;
        dialogConfirmBtn.clicked += ConfirmAddRule;
    }

    private void ToggleUI()
    {
        isUiVisible = !isUiVisible;

        // Hide/show layer controls and other content, but keep simulation controls visible
        layerControlsContainer.style.display = isUiVisible ? DisplayStyle.Flex : DisplayStyle.None;
        ruleDialog.style.display = DisplayStyle.None; // Also hide dialog when toggling

        // Keep simulation controls always visible
        simulationControls.style.display = DisplayStyle.Flex;

        toggleUiBtn.text = isUiVisible ? "⚙" : "⚙";
        Debug.Log($"UI toggled: {isUiVisible} (simulation controls remain visible)");
    }

    private void RefreshLayerList()
    {
        // Clear existing layer elements
        foreach (var element in layerRuleElements)
        {
            if (element != null && element.parent != null)
                element.parent.Remove(element);
        }
        layerRuleElements.Clear();

        Debug.Log($"Refreshing layer list with {ruleSet.rules.Length} rules");

        // Create UI elements for each rule
        for (int i = 0; i < ruleSet.rules.Length; i++)
        {
            Debug.Log($"Creating rule {i}: {ruleSet.rules[i].ruleName} (L{ruleSet.rules[i].startLayer}-{ruleSet.rules[i].endLayer})");
            CreateLayerRuleElement(i);
        }
    }

    private void CreateLayerRuleElement(int ruleIndex)
    {
        var rule = ruleSet.rules[ruleIndex];

        Debug.Log($"Creating UI element for rule: {rule.ruleName}, Layers: {rule.startLayer}-{rule.endLayer}");

        // Create compact container
        var ruleElement = new VisualElement();
        ruleElement.AddToClassList("layer-rule-item");

        // Layer info (compact display)
        var layerInfo = new VisualElement();
        layerInfo.AddToClassList("layer-info");

        var rangeLabel = new Label($"L{rule.startLayer}-{rule.endLayer}");
        rangeLabel.AddToClassList("layer-range-label");
        rangeLabel.style.color = Color.white; // Force white text

        var ruleSummary = new Label($"S:{rule.survivalMin}-{rule.survivalMax} B:{rule.birthMin}-{rule.birthMax}");
        ruleSummary.AddToClassList("rule-summary");
        ruleSummary.style.color = Color.white; // Force white text

        layerInfo.Add(rangeLabel);
        layerInfo.Add(ruleSummary);

        // Remove button
        var removeBtn = new Button(() => RemoveLayerRule(ruleIndex));
        removeBtn.text = "×";
        removeBtn.AddToClassList("remove-layer-btn");
        removeBtn.style.color = Color.white; // Force white text

        // Assemble the element
        ruleElement.Add(layerInfo);
        ruleElement.Add(removeBtn);

        layerList.Add(ruleElement);
        layerRuleElements.Add(ruleElement);

        Debug.Log($"Added rule element to UI. Total elements: {layerRuleElements.Count}");
    }

    private VisualElement CreateParameterGroup(string label, int minValue, int maxValue, System.Action<int, int> onValueChanged)
    {
        var group = new VisualElement();
        group.AddToClassList("parameter-group");

        var minField = new IntegerField();
        minField.value = minValue;
        minField.AddToClassList("parameter-field");
        minField.RegisterValueChangedCallback(evt => onValueChanged(evt.newValue, maxValue));

        var maxField = new IntegerField();
        maxField.value = maxValue;
        maxField.AddToClassList("parameter-field");
        maxField.RegisterValueChangedCallback(evt => onValueChanged(minValue, evt.newValue));

        var minLabel = new Label($"{label} Min");
        minLabel.AddToClassList("parameter-label");
        var maxLabel = new Label($"{label} Max");
        maxLabel.AddToClassList("parameter-label");

        group.Add(minLabel);
        group.Add(minField);
        group.Add(maxLabel);
        group.Add(maxField);

        return group;
    }

    private void ShowAddRuleDialog()
    {
        // Set default values
        ruleNameField.value = $"Rule {ruleSet.rules.Length + 1}";
        startLayerField.value = ruleSet.rules.Length * 20;
        endLayerField.value = startLayerField.value + 19;
        survivalMinField.value = 2;
        survivalMaxField.value = 3;
        birthMinField.value = 3;
        birthMaxField.value = 3;

        // Replace layer controls with dialog
        isDialogMode = true;
        layerControlsContainer.style.display = DisplayStyle.None;
        ruleDialog.style.display = DisplayStyle.Flex;
        Debug.Log("Showing rule dialog, hiding layer controls");
    }

    private void HideAddRuleDialog()
    {
        // Restore layer controls
        isDialogMode = false;
        layerControlsContainer.style.display = DisplayStyle.Flex;
        ruleDialog.style.display = DisplayStyle.None;
        Debug.Log("Hiding rule dialog, showing layer controls");
    }

    private void ConfirmAddRule()
    {
        // Create new rule from dialog values
        var newRule = new CARule();
        newRule.ruleName = ruleNameField.value;
        newRule.startLayer = startLayerField.value;
        newRule.endLayer = endLayerField.value;
        newRule.survivalMin = survivalMinField.value;
        newRule.survivalMax = survivalMaxField.value;
        newRule.birthMin = birthMinField.value;
        newRule.birthMax = birthMaxField.value;

        // Add to rule set
        var newRules = new CARule[ruleSet.rules.Length + 1];
        ruleSet.rules.CopyTo(newRules, 0);
        newRules[ruleSet.rules.Length] = newRule;
        ruleSet.rules = newRules;

        RefreshLayerList();
        HideAddRuleDialog();
    }

    private void RemoveLayerRule(int index)
    {
        if (ruleSet.rules.Length <= 1) return; // Keep at least one rule

        var newRules = ruleSet.rules.Where((rule, i) => i != index).ToArray();
        ruleSet.rules = newRules;

        RefreshLayerList();
    }

    private void StartSimulation()
    {
        if (stackModel != null)
        {
            isSimulationRunning = true;
            stackModel.enabled = true;
            UpdateSimulationInfo();
        }
    }

    private void StopSimulation()
    {
        if (stackModel != null)
        {
            isSimulationRunning = false;
            stackModel.enabled = false;
            UpdateSimulationInfo();
        }
    }

    private void StepSimulation()
    {
        if (stackModel != null)
        {
            // Stop automatic simulation if it's running
            isSimulationRunning = false;
            stackModel.enabled = false;

            // Manually advance one step
            if (stackModel.currentLayer < stackModel.stack.layerCount - 1)
            {
                stackModel.currentLayer++;
                stackModel.MyCARuleStep();
                stackModel.UpdateStack();
                UpdateSimulationInfo();
                Debug.Log($"Manual step: Advanced to layer {stackModel.currentLayer}");
            }
            else
            {
                Debug.Log("Simulation already at final layer");
            }
        }
    }

    private void ResetSimulation()
    {
        if (stackModel != null)
        {
            stackModel.ResetModel();
            isSimulationRunning = false;
            stackModel.enabled = false;
            UpdateSimulationInfo();
        }
    }

    private void UpdateSimulationInfo()
    {
        if (stackModel != null)
        {
            layerInfoLabel.text = $"Layer: {stackModel.currentLayer + 1}/{stackModel.stack.layerCount}";
        }
    }

    private void Update()
    {
        if (isSimulationRunning && stackModel != null)
        {
            UpdateSimulationInfo();
        }
    }

    // Public method to get the current rule set (called by StackModel)
    public CARuleSet GetRuleSet()
    {
        return ruleSet;
    }
}
